using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Gorge.GorgeFramework.Input;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Signal;
using Gorge.GorgeFramework.Simulators;
using Gorge.GorgeFramework.Utilities;
using Gorge.GorgeLanguage.Objective;
using Math = Gorge.Native.GorgeFramework.Math;

// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162 // 检测到不可到达的代码

namespace Gorge.GorgeFramework.Runtime
{
    /// <summary>
    ///     模拟机，在模拟驱动器的驱动下完成模拟动作
    /// </summary>
    public class SimulationMachine
    {
        /// <summary>
        ///     模拟目标栈
        /// </summary>
        private Stack<SimulationTarget> _simulationTargetStack;

        /// <summary>
        ///     初始化完成标记
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// 当前正在执行的仿真任务
        /// 设置为null则在下次访问时自动计算新的仿真任务
        /// </summary>
        private SimulationTask NowSimulationTask
        {
            get
            {
                if (_nowSimulationTask == null)
                {
                    _nowSimulationTask = CalculateSimulationTask();
                }

                return _nowSimulationTask;
            }
            set => _nowSimulationTask = value;
        }

        /// <summary>
        ///     当前正在执行的反正任务
        /// </summary>
        private SimulationTask _nowSimulationTask;

        /// <summary>
        ///     谱面时间
        /// </summary>
        public float ChartTime { get; private set; }

        /// <summary>
        ///     模拟时间
        /// </summary>
        public float SimulateTime { get; private set; }

        private readonly GorgeSimulationRuntime _runtime;

        public SimulationMachine(GorgeSimulationRuntime runtime)
        {
            _runtime = runtime;
        }

        public void RuntimeInitialize()
        {
            SimulateTime = 0;
            ChartTime = _runtime.Chart.BeginChartTime;

            // 将初始目标进行压栈
            _simulationTargetStack = new Stack<SimulationTarget>();
            var initTarget = new SimulationTarget
            {
                ChartTime = _runtime.Chart.TerminateChartTime,
                SimulateSpeed = _runtime.Chart.BeginSimulateSpeed,
                PendingActions = new IGameplayAction[]
                {
                    new Terminate()
                }
            };
            _simulationTargetStack.Push(initTarget);

            _initialized = true;
        }

        public void RuntimeDestruct()
        {
            SimulateTime = 0;
            ChartTime = _runtime.Chart.BeginChartTime;
            _simulationTargetStack = null;
            _initialized = false;
            NowSimulationTask = null;
        }

        /// <summary>
        /// 由编辑器滑条使用的“推进至”语义
        /// 如果和模拟方向一致，则推进至目标时间
        /// 如果和模拟方向反向，或当前为零速模拟，则压栈1速推进新目标，并推进到目标完成
        /// </summary>
        /// <param name="chartTime"></param>
        public void SimulateTo(float chartTime)
        {
            if (_simulationTargetStack.Top() == null)
            {
                // 没有目标，推进拒绝
                return;
            }

            if (chartTime > ChartTime)
            {
                // 同向推进至目标时间
                if (_simulationTargetStack.Top().SimulateSpeed > 0)
                {
                    DriveToChartTime(chartTime);
                }
                // 反向压栈独立目标并推进至弹栈
                else
                {
                    _simulationTargetStack.Push(new SimulationTarget()
                    {
                        ChartTime = chartTime,
                        PendingActions = Array.Empty<IGameplayAction>(),
                        SimulateSpeed = 1
                    });

                    DriveToSimulationTarget();
                }
            }
            else if (chartTime < ChartTime)
            {
                // 同向推进至目标时间
                if (_simulationTargetStack.Top().SimulateSpeed < 0)
                {
                    DriveToChartTime(chartTime);
                }
                // 反向压栈独立目标并推进至弹栈
                else
                {
                    _simulationTargetStack.Push(new SimulationTarget()
                    {
                        ChartTime = chartTime,
                        PendingActions = Array.Empty<IGameplayAction>(),
                        SimulateSpeed = -1
                    });

                    DriveToSimulationTarget();
                }
            }
        }

