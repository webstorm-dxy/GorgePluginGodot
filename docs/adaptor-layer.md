# Adaptor Layer (Gorge ↔ Godot Bridge)

适配层 (`GodotAdaptor/`) 是 Gorge 引擎框架与 Godot 引擎之间的桥接层。它将 Gorge 框架定义的抽象接口（`GorgeFramework/Adaptor/`）用 Godot 的原生节点和 API 具体实现。

## 架构模式

```
Gorge Framework (抽象层)              Godot Adaptor (实现层)
────────────────────────────         ────────────────────────
IGorgeFrameworkBase          ←──    GodotBase : Node
    (创建 Sprites/Audio/Graph)           (Godot 引擎的实际入口)

ISprite                      ←──    GodotSprite : Sprite2D
INineSliceSprite             ←──    GodotNineSliceSprite : Node
ICurveSprite                 ←──    GodotCurveSprite : Line2D

IAudioPlayer                 ←──    GodotAudioPlayer
IAudioEffectPlayer           ←──    GodotAudioEffectPlayer

Graph                        ←──    GodotGraph
Audio                        ←──    GodotAudio
```

所有 Gorge 框架代码只依赖 `GorgeFramework/Adaptor/` 中的接口，不直接引用 Godot 命名空间。`Base.Instance` 静态属性保存全局适配器实例，由 `GamePlayer.InitializeRenderRoot()` 设置。

## IGorgeFrameworkBase 实现 (GodotBase)

`GodotBase` 继承 `Godot.Node` 并实现 `IGorgeFrameworkBase`，是适配层的根。

### 日志

```csharp
void Log(string msg)     → GD.Print(msg)
void Warning(string msg) → GD.PushWarning(msg)
```

### 持久路径

```csharp
string PersistentPath() → OS.GetUserDataDir()
```

### 精灵工厂

```csharp
ISprite CreateSprite()
    → new GodotSprite()                  // Sprite2D 子类
    → 添加到 GamePlayer.Instance (Camera2D)

INineSliceSprite CreateNineSliceSprite()
    → new GodotNineSliceSprite()         // 包装 Rust NineSliceSprite2D GDExtension
    → 添加到 GamePlayer.Instance

ICurveSprite CreateCurveSprite()
    → new GodotCurveSprite()            // Line2D 子类
    → 添加到 GamePlayer.Instance
```

所有精灵节点被添加为 `GamePlayer.Instance` 的子节点。`GamePlayer.Instance` 实际上是 `_camera`（Camera2D），因此所有精灵随相机一起移动。

### 资源工厂

```csharp
// 图像：PNG 字节 → Image → ImageTexture → GodotGraph
Graph CreateGraph(string assetFilePath, byte[] data)
    → Image.LoadPngFromBuffer(data)
    → ImageTexture.CreateFromImage(image)
    → new GodotGraph(texture)

// 音频：检查 WAV RIFF 头 → AudioStreamWav.LoadFromBuffer → GodotAudio
Audio CreateAudio(string assetFilePath, byte[] data)
    → 验证 "RIFF" 头
    → AudioStreamWav.LoadFromBuffer(data)
    → new GodotAudio(audioStream)
```

### 音频播放器工厂

```csharp
IAudioPlayer CreateAudioPlayer()
    → new GodotAudioPlayer()
    → 内部创建 AudioStreamPlayer
    → 通过 CallDeferred 添加到当前场景

IAudioEffectPlayer CreateAudioEffectPlayer(Audio audioEffect)
    → new GodotAudioEffectPlayer(audioEffect)
    → 通过 CallDeferred 添加到 GamePlayer.Instance
```

### 仿真环境工厂

```csharp
GorgeSimulationRuntime CreateSimulationRuntime(Action onTerminate)
    → new GorgeSimulationRuntime(onTerminate)
```

## 精灵实现

### GodotSprite (普通精灵)

继承 `Sprite2D`，实现 `ISprite` 和 `ISceneObject`。

```csharp
SetPosition(Vector3 position)
    Position = position.CoverGorgeCoodinateToGodot();
    ZIndex = (int)position.z;

SetRotation(Vector3 rotation)
    RotationDegrees = -rotation.z;  // Gorge Z → Godot 顺时针

SetScale(Vector3 scale)
    Scale = ConvertGorgeToGodot(new Vector2(scale.x, scale.y));
    // 使用 256×160 逻辑分辨率计算缩放

SetGraph(Graph? graph)
    Texture = GodotGraphExtension.ToTexture(graph);

SetColor(ColorArgb color)
    Modulate = new Color(color.r, color.g, color.b, color.a);

Destroy()
    QueueFree();
```

### GodotNineSliceSprite (九宫格精灵)

包装 Rust `NineSliceSprite2D` GDExtension。通过 `ClassDB.Instantiate("NineSliceSprite2D")` 动态创建，用 `Set()` 方法配置属性（因为 Rust 类型没有 C# 绑定）。

