using Gorge.GorgeFramework.Simulators;

namespace Gorge.GorgeFramework.Runtime
{
    /// <summary>
    ///     模拟目标
    /// </summary>
    public class SimulationTarget
    {
        /// <summary>
        ///     目标谱面时间
        /// </summary>
        public float ChartTime;

        /// <summary>
        ///     挂起动作，完成目标后顺次执行
        /// </summary>
        public IGameplayAction[] PendingActions;

        /// <summary>
        ///     模拟倍速
        /// </summary>
        public float SimulateSpeed;

        public SimulateDirection Direction
        {
            get
            {
                return SimulateSpeed switch
                {
                    > 0 => SimulateDirection.Forward,
                    < 0 => SimulateDirection.Backward,
                    _ => SimulateDirection.Infinitesimal
                };
            }
        }
    }
}