        /// <summary>
        ///     向前驱动一定模拟时间
        /// </summary>
        /// <param name="simulationTime">向前驱动的模拟时长</param>
        public void Drive(float simulationTime)
        {
            // GD.Print("Driving");
            _runtime.Logger.DebugLog($"调用驱动 {simulationTime}", 0);

            if (!_initialized) throw new Exception("在初始化前尝试驱动");

            if (simulationTime < 0) throw new Exception($"模拟机驱动时长为负值{simulationTime}");

            // 目前驱动向前驱动0不会有任何影响
            // TODO 是否应该调用一个零步长驱动？
            if (simulationTime == 0)
            {
                _runtime.Logger.DebugLog("零驱动，驱动拒绝", 1);
                return;
            }

            if (_simulationTargetStack.Top() == null)
            {
                _runtime.Logger.DebugLog("目标栈空，驱动拒绝", 1);
                return;
            }

            _runtime.Logger.DebugLog(
                $"当前模拟目标为{_simulationTargetStack.Top().ChartTime}，倍速{_simulationTargetStack.Top().SimulateSpeed}",
                1);

            var remainingSimulationTime = simulationTime; // 本次驱动的剩余模拟时间
            while (remainingSimulationTime > 0 && _simulationTargetStack.Top() != null) // 消耗完所有剩余模拟时间，则本次驱动结束
            {
                _runtime.Logger.DebugLog($"单步模拟，剩余时间{remainingSimulationTime}", 1);

                _runtime.Logger.DebugLog(
                    $"有模拟步骤，倍速{_simulationTargetStack.Top().SimulateSpeed}，目标{NowSimulationTask.SimulateTime}", 2);

                #region 计算模拟目标

                remainingSimulationTime = CalculateStepTime(remainingSimulationTime, _simulationTargetStack.Top(),
                    out var chartTimeTarget, out var simulateTimeTarget);

                #endregion

                // 执行复合步一步模拟
                // 一步复合模拟 = 一步推进 + 若干零步长模拟
                SimulateCompositeStep(simulateTimeTarget, chartTimeTarget, _simulationTargetStack.Top().Direction);

                // 尝试接收仿真任务
                TryAcceptSimulationTask(true);
            }

            // 执行尾独立仿真器
            LateIndependentSimulate(SimulateDirection.Forward, new MultichannelSnapshot());
        }

        /// <summary>
        /// 计算单步时间
        /// </summary>
        /// <param name="remainingSimulationTime"></param>
        /// <param name="nowSimulationTarget"></param>
        /// <param name="stepChartTime"></param>
        /// <param name="stepSimulateTime"></param>
        /// <returns>本步完成后的剩余仿真时间</returns>
        private float CalculateStepTime(float remainingSimulationTime, SimulationTarget nowSimulationTarget,
            out float stepChartTime, out float stepSimulateTime)
        {
            // 确定复合步的步长
            // 计算模拟器申报步长
            var simulatorTaskLength = NowSimulationTask.SimulateTime - SimulateTime;
            // 计算本次驱动剩余时间长度
            var remainingStepLength = remainingSimulationTime;
            // 计算最近边沿长度
            var signalStepLength = _runtime.Automaton.GetInputSignalEarliestEdgeTimeAfter(SimulateTime) -
                                   SimulateTime;
            // 取最短的步长
            var simulateTimeStepLength = Math.Min(simulatorTaskLength, remainingStepLength, signalStepLength);
            _runtime.Logger.DebugLog(
                $"确定综合步长，模拟步剩余{simulatorTaskLength}，驱动剩余{remainingStepLength}，信号边沿剩余{signalStepLength}", 2);

            // 更新剩余模拟时间
            remainingSimulationTime -= simulateTimeStepLength;

            _runtime.Logger.DebugLog($"综合后目标{simulateTimeStepLength}，驱动时间剩余{remainingSimulationTime}", 2);

            float chartTimeStepLength;

            // 如果和步尾相等，则ChartTime对齐
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (simulatorTaskLength == simulateTimeStepLength && NowSimulationTask.ChartTime != null)
            {
                // 计算起点终点
                chartTimeStepLength = NowSimulationTask.ChartTime.Value - ChartTime;
            }
            else
            {
                chartTimeStepLength = simulateTimeStepLength * nowSimulationTarget.SimulateSpeed;
            }

            // 计算起点终点
            stepChartTime = ChartTime + chartTimeStepLength;
            stepSimulateTime = SimulateTime + simulateTimeStepLength;

            return remainingSimulationTime;
        }

