using UnityEngine;

namespace Gameplay.User
{
    [System.Serializable]
    public class CollisionPair
    {
        public CollisionType Type;
        public Collider2D Col;
    }
    
    public enum CollisionType
    {
        None,
        LeftBarrier,
        RightBarrier,
        TopBarrier,
        OutOfScreen,
        IntoBunch
    }
}