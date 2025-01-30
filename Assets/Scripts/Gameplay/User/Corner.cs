using UnityEngine;

namespace Gameplay.User
{
    [System.Serializable]
    public class Corner
    {
        public Vector3 Origin, Direction;
        public float Length                     { get; private set; }
        public Vector3 Endpoint                 { get; private set; }
        public CollisionType CollisionReason    { get; private set; }
        
        public Corner(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }
        
        public void SubmitEnd(Vector3 Point, CollisionType Info)
        {
            Endpoint = Point;
            Length = Vector3.Distance(Endpoint, Origin);
            CollisionReason = Info;
        }
    }
}