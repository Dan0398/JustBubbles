using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        public SniperShotResult ProcessSniperShot(Vector3 StartWorldPos, Vector3 ShootDir, float ShootDistance)
        {
            List<BubbleDist> ShootedBubbles = new(5);
            List<Place> ShootedPlaces = new(5);
            Vector3 BarrierDir = Vector3.down;
            Vector3 BarrierPoint = Vector2.zero;
            Vector3 CrossPoint = Vector3.zero;
            
            var BubbleCountPerLine = GimmeBubbleCount();
            
            int LineStartID = 0;
            int PlaceStartID = 0, PlaceEndID = BubbleCountPerLine;
            
            CheckSideBannerIntersection();
            TryClampBubbleIDs();
            FillBubbleArray();
            SniperShotResult Result = null;
            Result = new SniperShotResult(ShootedBubbles, CleanupField);
            return Result;
            
            void CheckSideBannerIntersection()
            {
                BarrierDir = Vector3.down;
                BarrierPoint = new Vector3(_startPoint.x + _fieldSize.x * (ShootDir.x > 0? 1 : 0) - BubbleSize * 0.5f, _startPoint.y + BubbleSize * 0.5f);
                
                if (!IsIntersected()) return;
                LineStartID = Mathf.Max(0, GetLineNumberForPos(CrossPoint));
            }
            
            bool IsIntersected()
            {
                Vector3 lineVec3 = StartWorldPos - BarrierPoint;
                Vector3 crossVec1and2 = Vector3.Cross(BarrierDir, ShootDir);
                Vector3 crossVec3and2 = Vector3.Cross(lineVec3, ShootDir);
                float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

                //is coplanar, and not parallel
                if(Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
                {
                    float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                    CrossPoint = BarrierPoint + (BarrierDir * s);
                    return true;
                }
                return false;
            }
            
            void TryClampBubbleIDs()
            {
                BarrierDir = Vector3.right;
                BarrierPoint = _startPoint + BubbleSize * 0.5f * Vector2.up;
                if (ShootDir.x < 0) TryClamp(ref PlaceStartID);
                else                TryClamp(ref PlaceEndID);
                BarrierPoint += _lines.Count * _lineHeight * Vector3.down;
                if (ShootDir.x < 0) TryClamp(ref PlaceEndID);
                else                TryClamp(ref PlaceStartID);
                PlaceStartID =  Mathf.Max(PlaceStartID - 1  , 0);
                PlaceEndID =    Mathf.Min(PlaceEndID +1     , BubbleCountPerLine);
                
                void TryClamp(ref int SourceID)
                {
                    if (!IsIntersected()) return;
                    SourceID = GetPlaceNumberForPos(CrossPoint, LineStartID);
                }
            }
            
            void FillBubbleArray()
            {
                var HitDistance = BubbleSize * 0.5f;
                HitDistance *= HitDistance;
                var ShootDistanceSqr = ShootDistance * ShootDistance;
                var Step = Vector3.right * BubbleSize;
                for (int Line = LineStartID; Line < _lines.Count; Line ++)
                {
                    var BubbleCenter = PlaceToPos(Line, PlaceStartID);
                    for (int Place = PlaceStartID; Place < PlaceEndID; Place ++)
                    {
                        if (_lines[Line][Place] != null)
                        {
                            float distance = Vector3.Cross(ShootDir, BubbleCenter - StartWorldPos).sqrMagnitude;
                            if (distance <= HitDistance)
                            {
                                ShootedPlaces.Add(new Field.Place(Line, Place));
                                var Bubble = _lines[Line][Place];
                                _lines[Line][Place] = null;
                                ShootedBubbles.Add(new BubbleDist((BubbleCenter - StartWorldPos).magnitude,
                                                                        () => HideBubble(Bubble)));
                            }
                        }
                        BubbleCenter += Step;
                    }
                }
                
                void HideBubble(Bubble Bubble)
                {
                    _pool.Hide(Bubble);
                    ColorStats.DecountByBubble(Bubble);
                }
            }
            
            void CleanupField()
            {
                SeekNonConnectedToRoot();
                if (_nonRootChank.Count > 0)
                {
                    _effects.AnimateFallUnconnectedBubbles(CollectBubbles(_nonRootChank));
                }
                _reactOnBubbleSet.Invoke(ShootedPlaces, _nonRootChank, typeof(Instruments.SniperShot));
                CleanEmptyLines();
                OnFieldRefreshed?.Invoke();
                Result.OnRequireCleanField = null;
            }
        }

        public class SniperShotResult
        {
            public System.Action OnRequireCleanField;
            private readonly List<BubbleDist> _underFire;
            
            public SniperShotResult(List<BubbleDist> bubbles, System.Action CleanField)
            {
                OnRequireCleanField = CleanField;
                _underFire = bubbles;
            }
            
            public int RemoveBubbleByDistance(float AchievedDistance)
            {
                for(int i = 0; i < _underFire.Count; i++)
                {
                    if (_underFire[i].DistanceToRoot < AchievedDistance)
                    {
                        _underFire[i].OnClean.Invoke();
                        _underFire[i].OnClean = null;
                        _underFire.RemoveAt(i);
                        i--;
                    }
                }
                return _underFire.Count;
            }
        }
        
        public class BubbleDist
        {
            public float DistanceToRoot;
            public System.Action OnClean;
            
            public BubbleDist(float distance, System.Action onClean)
            {
                DistanceToRoot = distance;
                OnClean = onClean;
            }
        }
    }
}