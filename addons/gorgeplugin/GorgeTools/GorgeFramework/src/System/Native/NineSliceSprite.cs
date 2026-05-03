using System;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class NineSliceSprite
    {
        private INineSliceSprite _sprite;

        public NineSliceSprite(Injector injector, Graph graph, Vector2 sliceLeftTop, Vector2 sliceRightBottom,
            Vector2 baseSize) : base(injector)
        {
            FieldInitialize(injector);
            this.graph = graph;
            this.sliceLeftTop = sliceLeftTop;
            this.sliceRightBottom = sliceRightBottom;
            this.baseSize = baseSize;

            _sprite = Base.Instance.CreateNineSliceSprite();
            _sprite.SetGraph(graph, baseSize, sliceLeftTop, sliceRightBottom);
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        private static partial Graph InitializeField_graph() => default;

        private static partial Vector2 InitializeField_sliceLeftTop() => default;

        private static partial Vector2 InitializeField_sliceRightBottom() => default;

        private static partial Vector2 InitializeField_baseSize() => default;

        private static partial ColorArgb InitializeField_color() => ColorArgb.White;

        private static partial Vector3 InitializeField_hsl() => new(0, 0, 0);

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
                _sprite.SetHsl(hsl);

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