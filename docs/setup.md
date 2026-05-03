# Installation & Setup Guide

## Prerequisites

### Required

| Software | Version | Notes |
|----------|---------|-------|
| Godot Engine | 4.6+ | Must be the .NET/Mono variant |
| .NET SDK | 8.0 | 9.0 for Android export builds |
| Git LFS | 2.x+ | Project uses Git LFS for `.zip` files |

### Optional

| Software | Version | Purpose |
|----------|---------|---------|
| Rust toolchain | stable | Build the NineSliceSprite2D GDExtension |
| MCP Server | latest | AI-assisted editing via GodotMCP |
| Visual Studio / Rider | any | C# IDE for editing plugin code |

## Step-by-Step Installation

### 1. Clone the Repository

```bash
git clone <repo-url> GorgeGodotPlugin
cd GorgeGodotPlugin
```

Make sure Git LFS pulled the large files:

```bash
git lfs pull
```

This ensures `Dremu.zip`, `DremuTest.zip`, and `Native.zip` are complete.

### 2. Open in Godot

Launch Godot 4.6 (.NET variant) and import the project:

1. Open Godot
2. Click **Import** (or **Scan** if the project folder is in your project list)
3. Navigate to the `GorgeGodotPlugin` folder
4. Select `project.godot` and click **Import & Edit**

### 3. Configure C# Project Dependencies

The GorgePlugin addon requires specific NuGet packages. Your `.csproj` file **must** include the following:

```xml
<Project Sdk="Godot.NET.Sdk/4.6.2">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <TargetFramework Condition=" '$(GodotTargetPlatform)' == 'android' ">net9.0</TargetFramework>
    <EnableDynamicLoading>true</EnableDynamicLoading>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Antlr4.Runtime.Standard" Version="4.13.1" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="QuikGraph" Version="2.5.0" />
    <PackageReference Include="SharpZipLib" Version="1.4.2" />
  </ItemGroup>
</Project>
```

> **关键配置说明：**
>
> | 配置项 | 必须值 | 原因 |
> |--------|--------|------|
> | `Godot.NET.Sdk` | `4.6.2` | 匹配 Godot 编辑器版本，版本不对会导致 API 不兼容 |
> | `EnableDynamicLoading` | `true` | Godot 插件必须以动态加载方式编译，否则编辑器无法加载 |
> | `Antlr4.Runtime.Standard` | `4.13.1` | Gorge 框架内部引用，版本不一致会触发 `FileLoadException` |
> | `Newtonsoft.Json` | `13.0.3` | Gorge 框架内部引用，版本不一致会导致 JSON 序列化失败 |
> | `QuikGraph` | `2.5.0` | Gorge 框架内部引用 |
> | `SharpZipLib` | `1.4.2` | Gorge 框架内部引用 |

> **⚠️ 版本冲突警告：** GorgePlugin 内部通过 `Native.zip` 中的 DLL 引用 Gorge 框架。如果你的项目使用了上述 NuGet 包的**不同版本**，运行时可能因程序集版本不匹配抛出 `MissingMethodException` 或 `FileLoadException`。**务必保持版本号一致**。如果无法统一版本，可在 `.csproj` 中添加 assembly binding redirect 或使用 `<AutoGenerateBindingRedirects>`。

### 4. Build the C# Solution

Godot will prompt you to build the C# solution on first open. If it doesn't:

**Option A: From Godot**
- **MSBuild** tab at the bottom of the editor → Click **Build**

**Option B: From terminal**
```bash
dotnet build GorgeGodotPlugin.csproj
```

### 5. Enable the Plugins

1. Go to **Project → Project Settings → Plugins**
2. Enable **GorgePlugin** — this registers the `GamePlayer` custom node type
3. Enable **Godot MCP** (optional) — this enables AI-assisted editing

The GorgePlugin should now show a checkmark. The editor toolbar should show the MCP status indicator.

### 6. (Optional) Build the Rust Extension

If you need the NineSliceSprite2D node for Gorge content that uses nine-slice rendering:

```bash
cd addons/nine_slice_sprite_2d_godot2d
cargo build --release
```

