using System;

namespace Gorge.GorgeFramework.Runtime.Environment
{
    public abstract class SimulationModule
    {
        public bool IsScoreLoaded;
        public bool IsSimulating;

        public void LoadScore()
        {
            if (IsScoreLoaded)
            {
                UnloadScore();
            }

            DoLoadScore();

            IsScoreLoaded = true;
        }

        protected abstract void DoLoadScore();

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

            DoUnloadScore();

            IsScoreLoaded = false;
        }

        protected abstract void DoUnloadScore();

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

            DoStartSimulation();

            IsSimulating = true;
        }

        protected abstract void DoStartSimulation();

        public void StopSimulation()
        {
            if (!IsSimulating)
            {
                return;
            }

            DoStopSimulation();
            IsSimulating = false;
        }

        protected abstract void DoStopSimulation();
    }
}