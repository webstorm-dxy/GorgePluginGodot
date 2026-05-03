using System.Collections.Generic;
using System.Linq;
using Gorge.GorgeFramework.Automaton;
using Gorge.GorgeFramework.Input;
using Gorge.GorgeFramework.Signal;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Runtime.Environment
{
    /// <summary>
    /// 管理运行时自动机信息
    /// </summary>
    public class AutomatonManager
    {
        public void RuntimeInitialize()
        {
            Automatons = new List<SignalTsiga>();
            PendingDetectionConditions = new Dictionary<SignalTsiga, List<SignalDetectionCondition>>();
            InputSignals = new MultichannelSplit();
            _nextSignalId = 1;
        }

        public void RuntimeDestruct()
        {
            Automatons = null;
            PendingDetectionConditions = null;
            InputSignals = null;
            _nextSignalId = default;
        }

        #region 自动机

        /// <summary>
        ///     信号自动机表
        /// </summary>
        public List<SignalTsiga> Automatons;

        /// <summary>
        ///     待决测量情况表
        /// </summary>
        public Dictionary<SignalTsiga, List<SignalDetectionCondition>> PendingDetectionConditions;

        #endregion
        
        #region 信号

        /// <summary>
        ///     输入信号表。
        ///     Key是信号编号，Value是信号内容
        /// </summary>
        public MultichannelSplit InputSignals;

        private int _nextSignalId;

        /// <summary>
        ///     分配新的信号编号。
        ///     一次性使用，只是保证历史不重复
        /// </summary>
        /// <returns></returns>
        public int GetDisposableSignalId()
        {
            var id = _nextSignalId;
            _nextSignalId++;
            return id;
        }

        /// <summary>
        ///     获取信号表的切片，右包含，长度为零时包含该定点
        /// </summary>
        /// <param name="fromSimulateTime"></param>
        /// <param name="toSimulateTime"></param>
        /// <returns></returns>
        public MultichannelSplit SplitInputSignals(float fromSimulateTime,
            float toSimulateTime)
        {
            // TODO 信号的存储和切片有很大优化空间，先争取逻辑通
            var splitSignals = new MultichannelSplit();
            foreach (var (channelName, channelSignals) in InputSignals)
            {
                if (!splitSignals.ContainsKey(channelName))
                {
                    splitSignals.Add(channelName, new ChannelSplit());
                }

                foreach (var (id, signal) in channelSignals)
                {
                    var splitSignal = signal.Split(fromSimulateTime, toSimulateTime);
                    if (splitSignal != null) splitSignals[channelName][id] = splitSignal;
                }
            }


            return splitSignals;
        }

        /// <summary>
        ///     计算执行时间点后（不包含）的最早的边沿时间
        /// </summary>
        /// <returns></returns>
        public float GetInputSignalEarliestEdgeTimeAfter(float simulateTime)
        {
            if (InputSignals.Count == 0) return float.MaxValue;

            return InputSignals.Values.Min(channelSignals =>
            {
                if (channelSignals.Count == 0) return float.MaxValue;
                return channelSignals.Values.Min(fragment =>
                {
                    if (fragment.Edges.Count == 0) return float.MaxValue;
                    return fragment.Edges.Min(edge => edge.Time > simulateTime ? edge.Time : float.MaxValue);
                });
            });
        }

        /// <summary>
        /// 追加信号边沿
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="signalId"></param>
        /// <param name="simulateTime"></param>
        /// <param name="value"></param>
        /// <returns>是否真的追加了边沿，如果追加值和已有值一致，切不存在长度延续，则返回false</returns>
        public bool AddSignalEdge(string channelName, int signalId, float simulateTime, GorgeObject value)
        {
            if (!InputSignals.TryGetValue(channelName, out var channel))
            {
                channel = new ChannelSplit();
                InputSignals.Add(channelName, channel);
            }

            if (!channel.TryGetValue(signalId, out var signal))
            {
                signal = new Fragment<GorgeObject>()
                {
                    SignalId = signalId,
                    StartTime = simulateTime,
                    EndTime = float.PositiveInfinity,
                    StartValue = value,
                    Edges = new List<Edge<GorgeObject>>()
                };
                channel.Add(signalId, signal);
            }
            else
            {
                if (signal.Edges.Count > 0 && signal.Edges[^1].Value.Equals(value))
                {
                    // 信号值一致情况下不追加边沿
                    // 如果已有信号不早于当前位置，则不延续型号
                    if (signal.EndTime >= simulateTime)
                    {
                        return false;
                    }

                    // 否则延续到当下0
                    signal.EndTime = simulateTime;
                    return true;
                }
                else
                {
                    signal.EndTime = simulateTime;
                    // 信号值不一致情况下追加边沿
                    signal.Edges.Add(new Edge<GorgeObject>()
                    {
                        Time = simulateTime,
                        Value = value
                    });
                }
            }

            return true;
        }

        #endregion
    }
}