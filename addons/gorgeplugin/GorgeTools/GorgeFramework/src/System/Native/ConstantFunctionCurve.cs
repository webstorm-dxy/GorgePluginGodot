using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class ConstantFunctionCurve
    {
        public ConstantFunctionCurve(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        public ConstantFunctionCurve(Injector injector, float value) : base(injector)
        {
            FieldInitialize(injector);
            this.value = value;
        }

        private static partial float InitializeField_value(float value)
        {
            return value;
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_value()=> new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "常量值")
        };

        private static partial float InjectorFieldDefaultValue_value() => 0;

        public override partial float Evaluate(float x)
        {
            return value;
        }

        private static partial Annotation[] ClassAnnotations()=> new[]
        {
            new Annotation("Editable", null, new Dictionary<string, Metadata>()
            {
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "常函数"),
            })
        };
    }
}