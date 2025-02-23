namespace Gameplay.Instruments.Bubble
{
    public class BubblePack
    {
        public readonly Gameplay.User.ICircleObject Bubble;
        public readonly float OldAngle;
        public readonly float NewAngle;
        
        public BubblePack(Gameplay.User.ICircleObject bubble, float oldAngle, float newAngle)
        {
            Bubble = bubble;
            OldAngle = oldAngle;
            NewAngle = newAngle;
        }
    }
}