```csharp
SetPosition(Vector3 position)
    node2d.Position = position.CoverGorgeCoodinateToGodot();

SetRotation(Vector3 rotation)
    node2d.RotationDegrees = -rotation.z;

SetScale(Vector3 scale)
    _rustNineSlice.Set("size", scale.CoverGorgeSizeToGodot());
    // 注意：只有在 SetGraph 之后再设置 Scale 才有效 (_graphSet 标记)

SetGraph(Graph graph, Vector2 baseSize, Vector2 sliceLeftTop, Vector2 sliceRightBottom)
    _rustNineSlice.Set("texture", texture);
    _rustNineSlice.Set("base_size", baseSize.CoverGorgeSizeToGodot());
    _rustNineSlice.Set("patch_margin_left", (int)sliceLeftTop.x);
    _rustNineSlice.Set("patch_margin_top", (int)sliceLeftTop.y);
    _rustNineSlice.Set("patch_margin_right", (int)sliceRightBottom.x);
    _rustNineSlice.Set("patch_margin_bottom", (int)sliceRightBottom.y);

SetColor(ColorArgb color)
    _rustNineSlice.Set("modulate", new Color(color.r, color.g, color.b, color.a));

SetHsl(Vector3 hsl)
    // HSL 调整不直接支持，无操作
```

NineSlice 节点通过 `EnsureRustNode()` 惰性创建，可在 `_Ready()` 或首次 `SetGraph()` 时触发。

### GodotCurveSprite (曲线/线段精灵)

继承 `Line2D`，实现 `ICurveSprite`。

```csharp
SetLine(ObjectArray? pointArray)
    ClearPoints();
    for (i = 0; i < pointArray.length; i++)
        AddPoint(Vector2.FromGorgeObject(pointArray.Get(i)).CoverGorgeCoodinateToGodot());

SetColor(ColorArgb color)
    DefaultColor = new Color(color.r, color.g, color.b, color.a);
```

## 音频实现

### GodotAudioPlayer

包装 `AudioStreamPlayer` 节点，实现 `IAudioPlayer`：

```csharp
SetAudio(Audio audio)
    _player.Stream = audio.ToAudioClip();

Play()   → _player.Play()
Stop()   → _player.Stop()
Seek(t)  → _player.Seek(t)
AudioLength() → (float)_player.Stream.GetLength()
IsPlaying()   → _player.Playing
Destruct()    → _player.QueueFree()
```

音频播放器被添加到当前场景（`SceneTree.CurrentScene`），而不是 GamePlayer 的相机节点。

### GodotAudioEffectPlayer

简化版音频播放器，只支持一次性播放音效：

```csharp
Play()    → _effectPlayer.Play()
Destruct() → _effectPlayer.QueueFree()
```

音效播放器通过 `CallDeferred` 添加到 `GamePlayer.Instance`。

## 资源类型桥接

### GodotGraph

`Graph` 的 Godot 实现，内部持有 `Texture2D`：

```csharp
class GodotGraph : Graph {
    Texture2D _graph;
    width  = _graph.GetWidth();
    height = _graph.GetHeight();
}
```

扩展方法提供双向转换：
- `Texture2D.ToGraph()` — Godot 纹理 → Gorge Graph
- `Graph.ToTexture()` — Gorge Graph → Godot 纹理

### GodotAudio

`Audio` 的 Godot 实现，内部持有 `AudioStream`：

```csharp
class GodotAudio : Audio {
    AudioStream _audio;
}
```

扩展方法：
- `AudioStream.ToAudio()` — Godot 音频流 → Gorge Audio
- `Audio.ToAudioClip()` — Gorge Audio → Godot AudioStream

## 类型映射

Gorge 框架中有两种类型系统并存：

| Gorge 类型 | Native (AutoGenerated) | 实际 Godot 桥接 |
|-----------|----------------------|-----------------|
| `GorgeFramework.Graph` | `Gorge.Native.GorgeFramework.Graph` | `GodotGraph` (Texture2D) |
| `GorgeFramework.Audio` | `Gorge.Native.GorgeFramework.Audio` | `GodotAudio` (AudioStream) |
| `GorgeFramework.Vector2` | `Gorge.Native.GorgeFramework.Vector2` | C# struct (x, y) |
| `GorgeFramework.Vector3` | `Gorge.Native.GorgeFramework.Vector3` | C# struct (x, y, z) |
| `GorgeFramework.ColorArgb` | `Gorge.Native.GorgeFramework.ColorArgb` | C# struct (r, g, b, a) |

Native (AutoGenerated) 类型在 `System/Native/AutoGenerated/` 目录下，是 Gorge 语言标准库的 C# 映射。

## Godot 扩展方法

`GodotSprite.cs` 中定义了大量坐标转换扩展方法，参见 [Coordinate System & Rendering](coordinate-system.md)。