        /// <summary>
        /// 保持方向驱动到目标谱面时间
        /// </summary>
        public void DriveToChartTime(float targetChartTime)
        {
            _runtime.Logger.DebugLog($"调用驱动至目标谱面时间{targetChartTime}", 0);
            if (!_initialized) throw new Exception("在初始化前尝试驱动");

            // 如果目标栈弹空，则不进行任何模拟
            if (_simulationTargetStack.Top() == null)
            {
                _runtime.Logger.DebugLog("目标栈空，驱动拒绝", 1);
                return;
            }

            if (_simulationTargetStack.Top().SimulateSpeed == 0)
            {
                throw new Exception("无法对0速模拟目标是用推进至目标谱面时间");
            }

            _runtime.Logger.DebugLog(
                $"当前模拟目标为{_simulationTargetStack.Top().ChartTime}，倍速{_simulationTargetStack.Top().SimulateSpeed}",
                1);

            // 持续模拟，直到目标谱面时间
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            while (ChartTime != targetChartTime && _simulationTargetStack.Top() != null)
            {
                _runtime.Logger.DebugLog($"单步模拟，当前谱面时间{ChartTime}", 1);

                _runtime.Logger.DebugLog(
                    $"有模拟步骤，倍速{_simulationTargetStack.Top().SimulateSpeed}，目标{NowSimulationTask.SimulateTime}", 2);

                if (NowSimulationTask.ChartTime == null)
                {
                    // TODO 此处应该以更好的方式向外汇报推进失败
                    throw new Exception("驱动到目标谱面时间途中经历零速模拟阶段");
                }

                // 计算起点终点
                // 如果模拟步越过了目标时间，则截断到目标时间
                var chartTimeTarget = _simulationTargetStack.Top().SimulateSpeed > 0
                        ? MathF.Min(NowSimulationTask.ChartTime.Value, targetChartTime)
                        : MathF.Max(NowSimulationTask.ChartTime.Value, targetChartTime)
                    ;
                var simulateTimeTarget = SimulateTime;

                // 执行复合步一步模拟
                // 一步复合模拟 = 一步推进 + 若干零步长模拟
                SimulateCompositeStep(simulateTimeTarget, chartTimeTarget, _simulationTargetStack.Top().Direction);

                // 尝试接收仿真任务
                TryAcceptSimulationTask(false);
            }

            // 执行尾独立仿真器
            LateIndependentSimulate(SimulateDirection.Forward, new MultichannelSnapshot());
        }

