using System.Collections.Generic;
using Gorge.GorgeFramework.Simulators;
using QuikGraph.Collections;

namespace Gorge.GorgeFramework.Runtime.Environment
{
    /// <summary>
    /// 管理运行时仿真器
    /// 暂时将模拟流程中的回调桩也含在这里
    /// </summary>
    public class SimulationManager
    {
        private readonly GorgeSimulationRuntime _runtime;

        public SimulationManager(GorgeSimulationRuntime runtime)
        {
            _runtime = runtime;
            SimulationMachine = new SimulationMachine(runtime);
        }

        #region 仿真机

        public SimulationMachine SimulationMachine { get; private set; }

        #endregion

        #region 模拟流程回调

        /// <summary>
        /// 仿真
        /// </summary>
        public delegate void SimulationHook(float simulateTime, float simulateTimeTarget, float chartTime,
            float chartTimeTarget);

        /// <summary>
        ///     确定步长后，执行模拟前
        ///     参数为：当前模拟时间、目标模拟时间、当前谱面时间、目标谱面时间
        ///     TODO 这里也应该随Runtime初始化来初始化，但目前Input没有接进来 
        /// </summary>
        public event SimulationHook BeforeSimulate;

        public void InvokeBeforeSimulate(float simulateTime, float simulateTimeTarget, float chartTime, float chartTimeTarget)
        {
            BeforeSimulate?.Invoke(simulateTime, simulateTimeTarget, chartTime, chartTimeTarget);
        }

        #endregion

        #region 运行时

        /// <summary>
        /// 模拟器表
        /// </summary>
        public PriorityHeap<float, ISimulator> Simulators { get; private set; }
        
        /// <summary>
        /// 尾独立模拟器表
        /// </summary>
        public PriorityHeap<float, ISimulator> LateIndependentSimulators { get; private set; }

        /// <summary>
        /// 模拟器表斐波那契堆的倒查索引
        /// </summary>
        public Dictionary<ISimulator, FibonacciHeapCell<float, ISimulator>>
            SimulatorFibonacciHeapCellDictionary { get; private set; }

        public void RuntimeInitialize()
        {
            Simulators = new PriorityHeap<float, ISimulator>();
            LateIndependentSimulators = new PriorityHeap<float, ISimulator>();
            SimulatorFibonacciHeapCellDictionary = new Dictionary<ISimulator, FibonacciHeapCell<float, ISimulator>>();

            SimulationMachine.RuntimeInitialize();

            // 定时创生
            Simulators.Register(-1, new TimedElementGenerator());

            // 定时销毁
            Simulators.Register(-1, new TimedElementDestroyer());

            // 自动机仿真
            Simulators.Register(0, new PreciseAutomatonSimulator());

            // 音乐播放
            Simulators.Register(10000, new SongSimulator());

            // 图形更新
            LateIndependentSimulators.Register(100000, new GraphicsNodeSimulator());
        }

        public void RuntimeDestruct()
        {
            // 注销所有Simulator
            Simulators.Destruct();
            LateIndependentSimulators.Destruct();

            SimulationMachine.RuntimeDestruct();

            Simulators = null;
            LateIndependentSimulators = null;
            SimulatorFibonacciHeapCellDictionary = null;
        }

        #endregion
    }
}