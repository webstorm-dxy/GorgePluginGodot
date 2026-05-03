# GorgeGodotPlugin

A Godot 4 plugin that enables playback of [Gorge](https://gorge.icu) game engine charts within Godot. Drop a chart package and a runtime package into your project, add a `GamePlayer` node, and play.

## Architecture

The plugin consists of three addons:

| Addon | Language | Purpose |
|-------|----------|---------|
| `gorgeplugin` | C# | Core chart player: loads Gorge packages, manages simulation lifecycle, renders sprites/audio/graphs via Godot adaptors |
| `godot_mcp` | GDScript | MCP (Model Context Protocol) server integration — enables AI-assisted scene editing, debugging, and asset generation from within the Godot editor |
| `nine_slice_sprite_2d_godot2d` | Rust | GDExtension providing a `NineSliceSprite2D` node with Gorge-compatible nine-slice rendering |

## Prerequisites

- **Godot 4.6** (with .NET/Mono support)
- **.NET SDK 8.0** (9.0 for Android builds)
- **Rust** (optional — only needed to build the nine-slice GDExtension from source)

## Installation

### 1. Copy the addons folder

Copy the `addons/` folder into your Godot project's `res://` directory.

### 2. Configure C# project dependencies

Add the following NuGet dependencies to your `.csproj` file (**version numbers must match** or GorgePlugin may not work correctly):

```xml
<Project Sdk="Godot.NET.Sdk/4.6.2">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
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

> **Important:**
> - The `Godot.NET.Sdk` version must match your Godot editor version (this plugin uses **4.6.2**)
> - `EnableDynamicLoading` must be set to `true`
> - If your existing project has **different versions** of these packages, they may conflict with the Gorge framework DLLs referenced internally by GorgePlugin, causing `MissingMethodException` or `FileLoadException`. **Keep the versions consistent, or use assembly binding redirects**
> - For Android builds, add `<TargetFramework Condition=" '$(GodotTargetPlatform)' == 'android' ">net9.0</TargetFramework>`

### 3. Enable plugins and build

In Godot, go to **Project → Project Settings → Plugins** and enable both:
- **GorgePlugin** — adds the `GamePlayer` custom node type
- **Godot MCP** — optional, for AI-assisted workflows

Build the C# solution: open the solution in your IDE, or run `dotnet build`.

### 4. (Optional) Build the Rust nine-slice extension

```bash
cd addons/nine_slice_sprite_2d_godot2d
cargo build --release
```

## Quick Start

### 1. Create a scene with GamePlayer

Add a `GamePlayer` node to your scene. It appears as a custom node type in the "Create New Node" dialog under the name **GamePlayer**.

### 2. Configure packages

Set the following exported properties on the `GamePlayer` node:

- **ChartPackagePaths** — array of chart `.zip` package paths
- **RuntimePackagePaths** — array of runtime `.zip` package paths (defaults to `res://addons/gorgeplugin/Native.zip`)
- **AutoStartOnReady** — optionally start playback automatically when the scene loads
- **TerminateTime** — simulation terminate time in seconds (default: 135.0)
- **GorgeAutoPlay** — whether the chart auto-plays (default: true)

### 3. Use the API

```gdscript
# Assuming you have a GamePlayer node
@onready var player = $GamePlayer

func _ready():
    # Add packages programmatically
    player.add_runtime_package_path("res://MyRuntime.zip")
    player.add_chart_package_path("res://MyChart.zip")

    # Listen for events
    player.connect("ChartStarted", Callable(self, "_on_chart_started"))
    player.connect("ChartStopped", Callable(self, "_on_chart_stopped"))
    player.connect("PlayerError", Callable(self, "_on_player_error"))
    player.connect("PlaybackStateChanged", Callable(self, "_on_playback_changed"))
    player.connect("PackagesChanged", Callable(self, "_on_packages_changed"))
    player.connect("ChartPrepared", Callable(self, "_on_chart_prepared"))

    # Start playback
    player.play_chart()

func _on_chart_started():
    print("Chart is now playing")

func _on_chart_stopped(reason: String):
    print("Chart stopped: ", reason)

func _on_player_error(message: String):
    push_error("Player error: ", message)
```

### 4. Run the demo

Open `demo/game_player_demo.tscn` and press F5. The demo scene provides Play/Stop buttons and a status display. It expects:
- `res://Dremu.zip` — a runtime package
- `res://DremuTest.zip` — a chart package

## GamePlayer API Reference

### Signals

| Signal | Parameters | Description |
|--------|-----------|-------------|
| `PackagesChanged` | — | Emitted when package lists are modified |
| `ChartPrepared` | — | Emitted after runtime is prepared and score is loaded |
| `ChartStarted` | — | Emitted when the chart begins playing |
| `ChartStopped` | `reason: String` | Emitted when the chart stops (reason: "stopped", "restart", "terminated", "packages_changed", "window_close") |
| `PlaybackStateChanged` | `isPlaying: bool` | Emitted when playback state toggles |
| `PlayerError` | `message: String` | Emitted on errors |

### Properties (Exported)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RuntimePackagePaths` | `Array[String]` | `["res://addons/gorgeplugin/Native.zip"]` | Runtime package zip paths |
| `ChartPackagePaths` | `Array[String]` | `[]` | Chart package zip paths |
| `AutoStartOnReady` | `bool` | `false` | Auto-play on scene ready |
| `TerminateTime` | `float` | `135.0` | Simulation terminate timeout |
| `GorgeAutoPlay` | `bool` | `true` | Auto-play setting passed to the runtime |

### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `play_chart()` | `bool` | Start chart playback. Auto-prepares runtime if needed. |
| `stop_chart(reason: String = "stopped")` | `bool` | Stop chart playback. |
| `restart_chart()` | `bool` | Stop then immediately restart the chart. |
| `prepare_runtime()` | `bool` | Load packages and initialize the simulation runtime without playing. |
| `add_package_path(path: String, is_chart: bool)` | `bool` | Add a package by path. |
| `add_runtime_package_path(path: String)` | `bool` | Add a runtime package path. |
| `add_chart_package_path(path: String)` | `bool` | Add a chart package path. |
| `add_package(package: Package)` | `bool` | Add a pre-loaded Package object. |
| `clear_packages()` | `void` | Remove all packages and stop playback. |
| `request_play()` | `void` | Convenience wrapper — calls `play_chart()`. |
| `request_stop()` | `void` | Convenience wrapper — calls `stop_chart()`. |

### Static Members

| Member | Description |
|--------|-------------|
| `GamePlayer.IsPlaying` | `bool` — whether any chart is currently playing |
| `GamePlayer.Instance` | `Node` — the camera node for sprite rendering (used by adaptors) |
| `GamePlayer.viewportSize` | `Vector2` — cached viewport dimensions |
| `GamePlayer.SizeScale` | `float` — coordinate scale factor (default: 80.0) |

## GodotMCP Addon (AI Assistant Integration)

The `godot_mcp` addon connects the Godot editor to an AI assistant via WebSocket. It exposes tools that let an AI agent:

- Create, read, and edit `.tscn` scene files
- Modify node properties, attach scripts, set collision shapes, meshes, and materials
- Manage project settings, input maps, autoloads, and collision layers
- Create scripts, validate syntax, search across the codebase
- Generate 2D assets from SVG code
- Run scenes, capture screenshots, simulate input, and query runtime nodes
- Visualize project structure (`map_project`, `map_scenes`)

When enabled, the plugin registers the `MCPRuntime` autoload in your project so the running game can communicate with the MCP server for runtime operations.

## NineSliceSprite2D GDExtension

A Rust-based GDExtension that adds the `NineSliceSprite2D` node. It renders nine-slice textures with the following features:

- Configurable patch margins (left, top, right, bottom)
- Axis stretch modes (Stretch, Tile, TileFit)
- Gorge-compatible base-size-based scaling (when `base_size` is set, uses custom nine-slice algorithm matching Gorge's rendering)
- Frame-based sprite sheet support with `hframes`/`vframes`
- Flip and offset controls

The Gorge plugin's `GodotNineSliceSprite` adaptor creates and configures this node at runtime to match Gorge's nine-slice rendering behavior.

## Coordinate System

Gorge uses a coordinate system based on a 16:10 aspect ratio. The `GodotSprite` adaptor transforms Gorge coordinates to Godot screen coordinates:

```
godotX = gorgeX * (viewportWidth / 16)
godotY = -gorgeY * (viewportHeight / 10)
```

Note the Y-axis inversion — Gorge uses Y-up, Godot uses Y-down.

## Project Structure

```
GorgeGodotPlugin/
├── addons/
│   ├── gorgeplugin/                  # Gorge chart player (C#)
│   │   ├── plugin.cfg
│   │   ├── gorgeplugin.cs            # Editor plugin registration
│   │   ├── GamePlayer.cs             # Core player node
│   │   ├── GodotAdaptor/
│   │   │   ├── GodotBase.cs          # IGorgeFrameworkBase implementation
│   │   │   ├── GodotSprite.cs        # Sprite/curve/nine-slice adaptors
│   │   │   ├── GodotAudio.cs         # Audio playback adaptor
│   │   │   └── TestAdapterIntegration.cs
│   │   └── Native.zip                # Default runtime package
│   ├── godot_mcp/                    # MCP server integration (GDScript)
│   │   ├── plugin.cfg
│   │   ├── plugin.gd                 # Editor plugin
│   │   ├── mcp_client.gd             # WebSocket client
│   │   ├── tool_executor.gd          # Tool routing
│   │   ├── tools/
│   │   │   ├── scene_tools.gd        # Scene CRUD operations
│   │   │   ├── file_tools.gd         # File listing/reading/search
│   │   │   ├── script_tools.gd       # Script editing/validation
│   │   │   ├── project_tools.gd      # Project settings/debugging
│   │   │   ├── asset_tools.gd        # SVG→PNG asset generation
│   │   │   └── visualizer_tools.gd   # Project mapping & visualization
│   │   ├── runtime/
│   │   │   └── mcp_runtime.gd        # In-game MCP autoload
│   │   └── utils/
│   │       ├── paths.gd
│   │       └── variant_codec.gd
│   └── nine_slice_sprite_2d_godot2d/ # Rust GDExtension
│       ├── Cargo.toml
│       ├── nine_slice_sprite_2d.gdextension
│       └── src/
│           ├── lib.rs
│           └── nine_slice_sprite_2d.rs
├── demo/
│   ├── game_player_demo.tscn         # Demo scene
│   └── game_player_demo.gd           # Demo script
├── docs/                             # Documentation
├── project.godot                     # Project configuration
├── GorgeGodotPlugin.csproj           # C# project file
└── GorgeGodotPlugin.sln              # Solution file
```

## Dependencies

### NuGet Packages (C#)

- `Antlr4.Runtime.Standard` 4.13.1
- `Newtonsoft.Json` 13.0.3
- `QuikGraph` 2.5.0
- `SharpZipLib` 1.4.2

### Cargo Crates (Rust)

- `godot` 0.4.5

### Godot Project Settings

- Rendering method: Mobile
- Physics engine: Jolt Physics
- Default window: 1920×1200

## License

This project includes code from multiple sources:
- **GorgePlugin** — authored by HPCG
- **GodotMCP** — authored by Godot MCP
- **NineSliceSprite2D** — MIT licensed

See individual `LICENSE` files in each addon directory for details.
