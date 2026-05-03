using System;
using System.Collections.Generic;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeFramework.Chart;

namespace Gorge.GorgeFramework.Runtime.Environment
{
    /// <summary>
    /// 管理运行时音效
    /// </summary>
    public class AudioManager
    {
        #region 运行阶段

        private Dictionary<string, IAudioEffectPlayer> _respondEffects;

        public Dictionary<AudioPeriod, IAudioPlayer> PeriodAudioSources;

        public void StartSimulation()
        {
            _respondEffects = new Dictionary<string, IAudioEffectPlayer>();

            foreach (var (name, asset) in RuntimeStatic.Runtime.Score.InstantAudio)
            {
                _respondEffects.Add(name, Base.Instance.CreateAudioEffectPlayer(asset.GetAsset()));
            }

            PeriodAudioSources = new Dictionary<AudioPeriod, IAudioPlayer>();

            foreach (var staff in RuntimeStatic.Runtime.Score.Stave)
            {
                if (staff is AudioStaff audioStaff)
                {
                    foreach (var period in audioStaff.Periods)
                    {
                        DoAddPeriod(period);
                    }
                }
            }
        }

        public void StopSimulation()
        {
            foreach (var (_, effectPlayer) in _respondEffects)
            {
                effectPlayer.Destruct();
            }

            _respondEffects.Clear();

            foreach (var (_, audioPlayer) in PeriodAudioSources)
            {
                audioPlayer.Destruct();
            }

            PeriodAudioSources = null;
        }

        private void DoAddPeriod(AudioPeriod audioPeriod)
        {
            var audioPlayer = Base.Instance.CreateAudioPlayer();
            audioPlayer.SetAudio(audioPeriod.Audio);
            PeriodAudioSources.Add(audioPeriod, audioPlayer);
        }

        /// <summary>
        /// 播放响应音效
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="Exception"></exception>
        public void PlayRespondEffect(string name)
        {
            if (!_respondEffects.TryGetValue(name, out var effectPlayer))
            {
                throw new Exception($"没有注册名为{name}的响应音效");
            }

            effectPlayer.Play();
        }

        /// <summary>
        /// 停止所有音乐
        /// </summary>
        public void StopAllSong()
        {
            if (PeriodAudioSources != null)
            {
                foreach (var (_, audioPlayer) in PeriodAudioSources)
                {
                    audioPlayer.Stop();
                }
            }
        }

        #endregion

        #region 动态编辑

        public void UpdateAudioConfig(AudioPeriod period)
        {
            if (RuntimeStatic.Runtime.SimulationRuntime.IsSimulating)
            {
                PeriodAudioSources[period].Stop();
            }
        }

        public void UpdateAudioAsset(AudioPeriod period)
        {
            if (RuntimeStatic.Runtime.SimulationRuntime.IsSimulating)
            {
                PeriodAudioSources[period].SetAudio(period.Audio);
            }
        }

        public void AddPeriod(AudioPeriod audioPeriod)
        {
            if (RuntimeStatic.Runtime.SimulationRuntime.IsSimulating)
            {
                DoAddPeriod(audioPeriod);
            }
        }

        public void RemovePeriod(AudioPeriod audioPeriod)
        {
            if (RuntimeStatic.Runtime.SimulationRuntime.IsSimulating)
            {
                PeriodAudioSources[audioPeriod].Destruct();
                PeriodAudioSources.Remove(audioPeriod);
            }
        }

        #endregion
    }
}