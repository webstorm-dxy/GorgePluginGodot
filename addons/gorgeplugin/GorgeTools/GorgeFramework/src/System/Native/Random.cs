using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class Random
    {
        private static System.Random _random = new();

        public Random(Injector injector)
        {
            FieldInitialize(injector);
        }

        public static partial Vector2 RandomNormalized()
        {
            var angle = RandomFloat(0f, 2f * Math.Pi());
            var x = Math.Cos(angle);
            var y = Math.Sin(angle);
            return new Vector2(x, y);
        }

        public static partial float RandomFloat(float a, float b)
        {
            return (float) (_random.NextDouble() * (b - a) + a);
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();
    }
}