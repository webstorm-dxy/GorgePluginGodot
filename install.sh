#!/usr/bin/env bash
# GorgePluginGodot - One-Command Installer
# Usage: curl -fsSL <url> | bash
# Or:    bash install.sh [-p /path/to/godot/project]
set -euo pipefail

REPO_URL="https://github.com/webstorm-dxy/GorgePluginGodot.git"
REQUIRED_DOTNET_MAJOR=8

# ---- Colors ----
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'
CYAN='\033[0;36m'; BOLD='\033[1m'; NC='\033[0m'

# ---- State ----
WARNINGS=0
PROJECT_DIR=""
CSPROJ_FILE=""
TMP_DIR=""

# ---- Helpers ----
phase()  { echo -e "\n${BOLD}${CYAN}[$1/$TOTAL_PHASES]${NC} ${BOLD}$2${NC}"; }
ok()     { echo -e "  ${GREEN}[OK]${NC} $1"; }
fail()   { echo -e "  ${RED}[FAIL]${NC} $1"; exit 1; }
warn()   { echo -e "  ${YELLOW}[WARN]${NC} $1"; WARNINGS=$((WARNINGS + 1)); }
skip()   { echo -e "  ${YELLOW}[SKIP]${NC} $1"; }
info()   { echo -e "  $1"; }

check_cmd() {
    if ! command -v "$1" &>/dev/null; then
        fail "$1 is not installed. $2"
    fi
}

# Cross-platform sed -i (macOS requires empty backup extension)
sed_inplace() {
    if sed --version 2>/dev/null | grep -q "GNU"; then
        sed -i "$@"
    else
        sed -i '' "$@"
    fi
}

cleanup() {
    if [ -n "$TMP_DIR" ] && [ -d "$TMP_DIR" ]; then
        rm -rf "$TMP_DIR"
    fi
}
trap cleanup EXIT

# ---- Parse Args ----
while getopts "p:h" opt; do
    case $opt in
        p) PROJECT_DIR="$OPTARG" ;;
        h) echo "Usage: bash install.sh [-p /path/to/godot/project]"; exit 0 ;;
        *) echo "Usage: bash install.sh [-p /path/to/godot/project]"; exit 1 ;;
    esac
done

# ---- Header ----
echo -e "${BOLD}GorgePluginGodot Installer${NC}"
echo -e "Target repo: ${REPO_URL}"
echo ""

# ---- Phase 1: Toolchain Checks ----
TOTAL_PHASES=9
phase 1 "Checking toolchain"

check_cmd git "Install git: https://git-scm.com/downloads"
GIT_VERSION=$(git --version | awk '{print $3}')
ok "git $GIT_VERSION"

check_cmd dotnet ".NET SDK is required for the C# plugin. Install: https://dotnet.microsoft.com/download"
DOTNET_VERSION=$(dotnet --version 2>/dev/null)
DOTNET_MAJOR=$(echo "$DOTNET_VERSION" | cut -d. -f1)
if [ "$DOTNET_MAJOR" -lt "$REQUIRED_DOTNET_MAJOR" ]; then
    fail ".NET SDK $REQUIRED_DOTNET_MAJOR.0+ is required. Found: $DOTNET_VERSION"
fi
ok "dotnet $DOTNET_VERSION"

if command -v cargo &>/dev/null; then
    CARGO_VERSION=$(cargo --version | awk '{print $2}')
    ok "cargo $CARGO_VERSION"
    HAS_CARGO=true
else
    warn "cargo not found. Rust GDExtension (NineSliceSprite2D) will not be built."
    HAS_CARGO=false
fi

if command -v godot &>/dev/null; then
    GODOT_VER=$(godot --version --headless 2>/dev/null || true)
    if echo "$GODOT_VER" | grep -qi "mono\|\.net"; then
        ok "Godot .NET edition detected"
    else
        warn "Godot found but could not verify .NET edition. Make sure you use the .NET variant."
    fi
else
    warn "godot command not found in PATH. Cannot verify .NET edition."
fi

# ---- Phase 2: Project Detection ----
phase 2 "Detecting Godot project"

if [ -z "$PROJECT_DIR" ]; then
    PROJECT_DIR="$PWD"
fi

PROJECT_DIR="$(cd "$PROJECT_DIR" 2>/dev/null && pwd)" || fail "Directory does not exist: $PROJECT_DIR"

if [ ! -f "$PROJECT_DIR/project.godot" ]; then
    fail "No project.godot found in $PROJECT_DIR. Run from a Godot project root, or use -p <path>."
fi
ok "Found project.godot"