        /// <summary>
        /// 保持方向驱动到栈顶模拟目标完成
        /// </summary>
        public void DriveToSimulationTarget()
        {
            _runtime.Logger.DebugLog($"调用驱动至完成当前模拟目标", 0);
            if (!_initialized) throw new Exception("在初始化前尝试驱动");

            // 如果目标栈弹空，则不进行任何模拟
            if (_simulationTargetStack.Top() == null)
            {
                _runtime.Logger.DebugLog("目标栈空，驱动拒绝", 1);
                return;
            }

            if (_simulationTargetStack.Top().SimulateSpeed == 0)
            {
                throw new Exception("无法对0速模拟目标是用推进至目标谱面时间");
            }

            // 当该目标弹栈，则退出模拟
            var targetSimulationTarget = _simulationTargetStack.Top();

            while (_simulationTargetStack.Contains(targetSimulationTarget))
            {
                _runtime.Logger.DebugLog(
                    $"当前模拟目标为{_simulationTargetStack.Top().ChartTime}，倍速{_simulationTargetStack.Top().SimulateSpeed}",
                    1);

                // 仿真一次，至仿真目标完成
                // TODO 在产生新仿真目标的情况下这个流程可能是不正确的
                _runtime.Logger.DebugLog($"单步模拟，当前谱面时间{ChartTime}", 1);

                _runtime.Logger.DebugLog(
                    $"有模拟步骤，倍速{_simulationTargetStack.Top().SimulateSpeed}，目标{NowSimulationTask.SimulateTime}", 2);

                if (NowSimulationTask.ChartTime == null)
                {
                    // TODO 此处应该以更好的方式向外汇报推进失败
                    throw new Exception("驱动到目标谱面时间途中经历零速模拟阶段");
                }

                // 计算起点终点
                var chartTimeTarget = NowSimulationTask.ChartTime.Value;
                var simulateTimeTarget = SimulateTime;
                // 执行复合步一步模拟
                // 一步复合模拟 = 一步推进 + 若干零步长模拟
                SimulateCompositeStep(simulateTimeTarget, chartTimeTarget, _simulationTargetStack.Top().Direction);

                // 尝试接收仿真任务
                TryAcceptSimulationTask(false, targetSimulationTarget);
            }

            // 执行尾独立仿真器
            LateIndependentSimulate(SimulateDirection.Forward, new MultichannelSnapshot());
        }

        /// <summary>
        /// 获取信号切片
        /// </summary>
        /// <returns></returns>
        private (MultichannelSnapshot, MultichannelEdgeQueue) GetSplitSignals()
        {
            // 取复合步信号切片
            var splitSignals =
                _runtime.Automaton.SplitInputSignals(SimulateTime, SimulateTime);
            // 各信号的当前值
            var signalValues = new MultichannelSnapshot();
            // 计算各信号的边沿处理队列
            var signalEdgeStacks = new MultichannelEdgeQueue();

            foreach (var (channelName, signalChannel) in splitSignals)
            {
                signalValues[channelName] = new ChannelSnapshot();
                signalEdgeStacks[channelName] = new ChannelEdgeQueue();
                foreach (var (signalId, fragment) in signalChannel)
                {
                    signalValues[channelName][signalId] = fragment.StartValue;
                    var queue = new Queue<Edge<GorgeObject>>();
                    foreach (var edge in fragment.Edges) queue.Enqueue(edge);

                    signalEdgeStacks[channelName][signalId] = queue;
                }
            }

            return (signalValues, signalEdgeStacks);
        }

        /// <summary>
        /// 执行一次零步长驱动
        /// </summary>
        public void DriveInstantly()
        {
            _runtime.Logger.DebugLog($"调用零步长驱动", 0);
            if (!_initialized) throw new Exception("在初始化前尝试驱动");

            if (!_simulationTargetStack.TryPeek(out var target))
            {
                _runtime.Logger.DebugLog("目标栈空，驱动拒绝", 1);
                return;
            }

            var (signalValues, signalEdgeStacks) = GetSplitSignals();

            ZeroLengthSimulate(ChartTime, target.Direction, signalValues, signalEdgeStacks, false);

            // 执行尾独立仿真器
            LateIndependentSimulate(target.Direction, signalValues);
        }

