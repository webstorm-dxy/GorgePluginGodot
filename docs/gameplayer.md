# GamePlayer API Reference

`GamePlayer` is the core node type provided by the GorgePlugin addon. It is a custom Godot node (registered as `GamePlayer` extending `Node`) that loads and runs Gorge game engine chart packages.

## Class Hierarchy

```
Node
 └── GamePlayer (GorgePlugin.addons.gorgeplugin)
```

## Signals

### PackagesChanged

```gdscript
signal PackagesChanged()
```

Emitted whenever the package list is modified — adding/removing/clearing packages. At this point the runtime is marked as dirty and will be re-prepared on the next `play_chart()` call.

### ChartPrepared

```gdscript
signal ChartPrepared()
```

Emitted after `prepare_runtime()` successfully completes — packages are loaded, the simulation runtime is created, and the score is loaded. The chart is ready to play but not yet started.

### ChartStarted

```gdscript
signal ChartStarted()
```

Emitted when `play_chart()` successfully starts the simulation. At this point `IsPlaying` is `true`.

### ChartStopped

```gdscript
signal ChartStopped(reason: String)
```

Emitted when the chart stops. The `reason` parameter can be:

| Reason | Trigger |
|--------|---------|
| `"stopped"` | User called `stop_chart()` |
| `"restart"` | User called `restart_chart()` |
| `"terminated"` | Simulation reached its terminate time |
| `"packages_changed"` | Packages were modified while playing |
| `"window_close"` | Application window is closing |

### PlaybackStateChanged

```gdscript
signal PlaybackStateChanged(isPlaying: bool)
```

Emitted whenever playback toggles between playing and stopped.

### PlayerError

```gdscript
signal PlayerError(message: String)
```

Emitted when an error occurs in any operation. The error is also pushed to Godot's error log via `GD.PushError()`.

## Exported Properties

### RuntimePackagePaths

```gdscript
@export var RuntimePackagePaths: Array[String] = ["res://addons/gorgeplugin/Native.zip"]
```

Array of paths to runtime `.zip` package files. At least one runtime package is required. The default path points to the bundled `Native.zip`.

### ChartPackagePaths

```gdscript
@export var ChartPackagePaths: Array[String] = []
```

Array of paths to chart `.zip` package files. At least one chart package is required to play.

### AutoStartOnReady

```gdscript
@export var AutoStartOnReady: bool = false
```

If `true`, `play_chart()` is called automatically during `_ready()`. Disabled by default so you can configure packages before playback.

### TerminateTime

```gdscript
@export var TerminateTime: float = 135.0
```

The simulation terminate timeout in seconds. When the simulation clock reaches this value, the `OnRuntimeTerminated` callback fires and the chart stops automatically. Set to a high value for longer charts.

### GorgeAutoPlay

```gdscript
@export var GorgeAutoPlay: bool = true
```

Passed through to the Gorge runtime's `StaticConfig.IsAutoPlay`. Controls whether the chart auto-plays when the simulation starts.

## Public Methods

### play_chart()

```gdscript
func play_chart() -> bool
```

Starts chart playback. If the runtime hasn't been prepared (or is dirty from package changes), calls `prepare_runtime()` first. Emits `ChartStarted` on success.

Returns `false` if preparation or simulation start fails.

### stop_chart(reason: String = "stopped")

```gdscript
func stop_chart(reason: String = "stopped") -> bool
```

Stops the currently running chart. Emits `ChartStopped` if a chart was actually playing. Safe to call even when nothing is playing.

### restart_chart()

```gdscript
func restart_chart() -> bool
```

Stops playback (with reason `"restart"`) then immediately starts again. Equivalent to calling `stop_chart("restart")` followed by `play_chart()`.

### prepare_runtime()

```gdscript
func prepare_runtime() -> bool
```

Loads all packages, creates the simulation runtime, extracts simulation resources, creates the simulation, and loads the score — but does NOT start playback. Emits `ChartPrepared` on success.

Call this when you want to pre-load before playing, or when you want to swap packages without interrupting playback (the method will stop playback first if running).

### add_package_path(path: String, is_chart: bool)

```gdscript
func add_package_path(path: String, is_chart: bool) -> bool
```

Adds a package file by path. If `is_chart` is `true`, the package is treated as a chart package; otherwise, it's a runtime package. Stops playback if currently playing, then marks the runtime as dirty.

Returns `false` if the path is empty or whitespace.

### add_runtime_package_path(path: String)

```gdscript
func add_runtime_package_path(path: String) -> bool
```

Shorthand for `add_package_path(path, false)`.

### add_chart_package_path(path: String)

```gdscript
func add_chart_package_path(path: String) -> bool
```

Shorthand for `add_package_path(path, true)`.

