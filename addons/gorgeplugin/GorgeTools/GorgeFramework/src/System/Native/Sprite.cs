using System;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class Sprite
    {
        private ISprite _sprite;

        public Sprite(Injector injector, Graph graph) : base(injector)
        {
            FieldInitialize(injector);
            this.graph = graph;
            _sprite = Base.Instance.CreateSprite();
            UpdateGraph();
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        private static partial Graph InitializeField_graph() => default;

        private static partial ColorArgb InitializeField_color() => ColorArgb.White;

        #region 更新图片

        private Graph _nowGraph;
        private float _nowAspectRatio = 1;

        #endregion

        private void UpdateGraph()
        {
            // 没变化则不更新
            if (_nowGraph == graph)
            {
                return;
            }

            _nowGraph = graph;
            if (graph == null)
            {
                _sprite.SetGraph(null);
                return;
            }

            _nowAspectRatio = (float) graph.width / graph.height;
            _sprite.SetGraph(graph);
        }


        public override void UpdateNode()
        {
            base.UpdateNode();
            if (alive)
            {
                // TODO 目前上级信息是重复计算的，可以存储是否经过计算，并且可以融合污点方案做最少计算

                #region 位置

                _sprite.SetPosition(GlobalPosition());
                _sprite.SetRotation(GlobalRotation());

                var globalSize = GlobalSize();
                _sprite.SetScale(new Vector3(globalSize.x, globalSize.y * _nowAspectRatio, 1));

                #endregion

                #region 颜色

                _sprite.SetColor(color);

                #endregion

                #region 图像

                UpdateGraph();

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