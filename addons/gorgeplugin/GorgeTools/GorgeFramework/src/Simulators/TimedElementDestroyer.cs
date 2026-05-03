using System;
using System.Collections.Generic;
using System.Linq;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Signal;
using Gorge.Native;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Simulators
{
    /// <summary>
    /// 定时构造器
    /// </summary>
    public class TimedElementDestroyer : ISimulator
    {
        public float ForwardAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            if (RuntimeStatic.Runtime.SimulationRuntime.Chart.ForwardTimedDestroyElementObjects.Count == 0)
            {
                return float.MaxValue;
            }

            return RuntimeStatic.Runtime.SimulationRuntime.Chart.ForwardTimedDestroyElementObjects.Min(f =>
                f.Item1 > charTime ? f.Item1 : float.MaxValue);
        }

        public float BackwardAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            if (RuntimeStatic.Runtime.SimulationRuntime.Chart.BackwardTimedDestroyElementObjects.Count == 0)
            {
                return float.MinValue;
            }

            return RuntimeStatic.Runtime.SimulationRuntime.Chart.BackwardTimedDestroyElementObjects.Max(f =>
                f.Item1 < charTime ? f.Item1 : float.MinValue);
        }

        public float InfinitesimalAsyncSimulationTarget(float charTime, GorgeSimulationRuntime runtime)
        {
            // 零速模拟不响应
            return float.MaxValue;
        }

        public IGameplayAction[] ForwardSimulate(float chartTimeFrom, float chartTimeTo,
            MultichannelSnapshot signalSnapshot, GorgeSimulationRuntime runtime)
        {
            List<Element> destroyList = new();

            foreach (var (destroyTime, element) in RuntimeStatic.Runtime.SimulationRuntime.Chart
                         .ForwardTimedDestroyElementObjects)
            {
                if (destroyTime > chartTimeFrom && destroyTime <= chartTimeTo)
                {
                    destroyList.Add(element);
                }
            }

            foreach (var e in destroyList)
            {
                RuntimeStatic.Runtime.SimulationRuntime.Chart.ForwardTimedDestroyElementObjects.RemoveAll(t =>
                    t.Item2 == e);
                RuntimeStatic.Runtime.SimulationRuntime.Chart.BackwardTimedDestroyElementObjects.RemoveAll(t =>
                    t.Item2 == e);
            }

            return destroyList.Select<Element, IGameplayAction>(e => new DestroyElement(e)).ToArray();
        }

        public IGameplayAction[] BackwardSimulate(float chartTimeFrom, float chartTimeTo, MultichannelSnapshot signals,
            GorgeSimulationRuntime runtime)
        {
            List<Element> destroyList = new();

            foreach (var (destroyTime, element) in RuntimeStatic.Runtime.SimulationRuntime.Chart
                         .BackwardTimedDestroyElementObjects)
            {
                if (destroyTime < chartTimeFrom && destroyTime >= chartTimeTo)
                {
                    destroyList.Add(element);
                }
            }

            foreach (var e in destroyList)
            {
                RuntimeStatic.Runtime.SimulationRuntime.Chart.ForwardTimedDestroyElementObjects.RemoveAll(t =>
                    t.Item2 == e);
                RuntimeStatic.Runtime.SimulationRuntime.Chart.BackwardTimedDestroyElementObjects.RemoveAll(t =>
                    t.Item2 == e);
            }

            return destroyList.Select<Element, IGameplayAction>(e => new DestroyElement(e))
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