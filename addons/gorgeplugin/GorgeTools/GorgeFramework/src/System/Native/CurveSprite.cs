using System;
using Godot;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class CurveSprite
    {
        private ICurveSprite _sprite;

        public CurveSprite(Injector injector, ObjectArray points) : base(injector)
        {
            FieldInitialize(injector);
            this.points = points;

            _sprite = Base.Instance.CreateCurveSprite();
            _sprite.SetLine(points);
        }

        private static partial ObjectArray InitializeField_points() => default;

        private static partial ColorArgb InitializeField_color() => ColorArgb.White;

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();


        /// <summary>
        /// Unity承载对象
        /// </summary>
        public override void UpdateNode()
        {
            base.UpdateNode();
            if (alive)
            {
                // TODO 目前上级信息是重复计算的，可以存储是否经过计算，并且可以融合污点方案做最少计算

                #region 位置

                _sprite.SetPosition(GlobalPosition());
                _sprite.SetRotation(GlobalRotation());
                _sprite.SetScale(GlobalSize());

                #endregion

                #region 颜色

                _sprite.SetColor(color);

                #endregion

                #region 形状

                _sprite.SetLine(points);

                #endregion
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            _sprite.Destroy();
        }
    }
}