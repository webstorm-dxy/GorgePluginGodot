# GorgeGodotPlugin

一个 Godot 4 插件，能够在 Godot 引擎中播放 [Gorge](https://gorge.icu) 游戏引擎的谱面。只需将谱面包和运行包拖入项目，添加 `GamePlayer` 节点即可播放。

## 架构

插件由三个 addon 组成：

| addon | 语言 | 功能 |
|-------|------|------|
| `gorgeplugin` | C# | 核心谱面播放器：加载 Gorge 包、管理模拟生命周期、通过 Godot 适配层渲染精灵/音频/图形 |
| `godot_mcp` | GDScript | MCP（模型上下文协议）服务端集成，支持 AI 辅助编辑场景、调试、资产生成 |
| `nine_slice_sprite_2d_godot2d` | Rust | GDExtension，提供 `NineSliceSprite2D` 节点，兼容 Gorge 的九宫格渲染 |

## 环境要求

- **Godot 4.6**（需 .NET/Mono 版本）
- **.NET SDK 8.0**（Android 构建需要 9.0）
- **Rust**（可选，仅构建九宫格 GDExtension 时需要）

## 安装

### 快速安装（推荐）

在你的 Godot .NET 项目根目录下，执行一行命令即可完成安装：

**Linux / macOS / WSL / Git Bash：**
```bash
curl -fsSL https://raw.githubusercontent.com/webstorm-dxy/GorgePluginGodot/main/install.sh | bash
```

**Windows PowerShell：**
```powershell
irm https://raw.githubusercontent.com/webstorm-dxy/GorgePluginGodot/main/install.ps1 | iex
```

安装脚本会自动完成以下操作：
1. 检查工具链（git、.NET SDK、Rust/Cargo、Godot .NET 版本）
2. 下载框架 addons（稀疏克隆，不下载大文件）
3. 将 NuGet 依赖合并到你的 `.csproj`
4. 配置 `project.godot`（启用插件、添加 autoload）
5. 构建 Rust GDExtension（如果已安装 cargo）
6. 还原 NuGet 包

> **安全提示**：在通过管道执行脚本之前，建议先下载并检查内容：
> ```bash
> curl -fsSL https://raw.githubusercontent.com/webstorm-dxy/GorgePluginGodot/main/install.sh -o install.sh
> less install.sh
> bash install.sh
> ```

安装脚本是**幂等**的——多次运行不会产生重复条目。

### 手动安装

#### 1. 复制 addons 目录

将 `addons/` 目录复制到你 Godot 项目的 `res://` 目录下。

#### 2. 配置 C# 项目依赖

在你的 `.csproj` 文件中添加以下 NuGet 依赖（**注意版本号必须匹配**，否则 GorgePlugin 可能无法正常工作）：

```xml
<Project Sdk="Godot.NET.Sdk/4.6.2">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <EnableDynamicLoading>true</EnableDynamicLoading>
  </PropertyGroup>
    <!--添加的依赖-->
  <ItemGroup>
    <PackageReference Include="Antlr4.Runtime.Standard" Version="4.13.1" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="QuikGraph" Version="2.5.0" />
    <PackageReference Include="SharpZipLib" Version="1.4.2" />
  </ItemGroup>
</Project>
```

> **重要提示：**
> - `Godot.NET.Sdk` 版本需要匹配你的 Godot 编辑器版本（本插件使用 **4.6.2**）
> - `EnableDynamicLoading` 必须设为 `true`
> - 如果你现有的项目已有上述包的**不同版本**，可能与 GorgePlugin 内部引用的 Gorge 框架 DLL 产生冲突，导致 `MissingMethodException` 或 `FileLoadException`。**建议保持版本一致，或使用 assembly binding redirect**
> - 如果你的项目目标平台为 Android，需要额外添加 `<TargetFramework Condition=" '$(GodotTargetPlatform)' == 'android' ">net9.0</TargetFramework>`

#### 3. 启用插件并构建

在 Godot 中，前往 **项目 → 项目设置 → 插件**，启用：
- **GorgePlugin** — 添加 `GamePlayer` 自定义节点类型
- **Godot MCP** — 可选，用于 AI 辅助工作流

构建 C# 解决方案：在 IDE 中打开解决方案，或在终端执行 `dotnet build`。

#### 4. 构建 Rust 九宫格扩展

```bash
cd addons/nine_slice_sprite_2d_godot2d
cargo build --release
```

## 快速开始

### 1. 创建带 GamePlayer 的场景

在场景中添加一个 `GamePlayer` 节点。该节点作为自定义类型出现在"新建节点"对话框中，名称为 **GamePlayer**。

### 2. 配置包文件

在 `GamePlayer` 节点上设置以下导出属性：

- **ChartPackagePaths** — 谱面包 `.zip` 文件路径数组
- **RuntimePackagePaths** — 运行包 `.zip` 文件路径数组（默认值为 `res://addons/gorgeplugin/Native.zip`）
- **AutoStartOnReady** — 可选，场景加载时自动开始播放
- **TerminateTime** — 模拟终止时间，单位秒（默认：135.0）
- **GorgeAutoPlay** — 谱面是否自动播放（默认：true）

### 3. 调用 API

```gdscript
# 假设你已有一个 GamePlayer 节点
@onready var player = $GamePlayer

func _ready():
    # 以代码方式添加包文件
    player.add_runtime_package_path("res://MyRuntime.zip")
    player.add_chart_package_path("res://MyChart.zip")

    # 监听事件
    player.connect("ChartStarted", Callable(self, "_on_chart_started"))
    player.connect("ChartStopped", Callable(self, "_on_chart_stopped"))
    player.connect("PlayerError", Callable(self, "_on_player_error"))
    player.connect("PlaybackStateChanged", Callable(self, "_on_playback_changed"))
    player.connect("PackagesChanged", Callable(self, "_on_packages_changed"))
    player.connect("ChartPrepared", Callable(self, "_on_chart_prepared"))

    # 开始播放
    player.play_chart()

func _on_chart_started():
    print("谱面正在播放")

func _on_chart_stopped(reason: String):
    print("谱面已停止：", reason)

func _on_player_error(message: String):
    push_error("播放器出错：", message)
```

### 4. 运行演示场景

打开 `demo/game_player_demo.tscn` 并按 F5 运行。演示场景提供播放/停止按钮和状态显示。它需要：
- `res://Dremu.zip` — 运行包
- `res://DremuTest.zip` — 谱面包

## GamePlayer API 参考

### 信号

| 信号 | 参数 | 说明 |
|------|------|------|
| `PackagesChanged` | — | 包列表被修改时触发 |
| `ChartPrepared` | — | 运行时准备完毕、谱面加载完成后触发 |
| `ChartStarted` | — | 谱面开始播放时触发 |
| `ChartStopped` | `reason: String` | 谱面停止时触发（原因包括："stopped"、"restart"、"terminated"、"packages_changed"、"window_close"） |
| `PlaybackStateChanged` | `isPlaying: bool` | 播放状态切换时触发 |
| `PlayerError` | `message: String` | 发生错误时触发 |

### 导出属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `RuntimePackagePaths` | `Array[String]` | `["res://addons/gorgeplugin/Native.zip"]` | 运行包 zip 路径 |
| `ChartPackagePaths` | `Array[String]` | `[]` | 谱面包 zip 路径 |
| `AutoStartOnReady` | `bool` | `false` | 场景就绪时自动播放 |
| `TerminateTime` | `float` | `135.0` | 模拟终止超时 |
| `GorgeAutoPlay` | `bool` | `true` | 传递给运行时的自动播放设置 |

### 方法

| 方法 | 返回值 | 说明 |
|------|--------|------|
| `play_chart()` | `bool` | 开始播放谱面。如运行时尚未准备，会自动调用 prepare_runtime()。 |
| `stop_chart(reason: String = "stopped")` | `bool` | 停止谱面播放。 |
| `restart_chart()` | `bool` | 先停止再立即重新开始播放。 |
| `prepare_runtime()` | `bool` | 加载所有包并初始化模拟运行时，但不开始播放。 |
| `add_package_path(path: String, is_chart: bool)` | `bool` | 按路径添加包文件。 |
| `add_runtime_package_path(path: String)` | `bool` | 添加一个运行包路径。 |
| `add_chart_package_path(path: String)` | `bool` | 添加一个谱面包路径。 |
| `add_package(package: Package)` | `bool` | 直接添加一个预构建的 Package 对象。 |
| `clear_packages()` | `void` | 移除所有包并停止播放。 |
| `request_play()` | `void` | 便捷封装，调用 `play_chart()`。 |
| `request_stop()` | `void` | 便捷封装，调用 `stop_chart()`。 |

### GDScript 兼容别名

以下 snake_case 别名供 GDScript 调用：

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

### 静态成员

| 成员 | 说明 |
|------|------|
| `GamePlayer.IsPlaying` | `bool` — 当前是否有谱面正在播放 |
| `GamePlayer.Instance` | `Node` — 用于精灵渲染的相机节点（供适配层使用） |
| `GamePlayer.viewportSize` | `Vector2` — 缓存的视口尺寸 |
| `GamePlayer.SizeScale` | `float` — 坐标缩放因子（默认：80.0） |

## GodotMCP 插件（AI 助手集成）

`godot_mcp` 插件通过 WebSocket 将 Godot 编辑器连接到 AI 助手，提供以下能力：

- 创建、读取、编辑 `.tscn` 场景文件
- 修改节点属性，附加脚本，设置碰撞形状、网格、材质
- 管理项目设置、输入映射、自动加载、碰撞层
- 创建脚本、语法校验、代码库搜索
- 从 SVG 代码生成 2D 资源
- 运行场景、截图、模拟输入、查询运行时节点
- 项目结构可视化（`map_project`、`map_scenes`）

启用后，插件会在项目中注册 `MCPRuntime` 自动加载，使运行中的游戏能够与 MCP 服务端通信以执行运行时操作。

状态指示器显示在编辑器工具栏中：

| 文字 | 颜色 | 含义 |
|------|------|------|
| `MCP: Connecting...` | 黄色 | 正在连接 WebSocket |
| `MCP: No Agent` | 橙色 | 已连接服务器，无 AI 代理 |
| `MCP: Agent Active` | 绿色 | AI 代理已连接 |
| `MCP: Disconnected` | 红色 | 连接断开，正在重连 |

## NineSliceSprite2D GDExtension

基于 Rust 的 GDExtension，实现 `NineSliceSprite2D` 节点，具备以下功能：

- 可配置的九宫格边距（左、上、右、下）
- 轴向拉伸模式（Stretch、Tile、TileFit）
- Gorge 兼容的 base_size 缩放模式（当 `base_size` 设置为非零值时，使用与 Gorge 一致的九宫格算法）
- 基于帧的精灵表支持（`hframes`/`vframes`）
- 翻转和偏移控制

Gorge 插件的 `GodotNineSliceSprite` 适配层在运行时创建并配置此节点，以匹配 Gorge 的九宫格渲染行为。

## 坐标系

Gorge 基于 16:10 长宽比的坐标系。`GodotSprite` 适配层将 Gorge 坐标转换为 Godot 屏幕坐标：

```
godotX = gorgeX × (viewportWidth / 16)
godotY = -gorgeY × (viewportHeight / 10)
```

注意 Y 轴反转 —— Gorge 使用 Y 轴向上，Godot 使用 Y 轴向下。

## 项目结构

```
GorgeGodotPlugin/
├── addons/
│   ├── gorgeplugin/                  # Gorge 谱面播放器（C#）
│   │   ├── plugin.cfg
│   │   ├── gorgeplugin.cs            # 编辑器插件注册
│   │   ├── GamePlayer.cs             # 核心播放器节点
│   │   ├── GodotAdaptor/
│   │   │   ├── GodotBase.cs          # IGorgeFrameworkBase 实现
│   │   │   ├── GodotSprite.cs        # 精灵/曲线/九宫格适配层
│   │   │   ├── GodotAudio.cs         # 音频播放适配层
│   │   │   └── TestAdapterIntegration.cs
│   │   └── Native.zip                # 默认运行包
│   ├── godot_mcp/                    # MCP 服务端集成（GDScript）
│   │   ├── plugin.cfg
│   │   ├── plugin.gd                 # 编辑器插件
│   │   ├── mcp_client.gd             # WebSocket 客户端
│   │   ├── tool_executor.gd          # 工具路由
│   │   ├── tools/
│   │   │   ├── scene_tools.gd        # 场景增删改查
│   │   │   ├── file_tools.gd         # 文件列表/读取/搜索
│   │   │   ├── script_tools.gd       # 脚本编辑/校验
│   │   │   ├── project_tools.gd      # 项目设置/调试
│   │   │   ├── asset_tools.gd        # SVG→PNG 资产生成
│   │   │   └── visualizer_tools.gd   # 项目映射与可视化
│   │   ├── runtime/
│   │   │   └── mcp_runtime.gd        # 游戏内 MCP 自动加载
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
│   ├── game_player_demo.tscn         # 演示场景
│   └── game_player_demo.gd           # 演示脚本
├── docs/                             # 文档
│   ├── setup.md                      # 安装与配置指南
│   ├── gameplayer.md                 # GamePlayer API 详细参考
│   ├── godot-mcp.md                  # GodotMCP 插件指南
│   ├── nine-slice-sprite.md          # NineSliceSprite2D 指南
│   └── coordinate-system.md          # 坐标系与渲染管线
├── project.godot                     # 项目配置
├── GorgeGodotPlugin.csproj           # C# 项目文件
└── GorgeGodotPlugin.sln              # 解决方案文件
```

## 依赖

### NuGet 包（C#）

- `Antlr4.Runtime.Standard` 4.13.1
- `Newtonsoft.Json` 13.0.3
- `QuikGraph` 2.5.0
- `SharpZipLib` 1.4.2

### Cargo 包（Rust）

- `godot` 0.4.5

### Godot 项目设置

- 渲染方式：Mobile
- 物理引擎：Jolt Physics
- 默认窗口：1920×1200

## 许可证

本项目包含来自多个来源的代码：
- **GorgePlugin** — 作者 HPCG
- **GodotMCP** — 作者 Godot MCP
- **NineSliceSprite2D** — MIT 许可证

各 addon 目录下的独立 `LICENSE` 文件详见具体许可条款。
