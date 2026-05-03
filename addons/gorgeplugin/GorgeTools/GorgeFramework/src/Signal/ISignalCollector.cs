using Gorge.GorgeFramework.Simulators;

namespace Gorge.GorgeFramework.Signal
{
    /// <summary>
    /// 信号采集器接口
    /// </summary>
    public interface ISignalCollector
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(ISimulationDriver driver);

        /// <summary>
        /// 使能采集器
        /// </summary>
        public void Enable();

        /// <summary>
        /// 关闭采集器
        /// </summary>
        public void Disable();
    }
}