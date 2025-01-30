namespace Gameplay.User
{
    public interface ICircleObject: Gameplay.Pools.IWithTransform
    {
        Bubble.BubbleColor MyColor          { get; }
        System.Func<UnityEngine.Color> TrajectoryColor   { get; } 
    }
}