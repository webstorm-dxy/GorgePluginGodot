using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class LinearFunctionCurve
    {
        public LinearFunctionCurve(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        public LinearFunctionCurve(Injector injector, float k, float b) : base(injector)
        {
            FieldInitialize(injector);

            this.k = k;
            this.b = b;
        }
        
        private static partial float InitializeField_k(float k) => k;

        private static partial float InitializeField_b(float b) => b;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_k() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "k")
        };

        private static partial float InjectorFieldDefaultValue_k() => 1;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_b() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "b")
        };

        private static partial float InjectorFieldDefaultValue_b() => 0;

        public override partial float Evaluate(float x)
        {
            return k * x + b;
        }

        private static partial Annotation[] ClassAnnotations() => new[]
        {
            new Annotation("Editable", null, new Dictionary<string, Metadata>()
            {
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "一次函数"),
            })
        };
    }
}