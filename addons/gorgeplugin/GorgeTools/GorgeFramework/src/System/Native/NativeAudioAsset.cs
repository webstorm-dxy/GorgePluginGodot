using System;
using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class NativeAudioAsset
    {
        public NativeAudioAsset(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        private static partial Audio InitializeField_audio(Audio audio) => audio;

        private static partial Audio InjectorFieldDefaultValue_audio() => null;

        public override partial Audio GetAsset()
        {
            return audio;
        }

        public override partial bool LoadAsset()
        {
            // 值包装无需加载
            return true;
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_audio() => new();
    }
}