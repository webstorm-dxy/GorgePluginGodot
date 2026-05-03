using System;
using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class Asset
    {
        public Asset(Injector injector)
        {
            FieldInitialize(injector);
        }

        public Asset(string name)
        {
            this.name = name;
        }

        private static partial string InitializeField_name(string name) => name;

        private static partial Annotation[] ClassAnnotations()
        {
            return Array.Empty<Annotation>();
        }

        public virtual partial bool LoadAsset()
        {
            throw new Exception("当前调用的方法实际是抽象的，暂未实现抽象方法机制");
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_name()
        {
            var metadata = new Dictionary<string, Metadata>
            {
                ["type"] = new Metadata(GorgeType.String, "type", "资源设置"),
                ["order"] = new Metadata(GorgeType.Int, "order", 0),
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "资源名"),
                ["information"] = new Metadata(GorgeType.String, "information", "资源名"),
                ["check"] = new Metadata(GorgeType.Delegate(GorgeType.Bool, GorgeType.String), "check",
                    new NativeGorgeDelegate(GorgeType.Delegate(GorgeType.Bool, GorgeType.String), _ => true)),
            };

            return metadata;
        }
    }
}