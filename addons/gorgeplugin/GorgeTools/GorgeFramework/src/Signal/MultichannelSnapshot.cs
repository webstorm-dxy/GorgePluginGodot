using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;

namespace Gorge.GorgeFramework.Signal
{
    /// <summary>
    /// 多通道信号快照
    /// 包含一个时刻的全部信号值
    /// </summary>
    public class MultichannelSnapshot : Dictionary<string, ChannelSnapshot>
    {
        /// <summary>
        /// 设置信号值
        /// 自动解决新建Channel问题
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="signalId"></param>
        /// <param name="value"></param>
        public void Set(string channelName, int signalId, GorgeObject value)
        {
            if (!TryGetValue(channelName, out var channelSnapshot))
            {
                channelSnapshot = new ChannelSnapshot();
                Add(channelName, channelSnapshot);
            }

            channelSnapshot[signalId] = value;
        }
    }
}