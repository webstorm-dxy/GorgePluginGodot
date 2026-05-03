using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class LinearCurve
    {
        protected LinearCurve(Injector injector, float timeStart, float valueStart, float timeEnd, float valueEnd) :
            base(injector)
        {
            FieldInitialize(injector);
            this.timeStart = timeStart;
            this.valueStart = valueStart;
            this.timeEnd = timeEnd;
            this.valueEnd = valueEnd;
        }

        public LinearCurve(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        private static partial float InitializeField_timeStart(float timeStart) => timeStart;

        private static partial float InitializeField_valueStart(float valueStart) => valueStart;

        private static partial float InitializeField_timeEnd(float timeEnd) => timeEnd;

        private static partial float InitializeField_valueEnd(float valueEnd) => valueEnd;

        private static partial float InjectorFieldDefaultValue_timeStart() => 0;

        private static partial float InjectorFieldDefaultValue_valueStart() => 0;

        private static partial float InjectorFieldDefaultValue_timeEnd() => 1;

        private static partial float InjectorFieldDefaultValue_valueEnd() => 1;

        private static partial Annotation[] ClassAnnotations()
        {
            return new[]
            {
                new Annotation("Editable", null, new Dictionary<string, Metadata>()
                {
                    ["displayName"] = new Metadata(GorgeType.String, "displayName", "线段"),
                })
            };
        }

        public override partial float Evaluate(float time)
        {
            return Math.Lerp(valueStart, valueEnd, Math.InverseLerp(timeStart, timeEnd, time));
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_timeStart() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "起始时间")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_valueStart() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "起始值")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_timeEnd() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "终止时间")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_valueEnd() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "终止值")
        };
    }
}