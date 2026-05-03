using System;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Signal;

namespace Gorge.GorgeFramework.Simulators
{
    public class GraphicsNodeSimulator : ISimulator
    {
        public float ForwardAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            return float.MaxValue;
        }

        public float BackwardAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            return float.MinValue;
        }

        public float InfinitesimalAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            return float.MaxValue;
        }

        public IGameplayAction[] ForwardSimulate(float chartTimeFrom, float chartTimeTo,
            MultichannelSnapshot signalSnapshot, GorgeSimulationRuntime runtime)
        {
            return Simulate();
        }

        public IGameplayAction[] BackwardSimulate(float chartTimeFrom, float chartTimeTo, MultichannelSnapshot signals,
            GorgeSimulationRuntime runtime)
        {
            return Simulate();
        }

        public IGameplayAction[] InfinitesimalSimulate(float chartTimeTo, MultichannelSnapshot signals,
            GorgeSimulationRuntime runtime)
        {
            return Simulate();
        }

        public IGameplayAction[] InstantSimulate(float chartTimeTo, SimulateDirection direction,
            MultichannelSnapshot signalSnapshot, GorgeSimulationRuntime runtime)
        {
            return Simulate();
        }

        private IGameplayAction[] Simulate()
        {
            // TODO 这里更新一轮显然是不够的，应当循环更新到没有更新为止，或者维护依赖关系的拓扑序，按序更新
            // TODO 暂时更新一轮，应该不影响验证
            foreach (var node in RuntimeStatic.Runtime.SimulationRuntime.Graphics.Nodes)
            {
                node.UpdateNode();
            }

            return Array.Empty<IGameplayAction>();
        }
    }
}