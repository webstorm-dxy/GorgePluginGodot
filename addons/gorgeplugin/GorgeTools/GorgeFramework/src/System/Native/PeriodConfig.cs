using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class PeriodConfig
    {
        public PeriodConfig(Injector injector)
        {
            FieldInitialize(injector);
        }

        private static partial float InitializeField_timeOffset(float timeOffset)
        {
            return timeOffset;
        }

        private static partial float InjectorFieldDefaultValue_timeOffset() => 0;

        private static partial float InjectorFieldDefaultValue_minLength() => 10;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_timeOffset()
        {
            var metadata = new Dictionary<string, Metadata>
            {
                ["type"] = new Metadata(GorgeType.String, "type", "乐段设置"),
                ["order"] = new Metadata(GorgeType.Int, "order", 0),
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "起点"),
                ["information"] = new Metadata(GorgeType.String, "information", "乐段起点时间，秒"),
                ["check"] = new Metadata(GorgeType.Delegate(GorgeType.Bool, GorgeType.Float), "check",
                    new NativeGorgeDelegate(GorgeType.Delegate(GorgeType.Bool, GorgeType.Float), _ => true)),
            };

            return metadata;
        }

        private static partial Annotation[] ClassAnnotations()
        {
            var annotations = new Annotation[]
            {
                new("Editable", null, new Dictionary<string, Metadata>()
                {
                    ["displayName"] = new Metadata(GorgeType.String, "displayName", "乐段设置"),
                })
            };

            return annotations;
        }

        private static partial float InitializeField_minLength(float minLength) => minLength;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_minLength()
        {
            var metadata = new Dictionary<string, Metadata>
            {
                ["type"] = new Metadata(GorgeType.String, "type", "乐段设置"),
                ["order"] = new Metadata(GorgeType.Int, "order", 1),
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "最小长度"),
                ["information"] = new Metadata(GorgeType.String, "information", "最小显示长度，秒"),
                ["check"] = new Metadata(GorgeType.Delegate(GorgeType.Bool, GorgeType.Float), "check",
                    new NativeGorgeDelegate(GorgeType.Delegate(GorgeType.Bool, GorgeType.Float), _ => true)),
            };

            return metadata;
        }
    }
}