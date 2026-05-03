using System;
using System.Collections.Generic;
using System.Linq;
using Gorge.Native.GorgeFramework;
using Math = System.Math;

namespace Gorge.GorgeFramework.Stage
{
    /// <summary>
    ///     第一版计分器
    /// </summary>
    public class ScoringV1 : ScoringCounter
    {
        /// <summary>
        ///     连击分权重
        /// </summary>
        private const int ComboWeight = 700000;

        /// <summary>
        ///     准度分权重
        /// </summary>
        private const int AccuracyWeight = 300000;

        /// <summary>
        ///     大P额外加分
        /// </summary>
        private const int BestPerfectAddition = 1;

        /// <summary>
        ///     准度幂次
        /// </summary>
        private const int AccuracyExponent = 10;

        /// <summary>
        ///     最大准度奖励
        /// </summary>
        private readonly int _maxAccuracyBonus;

        /// <summary>
        ///     谱面的最大Combo奖励，是1到最大Combo数的和
        /// </summary>
        private readonly int _maxComboBonus;

        /// <summary>
        ///     各准度的准度奖励
        /// </summary>
        private readonly Dictionary<int, int> _respondResultAccuracyBonus = new()
        {
            {RespondResult.Miss, 0},
            {RespondResult.Good, 50},
            {RespondResult.Perfect, 100},
            {RespondResult.BestPerfect, 100}
        };

        /// <summary>
        ///     当前已获得的准度奖励
        /// </summary>
        private int _accuracyBonus;

        /// <summary>
        ///     当前已获得的连击奖励
        /// </summary>
        private int _comboBonus;

        public ScoringV1(int maxMapCombo)
        {
            if (maxMapCombo == 0) maxMapCombo = 1; // 修正除零错误
            _maxComboBonus = (maxMapCombo + 1) * maxMapCombo / 2;
            _maxAccuracyBonus = maxMapCombo * 100;
        }

        /**
         * 计分方案：
         * 分数：
         * 重要考虑：分数只增不减
         * 规范化到1000000，分为三部分：连击分、准度分、大P附加分
         * 连击分(满分700000)：
         * 每个Note获得当前Combo数的连击奖励，例如hit-hit-hit-miss-hit-hit获得的奖励为1+2+3+0+1+2=9
         * 每张图的最大连击奖励为1到最大Combo数的和
         * 实际连击分=700000*(当前累积连击奖励/最大连击奖励)
         * 准度分(满分300000)：
         * 每个Note根据判定结果获得准度奖励，Miss为0，Great为50，大小P均为100
         * 每张图的最大准度奖励为最大Combo数*100
         * 实际准度分=300000*((当前累积准度奖励/最大准度奖励)^10)
         * 这里取幂的考虑是：连击分记录总和，断连损失较大，所以通过取幂次来放大抓良率对分数的影响
         * 基础总分(满分1000000)：
         * 基础总分=sqrt(连击分+准度分)*1000
         * 这里的考虑是：直接相加得到的分数数值较小，为了增强对玩家的鼓励，采用开方*1000的方式在保持值域不变的情况下放大分数数值
         * 总分=基础总分+大P数量
         *
         * ACC：
         * 重要考虑：ACC动态增减
         * ACC=当前累积准度奖励/当前最大准度奖励
         * 相关规则与准度分一致
         */

        public override float Accuracy { get; protected set; } = 1;

        public override int Score { get; protected set; }

        public override void Respond(int respond)
        {
            base.Respond(respond);
            _comboBonus += Combo;
            _accuracyBonus += _respondResultAccuracyBonus[respond];
            var pureScore = (int) (ComboWeight * (_comboBonus / (float) _maxComboBonus)) +
                            (int) (AccuracyWeight *
                                   MathF.Pow(_accuracyBonus / (float) _maxAccuracyBonus, AccuracyExponent));
            var fixedScore = Math.Clamp((int) (MathF.Sqrt(pureScore) * 1000), 0, 1000000);
            Score = fixedScore + RespondResultCount[RespondResult.BestPerfect] * BestPerfectAddition;
            Accuracy = _accuracyBonus / (100f * RespondResultCount.Values.Sum());
        }
    }
}