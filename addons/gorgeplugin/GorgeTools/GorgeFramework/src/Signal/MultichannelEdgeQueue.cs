using System.Collections.Generic;
using System.Linq;
using Gorge.GorgeFramework.Input;
using Gorge.GorgeLanguage.Objective;

namespace Gorge.GorgeFramework.Signal
{
    /// <summary>
    /// 多通道信号边沿队列
    /// 包含一组边沿
    /// Key是频道名
    /// </summary>
    public class MultichannelEdgeQueue : Dictionary<string, ChannelEdgeQueue>
    {
        public int EdgeCount()
        {
            return Values.Sum(channelEdgeQueue => channelEdgeQueue.EdgeCount());
        }

        public bool TryDequeue(out string channelName, out int signalId, out Edge<GorgeObject> edge)
        {
            channelName = default;
            signalId = default;
            edge = default;
            if (EdgeCount() == 0)
            {
                return false;
            }

            foreach (var (name, channelEdgeQueue) in this)
            {
                if (channelEdgeQueue.TryDequeue(out signalId, out edge))
                {
                    channelName = name;
                    return true;
                }
            }

            return false;
        }

        public void Enqueue(string channelName, int signalId, Edge<GorgeObject> edge)
        {
            if (!TryGetValue(channelName, out var channelEdgeQueue))
            {
                channelEdgeQueue = new ChannelEdgeQueue();
                Add(channelName, channelEdgeQueue);
            }

            channelEdgeQueue.Enqueue(signalId, edge);
        }
    }
}