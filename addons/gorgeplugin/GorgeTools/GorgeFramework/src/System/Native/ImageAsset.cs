using System;
using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class ImageAsset
    {
        protected ImageAsset(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        public ImageAsset(string name, Graph graph) : base(name)
        {
            this.name = name;
            this.texture = graph;
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        private static partial Graph InitializeField_texture(Graph texture) => texture;

        private static partial Graph InjectorFieldDefaultValue_texture()
        {
            return null;
        }

        /// <summary>
        /// 0号方法
        /// </summary>
        /// <returns></returns>
        public virtual partial string DescriptorDisplayString()
        {
            return name;
        }

        public override partial Graph GetAsset()
        {
            return texture;
        }

        public override partial bool LoadAsset()
        {
            // 值引用无需加载
            return true;
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_texture() => new();
    }
}