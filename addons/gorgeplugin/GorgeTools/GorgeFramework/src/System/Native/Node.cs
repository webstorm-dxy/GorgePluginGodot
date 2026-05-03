using System;
using System.Numerics;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class Node
    {
        public Node(Injector injector)
        {
            FieldInitialize(injector);
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        private static partial bool InitializeField_alive() => true;
        private static partial Node InitializeField_existenceReference() => default;
        private static partial Vector3 InitializeField_position() => new(0, 0, 0);
        private static partial Node InitializeField_positionReference() => default;
        private static partial Vector3 InitializeField_rotation() => new(0, 0, 0);
        private static partial Node InitializeField_rotationReference() => default;
        private static partial Vector3 InitializeField_size() => new(1, 1, 1);
        private static partial Node InitializeField_sizeReference() => default;

        public virtual partial Vector3 LocalPositionToGlobalPosition(Vector3 position)
        {
            var selfPosition = GlobalPosition();
            var selfRotation = GlobalRotation();
            var selfSize = GlobalSize();

            return selfPosition +
                   new Vector3(selfSize.x * position.x, selfSize.y * position.y, selfSize.z * position.z).Transform(
                       selfRotation.ToQuaternion());
        }

        public virtual partial Vector3 GlobalPositionToLocalPosition(Vector3 position)
        {
            var selfPosition = GlobalPosition();
            var selfRotation = GlobalRotation();
            var selfSize = GlobalSize();

            // 计算差向量
            var localPositionInGlobalReference = position - selfPosition;
            // 转向正确坐标方向        
            var rotated = localPositionInGlobalReference.Transform(Quaternion.Inverse(selfRotation.ToQuaternion()));
            // 缩放坐标值
            return new Vector3(rotated.x / selfSize.x, rotated.y / selfSize.y, rotated.z / selfSize.z);
        }

        /// <summary>
        /// 更新节点状态
        /// </summary>
        public virtual void UpdateNode()
        {
            if (existenceReference != null && !existenceReference.alive)
            {
                alive = false;
            }
        }

        /// <summary>
        /// 执行销毁
        /// </summary>
        public virtual void Destroy()
        {
            alive = false;
        }

        #region 全局坐标系参数计算

        /*
         * TODO 目前实时计算，考虑更新和保存
         * TODO 目前设计的合理性需要再考察
         * 目前的方案是：
         *   单位元尺寸按轴依赖，乘法关系
         *   角度按轴依赖，加法关系 TODO 考虑四元数操作
         *   位置依赖于上级位置、角度、单位元尺寸，类似于unity的transform
         */

        #endregion

        public virtual partial Vector3 GlobalPosition()
        {
            if (positionReference == null)
            {
                return position;
            }

            return positionReference.LocalPositionToGlobalPosition(position);
        }

        /// <summary>
        /// 全局角度
        /// </summary>
        /// <returns></returns>
        protected Vector3 GlobalRotation()
        {
            if (rotationReference == null)
            {
                return rotation;
            }

            var referenceRotation = rotationReference.GlobalRotation();
            return Vector3.FromQuaternion(
                Quaternion.Multiply(referenceRotation.ToQuaternion(), rotation.ToQuaternion()));
        }

        /// <summary>
        /// 全局单位元尺寸
        /// </summary>
        /// <returns></returns>
        protected Vector3 GlobalSize()
        {
            if (sizeReference == null)
            {
                return size;
            }

            var referenceSize = sizeReference.GlobalSize();

            return new Vector3(referenceSize.x * size.x,
                referenceSize.y * size.y,
                referenceSize.z * size.z);
        }
    }
}