using System;
using System.Collections.Generic;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Signal;
using Gorge.GorgeFramework.Simulators;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class ElementSimulator : ISimulator
    {
        private readonly GorgeObject[] _transformers;

        protected ElementSimulator(Injector injector, ObjectArray transformers)
        {
            FieldInitialize(injector);
            this.transformers = transformers;

            if (this.transformers == null)
            {
                _transformers = Array.Empty<GorgeObject>();
            }
            else
            {
                var length = transformers.length;
                _transformers = new GorgeObject[length];

                for (var i = 0; i < length; i++)
                {
                    _transformers[i] = transformers.Get(i);
                }
            }
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        private static partial ObjectArray InitializeField_transformers() => null;

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
            return Simulate(chartTimeTo,SimulateDirection.Forward);
        }

        public IGameplayAction[] BackwardSimulate(float chartTimeFrom, float chartTimeTo,
            MultichannelSnapshot signals, GorgeSimulationRuntime runtime)
        {
            return Simulate(chartTimeTo,SimulateDirection.Backward);
        }

        public IGameplayAction[] InfinitesimalSimulate(float chartTimeTo,
            MultichannelSnapshot signals, GorgeSimulationRuntime runtime)
        {
            return Simulate(chartTimeTo,SimulateDirection.Infinitesimal);
        }

        public IGameplayAction[] InstantSimulate(float chartTimeTo, SimulateDirection direction,
            MultichannelSnapshot signalSnapshot, GorgeSimulationRuntime runtime)
        {
            return Simulate(chartTimeTo,direction);
        }

        private IGameplayAction[] Simulate(float chartTime, SimulateDirection direction)
        {
            var commands = new List<IGameplayAction>();
            foreach (var transformer in _transformers)
            {
                var commandArray = (ObjectArray) transformer.RealObject.InvokeInterfaceMethod(
                    ITransformer.Interface.Type, "Transform", new[] {GorgeType.Float}, new object[] {chartTime});
                commands.AddRange(SignalTsiga.ConvertAutomatonCommands(commandArray, direction));
            }

            return commands.ToArray();
        }
    }
}