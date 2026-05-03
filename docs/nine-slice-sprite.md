# NineSliceSprite2D GDExtension

A Rust-based Godot GDExtension that provides a `NineSliceSprite2D` node for rendering nine-slice (9-patch) textured sprites with Gorge-compatible scaling behavior.

## Overview

`NineSliceSprite2D` extends `Node2D` and renders a texture split into 9 regions (4 corners, 4 edges, 1 center). The corner regions are drawn at their original size, while the edge and center regions are stretched or tiled to fill the target area.

The Gorge plugin's `GodotNineSliceSprite` C# class creates and configures this node at runtime to match Gorge's nine-slice rendering pipeline.

## Node Properties

### Texture

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `texture` | `Texture2D` | `null` | The source texture to nine-slice render |

### Size

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `size` | `Vector2` | `(0, 0)` | Target rendering size in pixels |
| `base_size` | `Vector2` | `(0, 0)` | When set to non-zero, enables Gorge-compatible scaling mode |

### Patch Margins

Defines the boundary between corner/edge/center regions in the source texture.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `patch_margin_left` | `int` | `0` | Left margin in pixels |
| `patch_margin_top` | `int` | `0` | Top margin in pixels |
| `patch_margin_right` | `int` | `0` | Right margin in pixels |
| `patch_margin_bottom` | `int` | `0` | Bottom margin in pixels |

### Axis Stretch

Controls how the edge and center segments behave when scaling.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `axis_stretch_horizontal` | `AxisStretchMode` | `Stretch` | Horizontal stretch mode |
| `axis_stretch_vertical` | `AxisStretchMode` | `Stretch` | Vertical stretch mode |

`AxisStretchMode` values:
- `Stretch` (0) — Scale the segment linearly
- `Tile` (1) — Repeat the segment to fill the space
- `TileFit` (2) — Tile and clip to fit exactly

### Rendering Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `draw_center` | `bool` | `true` | Whether to draw the center region |
| `centered` | `bool` | `true` | If true, the texture is centered on the node's position |
| `offset` | `Vector2` | `(0, 0)` | Rendering offset from the node position |
| `flip_h` | `bool` | `false` | Flip horizontally |
| `flip_v` | `bool` | `false` | Flip vertically |

### Sprite Sheet (Frame Animation)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `hframes` | `int` | `1` | Number of horizontal frames in the sprite sheet |
| `vframes` | `int` | `1` | Number of vertical frames in the sprite sheet |
| `frame` | `int` | `0` | Current frame index (0-based, row-major) |
| `frame_coords` | `Vector2i` | `(0, 0)` | Current frame as grid coordinates (x=column, y=row) |
| `region_rect` | `Rect2` | `(0,0,0,0)` | Source region rectangle. When size is zero, defaults to `texture_size / (hframes, vframes)` |

## Signals

| Signal | Description |
|--------|-------------|
| `frame_changed()` | Emitted when the frame or frame coordinates change |
| `texture_changed()` | Emitted when the texture is changed |
| `resized()` | Emitted when the size changes |

## Two Rendering Modes

### Standard Mode (`base_size == (0,0)`)

Uses Godot's built-in `RenderingServer.canvas_item_add_nine_patch_ex()` for standard nine-slice rendering. The patch margins are used directly, and the target size is clamped to be at least as large as the sum of the corresponding margins.

### Gorge Mode (`base_size != (0,0)`)

When `base_size` is set to a non-zero value, the extension switches to a custom nine-slice algorithm (`draw_gorge_nine_slice()`) that matches Gorge's rendering behavior:

1. **Scaling logic**: The corner size is computed as a fraction of `base_size`, then those pixels are reserved in the target area. The remaining space is filled by the edge segments (stretched to fill, not tiled).

2. **Nine regions**: The source texture is divided into 9 segments using the patch margins as split points. Each segment is then mapped to a corresponding target segment and drawn via `canvas_item_add_texture_rect_region()`.

```
Source texture (with patch margins):
┌───┬───────┬───┐
│TL │   T   │TR │  TL = (0,0) to (left, top)
│   │       │   │  T  = stretched horizontally
├───┼───────┼───┤  TR = (right,0) to (width, top)
│   │       │   │
│ L │   C   │ R │  L  = stretched vertically
│   │       │   │  C  = stretched both axes
├───┼───────┼───┤  R  = stretched vertically
│BL │   B   │BR │
│   │       │   │  BL = (0,bottom) to (left, height)
└───┴───────┴───┘  B  = stretched horizontally
                    BR = (right,bottom) to (width, height)
```

The `expand_gorge_slice()` function determines target segment sizes:
- When `target <= base`: scale linearly
- When `target > base`: corners keep their base-relative size; edges absorb the remaining space

## GDScript Usage

```gdscript
# Create a nine-slice sprite programmatically
var nine_slice = ClassDB.instantiate("NineSliceSprite2D")
nine_slice.texture = load("res://assets/panel_border.png")
nine_slice.size = Vector2(400, 200)
nine_slice.patch_margin_left = 16
nine_slice.patch_margin_top = 16
nine_slice.patch_margin_right = 16
nine_slice.patch_margin_bottom = 16
nine_slice.draw_center = true
nine_slice.centered = true
add_child(nine_slice)

# Gorge-compatible mode with base_size
nine_slice.base_size = Vector2(256, 128)
nine_slice.patch_margin_left = 8
nine_slice.patch_margin_top = 8
nine_slice.patch_margin_right = 28
nine_slice.patch_margin_bottom = 22
```

## How the Gorge Plugin Uses It

The `GodotNineSliceSprite` C# class in the GorgePlugin addon:

1. Instantiates a `NineSliceSprite2D` via `ClassDB.Instantiate("NineSliceSprite2D")`
2. Configures it via `Set()` calls since it's accessed as a generic `Godot.Node` (no C# bindings for the Rust type)
3. Sets `texture`, `base_size`, and patch margins based on Gorge's nine-slice data
4. Converts Gorge coordinates to Godot screen coordinates before setting position and size

```csharp
// In GodotNineSliceSprite.SetGraph()
_rustNineSlice.Set("texture", texture);
_rustNineSlice.Set("base_size", baseSize.CoverGorgeSizeToGodot());
_rustNineSlice.Set("patch_margin_left", (int)sliceLeftTop.x);
_rustNineSlice.Set("patch_margin_top", (int)sliceLeftTop.y);
_rustNineSlice.Set("patch_margin_right", (int)sliceRightBottom.x);
_rustNineSlice.Set("patch_margin_bottom", (int)sliceRightBottom.y);
```

## Building from Source

```bash
cd addons/nine_slice_sprite_2d_godot2d
cargo build --release
```

The compiled library is loaded by Godot via `nine_slice_sprite_2d.gdextension`, which maps platform-specific library paths.

### Platform Support

| Platform | Library |
|----------|---------|
| macOS (x86_64/arm64) | `libnine_slice_sprite_2d.dylib` |
| Linux (x86_64) | `libnine_slice_sprite_2d.so` |
| Windows (x86_64) | `nine_slice_sprite_2d.dll` |
| Android (arm64) | `libnine_slice_sprite_2d.so` |

## Requirements

- Godot 4.1+ (compatibility minimum set in `.gdextension`)
- Rust toolchain (for building)
- `godot` crate 0.4.5
