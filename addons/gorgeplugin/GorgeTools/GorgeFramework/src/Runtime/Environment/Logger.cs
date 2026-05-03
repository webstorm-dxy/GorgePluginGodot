using System.IO;
using System.Text;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeFramework.Signal;
using Gorge.GorgeFramework.Utilities;

#pragma warning disable CS0162 // 检测到不可到达的代码

namespace Gorge.GorgeFramework.Runtime.Environment
{
    public class Logger
    {
        private readonly GorgeSimulationRuntime _runtime;
        private StreamWriter _logFile;

        private static string _logFilePath = Base.Instance.PersistentPath() + "/log.txt";

        public Logger(GorgeSimulationRuntime runtime)
        {
            _runtime = runtime;
        }

        public void StartSimulation()
        {
            if (_runtime.IsSimulating)
            {
                StopSimulation();
            }

            File.Delete(_logFilePath);
            _logFile = new StreamWriter(File.OpenWrite(_logFilePath));
        }

        public void StopSimulation()
        {
            if (_runtime.IsSimulating)
            {
                _logFile.Close();
            }
        }

        public void DebugLog(string info, int table)
        {
            return;

            var sb = new StringBuilder();
            var simulateTime = _runtime.Simulation.SimulationMachine.SimulateTime;
            var chartTime = _runtime.Simulation.SimulationMachine.ChartTime;

            sb.Append(
                $"[S:{simulateTime:0.000}({simulateTime.BitInt()}) C:{chartTime:0.000}({chartTime.BitInt()})]");
            for (var i = 0; i < table; i++) sb.Append("  ");

            sb.Append(info);

            _logFile.WriteLine(sb);
        }
    }
}