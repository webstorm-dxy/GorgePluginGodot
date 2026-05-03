#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;

namespace Gorge.GorgeFramework.Input
{
    /// <summary>
    ///     信号片段，描述一段时间内的信号变化
    /// </summary>
    /// <typeparam name="TSignal">信号类型</typeparam>
    [JsonObject]
    public class Fragment<TSignal>
    {
        /// <summary>
        ///     信号编号
        /// </summary>
        [JsonProperty(Order = 1)] public int SignalId;

        /// <summary>
        ///     片段起始时刻，模拟时间
        /// </summary>
        [JsonProperty(Order = 2)] public float StartTime;

        /// <summary>
        ///     片段结束时刻，模拟时间
        /// </summary>
        [JsonProperty(Order = 3)] public float EndTime;

        /// <summary>
        ///     起始信号值
        /// </summary>
        [JsonProperty(Order = 4)] public TSignal StartValue;

        /// <summary>
        ///     片段内的信号边沿
        /// </summary>
        [JsonProperty(Order = 5)] public List<Edge<TSignal>> Edges;

        /// <summary>
        ///     最后值
        /// </summary>
        [JsonIgnore]
        public TSignal LatestValue
        {
            get
            {
                if (Edges == null || Edges.Count == 0)
                    return StartValue;
                return Edges[^1].Value;
            }
        }

        /// <summary>
        ///     向当前信号片段追加边沿
        /// </summary>
        /// <param name="edges">null视为无内容</param>
        public void AppendEdges( List<Edge<TSignal>>? edges)
        {
            if (edges == null || edges.Count == 0) return;

            // TODO 这里逻辑上是右包含，追加边沿不能和已有信号的末尾重合，但是由于SimulateTime和ChartTime转换的精度损失
            // TODO ChartTIme和SimulateTime的实际切分边界没有完全对齐，导致可能出现本该结束在第一段末尾的信号可能会认为不包含结束，从而后推到第二段开头
            // TODO 由于边界精确转换，这使得追加的边沿和已有边沿相同
            // TODO 这有可能是映射方案不统一导致的，目前模拟机内使用模拟速度互相转换，AutoPlay模拟使用等比缩放转换，这里可能有精度差别
            // TODO 如果最后统一到一个映射方法中，那么应该是模拟机对外暴露的API，供用户编写模拟方法用
            // TODO 所以目前允许相等情况的追加，如果有更好的映射方案可能可以还原
            if (edges.Any(edge => edge.Time <= EndTime)) // 由于原区间右包含，所以等于的情况也追加不上
                throw new Exception(
                    $"追加的边沿触发时间早于当前信号片段的结束时间，当前信号片段结束于{EndTime},追加边沿触发于{edges.Min(edge => edge.Time)}");

            // 更新信号终结时间
            EndTime = edges[^1].Time;
            if (LatestValue.Equals(edges[0].Value)) // 如果当前信号尾的值与第一个边沿一致，则将该边沿合并到上一个边沿中
                Edges.AddRange(edges.Skip(1));
            else
                Edges.AddRange(edges);
        }

        /// <summary>
        ///     对信号进行采样，如果采样点正好有边沿，则采样边沿变化后的值
        /// </summary>
        /// <param name="sampleTime">采样时间，模拟时间</param>
        /// <returns></returns>
        public TSignal Sample(float sampleTime)
        {
            if (sampleTime < StartTime || sampleTime > EndTime) throw new Exception("信号片段采样超界");

            try
            {
                // 寻找早于或等于采样时间的最晚边沿
                var edge = Edges.Last(edge => edge.Time <= sampleTime);
                // 未报异常即存在边沿早于采样时间（含等于），返回边沿对应时间
                return edge.Value;
            }
            catch (InvalidOperationException) // 所有边沿都晚于采样时间的情况，返回初始值
            {
                return StartValue;
            }
        }

        /// <summary>
        ///     对信号进行采样，如果采样点正好有边沿，则采样边沿变化前的值
        /// </summary>
        /// <param name="sampleTime">采样时间，模拟时间</param>
        /// <returns></returns>
        public TSignal SampleBeforeEdge(float sampleTime)
        {
            if (sampleTime < StartTime || sampleTime > EndTime)
            {
                throw new Exception($"信号片段采样超界\n采样时间{sampleTime}\n起始时间：{StartTime}\n结束时间：{EndTime}");
            }

            // 寻找采样点所在边沿
            // 如果无边沿精确落在采样点，则采样点在早于采样点前的最后一个边沿
            // 如果有边沿精确落在采样点，认为不包含
            Edge<TSignal> targetEdge = null;
            foreach (var edge in Edges)
            {
                if (edge.Time < sampleTime)
                {
                    targetEdge = edge;
                }
                else
                {
                    break;
                }
            }

            // 寻找早于采样时间的最晚边沿
            // targetEdge = Edges.Last(edge => edge.Time <= sampleTime);
            // 未报异常即存在边沿早于采样时间（含等于），返回边沿对应时间
            return targetEdge == null ? StartValue : targetEdge.Value;
        }

