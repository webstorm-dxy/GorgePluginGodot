using Gorge.GorgeFramework.Utilities;

namespace Gorge.GorgeFramework.Input
{
    /// <summary>
    ///     信号边沿，描述信号的一次突变
    /// </summary>
    /// <typeparam name="TSignal">信号类型</typeparam>
    public class Edge<TSignal>
    {
        /// <summary>
        ///     信号边沿所在时刻，模拟时间
        /// </summary>
        public float Time;

        /// <summary>
        ///     变化后信号值
        /// </summary>
        public TSignal Value;

        public override string ToString()
        {
            return $"{nameof(Time)}: {Time}({Time.BitInt()}), {nameof(Value)}: {Value}";
        }
    }
}