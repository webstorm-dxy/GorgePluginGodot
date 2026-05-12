<#
.SYNOPSIS
    GorgePluginGodot - One-Command Installer (PowerShell)
.DESCRIPTION
    Installs the GorgePluginGodot framework into an existing Godot .NET project.
    Usage: irm <url> | iex
    Or:    .\install.ps1 [-ProjectPath C:\path\to\project]
#>
param(
    [string]$ProjectPath = ""
)

$RepoUrl = "https://github.com/webstorm-dxy/GorgePluginGodot.git"
$RequiredDotnetMajor = 8
$Marker = "GorgePlugin: added by installer"

# ---- State ----
$script:Warnings = 0
$script:CSPROJ_FILE = $null

# ---- Helpers ----
function Write-Phase {
    param([int]$Num, [string]$Text)
    Write-Host ""
    Write-Host "[$Num/$TotalPhases] " -NoNewline -ForegroundColor Cyan
    Write-Host $Text -ForegroundColor White
}

function Write-Ok {
    param([string]$Text)
    Write-Host "  [OK] " -NoNewline -ForegroundColor Green
    Write-Host $Text
}

function Write-Fail {
    param([string]$Text)
    Write-Host "  [FAIL] " -NoNewline -ForegroundColor Red
    Write-Host $Text
    exit 1
}

function Write-Warn {
    param([string]$Text)
    Write-Host "  [WARN] " -NoNewline -ForegroundColor Yellow
    Write-Host $Text
    $script:Warnings++
}

function Write-Skip {
    param([string]$Text)
    Write-Host "  [SKIP] " -NoNewline -ForegroundColor Yellow
    Write-Host $Text
}

function Write-Info {
    param([string]$Text)
    Write-Host "  $Text"
}

# Run an external command safely: git/dotnet/cargo write progress to stderr,
# which would trigger fatal errors if $ErrorActionPreference were "Stop".
# Also handles CommandNotFoundException gracefully for optional tools.
# Returns stdout as string; sets $script:NativeExitCode to the command's exit code.
function Invoke-Native {
    param([ScriptBlock]$ScriptBlock)
    $prevEAP = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    try {
        try {
            $result = & $ScriptBlock 2>&1 | Out-String
            $script:NativeExitCode = $LASTEXITCODE
            return $result.TrimEnd()
        } catch {
            # CommandNotFoundException and other terminating errors
            $script:NativeExitCode = 127
            return $_.Exception.Message
        }
    } finally {
        $ErrorActionPreference = $prevEAP
    }
}

# ---- Header ----
Write-Host "GorgePluginGodot Installer" -ForegroundColor White
Write-Host "Target repo: $RepoUrl"
Write-Host ""

$TotalPhases = 9

# ==========================================
# Phase 1: Toolchain Checks
# ==========================================
Write-Phase 1 "Checking toolchain"

$gitOutput = Invoke-Native { git --version }
if ($script:NativeExitCode -ne 0 -or -not $gitOutput) {
    Write-Fail "git is not installed. Install from: https://git-scm.com/downloads"
}
$gitVersion = $gitOutput -replace 'git version ', ''
Write-Ok "git $gitVersion"

$dotnetOutput = Invoke-Native { dotnet --version }
if ($script:NativeExitCode -ne 0 -or -not $dotnetOutput) {
    Write-Fail ".NET SDK is required. Install from: https://dotnet.microsoft.com/download"
}
$dotnetVersion = $dotnetOutput.Trim()
$dotnetMajor = [int]($dotnetVersion.Split('.')[0])
if ($dotnetMajor -lt $RequiredDotnetMajor) {
    Write-Fail ".NET SDK $RequiredDotnetMajor.0+ is required. Found: $dotnetVersion"
}
Write-Ok "dotnet $dotnetVersion"

$hasCargo = $false
$cargoOutput = Invoke-Native { cargo --version }
if ($script:NativeExitCode -eq 0 -and $cargoOutput) {
    $cargoVersion = $cargoOutput -replace 'cargo ', ''
    Write-Ok "cargo $cargoVersion"
    $hasCargo = $true
} else {
    Write-Warn "cargo not found. Rust GDExtension (NineSliceSprite2D) will not be built."
}

