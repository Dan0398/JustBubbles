using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.User
{
    [System.Serializable]
    public class Trajectory
    {
        public bool Completed   { get; private set; }
        public bool WayEnded    { get; private set; }
        public Vector3 PosOnWay { get; private set; }
        public List<Corner> Corners;
        public float StepLengthOnWay;
        
        private readonly System.Func<Collider2D, CollisionType> ResponseCollision;
        private readonly float CollisionRadius;
        private readonly int CollisionLayer;
        private readonly float maxDistance;
        
        private int _cornerNumberOnWay;
        private int _maxCornersCount;
        private float _wayFollowedBetweenCorners;
        private float _achievedDistance;
        private RaycastHit2D[] _raycasts;
        
        public Vector3 CurrentCornerPointOnWay => Corners[_cornerNumberOnWay].Origin;
        
        public Vector3 LastDirOnWay => Corners[_cornerNumberOnWay].Direction;
        
        public Trajectory(float collisionRadius, System.Func<Collider2D, CollisionType> responseCollision, int collisionLayer, int MaxCollisions = 20, float MaxDistance = float.MaxValue)
        {
            Corners = new List<Corner>(8);
            CollisionRadius = collisionRadius;
            ResponseCollision = responseCollision;
            Completed = false;
            _cornerNumberOnWay = 0;
            _raycasts = new RaycastHit2D[5];
            CollisionLayer = collisionLayer;
            _maxCornersCount = MaxCollisions;
            maxDistance = MaxDistance;
        }
        
        public void CalculateFullWayClean(Vector3 Pos, Vector3 Dir)
        {
            PrepareFirst(Pos, Dir);
            ProcessCorners();
            
            void ProcessCorners()
            {
                for (int i = 0; i < _maxCornersCount; i ++)
                {
                    CollisionType Info = CollisionType.OutOfScreen;
                    var dist = Mathf.Clamp(maxDistance - _achievedDistance, 0 , 40);
                    if (Physics2D.CircleCastNonAlloc(Corners[i].Origin, CollisionRadius, Corners[i].Direction, _raycasts, dist, CollisionLayer) == 0) 
                    {
                        Corners[i].SubmitEnd(Corners[i].Origin + Corners[i].Direction * dist, Info);
                        break;
                    }
                    _achievedDistance += ((Vector2)Corners[i].Origin - _raycasts[0].centroid).magnitude;
                    Info = ResponseCollision.Invoke(_raycasts[0].collider);
                    Corners[i].SubmitEnd(_raycasts[0].centroid, Info);
                    if (Info == CollisionType.IntoBunch)
                    {
                        break;
                    }
                    //Barrier
                    if (i < _maxCornersCount-1)
                    {
                        var Cor = new Corner((Vector3)_raycasts[0].centroid - Corners[i].Direction * 0.01f, Vector3.Reflect(Corners[i].Direction, _raycasts[0].normal).normalized);
                        Corners.Add(Cor);
                    }
                }
                Completed = true;
            }
        }
        
        public void PrepareFirst(Vector3 Pos, Vector3 Dir)
        {
            Corners.Clear();
            Completed = false;
            Corners.Add(new Corner(Pos, Dir));
            PosOnWay = Pos;
            _achievedDistance = 0;
        }
        
        public CollisionType TryStepAndCheckCollisions()
        {
            var Info = CollisionType.None;
            var corner = Corners[Corners.Count-1];
            var Dist = Mathf.Clamp(maxDistance - _achievedDistance, 0, StepLengthOnWay);
            if (Physics2D.CircleCastNonAlloc(PosOnWay, CollisionRadius, corner.Direction, _raycasts, Dist, CollisionLayer) == 0) 
            {
                _achievedDistance += Dist;
                PosOnWay += corner.Direction * StepLengthOnWay;
                _wayFollowedBetweenCorners += StepLengthOnWay;
                if (_wayFollowedBetweenCorners > 40 || Dist == 0)
                {
                    Info = CollisionType.OutOfScreen;
                    corner.SubmitEnd(PosOnWay, Info);
                    _wayFollowedBetweenCorners = corner.Length;
                    Completed = true;
                    WayEnded = true;
                    return Info;
                }
            }
            else 
            {
                Info = ResponseCollision.Invoke(_raycasts[0].collider);
                corner.SubmitEnd(_raycasts[0].centroid, Info);
                if (Info == CollisionType.IntoBunch || Corners.Count == _maxCornersCount)
                {
                    PosOnWay = corner.Endpoint;
                    _wayFollowedBetweenCorners = corner.Length;
                    Completed = true;
                    WayEnded = true;
                }
                else 
                {
                    var Cor = new Corner((Vector3)_raycasts[0].centroid - corner.Direction * 0.01f, Vector3.Reflect(corner.Direction, _raycasts[0].normal).normalized);
                    Corners.Add(Cor);
                    PosOnWay = Cor.Origin + Cor.Direction * (StepLengthOnWay - _raycasts[0].distance);
                    _wayFollowedBetweenCorners = 0;
                }
            }
            _cornerNumberOnWay = Corners.Count-1;
            return Info;
        }
        
        public void ResetWay()
        {
            WayEnded = false;
            _cornerNumberOnWay = 0;
            PosOnWay = Corners.Count == 0? Vector3.zero : Corners[0].Origin;
            _wayFollowedBetweenCorners = 0;
            _achievedDistance = 0;
        }
        
        public void MakeStep()
        {
            MakeStep(StepLengthOnWay);
        }
        
        public void MakeStep(float Scale)
        {
            if (WayEnded && Scale > 0)
            {
                Debug.LogError("Way ends");
                return;
            }
            _wayFollowedBetweenCorners += Scale;
            if (Scale >= 0)
            {
                try
                {
                    while(_wayFollowedBetweenCorners > Corners[_cornerNumberOnWay].Length)
                    {
                        _wayFollowedBetweenCorners -= Corners[_cornerNumberOnWay].Length;
                        _cornerNumberOnWay++;
                        if (_cornerNumberOnWay == Corners.Count) 
                        {
                            PosOnWay = Corners[Corners.Count-1].Endpoint;
                            WayEnded = true;
                            _wayFollowedBetweenCorners = 0;
                            return;
                        }
                    }
                }
                catch(System.Exception ex)
                {
                    Debug.Log("Scale >= 0 statement: " + Scale.ToString());
                    Debug.LogError("Corners count: " + Corners.Count + " CornerNumberOnWay: " + _cornerNumberOnWay);
                    throw ex;
                }
            }
            else
            {
                try
                {
                    while(_wayFollowedBetweenCorners < 0)
                    {
                        if (_cornerNumberOnWay > 0)
                        {
                            _cornerNumberOnWay--;
                            _wayFollowedBetweenCorners += Corners[_cornerNumberOnWay].Length;
                        }
                        else 
                        {
                            _wayFollowedBetweenCorners = 0;
                            break;
                        }
                    }
                }
                catch(System.Exception ex)
                {
                    Debug.Log("Scale < 0 statement: " + Scale.ToString());
                    Debug.LogError("Corners count: " + Corners.Count + " CornerNumberOnWay: " + _cornerNumberOnWay);
                    throw ex;
                }
            }
            PosOnWay = Corners[_cornerNumberOnWay].Origin + Corners[_cornerNumberOnWay].Direction * _wayFollowedBetweenCorners;
        }
    }
}