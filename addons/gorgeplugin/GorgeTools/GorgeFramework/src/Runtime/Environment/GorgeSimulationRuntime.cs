using System;
using System.Collections.Generic;

namespace Gorge.GorgeFramework.Runtime.Environment
{
    /// <summary>
    ///     模拟机的运行时环境
    /// </summary>
    public class GorgeSimulationRuntime
    {
        public readonly Action OnTerminate;
        public ChartManager Chart { get; private set; }
        public AudioManager Audio { get; private set; }
        public AutomatonManager Automaton { get; private set; }
        public SimulationManager Simulation { get; private set; }
        public SceneManager Scene { get; private set; }
        public GraphicsManager Graphics { get; private set; }
        public Logger Logger { get; private set; }

        private List<SimulationModule> _simulationModules;

        public bool IsScoreLoaded { get; private set; }
        public bool IsSimulating { get; private set; }

        public GorgeSimulationRuntime(Action onTerminate = null)
        {
            OnTerminate = onTerminate;
            Logger = new Logger(this);
            Scene = new SceneManager();
            Audio = new AudioManager();
            Graphics = new GraphicsManager();
            Simulation = new SimulationManager(this);
            Automaton = new AutomatonManager();
            Chart = new ChartManager(this);
        }

        public void LoadScore()
        {
            if (IsScoreLoaded)
            {
                UnloadScore();
            }

            // 读取谱面
            Chart.LoadScore();

            IsScoreLoaded = true;
        }

        public void UnloadScore()
        {
            if (!IsScoreLoaded)
            {
                return;
            }

            if (IsSimulating)
            {
                StopSimulation();
            }

            Chart.UnloadScore();

            IsScoreLoaded = false;
        }

        public void StartSimulation()
        {
            if (!IsScoreLoaded)
            {
                throw new Exception("在尝试在谱面加载前");
            }

            if (IsSimulating)
            {
                StopSimulation();
            }

            Logger.StartSimulation();
            Scene.RuntimeInitialize();
            Audio.StartSimulation();
            Graphics.StartSimulation();
            Simulation.RuntimeInitialize();
            Automaton.RuntimeInitialize();
            Chart.StartSimulation();
            Simulation.SimulationMachine.DriveInstantly();

            IsSimulating = true;
        }

        public void StopSimulation()
        {
            if (!IsSimulating)
            {
                return;
            }

            Chart.StopSimulation();
            Automaton.RuntimeDestruct();
            Simulation.RuntimeDestruct();
            Graphics.StopSimulation();
            Audio.StopSimulation();
            Scene.RuntimeDestruct();
            Logger.StopSimulation();

            IsSimulating = false;
        }

        /// <summary>
        /// 复位后重新跳转到当前位置
        /// </summary>
        public void RePlay()
        {
            var nowChartTime = Simulation.SimulationMachine.ChartTime;

            StopSimulation();
            StartSimulation();
            Simulation.SimulationMachine.DriveToChartTime(nowChartTime);
            Audio.StopAllSong();
        }
    }
}