using System;
using System.Collections.Generic;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Simulators;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class HistoryStack
    {
        private readonly Stack<IHistoryItem> _stack;

        public HistoryStack(Injector injector)
        {
            FieldInitialize(injector);
            _stack = new Stack<IHistoryItem>();
        }

        public HistoryStack()
        {
            _stack = new Stack<IHistoryItem>();
        }
        
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        /// <summary>
        ///     状态恢复时间
        /// </summary>
        public float RevertTime => _stack.TryPeek(out var topHistory) ? topHistory.ChartTime : float.MinValue;

        public void Push(IHistoryItem historyItem)
        {
            _stack.Push(historyItem);
        }

        /// <summary>
        ///     弹栈并执行动作，直到目标时间
        /// </summary>
        /// <param name="targetChartTime"></param>
        /// <param name="automaton"></param>
        /// <param name="simulateDirection"></param>
        /// <param name="inputGraph"></param>
        /// <param name="timeStack"></param>
        public IGameplayAction[] PopUntil(float targetChartTime, SignalTsiga automaton, SimulateDirection simulateDirection,
            InputGraph inputGraph, TimeStack timeStack)
        {
            List<IGameplayAction> actions = new();
            while (_stack.TryPeek(out var historyItem) && historyItem.ChartTime >= targetChartTime)
            {
                if (historyItem is TimeStackPopHistory)
                    actions.Add(new UpdatePendingDetectionCondition(automaton, simulateDirection));

                _stack.Pop().Revert(inputGraph, timeStack);
            }
            
            return actions.ToArray();
        }
    }

    public interface IHistoryItem
    {
        public float ChartTime { get; }

        public void Revert(InputGraph inputGraph, TimeStack timeStack);
    }
}