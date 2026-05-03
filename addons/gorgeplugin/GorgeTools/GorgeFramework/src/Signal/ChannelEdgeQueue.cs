using System.Collections.Generic;
using System.Linq;
using Gorge.GorgeFramework.Input;
using Gorge.GorgeLanguage.Objective;

namespace Gorge.GorgeFramework.Signal
{
    /// <summary>
    /// 单通道信号边沿队列
    /// 包含一组边沿
    /// Key是信号id
    /// </summary>
    public class ChannelEdgeQueue : Dictionary<int, Queue<Edge<GorgeObject>>>
    {
        public int EdgeCount()
        {
            return Values.Sum(queue => queue.Count);
        }

        public bool TryDequeue(out int signalId, out Edge<GorgeObject> edge)
        {
            signalId = default;
            edge = default;
            if (EdgeCount() == 0)
            {
                return false;
            }

            foreach (var (id, queue) in this)
            {
                if (queue.TryDequeue(out edge))
                {
                    signalId = id;
                    return true;
                }
            }

            return false;
        }

        public void Enqueue(int signalId, Edge<GorgeObject> edge)
        {
            if (!TryGetValue(signalId, out var queue))
            {
                queue = new Queue<Edge<GorgeObject>>();
                Add(signalId, queue);
            }
            
            queue.Enqueue(edge);
        }
    }
}