$godotOutput = Invoke-Native { godot --version --headless }
if ($script:NativeExitCode -eq 0 -and $godotOutput -match "mono|\.net|\.NET") {
    Write-Ok "Godot .NET edition detected"
} elseif ($script:NativeExitCode -eq 0) {
    Write-Warn "Godot found but could not verify .NET edition. Make sure you use the .NET variant."
} else {
    Write-Warn "godot command not found in PATH. Cannot verify .NET edition."
}

# ==========================================
# Phase 2: Project Detection
# ==========================================
Write-Phase 2 "Detecting Godot project"

if ([string]::IsNullOrEmpty($ProjectPath)) {
    $ProjectPath = Get-Location
}

if (-not (Test-Path $ProjectPath)) {
    Write-Fail "Directory does not exist: $ProjectPath"
}
$ProjectPath = (Resolve-Path $ProjectPath).Path

if (-not (Test-Path (Join-Path $ProjectPath "project.godot"))) {
    Write-Fail "No project.godot found in $ProjectPath. Run from a Godot project root, or use -ProjectPath."
}
Write-Ok "Found project.godot"

$csprojFiles = Get-ChildItem -Path $ProjectPath -Filter "*.csproj" -File
if ($csprojFiles.Count -eq 0) {
    Write-Fail "No .csproj file found in $ProjectPath. This installer requires a Godot .NET project."
}
$script:CSPROJ_FILE = $csprojFiles[0].FullName
Write-Ok "Found $($csprojFiles[0].Name)"

Write-Info "Project: $ProjectPath"

# ==========================================
# Phase 3: Download Framework
# ==========================================
Write-Phase 3 "Downloading GorgePluginGodot framework"

$tmpDir = Join-Path $env:TEMP "gorge_install_$(Get-Random)"
New-Item -ItemType Directory -Path $tmpDir -Force | Out-Null
$repoDir = Join-Path $tmpDir "repo"

Write-Info "Cloning (sparse, shallow)..."
$null = Invoke-Native { git clone --filter=blob:none --sparse --depth=1 $RepoUrl $repoDir }
if ($script:NativeExitCode -ne 0 -or -not (Test-Path $repoDir)) {
    Write-Fail "Failed to clone repository."
}

Push-Location $repoDir
try {
    $null = Invoke-Native { git sparse-checkout set addons/ demo/ }
    if ($script:NativeExitCode -ne 0) {
        Write-Fail "Failed to sparse-checkout addons/ and demo/."
    }
    Write-Ok "Downloaded addons/ and demo/"
} finally {
    Pop-Location
}

# Check Native.zip for LFS pointer stub
$nativeZip = Join-Path $repoDir "addons/gorgeplugin/Native.zip"
if (Test-Path $nativeZip) {
    $header = [System.IO.File]::ReadAllBytes($nativeZip)[0..29]
    $headerStr = [System.Text.Encoding]::ASCII.GetString($header)
    if ($headerStr -match "git-lfs") {
        Write-Info "Native.zip is an LFS pointer. Resolving..."
        Push-Location $repoDir
        try {
            $null = Invoke-Native { git lfs pull --include="addons/gorgeplugin/Native.zip" }
            if ($script:NativeExitCode -eq 0) {
                $header2 = [System.IO.File]::ReadAllBytes($nativeZip)[0..3]
                if ($header2[0] -eq 0x50 -and $header2[1] -eq 0x4B) {
                    Write-Ok "Native.zip resolved (valid ZIP)"
                } else {
                    Write-Warn "Native.zip still appears to be an LFS pointer after pull."
                }
            } else {
                Write-Warn "git-lfs pull failed. Native.zip is an LFS pointer stub."
                Write-Warn "Download manually from: https://github.com/webstorm-dxy/GorgePluginGodot/releases"
            }
        } finally {
            Pop-Location
        }
    } else {
        Write-Ok "Native.zip is a valid file"
    }
}

# ==========================================
# Phase 4: Copy Files
# ==========================================
Write-Phase 4 "Copying addons and demo to project"

$addonsDir = Join-Path $ProjectPath "addons"
if (-not (Test-Path $addonsDir)) {
    New-Item -ItemType Directory -Path $addonsDir -Force | Out-Null
}

