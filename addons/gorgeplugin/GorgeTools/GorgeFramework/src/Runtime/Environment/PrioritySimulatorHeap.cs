using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuikGraph.Collections;

namespace Gorge.GorgeFramework.Runtime.Environment
{
    public class PriorityHeap<TPriority, TValue> : IEnumerable<KeyValuePair<TPriority, TValue>>
    {
        /// <summary>
        /// 斐波那契堆
        /// </summary>
        private readonly FibonacciHeap<TPriority, TValue> _heap = new();

        /// <summary>
        /// 斐波那契堆的倒查索引
        /// </summary>
        private readonly Dictionary<TValue, FibonacciHeapCell<TPriority, TValue>> _fibonacciHeapCellDictionary = new();

        public void Register(TPriority priority, TValue simulator)
        {
            // TODO 这里潜在假定单例

            if (simulator != null)
            {
                var cell = _heap.Enqueue(priority, simulator);
                _fibonacciHeapCellDictionary.Add(simulator, cell);
            }
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="simulator"></param>
        public void Remove(TValue simulator)
        {
            if (simulator != null)
            {
                var cell = _fibonacciHeapCellDictionary[simulator];
                _fibonacciHeapCellDictionary.Remove(simulator);
                _heap.Delete(cell);
            }
        }

        /// <summary>
        /// 销毁，目前就是注销全部
        /// </summary>
        public void Destruct()
        {
            var list = _heap.ToList();
            foreach (var simulator in list)
            {
                Remove(simulator.Value);
            }
        }

        public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
        {
            return _heap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}