        /// <summary>
        /// 计算仿真任务
        /// 依据是当前仿真目标和当前存活的自动机
        /// </summary>
        /// <returns>仿真任务</returns>
        /// <exception cref="Exception"></exception>
        private SimulationTask CalculateSimulationTask()
        {
            switch (_simulationTargetStack.Top().SimulateSpeed)
            {
                case > 0: // 正转情况
                {
                    // var log = new StringBuilder();
                    var minSimulatorTarget = _runtime.Simulation.Simulators.Min(simulator =>
                    {
                        var result = simulator.Value.ForwardAsyncSimulationTarget(ChartTime, _runtime);
                        // log.AppendLine($"{simulator} {result} {ChartTime}");
                        return result;
                    });
                    if (minSimulatorTarget <= ChartTime)
                    {
                        // Debug.Log(log);
                        throw new Exception($"正转步进长度不为正，当前谱面时间为{ChartTime}，最小模拟目标为{minSimulatorTarget}");
                    }

                    // 如果目标时间在前进方向上，则截断到目标时间
                    var stepTargetChartTime = ChartTime < _simulationTargetStack.Top().ChartTime
                        ? MathF.Min(minSimulatorTarget, _simulationTargetStack.Top().ChartTime)
                        : minSimulatorTarget;
                    var targetSimulationTime = SimulateTime +
                                               (stepTargetChartTime - ChartTime) /
                                               _simulationTargetStack.Top().SimulateSpeed;

                    var newStep = new SimulationTask
                    {
                        SimulateTime = targetSimulationTime,
                        ChartTime = stepTargetChartTime,
                        PendingActions = Array.Empty<IGameplayAction>()
                    };

                    _runtime.Logger.DebugLog($"计算完成，正转至{newStep.SimulateTime}", 3);

                    return newStep;
                }
                    break;
                case < 0: // 反转情况
                {
                    var log = new StringBuilder();
                    var maxSimulatorTarget = _runtime.Simulation.Simulators.Max(
                        simulator =>
                        {
                            var result = simulator.Value.BackwardAsyncSimulationTarget(ChartTime, _runtime);
                            log.AppendLine($"{simulator} {result} {ChartTime}");
                            return result;
                        });
                    if (maxSimulatorTarget >= ChartTime)
                    {
                        log.AppendLine($"{maxSimulatorTarget} {ChartTime}");
                        throw new Exception("返转步进长度不为负");
                    }

                    // 如果目标时间在前进方向上，则截断到目标时间
                    var stepTargetChartTime = ChartTime > _simulationTargetStack.Top().ChartTime
                        ? MathF.Max(maxSimulatorTarget, _simulationTargetStack.Top().ChartTime)
                        : maxSimulatorTarget;
                    var targetSimulationTime = SimulateTime +
                                               (stepTargetChartTime - ChartTime) /
                                               _simulationTargetStack.Top().SimulateSpeed;
                    var newStep = new SimulationTask
                    {
                        SimulateTime = targetSimulationTime,
                        ChartTime = stepTargetChartTime,
                        PendingActions = Array.Empty<IGameplayAction>()
                    };
                    _runtime.Logger.DebugLog($"计算完成，反转至{newStep.SimulateTime}", 3);

                    return newStep;
                }
                    break;
                default: // 零速模拟
                {
                    var minSimulatorTarget = _runtime.Simulation.Simulators.Min(simulator =>
                        simulator.Value.InfinitesimalAsyncSimulationTarget(ChartTime, _runtime));
                    if (minSimulatorTarget <= SimulateTime) throw new Exception("零速模拟时间步进长度不为正");

                    var newStep = new SimulationTask
                    {
                        SimulateTime = minSimulatorTarget,
                        PendingActions = Array.Empty<IGameplayAction>()
                    };
                    _runtime.Logger.DebugLog($"计算完成，零速模拟至{newStep.SimulateTime}", 3);
                    return newStep;
                }
                    break;
            }
        }

