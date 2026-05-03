using Gorge.GorgeFramework.Runtime.Environment;

namespace Gorge.GorgeFramework.Simulators
{
    public interface ISimulationDriver
    {
        public GorgeSimulationRuntime Runtime { get; }

        /// <summary>
        /// 获得实时模式时间
        /// 这是当前精确的现实时间对应的模拟时间，用于信号采集器生成正确的精确时间戳
        /// </summary>
        /// <returns></returns>
        public float GetRealSimulateTime();
    }
}