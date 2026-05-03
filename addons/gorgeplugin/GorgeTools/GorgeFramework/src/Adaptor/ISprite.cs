#nullable enable
using Gorge.Native.Gorge;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Adaptor
{
    public interface ISceneObject
    {
        /// <summary>
        /// 设置坐标
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector3 position);

        /// <summary>
        /// 设置角度，欧拉角
        /// </summary>
        /// <param name="rotation"></param>
        public void SetRotation(Vector3 rotation);

        /// <summary>
        /// 设置比例
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(Vector3 scale);

        /// <summary>
        /// 销毁
        /// </summary>
        public void Destroy();
    }

    public interface ISprite : ISceneObject
    {
        /// <summary>
        /// 设置精灵图像
        /// </summary>
        /// <param name="graph"></param>
        public void SetGraph(Graph? graph);

        /// <summary>
        /// 设置精灵图像颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(ColorArgb color);
    }

    public interface INineSliceSprite : ISceneObject
    {
        /// <summary>
        /// 设置图像
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="baseSize"></param>
        /// <param name="sliceLeftTop"></param>
        /// <param name="sliceRightBottom"></param>
        public void SetGraph(Graph graph, Vector2 baseSize, Vector2 sliceLeftTop, Vector2 sliceRightBottom);

        /// <summary>
        /// 设置精灵图像颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(ColorArgb color);

        /// <summary>
        /// 设置HSL色偏
        /// </summary>
        /// <param name="hsl"></param>
        public void SetHsl(Vector3 hsl);
    }
    
    public interface ICurveSprite : ISceneObject
    {
        /// <summary>
        /// 设置曲线
        /// </summary>
        /// <param name="pointArray"></param>
        public void SetLine(ObjectArray? pointArray);

        /// <summary>
        /// 设置精灵图像颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(ColorArgb color);
        
    }
}