# GorgePlugin Architecture Overview

GorgePlugin 是一个 Godot 4.6 (.NET) 编辑器插件，将完整的 Gorge 游戏引擎集成到 Godot 中。Gorge 引擎包括自定义脚本语言、编译器、虚拟机、仿真系统和谱面播放框架。

## 项目结构

```
addons/gorgeplugin/
├── gorgeplugin.cs              # EditorPlugin 入口，注册 GamePlayer 自定义节点
├── GamePlayer.cs               # 用户面 API 节点，管理生命周期和包加载
├── plugin.cfg                  # 插件元数据
├── Native.zip                  # Gorge 标准库（预编译的 Native 类型）
├── GodotAdaptor/               # Godot ↔ Gorge 适配桥接层
│   ├── GodotBase.cs            # IGorgeFrameworkBase 实现，资源/渲染工厂
│   ├── GodotSprite.cs          # ISprite / INineSliceSprite / ICurveSprite 渲染实现
│   ├── GodotAudio.cs           # IAudioPlayer / IAudioEffectPlayer 音频实现
│   └── TestAdapterIntegration.cs
└── GorgeTools/
    ├── GorgeCompiler/          # Gorge 语言编译器（Antlr4）
    ├── GorgeCoreCSharp/        # Gorge 语言运行时（虚拟机、类型系统）
    └── GorgeFramework/         # Gorge 游戏框架（仿真系统、谱面、计分）
```

## 总体架构

```
用户 GDScript/C# 代码
        │
        ▼
   GamePlayer (Node)         ← 用户操作 API
        │
        ▼
   RuntimeManager            ← 管理编译→仿真全生命周期
   ├── Compiler              ← .g 源文件 → 中间代码
   ├── GorgeLanguageRuntime  ← 类/枚举/接口反射 + 虚拟机
   ├── RuntimeFormContainer  ← 模态/Element/即时音效元数据
   ├── SimulationScore       ← 谱面时间轴 + 资产表
   └── GorgeSimulationRuntime ← 仿真环境容器
       ├── ChartManager      ← 谱面时间控制
       ├── SimulationManager ← 仿真器调度
       │   └── SimulationMachine  ← 仿真机，驱动仿真步进
       ├── AutomatonManager  ← 自动机（信号处理）
       ├── AudioManager      ← 音频播放管理
       ├── GraphicsManager   ← 图形渲染管理
       ├── SceneManager      ← 场景对象生命周期
       └── IGorgeFrameworkBase (GodotBase) ← Godot 适配器
           ├── 创建 Sprites / Audio / Graph
           ├── 坐标系统转换
           └── 日志输出
```

## 三大子系统

### 1. Gorge 语言（Compiler + CoreCSharp）

Gorge 是一门自定义的面向对象脚本语言，使用 `.g` 扩展名。语言特性包括：
- 类 (class)、接口 (interface)、枚举 (enum)、委托 (delegate)
- 单继承 + 多接口实现
- 依赖注入 (Injector)
- 注解 (Annotation) 驱动的元编程
- Lambda 表达式

编译管线（四轮）：
1. **词法/语法分析** — Antlr4 生成 AST
2. **第一轮** — TypeIdentifierVisitor：收集所有类型定义符号
3. **第二轮** — TypeExtensionVisitor：解析继承关系和类型引用
4. **第三轮** — TypeDeclarationVisitor：生成实现编译任务
5. **第四轮** — ImplementationCompileTask：生成中间代码 (IntermediateCode)

### 2. Gorge 虚拟机（VirtualMachine）

`IntermediateCodeVirtualMachine` 是一个基于栈的虚拟机，直接执行中间代码。支持：
- 5 个独立类型栈：int, float, bool, string, object
- 70+ 条中间代码指令
- 方法调用、字段存取、Injector 字段存取
- 控制流跳转
- 构造方法、委托构造、数组构造

### 3. 仿真系统（Simulation）

`SimulationMachine` 实现了一个离散时间仿真引擎：
- **仿真目标栈**：管理仿真速度变化，支持正转/反转/零速模拟
- **复合步模拟**：每步 = 步进推进 + 信号边沿收敛
- **零步长收敛循环**：同时间点内信号边沿和自动机变更的迭代收敛
- **仿真器并行调度**：每步内所有 ISimulator 并行模拟

## 运行时状态机

```
Uninitialized → Compiled → SimulationResourceLoaded → SimulationInitialized → ScoreLoaded → Simulating
                   ↑                                                              |
                   └──────────────────────────────────────────────────────────────┘
                                              (stop)
```

## 外部依赖

| 包 | 版本 | 用途 |
|---|------|------|
| Antlr4.Runtime.Standard | 4.13.1 | Gorge 语言语法解析 |
| Newtonsoft.Json | 13.0.3 | JSON 序列化 |
| QuikGraph | 2.5.0 | 图数据结构（信号自动机） |
| SharpZipLib | 1.4.2 | ZIP 包文件读写 |
| NineSliceSprite2D | Rust GDExtension | 九宫格精灵渲染 |