        /// <summary>
        /// 执行复合步一步模拟
        /// </summary>
        /// <param name="simulateTimeTarget"></param>
        /// <param name="chartTimeTarget"></param>
        /// <param name="direction"></param>
        /// <returns>是否发生了自动机变化</returns>
        private void SimulateCompositeStep(float simulateTimeTarget, float chartTimeTarget, SimulateDirection direction)
        {
            // TODO 应当在某个位置录制AutoPlay的模拟输入，但是目前Note管理不清晰，暂缓
            // TODO 回调，目前这里的回调只用于录制AutoPlay的模拟输入
            // TODO 应该是考虑提供修改信号的指令，在同步阶段执行
            // TODO 设计成非指令的外部机制可能更符合这个功能的意义，如果在内部则更像是第三种自动机，但这个功能本质上和自动机无关，有点写测试用例的意思，程序不能自己测自己
            // TODO 实质上是静态的，由Descriptor根据Note的定义导出这个方法，如果不考虑动态派生出的Note，则可以仅根据Map文件导出一套模拟输入，这个应该会在Editor中使用
            // TODO 考虑到动态派生出的Note自己也会有自己的Descriptor，所以这个认识依然是正确的
            // TODO   Map记录了一组Descriptor，并且全部假定单例，这是从Map导出模拟输入的根本逻辑
            // TODO     这些Descriptor具有一个根据ChartTime自动构建的方法
            // TODO     是否应当有一个平行结构，不带自动构建方法，专由指令构建

            _runtime.Simulation.InvokeBeforeSimulate(SimulateTime, simulateTimeTarget, ChartTime,
                chartTimeTarget);

            var (signalValues, signalEdgeStacks) = GetSplitSignals();

            _runtime.Logger.DebugLog($"开始模拟，目标S：{simulateTimeTarget} C：{chartTimeTarget}", 2);

            _runtime.Logger.DebugLog("开始执行复合步推进阶段", 2);
            // 执行前进模拟
            var actionQueue = new List<IGameplayAction>();
            // 并行模拟所有模拟器
            foreach (var (_, simulator) in _runtime.Simulation.Simulators)
            {
                _runtime.Logger.DebugLog($"执行模拟器{simulator.GetType()}", 3);
                var actions = direction switch
                {
                    SimulateDirection.Forward => simulator.ForwardSimulate(ChartTime, chartTimeTarget,
                        signalValues, _runtime),
                    SimulateDirection.Backward => simulator.BackwardSimulate(ChartTime, chartTimeTarget,
                        signalValues, _runtime),
                    SimulateDirection.Infinitesimal => simulator.InfinitesimalSimulate(ChartTime, signalValues,
                        _runtime),
                    _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
                };
                _runtime.Logger.DebugLog($"注册同步动作{actions?.Length ?? 0}条", 4);
                if (actions != null)
                {
                    actionQueue.AddRange(actions);
                    if (actions.Length != 0)
                        foreach (var action in actions)
                            _runtime.Logger.DebugLog($"{action.GetType()}", 5);
                }
            }

            var automatonChangeFlag = DoStepGameplayActions(actionQueue, signalEdgeStacks);

            ZeroLengthSimulate(chartTimeTarget, direction, signalValues, signalEdgeStacks, automatonChangeFlag);

            // 执行复合步尾处理
            // 更新时间
            ChartTime = chartTimeTarget;
            SimulateTime = simulateTimeTarget;
            _runtime.Logger.DebugLog("模拟完毕，更新时间", 2);
        }

