using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class ColorArgb
    {
        protected ColorArgb(Injector injector)
        {
            FieldInitialize(injector);
        }

        protected ColorArgb(Injector injector, float a, float r, float g, float b)
        {
            FieldInitialize(injector);
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public ColorArgb(float a, float r, float g, float b)
        {
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public static ColorArgb Lerp(ColorArgb from, ColorArgb to, float t)
        {
            t = Math.Clamp(t, 0, 1);
            var newA = from.a + (to.a - from.a) * t;
            var newR = from.r + (to.r - from.r) * t;
            var newG = from.g + (to.g - from.g) * t;
            var newB = from.b + (to.b - from.b) * t;
            return new ColorArgb(newA, newR, newG, newB);
        }

        public static ColorArgb White => new ColorArgb(1,1,1,1);

        // public ColorArgb(Color color)
        // {
        //     a = color.a;
        //     r = color.r;
        //     g = color.g;
        //     b = color.b;
        // }
        //
        // public Color UnityColor()
        // {
        //     return new Color(r, g, b, a);
        // }

        private static partial Annotation[] ClassAnnotations() => new[]
        {
            new Annotation("Editable", null, new Dictionary<string, Metadata>()
            {
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "ARGB"),
            })
        };

        private static partial float InitializeField_a(float a) => a;
        private static partial float InitializeField_r(float r) => r;
        private static partial float InitializeField_g(float g) => g;
        private static partial float InitializeField_b(float b) => b;
        private static partial float InjectorFieldDefaultValue_a() => 1;
        private static partial float InjectorFieldDefaultValue_r() => 1;
        private static partial float InjectorFieldDefaultValue_g() => 1;
        private static partial float InjectorFieldDefaultValue_b() => 1;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_a() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "不透明度")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_r() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "红")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_g() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "绿")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_b() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "蓝")
        };
    }
}