using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Adaptor
{
    public interface IAudioPlayer
    {
        /// <summary>
        /// 设置音频
        /// </summary>
        /// <param name="audio"></param>
        public void SetAudio(Audio audio);

        public void Play();
        
        public void Stop();

        public float AudioLength();
        
        public bool IsPlaying();

        /// <summary>
        /// 设置当前播放进度
        /// </summary>
        /// <returns></returns>
        public void SetTime(float time);
        
        public void Destruct();
    }

    /// <summary>
    /// 音效播放器
    /// </summary>
    public interface IAudioEffectPlayer
    {
        void Play();
        void Destruct();
    }
}