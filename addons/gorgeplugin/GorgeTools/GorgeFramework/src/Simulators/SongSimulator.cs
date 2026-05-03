using System;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Signal;

namespace Gorge.GorgeFramework.Simulators
{
    /// <summary>
    /// 定时音乐播放器
    /// TODO 暂时使用简单实现
    /// </summary>
    public class SongSimulator : ISimulator
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
            // TODO 简单实现，如果应当播放而未播放，则调整时间并播放
            // TODO 如果不应播放而在播放，则停止
            // TODO 考虑将音频操作作为一种GameplayAction？
            foreach (var (period, audioPlayer) in runtime.Audio.PeriodAudioSources)
            {
                var startChartTime = period.Config.timeOffset;
                // 临时使用的延迟调整
                // startChartTime += DataPool.Gameplay?.Setting.RespondDelay ?? 0;
                startChartTime += 0;
                var endChartTime = startChartTime + audioPlayer.AudioLength();
                if (chartTimeTo >= startChartTime && chartTimeTo < endChartTime)
                {
                    if (!audioPlayer.IsPlaying())
                    {
                        audioPlayer.SetTime(chartTimeTo - startChartTime);
                        audioPlayer.Play();
                    }
                }
                else
                {
                    if (audioPlayer.IsPlaying())
                    {
                        audioPlayer.Stop();
                    }
                }
            }

            return Array.Empty<IGameplayAction>();
        }

        public IGameplayAction[] BackwardSimulate(float chartTimeFrom, float chartTimeTo, MultichannelSnapshot signals,
            GorgeSimulationRuntime runtime)
        {
            return Array.Empty<IGameplayAction>();
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