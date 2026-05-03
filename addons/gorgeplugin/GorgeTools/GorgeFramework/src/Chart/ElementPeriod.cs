using System.Collections.Generic;
using System.Text;
using Gorge.Native.Gorge;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Chart
{
    public class ElementPeriod : Period
    {
        /// <summary>
        /// 元素表
        /// </summary>
        public readonly List<Injector> Elements;

        public ElementPeriod(string methodName, Injector configInjector) : base(methodName, configInjector)
        {
            Elements = new List<Injector>();
        }

        /// <summary>
        /// 转换为代码
        /// </summary>
        /// <returns></returns>
        public override string ToGorgeCode(int indentation)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[", indentation);
            sb.AppendLine(
                $"GorgeFramework.PeriodConfig^ config = {InjectorHardcodeGenerator.Generate(ConfigInjector, indentation + 1)}",
                indentation + 1);
            sb.AppendLine("]", indentation);
            sb.AppendLine("@Chart", indentation);
            sb.AppendLine($"static GorgeFramework.Element^[] {MethodName}()", indentation);
            sb.AppendLine("{", indentation);
            sb.AppendLine(
                $"return new GorgeFramework.Element^[{Elements.Count}]{InjectorHardcodeGenerator.Generate(Element.Class.Declaration.Type, Elements, false, indentation + 1)};",
                indentation + 1);
            sb.AppendLine("}", indentation);
            return sb.ToString();
        }

        public override Period Clone()
        {
            var elementPeriod = new ElementPeriod(MethodName, (Injector) ConfigInjector.Clone());
            foreach (var element in Elements)
            {
                elementPeriod.Elements.Add((Injector) element.Clone());
            }

            return elementPeriod;
        }
    }
}