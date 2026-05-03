using System;
using System.Collections.Generic;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeFramework.Automaton;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Simulators;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    /// <summary>
    ///     输入图-时序栈自动机
    /// </summary>
    public partial class SignalTsiga
    {
        #region Natie实现

        private SignalTsiga(Injector injector, Note note, TimeStack timeStack, InputGraph inputGraph,
            HistoryStack historyStack)
        {
            FieldInitialize(injector);

            _note = note;
            _inputGraph = inputGraph;
            _timeStack = timeStack;
            _historyStack = historyStack;
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        public virtual partial string GetState()
        {
            return State;
        }

        #endregion


        /*
         * 输入列：一列输入，本身是正则的，或者可能弱于正则，表达和/或概念；主要解决输入识别问题，里面的时间是影响输入识别本质的时间
         *   输入定义为：输入内容,输入区域,输入截止时间点,是否消耗，是否响应，是否终结
         *     输入时间有两种模式：时内触发、时内保持；时内保持即时内触发一个非则进入失败
         * 时序栈：一个栈，可以在输入状态转换时操作栈中内容，随着时间自动弹栈；主要解决判定结果问题
         *   栈元素定义为：时间点，响应结果，是否响应，是否终结
         *     当时间超过栈顶时间点时，自动弹出栈顶
         * 响应和停机规则：
         *   当输入列失败时，响应一个Miss，停机
         *   当输入列推进或时序栈弹栈时，做一次检查：如果输入列位置和时序栈栈顶都是响应状态，则按栈顶的响应结果做一次响应；如果都是停机状态，则停机接收（不响应Miss）
         */

        #region 输入图时序栈结构

        /// <summary>
        ///     输入图，以列表存储
        /// </summary>
        private readonly InputGraph _inputGraph;

        /// <summary>
        ///     时序栈
        /// </summary>
        private readonly TimeStack _timeStack;

        /// <summary>
        ///     历史栈
        /// </summary>
        private readonly HistoryStack _historyStack;

        #endregion

        /// <summary>
        ///     信号状态
        /// </summary>
        private readonly Dictionary<string, Dictionary<int, GorgeObject>> _signalState = new();

        private readonly Note _note;

        public float ForwardStateChangeTime()
        {
            if (_timeStack.Accept && _inputGraph.Accept) return float.MaxValue;

            return Math.Min(_inputGraph.StateTimeout, _timeStack.PopTime);
        }

        public IGameplayAction[] ForwardStateChange(float chartTime)
        {
            if (_timeStack.Accept && _inputGraph.Accept) return null;

            var actions = new List<IGameplayAction>();
            var action1 = PopUntil(chartTime, SimulateDirection.Forward);
            if (action1 != null) actions.AddRange(action1);

            var action2 = TimeoutUntil(chartTime, SimulateDirection.Forward);
            if (action2 != null) actions.AddRange(action2);

            return actions.ToArray();
        }

        public float BackwardStateChangeTime()
        {
            return _historyStack.RevertTime;
        }

        public IGameplayAction[] BackwardStateChange(float chartTime)
        {
            return _historyStack.PopUntil(chartTime, this, SimulateDirection.Backward, _inputGraph, _timeStack);
        }

        public List<SignalDetectionCondition> GetDetectionConditions(SimulateDirection simulateDirection)
        {
            // 倒转不检测任何输入
            if (simulateDirection is SimulateDirection.Backward) return new List<SignalDetectionCondition>();

            var state = _inputGraph.State;
            if (state == null) return new List<SignalDetectionCondition>();

            // 必须判断停机状态，因为允许停在中间，而不是最终状态
            if (_inputGraph.Accept && _timeStack.Accept) return new List<SignalDetectionCondition>();

            var filter = state.filter;

            var list = new List<SignalDetectionCondition>();

            for (var i = 0; i < filter.conditionTypes.length; i++)
            {
                var inputType = filter.conditionTypes.Get(i);
                var condition = new SignalDetectionCondition
                {
                    Priority = (ObjectArray) filter.priority.Invoke(Array.Empty<object>()),
                    CanDetect = channelName => filter.CanDetect(channelName),
                    Detect = (channelName, id, value) =>
                    {
                        #region 维护信号记录

                        GorgeObject lastValue;

                        if (!_signalState.TryGetValue(channelName, out var channelState))
                        {
                            channelState = new Dictionary<int, GorgeObject>();
                            _signalState[channelName] = channelState;
                            lastValue = null;
                        }
                        else
                        {
                            lastValue = channelState.GetValueOrDefault(id);
                        }

                        channelState[id] = value;

                        #endregion

                        var accept = filter.Detect(channelName, id, inputType, value, lastValue);

                        return new Tuple<bool, bool>(accept, accept ? filter.acceptConsume : filter.denyConsume);
                    },
                    Accept = (_, _, _, chartTime) =>
                    {
                        if (filter.timeMode is TimeMode.CatchBefore) // 满足则进入接收分支
                            return DoEdgeRespond(_inputGraph.GoAcceptEdge(chartTime, _historyStack), chartTime,
                                simulateDirection);
                        // 满足则继续等待
                        return Array.Empty<IGameplayAction>();
                    }
                };


                list.Add(condition);
            }

            return list;
        }

        public IGameplayAction[] DetectionDeny(float denyChartTime, SimulateDirection simulateDirection)
        {
            var state = _inputGraph.State;
            if (state == null) return Array.Empty<IGameplayAction>();

            var filter = state.filter;

            if (filter.timeMode is TimeMode.CatchBefore) // 不满足则继续等待
                return Array.Empty<IGameplayAction>();
            // 不满足则进入接收分支
            return DoEdgeRespond(_inputGraph.GoDenyEdge(denyChartTime, _historyStack), denyChartTime,
                simulateDirection);
        }

        public string State => _inputGraph.ExportState;

        /// <summary>
        ///     执行响应
        /// </summary>
        /// <param name="respondChartTime"></param>
        /// <param name="simulateDirection"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private IGameplayAction[] DoRespond(float respondChartTime, SimulateDirection simulateDirection)
        {
            var automatonCommands = (ObjectArray) _note.RealObject.InvokeMethod("DoRespond", new GorgeType[]
            {
                GorgeType.String,
                GorgeType.Float,
            }, new Dictionary<GorgeType, GorgeType>(), new object[]
            {
                _timeStack.RespondMode, respondChartTime
            });

            return ConvertAutomatonCommands(automatonCommands, simulateDirection);
        }

        /// <summary>
        ///     执行拒绝
        /// </summary>
        /// <param name="denyChartTime"></param>
        /// <param name="simulateDirection"></param>
        /// <returns></returns>
        private IGameplayAction[] DoDeny(float denyChartTime, SimulateDirection simulateDirection)
        {
            var automatonCommands = (ObjectArray) _note.RealObject.InvokeMethod("DoRespond", new GorgeType[]
            {
                GorgeType.String,
                GorgeType.Float,
            }, new Dictionary<GorgeType, GorgeType>(), new object[]
            {
                "Miss", denyChartTime
            });


            return ConvertAutomatonCommands(automatonCommands, simulateDirection);
        }

        /// <summary>
        /// 将自动机指令转换为Gameplay指令
        /// </summary>
        /// <param name="automatonCommands"></param>
        /// <param name="simulateDirection"></param>
        /// <returns></returns>
        public static IGameplayAction[] ConvertAutomatonCommands(ObjectArray automatonCommands,
            SimulateDirection simulateDirection)
        {
            if (automatonCommands == null)
            {
                return Array.Empty<IGameplayAction>();
            }

            var gameplayActions = new List<IGameplayAction>();

            for (var i = 0; i < automatonCommands.length; i++)
            {
                var automatonCommand = automatonCommands.Get(i);
                if (automatonCommand.GorgeClass.Declaration.Is("GorgeFramework.DeriveElementCommand"))
                {
                    var deriveElement = DeriveElementCommand.FromGorgeObject(automatonCommand);
                    gameplayActions.Add(new DeriveElement(deriveElement.element, deriveElement.changeAutomaton,
                        simulateDirection));
                }
                else if (automatonCommand.GorgeClass.Declaration.Is("GorgeFramework.AppendSignalCommand"))
                {
                    Base.Instance.Log("追加信号功能暂时被关闭");
                    // TODO 为了保持最小移植环境，暂时注释实际功能
                    // var appendSignalCommand = AppendSignalCommand.FromGorgeObject(automatonCommand);
                    // gameplayActions.Add(new AppendSignal(appendSignalCommand.channelName, appendSignalCommand.id,
                    //     appendSignalCommand.delaySimulateTime, appendSignalCommand.value));
                }
            }

            return gameplayActions.ToArray();
        }
        
        /// <summary>
        ///     将时间栈弹栈到目标时间，同时执行响应
        /// </summary>
        /// <param name="targetChartTime"></param>
        /// <param name="simulateDirection"></param>
        /// <returns></returns>
        private IGameplayAction[] PopUntil(float targetChartTime, SimulateDirection simulateDirection)
        {
            var actionList = new List<IGameplayAction>();
            // 持续弹栈，直到目标时间
            while (_timeStack.PopTime <= targetChartTime)
            {
                if (!_timeStack.TryPop(targetChartTime, out var timeItem, _historyStack)) break;
                if (_inputGraph.StackRespond && _timeStack.RespondMode != null)
                {
                    var actions = DoRespond((float) timeItem.time.Invoke(Array.Empty<object>()), simulateDirection);
                    if (actions != null) actionList.AddRange(actions);
                }

                if (_timeStack.Accept && _inputGraph.Accept) break;
            }

            return actionList.ToArray();
        }

        /// <summary>
        ///     执行输入图的超时推进，直到目标时间，同时执行响应
        /// </summary>
        /// <param name="targetChartTime"></param>
        /// <param name="simulateDirection"></param>
        /// <returns></returns>
        private IGameplayAction[] TimeoutUntil(float targetChartTime, SimulateDirection simulateDirection)
        {
            var actionList = new List<IGameplayAction>();

            // 持续判断超时，直到目标时间
            while (_inputGraph.StateTimeout <= targetChartTime)
            {
                var edge = _inputGraph.DoTimeout(targetChartTime, _historyStack);
                if (edge == null) // 这里实际和循环的判断是重复的，可能可以减少判断次数
                    break;

                var action = DoEdgeRespond(edge, targetChartTime, simulateDirection);
                if (action != null) actionList.AddRange(action);
            }

            return actionList.ToArray();
        }

        /// <summary>
        ///     执行输入图的出边响应动作
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="targetChartTime"></param>
        /// <param name="simulateDirection"></param>  
        /// <returns></returns>
        private IGameplayAction[] DoEdgeRespond(InputGraphEdge edge, float targetChartTime,
            SimulateDirection simulateDirection)
        {
            var actions = new List<IGameplayAction>();
            // 执行边响应，认为发生了状态转换，添加更新动作
            // 由于认为边动作是状态变换的附属，所以先发生状态变换，后执行边动作，所以先添加状态变换导致的刷新待决
            actions.Add(new UpdatePendingDetectionCondition(this, simulateDirection));
            // 如果要求响应，则做一次响应
            if (edge.edgeRespond)
            {
                // 如果进入停机，则响应拒绝
                if (edge.deny)
                {
                    var action = DoDeny(targetChartTime, simulateDirection);
                    if (action != null) actions.AddRange(action);
                }
                else // 如果不进入停机，则根据TimeStack响应
                {
                    if (_timeStack.RespondMode != null)
                    {
                        var action = DoRespond(targetChartTime, simulateDirection);
                        if (action != null) actions.AddRange(action);
                    }
                }
            }

            // 完成栈操作
            edge.stackAction?.Invoke(_note, _timeStack, targetChartTime, _historyStack);
            // 进行一次时间推进，因为栈操作可能使得早于当前的时间点压栈
            var popAction = ForwardStateChange(targetChartTime);
            if (popAction != null) actions.AddRange(popAction);

            return actions.ToArray();
        }
    }
}