# Simulation System

Gorge 的仿真系统是一个离散时间的游戏模拟引擎，核心是 `SimulationMachine`。它驱动谱面播放过程中的所有游戏逻辑更新。

## 核心概念

### 双时间轴

仿真系统维护两条独立的时间轴：

| 时间轴 | 变量 | 含义 |
|--------|------|------|
| ChartTime | 谱面时间 | 以秒为单位，对应音频播放位置。可正转/反转/跳转 |
| SimulateTime | 模拟时间 | 单调递增的逻辑时间，始终向前，用于信号采样 |

二者通过 `SimulateSpeed` 关联：
```
ChartTime += SimulateTime × SimulateSpeed
```

### 仿真目标 (SimulationTarget)

仿真目标定义了一次仿真驱动要达到的状态：

```csharp
class SimulationTarget {
    float ChartTime;              // 目标谱面时间
    float SimulateSpeed;          // 仿真速度倍率（正转>0, 反转<0, 零速=0）
    IGameplayAction[] PendingActions;  // 达到目标后执行的挂起动作
}
```

目标按序压栈 (`_simulationTargetStack`)：
- 初始目标在 `RuntimeInitialize()` 时压入，包含终止动作 `Terminate`
- 用户跳转/速度变更会产生新目标压栈
- 当 ChartTime 到达目标时间 → 弹栈并执行 PendingActions

### 仿真方向

```csharp
enum SimulateDirection {
    Forward,         // 正转：ChartTime 增加
    Backward,        // 反转：ChartTime 减少
    Infinitesimal    // 零速：ChartTime 不变，仅处理信号
}
```

## SimulationMachine 工作流

### 主驱动循环 (Drive)

```csharp
void Drive(float simulationTime)
```

每帧由 `GamePlayer._Process(delta)` 调用。内部循环：

```
剩余时间 = delta
while (剩余时间 > 0 && 栈顶目标 != null) {
    计算单步时间 → 复合步模拟 → 尝试接收仿真任务
}
尾独立仿真
```

### 复合步模拟 (SimulateCompositeStep)

每步模拟由三阶段构成：

```
┌─ SimulateCompositeStep ────────────────────────────────┐
│                                                         │
│  1. 推进阶段                                             │
│     取信号切片 → 所有模拟器并行 ForwardSimulate()          │
│     → 收集 IGameplayAction 队列                          │
│     → DoStepGameplayActions() 执行动作                   │
│     → 检查自动机变更标记                                   │
│                                                         │
│  2. 零步长收敛 (ZeroLengthSimulate)                      │
│     while (未收敛) {                                     │
│         所有模拟器 InstantSimulate()                      │
│         → 执行同步动作                                    │
│         → 处理信号边沿（如自动机变更则需要额外迭代）          │
│     }                                                   │
│                                                         │
│  3. 更新时间                                              │
│     ChartTime = target, SimulateTime = target            │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

**零步长收敛循环**：在同一逻辑时间点内，当信号边沿或自动机变更产生连锁反应时，循环执行 InstantSimulate 直到状态稳定。

### 步长计算 (CalculateStepTime)

单步时间的决定取三者最小值：
1. **仿真器申报步长** — 最近的模拟器下一次动作距离
2. **驱动剩余时间** — 本次 Drive() 的剩余 delta
3. **信号边沿时间** — 最近的输入信号边沿

```csharp
var simulateTimeStepLength = Math.Min(simulatorTaskLength, remainingStepLength, signalStepLength);
```

### 仿真任务 (SimulationTask)

```csharp
class SimulationTask {
    float SimulateTime;              // 模拟时间目标
    float? ChartTime;                // 谱面时间目标（零速模拟时为 null）
    IGameplayAction[] PendingActions; // 达到目标后执行的挂起动作
}
```

`CalculateSimulationTask()` 根据当前仿真目标的 SimulateSpeed 来决定：
- **正转**：取所有模拟器 `ForwardAsyncSimulationTarget()` 的最小值，按目标栈顶截断
- **反转**：取所有模拟器 `BackwardAsyncSimulationTarget()` 的最大值，按目标栈顶截断
- **零速**：取所有模拟器 `InfinitesimalAsyncSimulationTarget()` 的最小值

## 仿真器接口 (ISimulator)

```csharp
interface ISimulator {
    // 异步目标计算
    float ForwardAsyncSimulationTarget(float chartTime, GorgeSimulationRuntime runtime);
    float BackwardAsyncSimulationTarget(float chartTime, GorgeSimulationRuntime runtime);
    float InfinitesimalAsyncSimulationTarget(float chartTime, GorgeSimulationRuntime runtime);