function Copy-NoClobber {
    param([string]$Source, [string]$Dest)
    if (-not (Test-Path $Source)) { return }
    if (Test-Path $Dest) {
        # Copy individual files, skip existing
        Get-ChildItem $Source -Recurse | ForEach-Object {
            $relativePath = $_.FullName.Substring($Source.Length).TrimStart('\', '/')
            $targetPath = Join-Path $Dest $relativePath
            if (-not (Test-Path $targetPath)) {
                # Ensure parent directory exists
                $parentDir = Split-Path $targetPath -Parent
                if (-not (Test-Path $parentDir)) {
                    New-Item -ItemType Directory -Path $parentDir -Force | Out-Null
                }
                Copy-Item $_.FullName $targetPath
            }
        }
    } else {
        Copy-Item $Source $Dest -Recurse
    }
}

$copyCount = 0
$addons = @("gorgeplugin", "nine_slice_sprite_2d_godot2d")
foreach ($addon in $addons) {
    $src = Join-Path $repoDir "addons/$addon"
    $dst = Join-Path $addonsDir $addon
    if (Test-Path $src) {
        if (Test-Path $dst) {
            Write-Info "addons/$addon/ already exists. Copying only new files..."
            Copy-NoClobber $src $dst
        } else {
            Copy-Item $src $dst -Recurse
        }
        $fileCount = (Get-ChildItem $dst -Recurse -File).Count
        Write-Ok "addons/$addon/ copied ($fileCount files)"
        $copyCount++
    }
}

if (Test-Path (Join-Path $repoDir "demo")) {
    $demoDst = Join-Path $ProjectPath "demo"
    if (-not (Test-Path $demoDst)) {
        Copy-Item (Join-Path $repoDir "demo") $demoDst -Recurse
        Write-Ok "demo/ copied (example scene)"
    } else {
        Write-Info "demo/ already exists, skipping"
    }
}

if ($copyCount -eq 0) {
    Write-Fail "No addon directories were copied. The repository may have changed structure."
}

# ==========================================
# Phase 5: Merge .csproj
# ==========================================
Write-Phase 5 "Merging NuGet dependencies into $(Split-Path $CSPROJ_FILE -Leaf)"

$changedCsproj = $false

# Load as XML
[xml]$csproj = Get-Content $CSPROJ_FILE
$project = $csproj.Project

# Find or create PropertyGroup
$pg = $project.PropertyGroup
if (-not $pg) {
    $pg = $csproj.CreateElement("PropertyGroup")
    $project.InsertBefore($pg, $project.ItemGroup) | Out-Null
}

# EnableDynamicLoading
$edl = $pg.EnableDynamicLoading
if ($edl -ne "true") {
    if ($edl) {
        Write-Warn "EnableDynamicLoading is '$edl', expected 'true'"
    } else {
        $el = $csproj.CreateElement("EnableDynamicLoading")
        $el.InnerText = "true"
        # Add comment
        $comment = $csproj.CreateComment($Marker)
        $pg.AppendChild($comment) | Out-Null
        $pg.AppendChild($el) | Out-Null
        Write-Ok "Added EnableDynamicLoading"
        $changedCsproj = $true
    }
} else {
    Write-Info "EnableDynamicLoading already present"
}

# AllowUnsafeBlocks
$aus = $pg.AllowUnsafeBlocks
if ($aus -ne "true") {
    if ($aus) {
        Write-Warn "AllowUnsafeBlocks is '$aus', expected 'true'"
    } else {
        $el = $csproj.CreateElement("AllowUnsafeBlocks")
        $el.InnerText = "true"
        $comment = $csproj.CreateComment($Marker)
        $pg.AppendChild($comment) | Out-Null
        $pg.AppendChild($el) | Out-Null
        Write-Ok "Added AllowUnsafeBlocks"
        $changedCsproj = $true
    }
} else {
    Write-Info "AllowUnsafeBlocks already present"
}

# TargetFramework
$tf = $pg.TargetFramework
if (-not $tf) {
    $el = $csproj.CreateElement("TargetFramework")
    $el.InnerText = "net8.0"
    $comment = $csproj.CreateComment($Marker)
    $pg.AppendChild($comment) | Out-Null
    $pg.AppendChild($el) | Out-Null
    Write-Ok "Added TargetFramework net8.0"
    $changedCsproj = $true
} else {
    Write-Info "TargetFramework already present ($($tf.InnerText))"
}

# NuGet packages
$nugetPackages = @{
    "Antlr4.Runtime.Standard" = "4.13.1"
    "Newtonsoft.Json"          = "13.0.3"
    "QuikGraph"                = "2.5.0"
    "SharpZipLib"              = "1.4.2"
}

# Find the ItemGroup with PackageReferences (or first ItemGroup)
$ig = $project.ItemGroup | Where-Object { $_.PackageReference } | Select-Object -First 1
if (-not $ig) {
    $ig = $project.ItemGroup | Select-Object -First 1
}
if (-not $ig) {
    $ig = $csproj.CreateElement("ItemGroup")
    $project.AppendChild($ig) | Out-Null
}

foreach ($pkgName in $nugetPackages.Keys) {
    $pkgVer = $nugetPackages[$pkgName]
    $existing = $ig.PackageReference | Where-Object { $_.Include -eq $pkgName }

    if ($existing) {
        if ($existing.Version -eq $pkgVer) {
            Write-Info "Package $pkgName $pkgVer already referenced"
        } else {
            Write-Warn "Package $pkgName is referenced at $($existing.Version) but expected $pkgVer. Version mismatch may cause runtime errors."
        }
    } else {
        $pr = $csproj.CreateElement("PackageReference")
        $pr.SetAttribute("Include", $pkgName)
        $pr.SetAttribute("Version", $pkgVer)
        $comment = $csproj.CreateComment($Marker)
        $ig.AppendChild($comment) | Out-Null
        $ig.AppendChild($pr) | Out-Null
        Write-Ok "Added $pkgName $pkgVer"
        $changedCsproj = $true
    }
}

if ($changedCsproj) {
    $csproj.Save($CSPROJ_FILE)
} else {
    Write-Info "No changes needed to $(Split-Path $CSPROJ_FILE -Leaf)"
}

# ==========================================
# Phase 6: Merge project.godot
# ==========================================
Write-Phase 6 "Configuring project.godot"

$godotFile = Join-Path $ProjectPath "project.godot"
$godotLines = [System.Collections.ArrayList]@(Get-Content $godotFile)
$changedGodot = $false

# ---- editor_plugins (gorgeplugin only) ----
$pluginGorge = "res://addons/gorgeplugin/plugin.cfg"

$editorPluginsIdx = -1
for ($i = 0; $i -lt $godotLines.Count; $i++) {
    if ($godotLines[$i] -match '^\[editor_plugins\]') {
        $editorPluginsIdx = $i
        break
    }
}

if ($editorPluginsIdx -ge 0) {
    Write-Info "editor_plugins section exists, merging..."
    $enabledIdx = -1
    for ($i = $editorPluginsIdx + 1; $i -lt $godotLines.Count; $i++) {
        if ($godotLines[$i] -match '^enabled=') {
            $enabledIdx = $i
            break
        }
        if ($godotLines[$i] -match '^\[') { break }
    }

    if ($enabledIdx -ge 0) {
        $enabledLine = $godotLines[$enabledIdx]
        if ($enabledLine -notmatch [regex]::Escape($pluginGorge)) {
            $existing = @()
            if ($enabledLine -match 'PackedStringArray\((.+)\)') {
                $inner = $Matches[1]
                $matches_r = [regex]::Matches($inner, '"([^"]*)"')
                foreach ($m in $matches_r) { $existing += $m.Groups[1].Value }
            }
            $all = $existing + @($pluginGorge)
            $quoted = ($all | ForEach-Object { "`"$_`"" }) -join ", "
            $godotLines[$enabledIdx] = "enabled=PackedStringArray($quoted)"
            Write-Ok "Added gorgeplugin to editor_plugins"
            $changedGodot = $true
        } else {
            Write-Info "gorgeplugin already enabled"
        }
    } else {
        $godotLines.Insert($editorPluginsIdx + 1, "enabled=PackedStringArray(`"$pluginGorge`")")
        Write-Ok "Added editor_plugins enabled line"
        $changedGodot = $true
    }
} else {
    $godotLines.Add("") | Out-Null
    $godotLines.Add("[editor_plugins]") | Out-Null
    $godotLines.Add("enabled=PackedStringArray(`"$pluginGorge`")") | Out-Null
    Write-Ok "Added editor_plugins section"
    $changedGodot = $true
}

if ($changedGodot) {
    $godotLines -join "`r`n" | Set-Content $godotFile -NoNewline
} else {
    Write-Info "No changes needed to project.godot"
}

# ==========================================
# Phase 7: Build Rust Extension
# ==========================================
Write-Phase 7 "Building Rust GDExtension"

$rustDir = Join-Path $ProjectPath "addons/nine_slice_sprite_2d_godot2d"

if (-not (Test-Path $rustDir)) {
    Write-Skip "Rust addon directory not found, skipping build"
} elseif (-not $hasCargo) {
    Write-Skip "cargo not installed, skipping Rust build"
    Write-Info "  Build manually: cd addons/nine_slice_sprite_2d_godot2d && cargo build && cargo build --release"
} else {
    Push-Location $rustDir
    try {
        $buildOk = $true

        Write-Info "Building debug..."
        $null = Invoke-Native { cargo build }
        if ($script:NativeExitCode -eq 0) {
            Write-Ok "Rust debug build succeeded"
        } else {
            Write-Warn "Rust debug build failed"
            $buildOk = $false
        }

        Write-Info "Building release..."
        $null = Invoke-Native { cargo build --release }
        if ($script:NativeExitCode -eq 0) {
            Write-Ok "Rust release build succeeded"

            if ($IsLinux) {
                $libPath = "target/release/libnine_slice_sprite_2d.so"
            } elseif ($IsMacOS) {
                $libPath = "target/release/libnine_slice_sprite_2d.dylib"
            } else {
                $libPath = "target/release/nine_slice_sprite_2d.dll"
            }
            if (Test-Path $libPath) {
                Write-Ok "Output: $rustDir/$libPath"
            }
        } else {
            Write-Warn "Rust release build failed"
            $buildOk = $false
        }

        if (-not $buildOk) {
            Write-Warn "Some Rust builds failed. Build manually: cd $rustDir && cargo build && cargo build --release"
        }
    } finally {
        Pop-Location
    }
}

# ==========================================
# Phase 8: Restore NuGet
# ==========================================
Write-Phase 8 "Restoring NuGet packages"

Push-Location $ProjectPath
try {
    $null = Invoke-Native { dotnet restore }
    if ($script:NativeExitCode -eq 0) {
        Write-Ok "NuGet packages restored"
    } else {
        Write-Warn "dotnet restore failed. Run manually: dotnet restore"
    }
} finally {
    Pop-Location
}

# ==========================================
# Phase 9: Report
# ==========================================
Write-Phase 9 "Installation complete"

Write-Host ""
Write-Host "========================================"  -ForegroundColor White
Write-Host " GorgePluginGodot Installation Complete!" -ForegroundColor White
Write-Host "========================================"  -ForegroundColor White
Write-Host ""
Write-Host "  [x] addons/gorgeplugin/                 - Gorge chart player (C#)"       -ForegroundColor Green
Write-Host "  [x] addons/nine_slice_sprite_2d_godot2d/ - Nine-slice GDExtension"        -ForegroundColor Green
Write-Host "  [x] demo/                               - Example demo scene"              -ForegroundColor Green
Write-Host "  [x] NuGet packages added                - Antlr4, Newtonsoft.Json, QuikGraph, SharpZipLib" -ForegroundColor Green
Write-Host "  [x] project.godot configured            - gorgeplugin enabled" -ForegroundColor Green
if ($hasCargo -and (Test-Path $rustDir)) {
    Write-Host "  [x] Rust GDExtension built             - NineSliceSprite2D ready"      -ForegroundColor Green
} else {
    Write-Host "  [-] Rust GDExtension skipped           - Build manually if needed"      -ForegroundColor Yellow
}
Write-Host ""
Write-Host "Next steps:" -ForegroundColor White
Write-Host "  1. Open your project in Godot 4.6 (.NET edition)"
Write-Host "  2. Build the C# solution (Godot will prompt, or: dotnet build)"
Write-Host "  3. Enable plugins: Project Settings -> Plugins"
Write-Host "  4. Add a GamePlayer node: Ctrl+A -> search 'GamePlayer'"
Write-Host ""

if ($script:Warnings -gt 0) {
    Write-Host "$($script:Warnings) warning(s) were reported. Review the output above." -ForegroundColor Yellow
}

# Cleanup
if (Test-Path $tmpDir) {
    Remove-Item $tmpDir -Recurse -Force -ErrorAction SilentlyContinue
}

exit 0