### add_package(package: Package)

```gdscript
func add_package(package: Package) -> bool
```

Adds a pre-constructed `Package` object directly (instead of loading from a file path). This is for advanced use cases where you construct packages programmatically. Returns `false` if the package is `null`.

### clear_packages()

```gdscript
func clear_packages() -> void
```

Removes all packages (runtime, chart, and manually-added), then emits `PackagesChanged`. Stops playback first if running.

### request_play() / request_stop()

```gdscript
func request_play() -> void
func request_stop() -> void
```

Convenience wrappers around `play_chart()` and `stop_chart()`. These exist to be connected directly to button `pressed` signals without needing intermediate methods.

### GDScript-compatible aliases

These snake_case aliases are provided for GDScript callers:

| PascalCase | snake_case |
|-----------|-----------|
| `AddPackagePath()` | `add_package_path()` |
| `AddRuntimePackagePath()` | `add_runtime_package_path()` |
| `AddChartPackagePath()` | `add_chart_package_path()` |
| `ClearPackages()` | `clear_packages()` |
| `PrepareRuntime()` | `prepare_runtime()` |
| `PlayChart()` | `play_chart()` |
| `StopChart()` | `stop_chart()` |
| `RestartChart()` | `restart_chart()` |
| `RequestPlay()` | `request_play()` |
| `RequestStop()` | `request_stop()` |

## Static Members

### IsPlaying

```gdscript
static var IsPlaying: bool = false
```

Global playing state. `true` when any `GamePlayer` instance has an active simulation. Check this before starting a new chart or to guard against duplicate playback.

### Instance

```gdscript
static var Node: Instance
```

The camera `Node2D` child of the most recently initialized `GamePlayer`. The Gorge adaptors (`GodotBase.CreateSprite()`, `GodotBase.CreateNineSliceSprite()`, etc.) add their rendered nodes as children of this camera node.

### viewportSize

```gdscript
static var viewportSize: Vector2
```

Cached viewport dimensions, set during `InitializeRenderRoot()`. Used by coordinate transformation functions in the sprite adaptor.

### SizeScale

```gdscript
static var SizeScale: float = 80.0
```

Global scale factor for coordinate transforms. Used in the sprite scale calculation path.

## Internal Lifecycle

```
_Ready()
  ├── InitializeRenderRoot()    # Creates Node2D + Camera2D tree
  │     ├── Base.Instance = new GodotBase()
  │     ├── _playerRoot = new Node2D
  │     └── _camera = new Camera2D
  ├── ApplyStaticConfig()       # Sets TerminateTime, IsAutoPlay
  └── (if AutoStartOnReady) PlayChart()

Packages flow:
  load(RuntimePackagePaths[i]) ──┐
  load(ChartPackagePaths[i])  ──┤
  Package objects             ──┘
        │
        ▼
  RuntimeManager.CreateLanguageRuntime(Packages)
  RuntimeManager.ExtractSimulationResources()
  RuntimeManager.CreateSimulationRuntime(callback)
  RuntimeManager.LoadScore()
        │
        ▼
  RuntimeManager.StartSimulation()
        │
        ▼
  _Process(delta) drives SimulationMachine.Drive(delta)
```

## Error Handling

All public methods catch exceptions and:
1. Call `ReportError()` which pushes to `GD.PushError()` AND emits `PlayerError`
2. Return `false` (or void) on failure

Check the Godot Output panel or connect to the `PlayerError` signal to monitor errors.

## Example: Full Scene Script

```gdscript
extends Control

@export var runtime_package_path: String = "res://Dremu.zip"
@export var chart_package_path: String = "res://DremuTest.zip"

@onready var player = $GamePlayer
@onready var status_label = $StatusLabel

func _ready():
    # Wire up signals
    player.PackagesChanged.connect(_on_packages_changed)
    player.ChartPrepared.connect(_on_chart_prepared)
    player.ChartStarted.connect(_on_chart_started)
    player.ChartStopped.connect(_on_chart_stopped)
    player.PlaybackStateChanged.connect(_on_playback_changed)
    player.PlayerError.connect(_on_error)

    # Load packages
    player.add_runtime_package_path(runtime_package_path)
    player.add_chart_package_path(chart_package_path)

func _on_packages_changed():
    status_label.text = "Packages loaded, runtime dirty"

func _on_chart_prepared():
    status_label.text = "Chart prepared"

func _on_chart_started():
    status_label.text = "Playing..."

func _on_chart_stopped(reason: String):
    status_label.text = "Stopped: %s" % reason

func _on_playback_changed(is_playing: bool):
    pass  # Update UI state

func _on_error(message: String):
    status_label.text = "ERROR: %s" % message
```
