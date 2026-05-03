using System;
using System.Collections.Generic;
using Gorge.GorgeFramework.Utilities;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;
using MathS = System.Math;

namespace Gorge.Native.GorgeFramework
{
    public partial class Vector2
    {
        protected Vector2(Injector injector)
        {
            FieldInitialize(injector);
        }

        protected Vector2(Injector injector, float x, float y)
        {
            FieldInitialize(injector);

            this.x = x;
            this.y = y;
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        private static partial Annotation[] ClassAnnotations() => new[]
        {
            new Annotation("Editable", null, new Dictionary<string, Metadata>()
            {
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "二维向量"),
            })
        };

        private static partial float InitializeField_x(float x) => x;
        private static partial float InitializeField_y(float y) => y;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_x() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "x")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_y() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "y")
        };

        private static partial float InjectorFieldDefaultValue_x()
        {
            return 0;
        }

        private static partial float InjectorFieldDefaultValue_y()
        {
            return 0;
        }

        public virtual partial Vector3 ToVector3()
        {
            return new Vector3(x, y, 0);
        }

        public static partial Vector2 Scale(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x * v2.x, v1.y * v2.y);
        }

        public static partial float Distance(Vector2 v1, Vector2 v2)
        {
            double n1 = v1.x - v2.x;
            double n2 = v1.y - v2.y;
            return (float) MathS.Sqrt(n1 * n1 + n2 * n2);
        }

        public static partial Vector2 Normalize(Vector2 v)
        {
            var normalized = System.Numerics.Vector2.Normalize(new System.Numerics.Vector2(v.x, v.y));
            return new Vector2(normalized.X, normalized.Y);
        }

        public static float UnsignedAngle(Vector2 from, Vector2 to)
        {
            var a = new System.Numerics.Vector2(from.x, from.y);
            var b = new System.Numerics.Vector2(to.x, to.y);

            // 计算点积
            var dotProduct = System.Numerics.Vector2.Dot(a, b);

            // 计算向量长度
            var magnitudeA = a.Length();
            var magnitudeB = b.Length();

            // 避免除以零
            if (magnitudeA == 0 || magnitudeB == 0)
            {
                return 0;
            }

            // 计算夹角反余弦
            var cosAngle = dotProduct / (magnitudeA * magnitudeB);

            // 由于浮点精度问题，确保值在-1到1之间
            cosAngle = Math.Clamp(cosAngle, -1.0f, 1.0f);
            return MathF.Acos(cosAngle) * 180f / MathF.PI;
        }

        public static float SignedAngle(Vector2 from, Vector2 to)
        {
            // 计算无符号角度
            var angle = UnsignedAngle(from, to);

            // 计算叉乘
            var cross = from.x * to.y - from.y * to.x;

            // 计算叉乘方向
            if (cross < 0)
            {
                angle = -angle;
            }

            return angle;
        }


        public static partial float Angle(Vector2 v)
        {
            return SignedAngle(new Vector2(1, 0), v);
        }

        public static partial Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(Math.Lerp(a.x, b.x, t), Math.Lerp(a.y, b.y, t));
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 touch && Equals(touch);
        }

        public bool Equals(Vector2 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x.Equals(other.x) && y.Equals(other.y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public override string ToString()
        {
            return $"{nameof(x)}: {x}, {nameof(y)}: {y}";
        }
    }
}