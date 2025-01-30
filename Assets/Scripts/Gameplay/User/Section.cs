using UnityEngine;

namespace Gameplay.User
{
    [System.Serializable]
    public struct Section
    {
        public readonly Vector3 StartPoint;
        public readonly Vector3 EndPoint;
        public readonly Vector3 Direction;
        public readonly Vector3 CollisionPoint;
        public readonly float Length;
        public readonly CollisionType EndCollisionInfo;
        
        public Section(Vector3 Start, Vector3 End, Vector3 CollisionPoint, CollisionType Type)
        {
            StartPoint = Start;
            EndPoint = End;
            Direction = (EndPoint - StartPoint).normalized;
            Length = Vector3.Distance(StartPoint, EndPoint);
            this.CollisionPoint = CollisionPoint;
            EndCollisionInfo = Type;
        }
    }
}