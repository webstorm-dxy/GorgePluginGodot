using System;
using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class AudioAsset
    {
        private Audio _audio;

        public AudioAsset(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        private static partial Annotation[] ClassAnnotations()
        {
            var annotations = new Annotation[]
            {
                new("Editable", null, new Dictionary<string, Metadata>()
                {
                    ["displayName"] = new Metadata(GorgeType.String, "displayName", "预载音频"),
                })
            };

            return annotations;
        }

        public virtual partial Audio GetAsset()
        {
            return _audio;
        }

        public override partial bool LoadAsset()
        {
            try
            {
                var asset = Environment.GetAssetByName(name);
                _audio = FromGorgeObject(asset).GetAsset();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}