CSPROJ_FILE=$(ls "$PROJECT_DIR"/*.csproj 2>/dev/null | head -1)
if [ -z "$CSPROJ_FILE" ]; then
    fail "No .csproj file found in $PROJECT_DIR. This installer requires a Godot .NET project."
fi
CSPROJ_NAME=$(basename "$CSPROJ_FILE")
ok "Found $CSPROJ_NAME"

info "Project: $PROJECT_DIR"

# ---- Phase 3: Download Framework ----
phase 3 "Downloading GorgePluginGodot framework"

TMP_DIR=$(mktemp -d)
info "Cloning (sparse, shallow)..."

git clone --filter=blob:none --sparse --depth=1 \
    "$REPO_URL" "$TMP_DIR/repo" 2>&1 | while IFS= read -r line; do
    info "  git: $line"
done

if [ ! -d "$TMP_DIR/repo" ]; then
    fail "Failed to clone repository."
fi

cd "$TMP_DIR/repo"
git sparse-checkout set addons/ demo/ 2>&1 | while IFS= read -r line; do
    info "  git: $line"
done
ok "Downloaded addons/ and demo/"

# Check Native.zip for LFS pointer stub
NATIVE_ZIP="$TMP_DIR/repo/addons/gorgeplugin/Native.zip"
if [ -f "$NATIVE_ZIP" ]; then
    HEADER=$(head -c 30 "$NATIVE_ZIP" 2>/dev/null || true)
    if echo "$HEADER" | grep -q "git-lfs"; then
        info "Native.zip is an LFS pointer. Resolving..."
        if command -v git-lfs &>/dev/null; then
            git lfs pull --include="addons/gorgeplugin/Native.zip" 2>&1 | while IFS= read -r line; do
                info "  lfs: $line"
            done
            HEADER2=$(head -c 4 "$NATIVE_ZIP" 2>/dev/null || true)
            if echo "$HEADER2" | grep -q "PK"; then
                ok "Native.zip resolved (valid ZIP)"
            else
                warn "Native.zip still appears to be an LFS pointer after pull."
            fi
        else
            warn "git-lfs not installed. Native.zip is an LFS pointer stub."
            warn "Download manually from: https://github.com/webstorm-dxy/GorgePluginGodot/releases"
        fi
    else
        ok "Native.zip is a valid file"
    fi
fi
cd "$PROJECT_DIR"

# ---- Phase 4: Copy Files ----
phase 4 "Copying addons and demo to project"

mkdir -p "$PROJECT_DIR/addons"

COPY_COUNT=0
for addon in gorgeplugin nine_slice_sprite_2d_godot2d; do
    SRC="$TMP_DIR/repo/addons/$addon"
    DST="$PROJECT_DIR/addons/$addon"
    if [ -d "$SRC" ]; then
        # no-clobber: only copy files that don't already exist
        if [ -d "$DST" ]; then
            info "addons/$addon/ already exists. Copying only new files..."
            # Use cp -rn for no-clobber recursive
            cp -rn "$SRC"/* "$DST"/ 2>/dev/null || true
            NEW_COUNT=$(find "$DST" -type f | wc -l)
            ok "addons/$addon/ merged ($NEW_COUNT files present)"
        else
            cp -r "$SRC" "$DST"
            NEW_COUNT=$(find "$DST" -type f | wc -l)
            ok "addons/$addon/ copied ($NEW_COUNT files)"
        fi
        COPY_COUNT=$((COPY_COUNT + 1))
    fi
done

# Copy demo/ if not present
if [ -d "$TMP_DIR/repo/demo" ]; then
    if [ ! -d "$PROJECT_DIR/demo" ]; then
        cp -r "$TMP_DIR/repo/demo" "$PROJECT_DIR/demo"
        ok "demo/ copied (example scene)"
    else
        info "demo/ already exists, skipping"
    fi
fi

if [ "$COPY_COUNT" -eq 0 ]; then
    fail "No addon directories were copied. The repository may have changed structure."
fi

# ---- Phase 5: Merge .csproj ----
phase 5 "Merging NuGet dependencies into $CSPROJ_NAME"

MARKER="GorgePlugin: added by installer"

csproj_has() {
    grep -q "$1" "$CSPROJ_FILE" 2>/dev/null
}

csproj_add_after() {
    # Insert $2 after the first line matching $1, with marker comment
    local pattern="$1"
    local insert="$2"
    local tmp="${CSPROJ_FILE}.tmp"

    awk -v pat="$pattern" -v ins="$insert" -v marker="$MARKER" '
    BEGIN { done = 0 }
    {
        print
        if (!done && $0 ~ pat) {
            print "    <!-- " marker " -->"
            print ins
            done = 1
        }
    }
    ' "$CSPROJ_FILE" > "$tmp" && mv "$tmp" "$CSPROJ_FILE"
}

CHANGED_CSPROJ=false

# EnableDynamicLoading
if ! csproj_has "<EnableDynamicLoading>true</EnableDynamicLoading>"; then
    csproj_add_after "<PropertyGroup>" "    <EnableDynamicLoading>true</EnableDynamicLoading>"
    ok "Added EnableDynamicLoading"
    CHANGED_CSPROJ=true
else
    info "EnableDynamicLoading already present"
fi

# AllowUnsafeBlocks
if ! csproj_has "<AllowUnsafeBlocks>true</AllowUnsafeBlocks>"; then
    csproj_add_after "<PropertyGroup>" "    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>"
    ok "Added AllowUnsafeBlocks"
    CHANGED_CSPROJ=true
else
    info "AllowUnsafeBlocks already present"
fi

# TargetFramework (check PropertyGroup area)
if ! csproj_has "<TargetFramework>net8.0</TargetFramework>" && ! csproj_has "<TargetFramework>net9.0</TargetFramework>"; then
    csproj_add_after "<PropertyGroup>" "    <TargetFramework>net8.0</TargetFramework>"
    ok "Added TargetFramework net8.0"
    CHANGED_CSPROJ=true
else
    info "TargetFramework already present"
fi

# NuGet packages
declare -A NUGET_PACKAGES=(
    ["Antlr4.Runtime.Standard"]="4.13.1"
    ["Newtonsoft.Json"]="13.0.3"
    ["QuikGraph"]="2.5.0"
    ["SharpZipLib"]="1.4.2"
)

# Ensure there's an ItemGroup for packages
if ! csproj_has "<ItemGroup>"; then
    # Add empty ItemGroup before </Project>
    sed_inplace 's|</Project>|  <ItemGroup>\n  </ItemGroup>\n</Project>|' "$CSPROJ_FILE"
fi

for pkg in "${!NUGET_PACKAGES[@]}"; do
    ver="${NUGET_PACKAGES[$pkg]}"
    if csproj_has "Include=\"$pkg\""; then
        if csproj_has "Include=\"$pkg\" Version=\"$ver\""; then
            info "Package $pkg $ver already referenced"
        else
            warn "Package $pkg is referenced but with a different version (expected $ver). Version mismatch may cause runtime errors."
        fi
    else
        # Insert before the first </ItemGroup> that contains PackageReference or before </Project>
        if grep -q '<PackageReference' "$CSPROJ_FILE"; then
            # Insert before first </ItemGroup> after the last PackageReference
            sed_inplace "/<PackageReference.*\/>/a\    <!-- $MARKER -->\n    <PackageReference Include=\"$pkg\" Version=\"$ver\" />" "$CSPROJ_FILE"
        else
            # Insert into first ItemGroup
            sed_inplace "s|<ItemGroup>|<ItemGroup>\n    <!-- $MARKER -->\n    <PackageReference Include=\"$pkg\" Version=\"$ver\" />|" "$CSPROJ_FILE"
        fi
        ok "Added $pkg $ver"
        CHANGED_CSPROJ=true
    fi
done

if [ "$CHANGED_CSPROJ" = false ]; then
    info "No changes needed to $CSPROJ_NAME"
fi

# ---- Phase 6: Merge project.godot ----
phase 6 "Configuring project.godot"

GODOT_FILE="$PROJECT_DIR/project.godot"
CHANGED_GODOT=false

# Helper: check if a line exists in project.godot
godot_has() {
    grep -qF "$1" "$GODOT_FILE" 2>/dev/null
}

# Enable the gorgeplugin editor plugin
PLUGIN_GORGE="res://addons/gorgeplugin/plugin.cfg"

if grep -q '^\[editor_plugins\]' "$GODOT_FILE"; then
    info "editor_plugins section exists, merging..."
    ENABLED_LINE=$(grep '^enabled=' "$GODOT_FILE" || true)

    if [ -n "$ENABLED_LINE" ]; then
        if ! echo "$ENABLED_LINE" | grep -qF "$PLUGIN_GORGE"; then
            # Extract existing plugins, add gorgeplugin
            EXISTING=$(echo "$ENABLED_LINE" | sed 's/enabled=PackedStringArray(//;s/)//')
            ALL_PARTS=()
            while IFS= read -r part; do
                part=$(echo "$part" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')
                if [ -n "$part" ]; then
                    ALL_PARTS+=("$part")
                fi
            done < <(echo "$EXISTING" | tr ',' '\n')

            ALL_PARTS+=("\"$PLUGIN_GORGE\"")
            NEW_ENABLED="enabled=PackedStringArray($(IFS=,; echo "${ALL_PARTS[*]}"))"
            sed_inplace "s|^enabled=.*|$NEW_ENABLED|" "$GODOT_FILE"
            ok "Added gorgeplugin to editor_plugins"
            CHANGED_GODOT=true
        else
            info "gorgeplugin already enabled"
        fi
    else
        sed_inplace "/^\[editor_plugins\]/a\enabled=PackedStringArray(\"$PLUGIN_GORGE\")" "$GODOT_FILE"
        ok "Added editor_plugins enabled line"
        CHANGED_GODOT=true
    fi
else
    {
        echo ""
        echo "[editor_plugins]"
        echo "enabled=PackedStringArray(\"$PLUGIN_GORGE\")"
    } >> "$GODOT_FILE"
    ok "Added editor_plugins section"
    CHANGED_GODOT=true
fi

if [ "$CHANGED_GODOT" = false ]; then
    info "No changes needed to project.godot"
fi

# ---- Phase 7: Build Rust Extension ----
phase 7 "Building Rust GDExtension"

RUST_DIR="$PROJECT_DIR/addons/nine_slice_sprite_2d_godot2d"

if [ ! -d "$RUST_DIR" ]; then
    skip "Rust addon directory not found, skipping build"
elif [ "$HAS_CARGO" = false ]; then
    skip "cargo not installed, skipping Rust build"
    info "  Build manually: cd addons/nine_slice_sprite_2d_godot2d && cargo build && cargo build --release"
else
    cd "$RUST_DIR"
    BUILD_OK=true

    info "Building debug..."
    if cargo build 2>&1; then
        ok "Rust debug build succeeded"
    else
        warn "Rust debug build failed"
        BUILD_OK=false
    fi

    info "Building release..."
    if cargo build --release 2>&1; then
        ok "Rust release build succeeded"

        case "$(uname -s)" in
            Linux)   LIB="target/release/libnine_slice_sprite_2d.so" ;;
            Darwin)  LIB="target/release/libnine_slice_sprite_2d.dylib" ;;
            MINGW*|MSYS*|CYGWIN*) LIB="target/release/nine_slice_sprite_2d.dll" ;;
            *)       LIB="" ;;
        esac
        if [ -n "$LIB" ] && [ -f "$LIB" ]; then
            ok "Output: $RUST_DIR/$LIB"
        fi
    else
        warn "Rust release build failed"
        BUILD_OK=false
    fi

    if [ "$BUILD_OK" = false ]; then
        warn "Some Rust builds failed. Build manually: cd $RUST_DIR && cargo build && cargo build --release"
    fi
    cd "$PROJECT_DIR"
fi

# ---- Phase 8: Restore NuGet ----
phase 8 "Restoring NuGet packages"

cd "$PROJECT_DIR"
if dotnet restore 2>&1; then
    ok "NuGet packages restored"
else
    warn "dotnet restore failed. Run manually: dotnet restore"
fi

# ---- Phase 9: Report ----
phase 9 "Installation complete"

echo ""
echo -e "${BOLD}========================================${NC}"
echo -e "${BOLD} GorgePluginGodot Installation Complete!${NC}"
echo -e "${BOLD}========================================${NC}"
echo ""
echo -e "  ${GREEN}[x]${NC} addons/gorgeplugin/               - Gorge chart player (C#)"
echo -e "  ${GREEN}[x]${NC} addons/nine_slice_sprite_2d_godot2d/ - Nine-slice GDExtension"
echo -e "  ${GREEN}[x]${NC} demo/                             - Example demo scene"
echo -e "  ${GREEN}[x]${NC} NuGet packages added              - Antlr4, Newtonsoft.Json, QuikGraph, SharpZipLib"
echo -e "  ${GREEN}[x]${NC} project.godot configured          - gorgeplugin enabled"
if [ "$HAS_CARGO" = true ] && [ -d "$RUST_DIR" ]; then
    echo -e "  ${GREEN}[x]${NC} Rust GDExtension built           - NineSliceSprite2D ready"
else
    echo -e "  ${YELLOW}[-]${NC} Rust GDExtension skipped         - Build manually if needed"
fi
echo ""
echo -e "${BOLD}Next steps:${NC}"
echo "  1. Open your project in Godot 4.6 (.NET edition)"
echo "  2. Build the C# solution (Godot will prompt, or: dotnet build)"
echo "  3. Enable plugins: Project Settings -> Plugins"
echo "  4. Add a GamePlayer node: Ctrl+A -> search 'GamePlayer'"
echo ""

if [ "$WARNINGS" -gt 0 ]; then
    echo -e "${YELLOW}${WARNINGS} warning(s) were reported. Review the output above.${NC}"
fi

exit 0
