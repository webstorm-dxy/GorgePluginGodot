# Coordinate System & Rendering

This document explains how the Gorge game engine's coordinate system maps to Godot's screen space, and how the adaptor layer renders Gorge content.

## Coordinate Spaces

### Gorge Coordinate System

Gorge uses a 3D coordinate system (`Vector3`) with the following characteristics:
- **16:10 aspect ratio** base (logical canvas)
- **Y-up** convention (positive Y goes upward)
- **Z** is used for depth/z-index ordering

### Godot 2D Coordinate System

Godot's 2D space uses:
- **Pixel-based** coordinates
- **Y-down** convention (positive Y goes downward)
- **Z** is used for z-index/draw order

## Core Transformation Functions

The coordinate transformation is implemented in `GodotSprite.cs` via extension methods. The base formula is:

```
godotX = gorgeX × (viewportWidth / 16)
godotY = -gorgeY × (viewportHeight / 10)
```

Note the **Y-axis inversion** (`-gorgeY`): Gorge uses Y-up, Godot uses Y-down.

### Position Transformation

```csharp
// CoverGorgeCoodinateToGodot
public static Vector2 CoverGorgeCoodinateToGodot(this Vector2 vector2)
{
    var scaleX = GamePlayer.viewportSize.X / 16f;
    var scaleY = GamePlayer.viewportSize.Y / 10f;
    var x = vector2.x * scaleX;
    var y = -vector2.y * scaleY;
    return new Vector2(x, y);
}
```

### Size Transformation

```csharp
// CoverGorgeSizeToGodot
public static Vector2 CoverGorgeSizeToGodot(this Vector2 vector2)
{
    var scaleX = GamePlayer.viewportSize.X / 16f;
    var scaleY = GamePlayer.viewportSize.Y / 10f;
    return new Vector2(vector2.x * scaleX, vector2.y * scaleY);
}
```

Size transformation does NOT invert Y — it just scales, since sizes are always positive.

### Scale Transformation (Special Case)

The `GodotSprite.SetScale()` method uses a different scaling approach:

```csharp
public void SetScale(Vector3 scale)
{
    Scale = ConvertGorgeToGodot(new Vector2(scale.x, scale.y));
}

private Vector2 ConvertGorgeToGodot(Vector2 gorgeScale)
{
    var x = 256f;
    var y = 160f;
    var scaleX = gorgeScale.X * (GamePlayer.viewportSize.X / x);
    var scaleY = gorgeScale.Y * (GamePlayer.viewportSize.Y / y);
    return new Vector2(scaleX, scaleY);
}
```

This uses a **256×160** logical base resolution for scale transforms, different from the 16×10 base for position transforms.

## Rotation

Rotation is straightforward — Gorge's Z-rotation is negated for Godot's clockwise convention:

```csharp
public void SetRotation(Vector3 rotation)
{
    RotationDegrees = -rotation.z;
}
```

## Z-Index / Depth

Gorge's Z coordinate maps directly to Godot's `ZIndex`:

```csharp
ZIndex = (int)position.z;
```

This ensures sprites render in the correct draw order.

## Rendering Pipeline

### Scene Tree Structure

The `GamePlayer` node creates this rendering tree during `InitializeRenderRoot()`:

```
GamePlayer (Node)
├── Node2D (_playerRoot)          # Root for all Gorge content
│   └── Camera2D (_camera)       # All sprites are added as children here
│       ├── Sprite2D              # Regular sprites
│       ├── NineSliceSprite2D     # Nine-slice sprites
│       ├── Line2D                # Curve sprites
│       └── AudioStreamPlayer     # Audio players (added via DeferredCall)
```

All visual elements created by the Gorge runtime are added as children of the `_camera` node (stored as `GamePlayer.Instance`).

### Sprite Types

#### GodotSprite (Regular Sprite)
- Extends `Sprite2D` and implements `ISprite`
- Renders with `Texture`, `Position`, `Scale`, `RotationDegrees`, `Modulate`
- Position/size/scale all go through the coordinate transforms described above

#### GodotNineSliceSprite (Nine-Slice)
- Wraps the Rust `NineSliceSprite2D` GDExtension node
- Configured via `Set()` calls (no C# bindings for the Rust type)
- Uses `base_size` + `patch_margin_*` properties for Gorge-compatible scaling
- Lazily creates the Rust node on first `SetGraph()` call or `_Ready()`

#### GodotCurveSprite (Line/Curve)
- Extends `Line2D` and implements `ICurveSprite`
- Points are set via `SetLine(ObjectArray pointArray)` which converts each Gorge `Vector3` to a Godot `Vector2` point
- Color is set via `DefaultColor`

### Graph (Texture) Rendering

Gorge's `Graph` type wraps a Godot `Texture2D`:

```csharp
public class GodotGraph : Graph
{
    private readonly Texture2D _graph;
    
    public GodotGraph(Texture2D graph)
    {
        _graph = graph;
        width = _graph.GetWidth();
        height = _graph.GetHeight();
    }
}
```

Graphs are created from PNG byte data in `GodotBase.CreateGraph()`:

```csharp
public Graph CreateGraph(string assetFilePath, byte[] data)
{
    var image = new Image();
    image.LoadPngFromBuffer(data);
    Texture2D texture = ImageTexture.CreateFromImage(image);
    return new GodotGraph(texture);
}
```

### Color Conversion

Gorge's `ColorArgb` (floats in 0-1 range) maps directly to Godot's `Color`:

```csharp
public void SetColor(ColorArgb color)
{
    Modulate = new Color(color.r, color.g, color.b, color.a);
}
```

## Audio Rendering

Audio is handled by `GodotAudioPlayer` and `GodotAudioEffectPlayer`:

1. WAV data is validated by checking the RIFF header
2. `AudioStreamWav.LoadFromBuffer(data)` creates the stream
3. An `AudioStreamPlayer` node is created and added to the scene tree
4. Playback is controlled via `Play()` / `Stop()` / `Seek()`

Audio players are added as children of `GamePlayer.Instance` (the camera node) via `CallDeferred("add_child", _player)`.

## Simulation Loop

The Gorge simulation is driven in `GamePlayer._Process()`:

```csharp
public override void _Process(double delta)
{
    if (IsPlaying && RuntimeStatic.Runtime?.SimulationRuntime?.Simulation?.SimulationMachine != null)
    {
        RuntimeStatic.Runtime.SimulationRuntime.Simulation.SimulationMachine.Drive((float)delta);
    }
}
```

Each frame, the simulation machine advances by `delta` seconds. The simulation calls into the adaptor layer (`IGorgeFrameworkBase`) which creates/destroys/updates sprites, plays audio, and renders effects.

## Viewport & Resolution

- Default viewport: 1920×1200 (configurable in Project Settings)
- `GamePlayer.viewportSize` is set from `GetViewport().GetVisibleRect().Size` during initialization
- All coordinate transforms use this viewport size, so the rendering automatically adapts to window resizes
