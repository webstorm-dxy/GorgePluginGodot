using System.Collections.Generic;

namespace Gorge.GorgeFramework.Stage
{
    /// <summary>
    ///     计分器接口
    /// TODO 暂时使用Deenty的版本，需要修改为模态可定义
    /// </summary>
    public interface IScoring
    {
        /// <summary>
        ///     获取当前准度
        /// </summary>
        public float Accuracy { get; }

        /// <summary>
        ///     获取当前分数
        /// </summary>
        public int Score { get; }

        /// <summary>
        ///     获取当前连击数
        /// </summary>
        public int Combo { get; }

        /// <summary>
        ///     获取最大连击
        /// </summary>
        public int MaxCombo { get; }

        /// <summary>
        ///     获取当前里程碑
        /// </summary>
        public ScoreMilepost Milepost { get; }

        /// <summary>
        ///     获取判定结果计数
        /// </summary>
        public Dictionary<int, int> RespondResultCount { get; }

        /// <summary>
        ///     处理一个响应结果
        /// </summary>
        /// <param name="respond"></param>
        public void Respond(int respond);
    }
}