        /// <summary>
        /// 零步长仿真
        /// 重复执行零步长仿真直到收敛
        /// 目前的收敛包括：信号边沿处理完毕且自动机不发生变化
        /// </summary>
        /// <param name="chartTime">仿真时间点</param>
        /// <param name="direction">仿真方向</param>
        /// <param name="snapshot">信号快照</param>
        /// <param name="pendingSignalEdges">待处理信号边沿</param>
        /// <param name="initialAutomatonChangeFlag">初始自动机变化标记，如果复合步的推进步中自动机发生变化，则应当传入true</param>
        /// <returns>最终是否修改自动机</returns>
        private void ZeroLengthSimulate(float chartTime, SimulateDirection direction, MultichannelSnapshot snapshot,
            MultichannelEdgeQueue pendingSignalEdges, bool initialAutomatonChangeFlag)
        {
            var automatonChangeFlag = initialAutomatonChangeFlag; // 自动机状态变换标记
            var firstInstantSimulate = true; // 是否为第一次进入零步长模拟阶段，此时必定执行，并且以当前信号为待决信号，而不读走后续边沿
            var step = 0;
            while (true)
            {
                step++;
                _runtime.Logger.DebugLog($"开始执行复合步第{step}次零步长推进", 2);
                List<IGameplayAction> actionQueue;
                if (automatonChangeFlag)
                {
                    _runtime.Logger.DebugLog("由于自动机状态发生变换，因此执行同步模拟", 2);
                    // 并行模拟所有模拟器
                    actionQueue = InstantSimulateAllSimulators(chartTime, direction, snapshot);
                }
                else if (firstInstantSimulate)
                {
                    // TODO 这里的原始设计是否是确保在自动机最后一次变换后再同步一次
                    // TODO 如果是这个逻辑，信号变换是否也需要？并且是否需要在自动机变换后恢复firstInstantSimulate的置位

                    _runtime.Logger.DebugLog("自动机状态未发生变换，第一次处理信号", 2);
                    // 并行模拟所有模拟器
                    actionQueue = InstantSimulateAllSimulators(chartTime, direction, snapshot);
                    firstInstantSimulate = false;
                }
                else if (pendingSignalEdges.TryDequeue(out var channelName, out var signalId, out var edge))
                {
                    _runtime.Logger.DebugLog("自动机状态未发生变换，有待决信号", 2);

                    // 根据边沿更新信号
                    snapshot.Set(channelName, signalId, edge.Value);
                    _runtime.Logger.DebugLog("读出待决信号", 3);

                    // 并行模拟所有模拟器
                    actionQueue = InstantSimulateAllSimulators(chartTime, direction, snapshot);
                }
                else // 所有边沿处理完成，并且没有自动机状态变换，则退出循环
                {
                    _runtime.Logger.DebugLog("自动机状态未发生变换，无待决信号", 2);
                    break;
                }

                automatonChangeFlag = DoStepGameplayActions(actionQueue, pendingSignalEdges);
            }
        }

        /// <summary>
        /// 对所有仿真器执行瞬时仿真
        /// </summary>
        /// <returns></returns>
        private List<IGameplayAction> InstantSimulateAllSimulators(float chartTime, SimulateDirection direction,
            MultichannelSnapshot snapshot)
        {
            var actionQueue = new List<IGameplayAction>();
            foreach (var (_, simulator) in _runtime.Simulation.Simulators)
            {
                _runtime.Logger.DebugLog($"执行模拟器{simulator.GetType()}", 3);
                var actions = simulator.InstantSimulate(chartTime, direction, snapshot, _runtime);
                _runtime.Logger.DebugLog($"注册同步动作{actions?.Length ?? 0}条", 4);
                if (actions != null)
                {
                    actionQueue.AddRange(actions);
                    if (actions.Length != 0)
                        foreach (var action in actions)
                            _runtime.Logger.DebugLog($"{action.GetType()}", 5);
                }
            }

            return actionQueue;
        }

        /// <summary>
        /// 执行仿真步内的GameplayAction
        /// </summary>
        /// <param name="actionQueue">Action队列</param>
        /// <param name="edgeQueue"></param>
        /// <returns>是否更新了自动机</returns>
        private bool DoStepGameplayActions(List<IGameplayAction> actionQueue, MultichannelEdgeQueue edgeQueue)
        {
            var automatonChangeFlag = false;
            // 执行动作
            for (var i = 0; i < actionQueue.Count; i++)
            {
                _runtime.Logger.DebugLog($"执行同步动作{i}，{actionQueue[i].GetType()}", 3);
                actionQueue[i].DoAction(_runtime, edgeQueue);

                // 如果有自动机状态变换，则设置变换标记
                if (actionQueue[i].ChangeAutomaton)
                {
                    _runtime.Logger.DebugLog("该同步动作触发自动机状态变换", 4);
                    automatonChangeFlag = true;
                }
            }

            return automatonChangeFlag;
        }

