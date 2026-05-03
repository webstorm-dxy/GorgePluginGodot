using System.Collections.Generic;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class AdditionFunctionCurve
    {
        public AdditionFunctionCurve(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        private static partial FunctionCurve InitializeField_firstFunctionCurve(Injector firstFunctionCurve)
        {
            if (firstFunctionCurve == null)
            {
                return null;
            }

            var gorgeClass =
                RuntimeStatic.Runtime.LanguageRuntime.GetClass(firstFunctionCurve.InjectedClassDeclaration.Name);
            var realIndex = gorgeClass.Declaration.InjectorConstructorImplementationId[0];
            return FunctionCurve.FromGorgeObject(firstFunctionCurve.Instantiate(realIndex));
        }

        private static partial FunctionCurve InitializeField_secondFunctionCurve(Injector secondFunctionCurve)
        {
            if (secondFunctionCurve == null)
            {
                return null;
            }

            var gorgeClass =
                RuntimeStatic.Runtime.LanguageRuntime.GetClass(secondFunctionCurve.InjectedClassDeclaration.Name);
            var realIndex = gorgeClass.Declaration.InjectorConstructorImplementationId[0];
            return FunctionCurve.FromGorgeObject(secondFunctionCurve.Instantiate(realIndex));
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_firstFunctionCurve() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "加数函数")
        };

        private static partial Injector InjectorFieldDefaultValue_firstFunctionCurve()
        {
            return null;
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_secondFunctionCurve() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "加数函数")
        };

        private static partial Injector InjectorFieldDefaultValue_secondFunctionCurve()
        {
            return null;
        }

        public override partial float Evaluate(float x)
        {
            if (firstFunctionCurve == null)
            {
                return 0;
            }

            return (firstFunctionCurve?.Evaluate(x) ?? 0) + (secondFunctionCurve?.Evaluate(x) ?? 0);
        }

        private static partial Annotation[] ClassAnnotations() => new[]
        {
            new Annotation("Editable", null, new Dictionary<string, Metadata>()
            {
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "加和函数"),
            })
        };
    }
}