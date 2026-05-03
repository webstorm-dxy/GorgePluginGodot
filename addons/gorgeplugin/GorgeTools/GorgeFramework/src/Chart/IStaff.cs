using System.Collections.Generic;

namespace Gorge.GorgeFramework.Chart
{
    /// <summary>
    /// 谱表
    /// </summary>
    public interface IStaff
    {
        /// <summary>
        /// 乐段记载类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 本乐段是否来源于谱面
        /// 否则来源于模态
        /// </summary>
        public bool IsChartClass { get; }

        /// <summary>
        /// 乐段显示名
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// 所含乐段
        /// </summary>
        public IEnumerable<IPeriod> Periods { get; }

        /// <summary>
        /// 取乐段
        /// </summary>
        /// <param name="periodName"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public bool TryGetPeriod(string periodName, out IPeriod period);
        
        /// <summary>
        /// 生成谱表代码
        /// </summary>
        /// <returns></returns>
        public string ToGorgeCode();

        /// <summary>
        /// 检查目标乐段名是否和已有乐段名冲突
        /// </summary>
        /// <param name="periodNameToInsert"></param>
        /// <returns>true表示冲突，不可添加该名字的乐段</returns>
        public bool CheckPeriodNameConflict(string periodNameToInsert);

        /// <summary>
        /// 添加乐段
        /// </summary>
        /// <param name="period"></param>
        public void AddPeriod(IPeriod period);

        /// <summary>
        /// 删除乐段
        /// </summary>
        /// <param name="period"></param>
        public void RemovePeriod(IPeriod period);
        
        /// <summary>
        /// 克隆本乐段
        /// </summary>
        /// <returns></returns>
        public IStaff Clone();
    }
}