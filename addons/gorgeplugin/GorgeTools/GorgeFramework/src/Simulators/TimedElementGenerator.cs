using System;
using System.Collections.Generic;
using System.Linq;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Signal;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native;
using Gorge.Native.Gorge;

namespace Gorge.GorgeFramework.Simulators
{
    /// <summary>
    /// 定时构造器
    /// </summary>
    public class TimedElementGenerator : ISimulator
    {
        public float ForwardAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            if (RuntimeStatic.Runtime.SimulationRuntime.Chart.ForwardTimedGenerateList.Count == 0)
            {
                return float.MaxValue;
            }

            return RuntimeStatic.Runtime.SimulationRuntime.Chart.ForwardTimedGenerateList.Min(e =>
                e.Time > charTime ? e.Time : float.MaxValue);
        }

        public float BackwardAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            if (RuntimeStatic.Runtime.SimulationRuntime.Chart.BackwardTimedGenerateList.Count == 0)
            {
                return float.MinValue;
            }

            return RuntimeStatic.Runtime.SimulationRuntime.Chart.BackwardTimedGenerateList.Max(e =>
                e.Time < charTime ? e.Time : float.MinValue);
        }

        public float InfinitesimalAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            // 零速模拟不响应
            return float.MaxValue;
        }

        public IGameplayAction[] ForwardSimulate(float chartTimeFrom, float chartTimeTo,
            MultichannelSnapshot signalSnapshot, GorgeSimulationRuntime runtime)
        {
            List<Tuple<Injector, ConstructorInformation>> generateList = new();

            foreach (var element in RuntimeStatic.Runtime.SimulationRuntime.Chart.ForwardTimedGenerateList)
            {
                if (element.Time > chartTimeFrom && element.Time <= chartTimeTo)
                {
                    generateList.Add(
                        new Tuple<Injector, ConstructorInformation>(element.Injector, element.Constructor));
                }
            }

            var autowire = new GenerateElementAutowireValues(StaticConfig.IsAutoPlay, false, SimulateDirection.Forward);

            return generateList.Select<Tuple<Injector, ConstructorInformation>, IGameplayAction>(t =>
                new GenerateElement(t.Item1, t.Item2, autowire)).ToArray();
        }

        public IGameplayAction[] BackwardSimulate(float chartTimeFrom, float chartTimeTo, MultichannelSnapshot signals,
            GorgeSimulationRuntime runtime)
        {
            List<Tuple<Injector, ConstructorInformation>> generateList = new();

            foreach (var element in RuntimeStatic.Runtime.SimulationRuntime.Chart.BackwardTimedGenerateList)
            {
                if (element.Time < chartTimeFrom && element.Time >= chartTimeTo)
                {
                    generateList.Add(
                        new Tuple<Injector, ConstructorInformation>(element.Injector, element.Constructor));
                }
            }

            var autowire = new GenerateElementAutowireValues(StaticConfig.IsAutoPlay, true, SimulateDirection.Backward);

            return generateList
                .Select<Tuple<Injector, ConstructorInformation>, IGameplayAction>(t =>
                    new GenerateElement(t.Item1, t.Item2, autowire))
                .ToArray();
        }

        public IGameplayAction[] InfinitesimalSimulate(float chartTimeTo, MultichannelSnapshot signals,
            GorgeSimulationRuntime runtime)
        {
            return Array.Empty<IGameplayAction>();
        }

        public IGameplayAction[] InstantSimulate(float chartTimeTo, SimulateDirection direction,
            MultichannelSnapshot signalSnapshot, GorgeSimulationRuntime runtime)
        {
            return Array.Empty<IGameplayAction>();
        }
    }
}