After building, restart the Godot editor so it loads the compiled `.dylib`/`.so`/`.dll`.

### 7. Verify Installation

1. Create a new scene
2. Press `Ctrl+A` to open the "Create New Node" dialog
3. Search for **GamePlayer** — it should appear under custom nodes
4. Add it to the scene to confirm it works

## Project Dependencies

### NuGet Packages

These are automatically restored when you build the C# project:

```
Antlr4.Runtime.Standard  4.13.1    — ANTLR grammar runtime
Newtonsoft.Json          13.0.3    — JSON serialization
QuikGraph                2.5.0     — Graph data structures
SharpZipLib              1.4.2     — ZIP archive handling
```

### Rust Crate

```
godot  0.4.5  — Godot Rust bindings for GDExtension
```

### Godot Project Settings

Key settings in `project.godot`:

```ini
[application]
config/name="GorgeGodotPlugin"
config/features=["4.6", "C#", "Mobile"]

[autoload]
MCPRuntime="*uid://bptem33yccaks"

[dotnet]
project/assembly_name="GorgeGodotPlugin"

[editor_plugins]
enabled=["res://addons/godot_mcp/plugin.cfg", "res://addons/gorgeplugin/plugin.cfg"]

[physics]
3d/physics_engine="Jolt Physics"

[rendering]
rendering_device/driver.windows="d3d12"
renderer/rendering_method="mobile"
```

The project uses:
- **Mobile** renderer (for performance)
- **Jolt Physics** for 3D physics
- **D3D12** render driver on Windows

## Using as a Submodule / Addon

To use this plugin in your own Godot project:

1. Copy these directories into your project's `addons/`:
   - `addons/gorgeplugin/`
   - `addons/nine_slice_sprite_2d_godot2d/` (if needed)
   - Optionally: `addons/godot_mcp/` (if you want AI-assisted editing)

2. Add the NuGet references to your own `.csproj`:
   ```xml
   <PackageReference Include="Antlr4.Runtime.Standard" Version="4.13.1" />
   <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
   <PackageReference Include="QuikGraph" Version="2.5.0" />
   <PackageReference Include="SharpZipLib" Version="1.4.2" />
   ```

3. Enable the plugins in Project Settings.

## Troubleshooting

### "GamePlayer" doesn't appear in the node list
- Make sure GorgePlugin is enabled in Project Settings → Plugins
- Make sure the C# solution built successfully (check the MSBuild tab or Output panel for errors)
- Check that `addons/gorgeplugin/GamePlayer.cs` exists and has no syntax errors

### "NineSliceSprite2D" is not available
- Build the Rust extension: `cd addons/nine_slice_sprite_2d_godot2d && cargo build --release`
- Check `.gdextension` file platform paths match your compiled library
- Restart the Godot editor after building

### Package loading fails
- Ensure `.zip` files are in the expected `res://` paths
- Check that Git LFS pulled the large files (`git lfs pull`)
- Verify the zip files are valid (not corrupted by LFS pointer files)

### MCP status shows "Disconnected"
- The MCP addon requires a running MCP server at `ws://127.0.0.1:6505`
- Without the server, it will keep trying to reconnect; this is harmless
- Disable the Godot MCP plugin if you don't need AI-assisted editing

### NuGet version mismatch errors

If you see errors like the following in Godot's Output panel:

```
FileLoadException: Could not load file or assembly 'Newtonsoft.Json, Version=13.0.0.0...'
MissingMethodException: Method not found: 'Void Newtonsoft.Json...'
```

This means your project uses a **different NuGet package version** than what GorgePlugin internally references. To fix:

1. Open your `.csproj` and set the package versions to **exactly match** the versions listed in step 3 above
2. Delete the `bin/` and `obj/` directories, then rebuild: `dotnet clean && dotnet build`
3. If you must use a different version, add this to your `.csproj`:
   ```xml
   <PropertyGroup>
     <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
   </PropertyGroup>
   ```

### Script errors about missing types
- Make sure the C# solution is built (`dotnet build`)
- Check that all NuGet packages are restored
- Verify your Godot version is 4.6+ with .NET support
- Check that `EnableDynamicLoading` is set to `true` in your `.csproj`
