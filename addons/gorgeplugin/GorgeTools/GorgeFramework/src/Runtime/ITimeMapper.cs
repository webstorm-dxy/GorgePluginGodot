using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Runtime
{
    /// <summary>
    ///     时间映射器接口，用于转换谱面时间和真实时间
    ///     TODO 可能已经无用了，目前采用播放倍速映射
    /// </summary>
    public interface ITimeMapper
    {
        /// <summary>
        ///     真实时间转换至谱面时间
        /// </summary>
        /// <param name="chartTimeStart">谱面时间区间起点</param>
        /// <param name="chartTimeEnd">谱面时间区间终点</param>
        /// <param name="realTimeStart">真实时间区间起点</param>
        /// <param name="realTimeEnd">真实时间区间终点</param>
        /// <param name="realTimeValue">待转换真实时间值</param>
        /// <returns>转换后的谱面时间</returns>
        public float RealTimeToChartTime(float chartTimeStart, float chartTimeEnd, float realTimeStart,
            float realTimeEnd, float realTimeValue);

        /// <summary>
        ///     谱面时间转换至真实时间
        /// </summary>
        /// <param name="chartTimeStart">谱面时间区间起点</param>
        /// <param name="chartTimeEnd">谱面时间区间终点</param>
        /// <param name="realTimeStart">真实时间区间起点</param>
        /// <param name="realTimeEnd">真实时间区间终点</param>
        /// <param name="chartTimeValue">待转换谱面时间值</param>
        /// <returns>转换后的真实时间</returns>
        public float ChartTimeToRealTime(float chartTimeStart, float chartTimeEnd, float realTimeStart,
            float realTimeEnd, float chartTimeValue);
    }

    /// <summary>
    ///     等比缩放映射器
    /// </summary>
    public class ScalingMapper : ITimeMapper
    {
        public float RealTimeToChartTime(float chartTimeStart, float chartTimeEnd, float realTimeStart,
            float realTimeEnd, float realTimeValue)
        {
            return IntervalScaling(realTimeStart, realTimeEnd, chartTimeStart, chartTimeEnd, realTimeValue);
        }

        public float ChartTimeToRealTime(float chartTimeStart, float chartTimeEnd, float realTimeStart,
            float realTimeEnd, float chartTimeValue)
        {
            return IntervalScaling(chartTimeStart, chartTimeEnd, realTimeStart, realTimeEnd, chartTimeValue);
        }

        /// <summary>
        ///     区间等比缩放映射（数学未必严格）。
        ///     如果源像区间长度为0，则固定映射到像区间的末尾。
        ///     如果源像值精确落在源像区间的边界，则返回像区间的精确边界值。
        ///     其余情况等比例映射，有精度损失
        /// </summary>
        /// <param name="preimageIntervalStart"></param>
        /// <param name="preimageIntervalEnd"></param>
        /// <param name="imageIntervalStart"></param>
        /// <param name="imageIntervalEnd"></param>
        /// <param name="preimageIntervalValue"></param>
        /// <returns></returns>
        private static float IntervalScaling(float preimageIntervalStart, float preimageIntervalEnd,
            float imageIntervalStart, float imageIntervalEnd, float preimageIntervalValue)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (preimageIntervalStart == preimageIntervalEnd) // 区间长度为0的情况，视为目标区间的末尾
                return imageIntervalEnd;

            // 边界精确相等的情况，返回对应精确值
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (preimageIntervalValue == preimageIntervalEnd) return imageIntervalEnd;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (preimageIntervalValue == preimageIntervalStart) return imageIntervalStart;

            // 正常情况，等比例映射
            return Math.Lerp(imageIntervalStart, imageIntervalEnd,
                Math.InverseLerp(preimageIntervalStart, preimageIntervalEnd, preimageIntervalValue));
        }
    }
}