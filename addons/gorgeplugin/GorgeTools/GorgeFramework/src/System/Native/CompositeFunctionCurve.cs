using System.Collections.Generic;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class CompositeFunctionCurve
    {
        public CompositeFunctionCurve(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }
        
        public CompositeFunctionCurve(Injector injector, FunctionCurve outerFunctionCurve, FunctionCurve innerFunctionCurve) : base(injector)
        {
            FieldInitialize(injector);
            this.outerFunctionCurve = outerFunctionCurve;
            this.innerFunctionCurve = innerFunctionCurve;
        }

        private static partial FunctionCurve InitializeField_outerFunctionCurve(Injector outerFunctionCurve)
        {
            if (outerFunctionCurve == null)
            {
                return null;
            }

            var gorgeClass =
                RuntimeStatic.Runtime.LanguageRuntime.GetClass(outerFunctionCurve.InjectedClassDeclaration.Name);
            var realIndex = gorgeClass.Declaration.InjectorConstructorImplementationId[0];
            return FunctionCurve.FromGorgeObject(outerFunctionCurve.Instantiate(realIndex));
        }

        private static partial FunctionCurve InitializeField_innerFunctionCurve(Injector innerFunctionCurve)
        {
            if (innerFunctionCurve == null)
            {
                return null;
            }

            var gorgeClass =
                RuntimeStatic.Runtime.LanguageRuntime.GetClass(innerFunctionCurve.InjectedClassDeclaration.Name);
            var realIndex = gorgeClass.Declaration.InjectorConstructorImplementationId[0];
            return FunctionCurve.FromGorgeObject(innerFunctionCurve.Instantiate(realIndex));
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_outerFunctionCurve() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "外层函数")
        };

        private static partial Injector InjectorFieldDefaultValue_outerFunctionCurve()
        {
            return null;
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_innerFunctionCurve() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "内层函数")
        };

        private static partial Injector InjectorFieldDefaultValue_innerFunctionCurve()
        {
            return null;
        }

        public override partial float Evaluate(float x)
        {
            if (outerFunctionCurve == null)
            {
                return 0;
            }

            return outerFunctionCurve?.Evaluate(innerFunctionCurve?.Evaluate(x) ?? 0) ?? 0;
        }

        private static partial Annotation[] ClassAnnotations() => new[]
        {
            new Annotation("Editable", null, new Dictionary<string, Metadata>()
            {
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "复合函数"),
            })
        };
    }
}