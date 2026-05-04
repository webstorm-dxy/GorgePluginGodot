using System;
using System.Collections.Generic;
using System.Linq;
using Gorge.GorgeFramework.Automaton;
using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Signal;
using Gorge.Native.GorgeFramework;
using Math = System.Math;

// using Unity.VisualScripting;

namespace Gorge.GorgeFramework.Simulators
{
    /// <summary>
    ///     精准自动机竞争器
    /// </summary>
    public class PreciseAutomatonSimulator : ISimulator
    {
        public float ForwardAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            if (runtime.Automaton.Automatons.Count == 0) return float.MaxValue;
            // 计算最早的自动机状态转换时间，谱面时间
            var earliestAutomatonTime = runtime.Automaton.Automatons.Min(a => a.ForwardStateChangeTime());
            // 自动机的状态转换时间与当前ChartTime无关，所以可能返回早于当前的值，处理这个异常的责任放在这里
            return earliestAutomatonTime;
        }

        public float BackwardAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            if (runtime.Automaton.Automatons.Count == 0) return float.MinValue;
            var latestAutomatonTime = runtime.Automaton.Automatons.Max(a => a.BackwardStateChangeTime());
            return latestAutomatonTime;
        }

        public float InfinitesimalAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            return float.MaxValue;
        }

        public IGameplayAction[] ForwardSimulate(float chartTimeFrom, float chartTimeTo,
            MultichannelSnapshot signalSnapshot, GorgeSimulationRuntime runtime)
        {
            /*
             * TODO
             * 目前认为这里的状态切换是同时打包执行的，也可以像信号沿一样展开到多个阶段中
             * 前向模拟执行一个，然后在零步长模拟中反复展开
             * 需要规定一个大顺序：超时导致的状态跳转整体优先于边沿
             * 各个超时状态跳转之间也需要有优先顺序，比如自动机编号序
             * 如果超时跳转触发信号改变，则要将信号压栈并插队处理
             *
             * 或者也像信号沿一样拉出来单独管理？
             *   可能逻辑优先级还比信号沿要高一层，信号无法引起时变，所以无法通过信号产生新的状态超时
             *   但是状态超时的动作可以修改信号内容，所以可以引起新的信号沿
             *   大循环是循环处理所有的超时，并且在最后附上所有的已有信号沿
             * 信号引起时变是跳转机制，这个需要单独考虑
             *   互相影响的环总要在某一处拆开，似乎拆在此处是一个比较好的选择
             *
             * 这种展开似乎与信号沿展开是同级的，但是影响相对比较小，因为可以要求谱师不这样写，但是信号沿由玩家产生，不能要求玩家不这样打
             */

            var actionList = new List<IGameplayAction>();
            // 计算最早的自动机状态转换时间，谱面时间
            foreach (var automaton in runtime.Automaton.Automatons)
            {
                var action = automaton.ForwardStateChange(chartTimeTo);
                if (action != null) actionList.AddRange(action);
            }

            return actionList.ToArray();
        }

        public IGameplayAction[] BackwardSimulate(float chartTimeFrom, float chartTimeTo, MultichannelSnapshot signals,
            GorgeSimulationRuntime runtime)
        {
            var actionList = new List<IGameplayAction>();
            // 计算最早的自动机状态转换时间，谱面时间
            foreach (var automaton in runtime.Automaton.Automatons)
            {
                var action = automaton.BackwardStateChange(chartTimeTo);
                if (action != null) actionList.AddRange(action);
            }

            return actionList.ToArray();
        }

        public IGameplayAction[] InfinitesimalSimulate(float chartTimeTo, MultichannelSnapshot signals,
            GorgeSimulationRuntime runtime)
        {
            return Array.Empty<IGameplayAction>();
        }

        public IGameplayAction[] InstantSimulate(float chartTimeTo, SimulateDirection direction,
            MultichannelSnapshot signalSnapshot, GorgeSimulationRuntime runtime)
        {
            // TODO 反向暂不对复合步零步长响应
            // 因为这会使得反转进入的自动机被正向推进一次
            // 需要做专门设计才能支持，暂时没有考虑
            if (direction is SimulateDirection.Backward)
            {
                return Array.Empty<IGameplayAction>();
            }

            /*
             * 针对当前信号状态执行一轮竞争响应
             * 1.收集当前所有自动机的待决测量情况
             * 2.逐信号响应测量，并处理遮蔽
             *   2.1 如果测量接受，则从待决总表中删除这个自动机的所有测量（因为是或关系，满足一个就均视为已决）
             * 3.遍历完成后，对所有待决测量响应测量拒绝
             */
            // 待决测量总表
            var detectionConditions =
                new Dictionary<SignalTsiga, List<SignalDetectionCondition>>(
                    runtime.Automaton.PendingDetectionConditions);
            // foreach (var (key, item) in runtime.Automaton.PendingDetectionConditions)
            // {
            //     detectionConditions[key] = item;
            // }

            var gameActions = new List<IGameplayAction>();

            foreach (var (channelName, channelSignal) in signalSnapshot)
            {
                foreach (var (id, value) in channelSignal)
                {
                    // 获取当前的待决测量条件
                    var allDetectionConditions = new List<SignalDetectionCondition>();
                    foreach (var (_, conditions) in detectionConditions) allDetectionConditions.AddRange(conditions);

                    SortDetectionConditionsByPriority(allDetectionConditions, value);

                    // 消耗标记
                    var consumeFlag = false;

                    // 已经接受的情况集合
                    var acceptedList = new List<SignalDetectionCondition>();

                    // 遍历排序后的表
                    foreach (var condition in allDetectionConditions)
                    {
                        // 如果信号不满足检测条件，则不处理
                        if (!condition.CanDetect(channelName)) continue;

                        // 如果当前情况已经被接收，则不处理
                        if (acceptedList.Contains(condition)) continue;

                        // 如果已经消耗，则传入null
                        var (accept, consume) = condition.Detect(channelName, id, consumeFlag ? null : value);

                        if (accept) // 如果可接收，则立即执行接收
                        {
                            var action = condition.Accept(channelName, id, consumeFlag ? null : value, chartTimeTo);
                            if (action != null) gameActions.AddRange(action);

                            // 如果要求消耗，则设置消耗标记
                            if (consume) consumeFlag = true;

                            // 找到当前情况所属的自动机，向接收表中添加该自动机的所有或关系情况，并且从待决表中删除
                            SignalTsiga targetAutomaton = null;

                            foreach (var (automaton, conditionList) in detectionConditions)
                                if (conditionList.Contains(condition))
                                {
                                    targetAutomaton = automaton;
                                    acceptedList.AddRange(conditionList);
                                    break;
                                }

                            // TODO 这里的逻辑不对，Condition的接收并不意味着发生了状态转换，是否真的有转换只有自动机自己知道，这取决于汇报出的Condition的语义如何

                            // 从待决表中删除
                            if (targetAutomaton != null) detectionConditions.Remove(targetAutomaton);
                        }
                        // 如果不可接受，则等待其他信号的处理结果
                    }
                }
            }

            // 对所有仍然待决的自动机响应拒绝
            foreach (var deniedAutomaton in detectionConditions.Keys)
            {
                var action = deniedAutomaton.DetectionDeny(chartTimeTo, SimulateDirection.Infinitesimal);
                if (action != null) gameActions.AddRange(action);
            }

            return gameActions.ToArray();
        }

        private static void SortDetectionConditionsByPriority(List<SignalDetectionCondition> detectionConditions,
            GorgeObject signalValue)
        {
            var prioritizedConditions = new List<PrioritizedDetectionCondition>(detectionConditions.Count);
            for (var i = 0; i < detectionConditions.Count; i++)
            {
                var condition = detectionConditions[i];
                prioritizedConditions.Add(new PrioritizedDetectionCondition(condition,
                    CapturePriorityValues(condition, signalValue), i));
            }

            prioritizedConditions.Sort(ComparePriorityValues);

            detectionConditions.Clear();
            foreach (var prioritizedCondition in prioritizedConditions)
                detectionConditions.Add(prioritizedCondition.Condition);
        }

        private static float[] CapturePriorityValues(SignalDetectionCondition condition, GorgeObject signalValue)
        {
            var values = new float[condition.Priority.length];
            for (var i = 0; i < values.Length; i++)
                values[i] = (float) ((Priority) condition.Priority.Get(i)).getPriority.Invoke(signalValue);

            return values;
        }

        private static int ComparePriorityValues(PrioritizedDetectionCondition a, PrioritizedDetectionCondition b)
        {
            var length = Math.Max(a.PriorityValues.Length, b.PriorityValues.Length);
            for (var i = 0; i < length; i++)
            {
                var aPriority = a.PriorityValues.Length > i ? a.PriorityValues[i] : 0;
                var bPriority = b.PriorityValues.Length > i ? b.PriorityValues[i] : 0;
                var result = aPriority.CompareTo(bPriority);

                if (result != 0) return result;
            }

            return a.OriginalIndex.CompareTo(b.OriginalIndex);
        }

        private readonly struct PrioritizedDetectionCondition
        {
            public readonly SignalDetectionCondition Condition;
            public readonly int OriginalIndex;
            public readonly float[] PriorityValues;

            public PrioritizedDetectionCondition(SignalDetectionCondition condition, float[] priorityValues,
                int originalIndex)
            {
                Condition = condition;
                PriorityValues = priorityValues;
                OriginalIndex = originalIndex;
            }
        }
    }
}
