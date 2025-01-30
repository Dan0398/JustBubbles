using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.User
{
    [System.Serializable]
    public class Trajectory
    {
        readonly System.Func<Collider2D, CollisionType> ResponseCollision;
        readonly float CollisionRadius;
        readonly int CollisionLayer;
        readonly float maxDistance;
        public List<Corner> Corners;
        public bool Completed { get; private set; }
        public bool WayEnded { get; private set; }
        public float StepLengthOnWay;
        public Vector3 PosOnWay { get; private set; }
        int CornerNumberOnWay;
        int MaxCornersCount;
        float WayFollowedBetweenCorners;
        float AchievedDistance;
        RaycastHit2D[] Raycasts;
        
        public Vector3 CurrentCornerPointOnWay => Corners[CornerNumberOnWay].Origin;
        public Vector3 LastDirOnWay => Corners[CornerNumberOnWay].Direction;
        
        public Trajectory(float collisionRadius, System.Func<Collider2D, CollisionType> responseCollision, int collisionLayer, int MaxCollisions = 20, float MaxDistance = float.MaxValue)
        {
            Corners = new List<Corner>(8);
            CollisionRadius = collisionRadius;
            ResponseCollision = responseCollision;
            Completed = false;
            CornerNumberOnWay = 0;
            Raycasts = new RaycastHit2D[5];
            CollisionLayer = collisionLayer;
            MaxCornersCount = MaxCollisions;
            maxDistance = MaxDistance;
        }
        
        public void CalculateFullWayClean(Vector3 Pos, Vector3 Dir)
        {
            PrepareFirst(Pos, Dir);
            ProcessCorners();
            
            void ProcessCorners()
            {
                for (int i = 0; i < MaxCornersCount; i ++)
                {
                    CollisionType Info = CollisionType.OutOfScreen;
                    var dist = Mathf.Clamp(maxDistance - AchievedDistance, 0 , 40);
                    if (Physics2D.CircleCastNonAlloc(Corners[i].Origin, CollisionRadius, Corners[i].Direction, Raycasts, dist, CollisionLayer) == 0) 
                    {
                        Corners[i].SubmitEnd(Corners[i].Origin + Corners[i].Direction * dist, Info);
                        break;
                    }
                    AchievedDistance += ((Vector2)Corners[i].Origin - Raycasts[0].centroid).magnitude;
                    Info = ResponseCollision.Invoke(Raycasts[0].collider);
                    Corners[i].SubmitEnd(Raycasts[0].centroid, Info);
                    if (Info == CollisionType.IntoBunch)
                    {
                        break;
                    }
                    //Barrier
                    if (i < MaxCornersCount-1)
                    {
                        var Cor = new Corner((Vector3)Raycasts[0].centroid - Corners[i].Direction * 0.01f, Vector3.Reflect(Corners[i].Direction, Raycasts[0].normal).normalized);
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
            AchievedDistance = 0;
        }
        
        public CollisionType TryStepAndCheckCollisions()
        {
            var Info = CollisionType.None;
            var corner = Corners[Corners.Count-1];
            var Dist = Mathf.Clamp(maxDistance - AchievedDistance, 0, StepLengthOnWay);
            if (Physics2D.CircleCastNonAlloc(PosOnWay, CollisionRadius, corner.Direction, Raycasts, Dist, CollisionLayer) == 0) 
            {
                AchievedDistance += Dist;
                PosOnWay += corner.Direction * StepLengthOnWay;
                WayFollowedBetweenCorners += StepLengthOnWay;
                if (WayFollowedBetweenCorners > 40 || Dist == 0)
                {
                    Info = CollisionType.OutOfScreen;
                    corner.SubmitEnd(PosOnWay, Info);
                    WayFollowedBetweenCorners = corner.Length;
                    Completed = true;
                    WayEnded = true;
                    return Info;
                }
            }
            else 
            {
                Info = ResponseCollision.Invoke(Raycasts[0].collider);
                corner.SubmitEnd(Raycasts[0].centroid, Info);
                if (Info == CollisionType.IntoBunch || Corners.Count == MaxCornersCount)
                {
                    PosOnWay = corner.Endpoint;
                    WayFollowedBetweenCorners = corner.Length;
                    Completed = true;
                    WayEnded = true;
                }
                else 
                {
                    var Cor = new Corner((Vector3)Raycasts[0].centroid - corner.Direction * 0.01f, Vector3.Reflect(corner.Direction, Raycasts[0].normal).normalized);
                    Corners.Add(Cor);
                    PosOnWay = Cor.Origin + Cor.Direction * (StepLengthOnWay - Raycasts[0].distance);
                    WayFollowedBetweenCorners = 0;
                }
            }
            CornerNumberOnWay = Corners.Count-1;
            return Info;
        }
        
        public void ResetWay()
        {
            WayEnded = false;
            CornerNumberOnWay = 0;
            PosOnWay = Corners.Count == 0? Vector3.zero : Corners[0].Origin;
            WayFollowedBetweenCorners = 0;
            AchievedDistance = 0;
        }
        
        public void MakeStep() => MakeStep(StepLengthOnWay);
        
        public void MakeStep(float Scale)
        {
            if (WayEnded && Scale > 0)
            {
                Debug.LogError("Way ends");
                return;
            }
            WayFollowedBetweenCorners += Scale;
            if (Scale >= 0)
            {
                try
                {
                    while(WayFollowedBetweenCorners > Corners[CornerNumberOnWay].Length)
                    {
                        WayFollowedBetweenCorners -= Corners[CornerNumberOnWay].Length;
                        CornerNumberOnWay++;
                        if (CornerNumberOnWay == Corners.Count) 
                        {
                            PosOnWay = Corners[Corners.Count-1].Endpoint;
                            WayEnded = true;
                            WayFollowedBetweenCorners = 0;
                            return;
                        }
                    }
                }
                catch(System.Exception ex)
                {
                    Debug.Log("Scale >= 0 statement: " + Scale.ToString());
                    Debug.LogError("Corners count: " + Corners.Count + " CornerNumberOnWay: " + CornerNumberOnWay);
                    throw ex;
                }
            }
            else
            {
                try
                {
                    while(WayFollowedBetweenCorners < 0)
                    {
                        if (CornerNumberOnWay > 0)
                        {
                            CornerNumberOnWay--;
                            WayFollowedBetweenCorners += Corners[CornerNumberOnWay].Length;
                        }
                        else 
                        {
                            WayFollowedBetweenCorners = 0;
                            break;
                        }
                    }
                }
                catch(System.Exception ex)
                {
                    Debug.Log("Scale < 0 statement: " + Scale.ToString());
                    Debug.LogError("Corners count: " + Corners.Count + " CornerNumberOnWay: " + CornerNumberOnWay);
                    throw ex;
                }
            }
            PosOnWay = Corners[CornerNumberOnWay].Origin + Corners[CornerNumberOnWay].Direction * WayFollowedBetweenCorners;
        }
    }
}