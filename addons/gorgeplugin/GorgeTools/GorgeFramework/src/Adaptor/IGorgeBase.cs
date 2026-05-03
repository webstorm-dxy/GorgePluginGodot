using System;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Adaptor
{
    public interface IGorgeFrameworkBase
    {
        /// <summary>
        /// 记录标准信息
        /// </summary>
        /// <param name="msg"></param>
        public void Log(string msg);
        
        /// <summary>
        /// 记录警告信息
        /// </summary>
        /// <param name="msg"></param>
        public void Warning(string msg);

        /// <summary>
        /// 获取持久数据地址
        /// </summary>
        /// <returns></returns>
        public string PersistentPath();
        
        public ISprite CreateSprite();
        
        public INineSliceSprite CreateNineSliceSprite();
        
        public ICurveSprite CreateCurveSprite();
        
        /// <summary>
        /// 从资源文件数据构建图像
        /// </summary>
        /// <param name="assetFilePath">资源文件路径</param>
        /// <param name="data">资源文件数据</param>
        /// <returns></returns>
        public Graph CreateGraph(string assetFilePath, byte[] data);
        
        /// <summary>
        /// 从资源文件数据构建音频
        /// </summary>
        /// <param name="assetFilePath">资源文件路径</param>
        /// <param name="data">资源文件数据</param>
        /// <returns></returns>
        public Audio CreateAudio(string assetFilePath, byte[] data);
        
        public IAudioEffectPlayer CreateAudioEffectPlayer(Audio audioEffect);
        
        public IAudioPlayer CreateAudioPlayer();
        
        public GorgeSimulationRuntime CreateSimulationRuntime(Action onTerminate = null);

        public Vector3 ScreenToWorldPoint(Vector3 screenPoint);
    }
}