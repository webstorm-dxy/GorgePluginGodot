using System;
using System.Collections.Generic;
using System.Numerics;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class Vector3
    {
        protected Vector3(Injector injector)
        {
            FieldInitialize(injector);
        }

        protected Vector3(Injector injector, float x, float y, float z)
        {
            FieldInitialize(injector);
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        private static partial float InitializeField_x(float x) => x;

        private static partial float InitializeField_y(float y) => y;

        private static partial float InitializeField_z(float z) => z;

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_x() => new();

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_y() => new();

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_z() => new();

        private static partial float InjectorFieldDefaultValue_x()
        {
            return 0;
        }

        private static partial float InjectorFieldDefaultValue_y()
        {
            return 0;
        }

        private static partial float InjectorFieldDefaultValue_z()
        {
            return 0;
        }

        public virtual partial Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        /// <summary>
        /// 转换为四元数旋转角。
        /// 假定当前向量以角度制表示欧拉角
        /// </summary>
        /// <returns></returns>
        public Quaternion ToQuaternion()
        {
            return Quaternion.CreateFromYawPitchRoll(
                y * Math.Deg2RadConstant,
                x * Math.Deg2RadConstant,
                z * Math.Deg2RadConstant
            );
        }

        public static Vector3 FromQuaternion(Quaternion q)
        {
            // 计算 pitch (x 轴旋转)
            var sinRollCosPitch = 2 * (q.W * q.X + q.Y * q.Z);
            var cosRollCosPitch = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            var x = MathF.Atan2(sinRollCosPitch, cosRollCosPitch) * Math.Rad2DegConstant;

            // 计算 yaw (y 轴旋转)
            var sinPitch = 2 * (q.W * q.Y - q.Z * q.X);
            float y;
            if (Math.Abs(sinPitch) >= 1)
            {
                // 使用 90 度，如果接近万向锁
                y = MathF.Sign(sinPitch) * MathF.PI / 2;
            }
            else
            {
                y = MathF.Asin(sinPitch);
            }

            // 计算 roll (z 轴旋转)
            var sinYawCosPitch = 2 * (q.W * q.Z + q.X * q.Y);
            var cosYawCosPitch = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            var z = MathF.Atan2(sinYawCosPitch, cosYawCosPitch);

            return new Vector3(x, y * Math.Rad2DegConstant, z * Math.Rad2DegConstant);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static implicit operator System.Numerics.Vector3(Vector3 vector3)
        {
            return new System.Numerics.Vector3(vector3.x, vector3.y, vector3.z);
        }

        public static implicit operator Vector3(System.Numerics.Vector3 vector3)
        {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        /// <summary>
        /// 将本点旋转目标角度
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public Vector3 Transform(Quaternion rotation)
        {
            return System.Numerics.Vector3.Transform(this, rotation);
        }

        public override string ToString()
        {
            return $"{nameof(x)}: {x}, {nameof(y)}: {y}, {nameof(z)}: {z}";
        }
    }
}