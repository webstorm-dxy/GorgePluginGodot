using System;
using System.Text;
using Gorge.GorgeCompiler.Visitors;

namespace Gorge.GorgeFramework.Chart
{
    public class AudioStaff : Staff<AudioPeriod>
    {
        public AudioStaff(string className, bool isChartClass, string displayName) : base(className, isChartClass,
            displayName)
        {
        }

        /// <summary>
        /// 转换为代码
        /// </summary>
        /// <returns></returns>
        public override string ToGorgeCode()
        {
            if (!IsChartClass)
            {
                throw new Exception("尝试将非谱面谱表转化为谱面代码");
            }

            var sb = new StringBuilder();

            sb.AppendLine("[");
            sb.AppendLine($"string displayName = {LiteralHelper.StringToStringLiteral(DisplayName)}", 1);
            sb.AppendLine("]");
            sb.AppendLine("@AudioStaff");
            sb.AppendLine($"class {ClassName}");
            sb.AppendLine("{");
            foreach (var period in Periods)
            {
                sb.AppendLine(period.ToGorgeCode(1));
                sb.AppendLine();
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
        
        protected override Staff<AudioPeriod> Clone()
        {
            var newStaff = new AudioStaff(ClassName, IsChartClass, DisplayName);
            foreach (var audioPeriod in Periods)
            {
                newStaff.Periods.Add((AudioPeriod) audioPeriod.Clone());
            }

            return newStaff;
        }
    }
}