    // 推进模拟
    IGameplayAction[] ForwardSimulate(...);
    IGameplayAction[] BackwardSimulate(...);
    IGameplayAction[] InfinitesimalSimulate(...);

    // 瞬时模拟（零步长内）
    IGameplayAction[] InstantSimulate(...);
}
```

### 内置仿真器

| 仿真器 | 职责 |
|--------|------|
| `SongSimulator` | 歌曲 Note 生成和判定 |
| `GraphicsNodeSimulator` | 图形节点生命周期管理 |
| `PreciseAutomatonSimulator` | 精密自动机信号处理 |
| `TimedElementGenerator` | 定时生成 Element |
| `TimedElementDestroyer` | 定时销毁 Element |
| `ElementSimulator` | Element 运行时行为 |

### 尾独立仿真器

与主仿真器不同，尾独立仿真器在每帧最后执行，不参与步长计算和零步长收敛循环，不贡献 IGameplayAction。

## 同步动作 (IGameplayAction)

仿真步内收集的动作在 `DoStepGameplayActions()` 中同步执行：

```csharp
interface IGameplayAction {
    void DoAction(GorgeSimulationRuntime runtime, MultichannelEdgeQueue edgeQueue);
    bool ChangeAutomaton { get; }  // 是否触发自动机状态变更
}
```

当 `ChangeAutomaton == true` 时，会触发新一轮的零步长收敛循环。

## 运行时环境 (GorgeSimulationRuntime)

```csharp
class GorgeSimulationRuntime {
    ChartManager Chart;           // 谱面时间控制
    AudioManager Audio;           // 音频播放管理
    AutomatonManager Automaton;   // 信号自动机（输入信号管理）
    SimulationManager Simulation; // 仿真器调度
    SceneManager Scene;           // 场景对象生命周期
    GraphicsManager Graphics;     // 图形渲染管理
    Logger Logger;                // 调试日志
}
```

### 启动流程

```
1. 加载谱面 (LoadScore)
2. 启动仿真 (StartSimulation)
   ├── Logger.StartSimulation()
   ├── Scene.RuntimeInitialize()
   ├── Audio.StartSimulation()
   ├── Graphics.StartSimulation()
   ├── Simulation.RuntimeInitialize()  → SimulationMachine.RuntimeInitialize()
   ├── Automaton.RuntimeInitialize()
   ├── Chart.StartSimulation()
   └── SimulationMachine.DriveInstantly()  // 首帧零步长驱动
```

## 信号处理系统

Gorge 的信号系统 (`GorgeFramework/Signal/`) 用于模拟输入信号的采集和处理：

| 类 | 职责 |
|----|------|
| `MultichannelSnapshot` | 多通道信号当前值快照 |
| `MultichannelEdgeQueue` | 多通道信号边沿队列（按时间排序） |
| `ChannelEdgeQueue` | 单通道边沿队列 |
| `ChannelSnapshot` | 单通道当前值 |
| `MultichannelSplit` / `ChannelSplit` | 信号时间切片（将连续信号切为离散片段） |
| `ISignalCollector` | 信号采集器接口 |

`AutomatonManager` 管理自动机，它持有输入信号并在模拟过程中提供信号切片和边沿信息。

## 输入处理

Gorge 的输入系统 (`GorgeFramework/Input/`) 定义了触摸/点击输入的图结构：

| 类 | 职责 |
|----|------|
| `InputGraph` | 输入判定图（Node + Edge） |
| `InputGraphState` | 图状态 |
| `InputGraphEdge` | 图边 |
| `TouchSignal` | 触摸信号（类型、坐标、时间） |
| `Fragment` | 信号片段 |
| `Edge` | 信号边沿 |
