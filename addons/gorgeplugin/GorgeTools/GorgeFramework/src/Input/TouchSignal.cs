namespace Gorge.GorgeFramework.Input
{
    // /// <summary>
    // ///     触控信号值定义
    // /// </summary>
    // public class TouchSignal
    // {
    //     /// <summary>
    //     ///     是否正在按下
    //     /// </summary>
    //     public readonly bool IsTouching;
    //
    //     /// <summary>
    //     ///     位置，Gameplay坐标
    //     /// </summary>
    //     public readonly Vector2 Position;
    //
    //     public TouchSignal(bool isTouching, Vector2 position)
    //     {
    //         IsTouching = isTouching;
    //         Position = position;
    //     }
    //
    //     public override bool Equals(object obj)
    //     {
    //         return obj is TouchSignal touch && Equals(touch);
    //     }
    //
    //     private bool Equals(TouchSignal other)
    //     {
    //         if (ReferenceEquals(null, other)) return false;
    //         if (ReferenceEquals(this, other)) return true;
    //         return IsTouching == other.IsTouching && Position.Equals(other.Position);
    //     }
    //
    //     public override int GetHashCode()
    //     {
    //         return HashCode.Combine(IsTouching, Position);
    //     }
    // }
}