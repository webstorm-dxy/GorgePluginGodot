using System.Collections.Generic;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class LerpColorCurve
    {
        public LerpColorCurve(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        private static partial ObjectArray InitializeField_colorPoints(ObjectList colorPoints)
        {
            if (colorPoints == null)
            {
                return null;
            }

            var objectList = new List<GorgeObject>();
            for (var i = 0; i < colorPoints.length; i++)
            {
                var functionCurve = (Injector) colorPoints.Get(i);
                if (functionCurve == null)
                {
                    objectList.Add(null);
                }
                else
                {
                    objectList.Add(functionCurve.Instantiate(0));
                }
            }

            return new ObjectArray(objectList.Count, new ObjectList(colorPoints.ItemClassType.SubTypes[0], objectList));
        }

        private static partial FunctionCurve InitializeField_progressCurve(Injector progressCurve)
        {
            if (progressCurve == null)
            {
                return null;
            }

            var gorgeClass =
                RuntimeStatic.Runtime.LanguageRuntime.GetClass(progressCurve.InjectedClassDeclaration.Name);
            var realIndex = gorgeClass.Declaration.InjectorConstructorImplementationId[0];
            return FunctionCurve.FromGorgeObject(progressCurve.Instantiate(realIndex));
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_colorPoints() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "函数分段")
        };

        private static partial ObjectList InjectorFieldDefaultValue_colorPoints() => default;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_progressCurve() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "进度曲线")
        };

        private static partial Injector InjectorFieldDefaultValue_progressCurve() => default;

        public override partial ColorArgb Evaluate(float x)
        {
            if (colorPoints == null || colorPoints.length == 0)
            {
                return ColorArgb.White;
            }

            if (progressCurve == null)
            {
                var color = colorPoints.Get(0);
                return color == null ? ColorArgb.White : ColorArgb.FromGorgeObject(color);
            }

            var progress = progressCurve.Evaluate(x);
            var point0 = Math.ClampInt(0, colorPoints.length - 1, Math.Floor(progress));
            var point1 = Math.ClampInt(0, colorPoints.length - 1, Math.Ceil(progress));

            if (point0 == point1)
            {
                var color = colorPoints.Get(point0);
                return color == null ? ColorArgb.White : ColorArgb.FromGorgeObject(color);
            }

            var color0 = ColorArgb.FromGorgeObject(colorPoints.Get(point0)) ?? ColorArgb.White;
            var color1 = ColorArgb.FromGorgeObject(colorPoints.Get(point1)) ?? ColorArgb.White;

            return ColorArgb.Lerp(color0, color1, progress - point0);
        }

        private static partial Annotation[] ClassAnnotations() => new[]
        {
            new Annotation("Editable", null, new Dictionary<string, Metadata>()
            {
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "补间颜色曲线"),
            })
        };
    }
}