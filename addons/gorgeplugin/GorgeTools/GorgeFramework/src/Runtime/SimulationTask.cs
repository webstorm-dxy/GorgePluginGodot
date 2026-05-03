using Gorge.GorgeFramework.Simulators;

namespace Gorge.GorgeFramework.Runtime
{
    /// <summary>
    /// 仿真任务
    /// </summary>
    public class SimulationTask
    {
        /// <summary>
        ///     目标模拟时间对应的参考谱面时间，如果结束在目标模拟时间上，则同时应当结束在谱面参考时间上，否则依赖谱面时间的模拟器可能无法正确工作
        /// </summary>
        public float? ChartTime;

        /// <summary>
        ///     挂起动作，完成目标后顺次执行
        /// </summary>
        public IGameplayAction[] PendingActions;

        /// <summary>
        ///     目标模拟时间
        /// </summary>
        public float SimulateTime;

        public override string ToString()
        {
            return
                $"{nameof(ChartTime)}: {ChartTime}, {nameof(PendingActions)}: {PendingActions}, {nameof(SimulateTime)}: {SimulateTime}";
        }
    }
}