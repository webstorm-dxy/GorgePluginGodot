using System.Collections.Generic;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class AxialSymmetricFunctionCurve
    {
        public AxialSymmetricFunctionCurve(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        private static partial FunctionCurve InitializeField_functionCurve(Injector functionCurve)
        {
            if (functionCurve == null)
            {
                return null;
            }

            var gorgeClass =
                RuntimeStatic.Runtime.LanguageRuntime.GetClass(functionCurve.InjectedClassDeclaration.Name);
            var realIndex = gorgeClass.Declaration.InjectorConstructorImplementationId[0];
            return FunctionCurve.FromGorgeObject(functionCurve.Instantiate(realIndex));
        }

        private static partial float InitializeField_axis(float axis) => axis;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_functionCurve() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "原函数")
        };

        private static partial Injector InjectorFieldDefaultValue_functionCurve() => null;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_axis() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "对称轴")
        };

        private static partial float InjectorFieldDefaultValue_axis() => 0;

        private static partial bool InitializeField_keepLeft(bool keepLeft) => keepLeft;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_keepLeft() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "保留左侧")
        };

        private static partial bool InjectorFieldDefaultValue_keepLeft() => true;

        public override partial float Evaluate(float x)
        {
            if (functionCurve == null)
            {
                return 0;
            }

            if (keepLeft)
            {
                if (x <= axis)
                {
                    return functionCurve.Evaluate(x);
                }
                else
                {
                    return functionCurve.Evaluate(axis - x + axis);
                }
            }
            else
            {
                if (x >= axis)
                {
                    return functionCurve.Evaluate(x);
                }
                else
                {
                    return functionCurve.Evaluate(axis - x + axis);
                }
            }
        }

        private static partial Annotation[] ClassAnnotations() => new[]
        {
            new Annotation("Editable", null, new Dictionary<string, Metadata>()
            {
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "轴对称函数"),
            })
        };
    }
}