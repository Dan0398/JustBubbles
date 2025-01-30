using UnityEngine;

namespace Gameplay.Instruments
{
    [System.Serializable]
    public class MultiBall: Gameplay.User.ICircleObject
    {
        [field:SerializeField] public Transform MyTransform { get; private set; }

        public Gameplay.Bubble.BubbleColor MyColor => Gameplay.Bubble.BubbleColor.Red;
        
        public System.Func<Color> TrajectoryColor => () => Color.HSVToRGB((UnityEngine.Time.time/2)%1, 1, 1);
    }
}
