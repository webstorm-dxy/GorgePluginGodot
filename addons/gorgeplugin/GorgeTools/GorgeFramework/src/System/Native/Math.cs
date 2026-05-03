using System;
using Gorge.GorgeLanguage.Objective;

namespace Gorge.Native.GorgeFramework
{
    public partial class Math
    {
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        public static partial float Abs(float f)
        {
            return MathF.Abs(f);
        }

        public static partial float Sqrt(float f)
        {
            return MathF.Sqrt(f);
        }

        public static partial float Max(float f1, float f2)
        {
            return MathF.Max(f1, f2);
        }

        public static partial float Max(float f1, float f2, float f3, float f4)
        {
            return Max(new[] {f1, f2, f3, f4});
        }

        public static float Max(params float[] values)
        {
            var length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            var num = values[0];
            for (var index = 1; index < length; ++index)
            {
                if (values[index] > num)
                {
                    num = values[index];
                }
            }

            return num;
        }

        public static partial float Min(float f1, float f2)
        {
            return MathF.Min(f1, f2);
        }

        public static float Min(params float[] values)
        {
            var length = values.Length;
            if (length == 0)
            {
                return 0;
            }

            var num = values[0];
            for (var index = 1; index < length; ++index)
            {
                if (values[index] < num)
                {
                    num = values[index];
                }
            }

            return num;
        }

        public static partial float Atan(float f)
        {
            return MathF.Atan(f);
        }

        public static partial float Sin(float f)
        {
            return MathF.Sin(f);
        }

        public static partial float Cos(float f)
        {
            return MathF.Cos(f);
        }

        public static partial float CosDeg(float f)
        {
            return MathF.Cos(f * Deg2Rad());
        }

        public static partial float SinDeg(float f)
        {
            return MathF.Sin(f * Deg2Rad());
        }

        public static partial float Pi()
        {
            return MathF.PI;
        }

        public static partial float FloatPositiveInfinity()
        {
            return float.PositiveInfinity;
        }

        public static partial float FloatNegativeInfinity()
        {
            return float.NegativeInfinity;
        }

        public static partial int Floor(float f)
        {
            return (int) MathF.Floor(f);
        }

        public static partial int Ceil(float f)
        {
            return (int) MathF.Ceiling(f);
        }

        public static partial int ClampInt(int a, int b, int value)
        {
            if (value < a)
            {
                value = a;
            }
            else if (value > b)
            {
                value = b;
            }

            return value;
        }

        public static partial float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp(t, 0, 1);
        }

        public static partial float InverseLerp(float a, float b, float v)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (a == b)
            {
                return 0;
            }

            return Clamp((float) (double) (v - a) / (b - a), 0, 1);
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public const float Rad2DegConstant = (float) (180 / System.Math.PI);
        public const float Deg2RadConstant = (float) (System.Math.PI / 180);

        public static float Deg2Rad()
        {
            return Deg2RadConstant;
        }

        public static float Rad2Deg()
        {
            return Rad2DegConstant;
        }
    }
}