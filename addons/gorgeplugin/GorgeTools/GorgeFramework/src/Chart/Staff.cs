using System;
using System.Collections.Generic;
using System.Linq;

namespace Gorge.GorgeFramework.Chart
{
    /// <summary>
    /// Gorge仿真谱表
    /// 对应一个Gorge类
    /// </summary>
    public abstract class Staff<T> : IStaff where T : class, IPeriod
    {
        // TODO 可以考虑Staff层次存储类型信息，不允许音乐和谱面混用
        // TODO 模态间是否允许混用？是否在Staff注解内？如何声明？

        /// <summary>
        /// 谱表类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 本谱表属于谱面还是模态
        /// 为true则认为是谱面谱表
        /// </summary>
        public bool IsChartClass { get; }

        /// <summary>
        /// 谱表显示名，如果为null则不显示在谱表界面上
        /// </summary>
        public string DisplayName { get; set; }

        IEnumerable<IPeriod> IStaff.Periods => Periods;
        public List<T> Periods { get; }

        public Staff(string className, bool isChartClass, string displayName)
        {
            ClassName = className;
            IsChartClass = isChartClass;
            DisplayName = displayName;
            Periods = new List<T>();
        }

        public abstract string ToGorgeCode();

        public void AddPeriod(IPeriod period)
        {
            if (period is not T periodT)
            {
                throw new Exception();
            }

            Periods.Add(periodT);
        }

        public void RemovePeriod(IPeriod period)
        {
            if (period is not T periodT)
            {
                throw new Exception();
            }

            Periods.Remove(periodT);
        }

        IStaff IStaff.Clone() => Clone();
        protected abstract Staff<T> Clone();

        public bool TryGetPeriod(string periodName, out IPeriod period)
        {
            period = Periods.FirstOrDefault(p => p.MethodName == periodName);
            return period != null;
        }

        /// <summary>
        /// 检查目标乐段名是否和已有乐段名冲突
        /// </summary>
        /// <param name="periodNameToInsert"></param>
        /// <returns>true表示冲突，不可添加该名字的乐段</returns>
        public bool CheckPeriodNameConflict(string periodNameToInsert)
        {
            if (periodNameToInsert == ClassName)
            {
                return true;
            }

            return Periods.Any(p => p.MethodName == periodNameToInsert);
        }
    }
}