        /// <summary>
        ///     对信号进行采样，如果采样点正好有边沿，则采样边沿变化后的值
        /// </summary>
        /// <param name="sampleTime">采样时间，模拟时间</param>
        /// <returns></returns>
        public TSignal SampleAfterEdge(float sampleTime)
        {
            if (sampleTime < StartTime || sampleTime > EndTime)
            {
                throw new Exception($"信号片段采样超界\n采样时间{sampleTime}\n起始时间：{StartTime}\n结束时间：{EndTime}");
            }

            // 寻找采样点所在边沿
            // 采样点在早于或等于采样时间前的最后一个边沿
            var targetEdge = Edges.LastOrDefault(edge => edge.Time <= sampleTime);
            // 如果无早于或等于采样点的边沿，则返回信号头
            return targetEdge == null ? StartValue : targetEdge.Value;
        }

        /// <summary>
        ///     将信号片段按时间切片，右包含（相等情况下该点包含）
        /// </summary>
        /// <param name="fromTime">起始模拟时间</param>
        /// <param name="toTime">中止模拟时间</param>
        /// <returns>如果切片内容为空，则返回null</returns>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public Fragment<TSignal>? Split(float fromTime, float toTime)
        {
            // TODO 考虑使用某种抽象指针？直接在总的信号流中界定一个范围作为切片，这样不用复制数据，但是可能不能随机访问，每次有计算成本，但这可能可以使用缓存来缓解
            // TODO 这是源于“切片不会被修改”，只会读取和再次切片，所以抽象指针是成立的
            // TODO 只有信号流会追加，信号切片并不会被追加
            // TODO 已终结的信号流不允许追加，未终结的信号流不允许向后超界切片？否则切片内容会变化，影响缓存的语义正确性
            // TODO 信号流的追加也是不会早于当下，因此未终结的信号流判断超界是判断当下，感觉在一个统一时钟的驱动下是可行的

            // 切分区间在本片段内的情况，由以下两种
            // 第一种情况，起点和终点都在末尾，则包含末尾点
            //   起点在末尾，终点晚于末尾的情况，由于左不包含，所以切片内容为空
            // 第二种情况，起点早于末尾，终点晚于开头（右包含，所以可以等于）

            if ((fromTime == EndTime && fromTime == toTime) || (fromTime < EndTime && toTime >= StartTime))
            {
                // 这里要区分结果StartTime的来源
                // 如果StartTime晚于fromTime，则不会被fromTime的左不包含排除端点
                if (StartTime > fromTime)
                {
                    // 含端情况下，直接继承初始值
                    var startValue = StartValue;
                    return new Fragment<TSignal>
                    {
                        SignalId = SignalId,
                        StartTime = StartTime,
                        EndTime = MathF.Min(EndTime, toTime),
                        Edges = Edges.FindAll(edge =>
                            edge.Time == toTime || (edge.Time < toTime && edge.Time > fromTime)),
                        StartValue = startValue
                    };
                }
                // fromTime == toTime的情况，精确采样对应点，包含所有该时间上的边沿，StartValue为边前采样
                // 边前采样包含了StartTime == fromTime时需要集成StartValue值的情况
                else if (fromTime == toTime)
                {
                    return new Fragment<TSignal>
                    {
                        SignalId = SignalId,
                        StartTime = fromTime,
                        EndTime = MathF.Min(EndTime, toTime),
                        Edges = Edges.FindAll(edge =>
                            edge.Time == toTime || (edge.Time < toTime && edge.Time > fromTime)),
                        StartValue = SampleBeforeEdge(fromTime)
                    };
                }
                // StartTime早于fromTime的情况，StartTime即fromTime之前和之上的边沿被排除，需要重新计算StartValue，为fromTime的边后采样
                else
                {
                    return new Fragment<TSignal>
                    {
                        SignalId = SignalId,
                        StartTime = fromTime,
                        EndTime = MathF.Min(EndTime, toTime),
                        Edges = Edges.FindAll(edge => edge.Time <= toTime && edge.Time > fromTime),
                        StartValue = SampleAfterEdge(fromTime)
                    };
                }
            }

            return null;
        }

        public override string ToString()
        {
            return
                $"{nameof(SignalId)}: {SignalId}, {nameof(StartTime)}: {StartTime}, {nameof(EndTime)}: {EndTime}, {nameof(StartValue)}: {StartValue}, {nameof(Edges)}: [{string.Join(",", Edges)}], {nameof(LatestValue)}: {LatestValue}";
        }
    }
}