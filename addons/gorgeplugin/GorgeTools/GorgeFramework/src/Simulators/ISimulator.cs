using System.Collections.Generic;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Signal;
using Gorge.GorgeLanguage.Objective;

namespace Gorge.GorgeFramework.Simulators
{
    /// <summary>
    ///     可模拟对象
    /// </summary>
    public interface ISimulator
    {
        /// <summary>
        ///     计算该模拟器的前向异步模拟的目标谱面时间点。
        ///     语义上讲，从模拟起点到该时间点期间，任何外部变化都不会影响模拟结果。
        ///     实际模拟中，Gameplay将会获取所有模拟对象的该时间点，并确定一个最保守的时间点，满足所有模拟对象的该该性质。
        ///     然后并行模拟各个模拟对象到该时间点，执行一次同步。
        /// </summary>
        /// <param name="charTime">当前谱面时间</param>
        /// <param name="runtime"></param>
        /// <returns>目标谱面时间</returns>
        public float ForwardAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime);

        /// <summary>
        ///     计算该模拟器的后向异步模拟的目标谱面时间点。
        ///     语义上讲，从模拟起点到该时间点期间，任何外部变化都不会影响模拟结果。
        ///     实际模拟中，Gameplay将会获取所有模拟对象的该时间点，并确定一个最保守的时间点，满足所有模拟对象的该该性质。
        ///     然后并行模拟各个模拟对象到该时间点，执行一次同步。
        /// </summary>
        /// <param name="charTime">当前谱面时间</param>
        /// <param name="runtime"></param>
        /// <returns>目标谱面时间</returns>
        public float BackwardAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime);

        /// <summary>
        ///     计算该模拟器的零速异步模拟的目标模拟时间点。
        ///     零速模拟是指谱面时间的推进速度为0，此时只有模拟时间前进，而谱面时间不变化
        ///     语义上讲，从模拟起点到该时间点期间，任何外部变化都不会影响模拟结果。
        ///     实际模拟中，Gameplay将会获取所有模拟对象的该时间点，并确定一个最保守的时间点，满足所有模拟对象的该该性质。
        ///     然后并行模拟各个模拟对象到该时间点，执行一次同步。
        /// </summary>
        /// <param name="charTime">当前谱面时间</param>
        /// <param name="runtime"></param>
        /// <returns>目标模拟时间</returns>
        public float InfinitesimalAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime);

        /// <summary>
        ///     前向模拟
        /// </summary>
        /// <param name="chartTimeFrom"></param>
        /// <param name="chartTimeTo"></param>
        /// <param name="signalSnapshot"></param>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public IGameplayAction[] ForwardSimulate(float chartTimeFrom, float chartTimeTo,
            MultichannelSnapshot signalSnapshot, GorgeSimulationRuntime runtime);

        /// <summary>
        ///     后向模拟
        /// </summary>
        /// <param name="chartTimeFrom"></param>
        /// <param name="chartTimeTo"></param>
        /// <param name="signals"></param>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public IGameplayAction[] BackwardSimulate(float chartTimeFrom, float chartTimeTo,
            MultichannelSnapshot signals, GorgeSimulationRuntime runtime);


        /// <summary>
        ///     零速模拟
        ///     零速模拟是指谱面时间的推进速度为0，此时只有模拟时间前进，而谱面时间不变化
        ///     两步模拟间，由模拟时间导出的状态会发生改变，如信号切片
        /// </summary>
        /// <param name="chartTimeTo"></param>
        /// <param name="signals"></param>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public IGameplayAction[] InfinitesimalSimulate(float chartTimeTo,
            MultichannelSnapshot signals, GorgeSimulationRuntime runtime);

        /// <summary>
        ///     零步长模拟
        ///     零步长模拟是指模拟时间不发生推进的模拟，模拟时间和谱面时间均不变化，由模拟时间导出的状态不变化（如信号切片）
        ///     两步模拟间，非时间状态可能会发生改变，如活跃中的Note列表
        /// </summary>
        /// <param name="chartTimeTo">当前谱面时间点</param>
        /// <param name="direction">模拟方向</param>
        /// <param name="signalSnapshot"></param>
        /// <param name="runtime"></param>
        /// <returns></returns>
        public IGameplayAction[] InstantSimulate(float chartTimeTo, SimulateDirection direction,
            MultichannelSnapshot signalSnapshot, GorgeSimulationRuntime runtime);
    }
}