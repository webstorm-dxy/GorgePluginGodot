#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;
using JetBrains.Annotations;

namespace Gorge.Native.GorgeFramework
{
    public partial class InputGraph
    {
        /*
         * 输入图由若干状态组成，表达对一组动作的接收
         *   每个状态下捕获一组输入，这些输入之间是或关系，任一满足则整组满足
         *   每个状态有两条出边，分别在捕获成功和捕获失败时执行
         *     执行一条出边，会跳转到所指示的状态，并且完成附带动作
         *     类似于是否接收、是否响应之类的状态调整，可能在两条出边中的变化是不一样的
         *       实际例子：考虑一个断掉后可以重新接回的hold类note
         *         状态图是Tap->Hold->Release
         *         但如果Tap捕获失败，而Hold在时间段内正确出现，则恢复捕获，所以Tap是否成功都是跳转到Hold
         *         如果Tap捕获成功，应当产生响应，而捕获失败则不产生
         *       从逻辑上可以认为：
         *         维护状态是出边附带动作的一部分
         *         或认为，两条出边指向两个不同的状态，严格图定义需要微调
         * 目前使用列表+指针的结构存储图，可能考虑替换其他数据结构
         *   出边存储为跳转步数
         */

        private readonly List<InputGraphState> _states;

        public int StateCount => _states.Count;

        public int InputPointer => _inputPointer;

        private int _inputPointer;

        public InputGraph(Injector injector, ObjectArray states, bool accept, bool stackRespond, int inputPointer,
            string exportState)
        {
            FieldInitialize(injector);
            _states = new List<InputGraphState>();
            for (var i = 0; i < states.length; i++)
            {
                _states.Add((InputGraphState) states.Get(i));
            }

            Accept = accept;
            StackRespond = stackRespond;
            _inputPointer = inputPointer;
            ExportState = exportState;
        }

        /// <summary>
        ///     输入图
        /// </summary>
        /// <param name="states">状态表</param>
        /// <param name="inputPointer">状态指针初始值</param>
        /// <param name="accept">接收状态初始值</param>
        /// <param name="stackRespond">弹栈响应初始值</param>
        /// <param name="exportState">导出状态初始值</param>
        public InputGraph(InputGraphState[] states, int inputPointer = 0,
            bool accept = false,
            bool stackRespond = false,  string? exportState = "Waiting")
        {
            _states = states.ToList();
            Accept = accept;
            StackRespond = stackRespond;
            ExportState = exportState;
            _inputPointer = inputPointer;
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        /// <summary>
        ///     当前选中的状态
        ///     如果在界外则返回null
        /// </summary>
        public InputGraphState? State =>
            _inputPointer >= 0 && _inputPointer < _states.Count ? _states[_inputPointer] : null;

        /// <summary>
        ///     状态超时时间
        /// </summary>
        public float StateTimeout =>
            State != null ? (float) State.filter.endTime.Invoke(Array.Empty<object>()) : float.MaxValue;

        public InputGraphEdge DoTimeout(float targetChartTime, HistoryStack historyStack)
        {
            var state = State;
            if (state == null) return null;

            var filter = state.filter;
            if ((float) filter.endTime.Invoke(Array.Empty<object>()) <= targetChartTime)
                return filter.timeMode is TimeMode.CatchBefore
                    ?
                    // 执行拒绝
                    GoDenyEdge(targetChartTime, historyStack)
                    :
                    // 执行接收
                    GoAcceptEdge(targetChartTime, historyStack);

            return null;
        }

        /// <summary>
        ///     进入接收边，只执行状态修改，不执行动作
        /// </summary>
        /// <returns>接受边对象</returns>
        public virtual partial InputGraphEdge GoAcceptEdge(float chartTime, HistoryStack historyStack)
        {
            var state = State;
            if (state == null) return null;

            var edge = state.acceptedEdge;
            GoEdge(chartTime, edge, historyStack);

            return edge;
        }

        /// <summary>
        ///     进入拒绝边，只执行状态修改，不执行动作
        /// </summary>
        /// <returns>接受边对象</returns>
        public virtual partial InputGraphEdge GoDenyEdge(float chartTime, HistoryStack historyStack)
        {
            var state = State;
            if (state == null) return null;

            var edge = state.deniedEdge;
            GoEdge(chartTime, edge, historyStack);
            return edge;
        }

        /// <summary>
        ///     进入出边，只执行状态修改，不执行动作
        /// </summary>
        /// <param name="chartTime"></param>
        /// <param name="edge"></param>
        /// <param name="historyStack"></param>
        private void GoEdge(float chartTime, InputGraphEdge edge, HistoryStack historyStack)
        {
            historyStack.Push(new InputGraphGoEdgeHistory(chartTime, _inputPointer, Accept, StackRespond,
                ExportState));

            Accept = edge.accept;
            StackRespond = edge.stackRespond;
            if (edge.exportState != null)
                ExportState = edge.exportState;

            // 执行跳转
            if (!edge.deny)
            {
                _inputPointer += edge.jump;
            }
            else
            {
                ExportState = "Denied";
                _inputPointer = -1;
            }
        }

        public void RevertGoEdge(int pointerBeforeGo, bool acceptBeforeGo, bool stackRespondBeforeGo,
            string exportStateBeforeGo)
        {
            Accept = acceptBeforeGo;
            StackRespond = stackRespondBeforeGo;
            _inputPointer = pointerBeforeGo;
            ExportState = exportStateBeforeGo;
        }

        #region 状态

        /// <summary>
        ///     接收模式
        /// </summary>
        public bool Accept { get; private set; }

        /// <summary>
        ///     栈响应模式
        /// </summary>
        public bool StackRespond { get; private set; }

        /// <summary>
        ///     导出状态
        /// </summary>
        public string? ExportState { get; private set; }

        #endregion
    }

    public class InputGraphGoEdgeHistory : IHistoryItem
    {
        private readonly int _pointerBeforeGo;
        private readonly bool _acceptBeforeGo;
        private readonly bool _stackRespondBeforeGo;
        private readonly string _exportStateBeforeGo;

        public InputGraphGoEdgeHistory(float chartTime, int pointerBeforeGo, bool acceptBeforeGo,
            bool stackRespondBeforeGo, string exportStateBeforeGo)
        {
            _pointerBeforeGo = pointerBeforeGo;
            _acceptBeforeGo = acceptBeforeGo;
            _stackRespondBeforeGo = stackRespondBeforeGo;
            _exportStateBeforeGo = exportStateBeforeGo;
            ChartTime = chartTime;
        }

        public float ChartTime { get; }

        void IHistoryItem.Revert(InputGraph inputGraph, TimeStack timeStack)
        {
            inputGraph.RevertGoEdge(_pointerBeforeGo, _acceptBeforeGo, _stackRespondBeforeGo, _exportStateBeforeGo);
        }
    }
}