        /// <summary>
        /// 执行尾独立仿真
        /// </summary>
        private void LateIndependentSimulate(SimulateDirection direction, MultichannelSnapshot snapshot)
        {
            // 暂时忽略同步动作，固定为前向，无信号
            foreach (var (_, simulator) in _runtime.Simulation.LateIndependentSimulators)
            {
                _runtime.Logger.DebugLog($"执行模拟器{simulator.GetType()}", 3);
                // TODO 这里的方向可能需要传入
                simulator.InstantSimulate(ChartTime, direction, snapshot, _runtime);
            }
        }

        /// <summary>
        /// 尝试接收仿真任务
        /// 如果当前的ChartTime和仿真任务相同，则清空仿真任务、执行挂起动作并尝试接收仿真目标
        /// </summary>
        /// <param name="checkSimulateTime">为true则检查仿真时间相等，否则检查谱面时间相等</param>
        /// <param name="popUntilTarget">接收仿真动作的弹栈截止目标，如果弹出该仿真目标，则停止进一步接收。为null则不设置截止目标</param>
        private void TryAcceptSimulationTask(bool checkSimulateTime, SimulationTarget popUntilTarget = null)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if ((checkSimulateTime && SimulateTime == NowSimulationTask.SimulateTime) ||
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                (!checkSimulateTime && ChartTime == NowSimulationTask.ChartTime))
            {
                _runtime.Logger.DebugLog("当前模拟步骤完成，执行挂起动作", 1);
                // 执行挂起动作
                Array.ForEach(NowSimulationTask.PendingActions, a => a.DoAction(_runtime, null));
                // 清除已完成的仿真任务
                NowSimulationTask = null;
                _runtime.Logger.DebugLog("当前模拟步骤完全完成，清除，并检查是否完成模拟目标", 1);

                // 尝试接收仿真目标
                TryAcceptSimulationTarget(popUntilTarget);
            }
        }

        /// <summary>
        /// 尝试接收仿真目标
        /// 如果当前的ChartTime和仿真目标相同，则弹栈并执行挂起动作
        /// 尝试连续弹栈，直到栈空或弹出截止目标
        /// </summary>
        /// <param name="popUntil">弹栈截止目标，如果弹出该仿真目标，则停止进一步接收。为null则不设置截止目标</param>
        private void TryAcceptSimulationTarget(SimulationTarget popUntil = null)
        {
            // 如果达成目标，则弹目标栈，并执行对应操作
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            while (ChartTime == _simulationTargetStack.Top().ChartTime)
            {
                _runtime.Logger.DebugLog("模拟目标完成，执行挂起动作", 2);
                Array.ForEach(_simulationTargetStack.Top().PendingActions, a => a.DoAction(_runtime, null));

                _runtime.Logger.DebugLog("模拟目标完全完成，弹出", 2);
                var lastPop = _simulationTargetStack.Pop();
                if (lastPop == popUntil)
                {
                    _runtime.Logger.DebugLog($"模拟目标完成，退出模拟", 1);
                    break;
                }

                if (_simulationTargetStack.Count == 0)
                {
                    _runtime.Logger.DebugLog("全部模拟目标完成，栈空", 3);
                    break;
                }

                _runtime.Logger.DebugLog(
                    $"切换至上一模拟目标{_simulationTargetStack.Top().ChartTime}，倍速{_simulationTargetStack.Top().SimulateSpeed}",
                    3);
            }
        }

        #region 对调试器暴露的信息

        public void ChangeSimulateSpeed(float newSimulateSpeed)
        {
            // TODO 应当刷新所有待决
            if (_simulationTargetStack.Top() != null) _simulationTargetStack.Top().SimulateSpeed = newSimulateSpeed;
        }

        #endregion

        /// <summary>
        /// 清空已计算的模拟步
        /// 用于运行时修改了模拟内容时的重新计算
        /// </summary>
        public void CleanSimulationStep()
        {
            NowSimulationTask = null;
        }
    }
}