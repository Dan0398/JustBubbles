using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    public partial class Circle : BaseInstrument
    {
        private void TryShoot()
        {
            if (IsClickedToSwitchBubbleButton())
            {
                RotateBubbleCircle();
            }
            else
            {
                ShootBubble();
            }
            
            bool IsClickedToSwitchBubbleButton()
            {
                var Ray = User.GetScreenRayAtCursor();
                var Hit = Physics2D.Raycast(Ray.origin, Ray.direction, 100, 2);
                return Hit.collider == _bubbleSwitchButton;
            }
        }
        
        private void ShootBubble()
        {
            if (!User.IsClickedInGameField()) return;
            if (_isBubblesOnReload) return;
            if (!InstrumentShown) return;
            
            TryBreakSwitchAnimation();
            var flyTrajectory = new User.Trajectory(CollisionRadius, Field.TryResponseCollision, 1, TrajectoryCollisionsCount);
            
            Gameplay.User.ICircleObject bubble = _bubblesInCircle[0];
            _bubblesInCircle.RemoveAt(0);
            
            bool isUsualBubble = bubble is Gameplay.Bubble;
            if (isUsualBubble)
            {
                _bubbleCount--;
            }
            else
            {
                MultiBallUsed = false;
            }
            _movingBubbles.ApplyParticleToMovingBubble(bubble);
            Sounds.Play(Services.Audio.Sounds.SoundType.BubbleShoot);
            
            StartCoroutine(AnimateBubbleFly(bubble.MyTransform, ProcessEndBubbleWay));
            StartCoroutine(AnimateBubblesShift(bubble.TrajectoryColor));
        
            void TryBreakSwitchAnimation()
            {
                if (_circleRotateAnimation == null) return;
                User.StopCoroutine(_circleRotateAnimation);
                PlaceBubblesAndRecolorTrajectory();
            }
        
            IEnumerator AnimateBubbleFly(Transform Target, System.Action OnEnd)
            {
                User.CollisionType Col;
                flyTrajectory.PrepareFirst(User.transform.position, MouseClampedDirection);
                Target.position = flyTrajectory.PosOnWay;
                flyTrajectory.StepLengthOnWay = _bubbleSpeed;
                while (!flyTrajectory.Completed)
                {
                    Col = flyTrajectory.TryStepAndCheckCollisions();
                    if (Col != Gameplay.User.CollisionType.None)
                    {
                        _collisions.ReactOnCollision(Col, flyTrajectory.CurrentCornerPointOnWay);
                    }
                    Target.position = flyTrajectory.PosOnWay;
                    yield return Wait;
                }
                OnEnd?.Invoke();
            }
            
            IEnumerator AnimateBubblesShift(System.Func<Color> OldColor, System.Action OnEnd = null)
            {
                _isBubblesOnReload = true;
                var Increment = isUsualBubble? 1 : 0;
                var AngleStep = 360 / (_bubblesInCircle.Count + Increment);
                
                bool Linear = _bubblesInCircle.Count + Increment == 2;
                
                BubblePack[] Rotated = new BubblePack[_bubblesInCircle.Count];
                for (int i = 0; i < Rotated.Length; i++)
                {
                    
                    Rotated[i] = new BubblePack(_bubblesInCircle[i], ObjToAngle(_bubblesInCircle[i]), (i + 1 + Increment) * AngleStep);
                }
                
                Vector2 newPos = Vector2.zero;
                Gameplay.Bubble newBubble = null;
                if (isUsualBubble)
                {
                    newPos = Angle2LocalPos(AngleStep);
                    newBubble = GiveAndSetupBubble();
                }
                
                StartCoroutine(RecolorTrajectory(OldColor, _bubblesInCircle[_bubblesInCircle.Count - 1].TrajectoryColor, 11));
                
                float Lerp = 0;
                for (int i = 0; i < 10; i ++)
                {
                    Lerp += .1f;
                    for (int k = 0; k < Rotated.Length; k++)
                    {
                        if (Linear)
                        {
                            var Pos = Vector2.Lerp(Angle2LocalPos(Rotated[k].OldAngle), Angle2LocalPos(Rotated[k].NewAngle), Lerp);
                            Rotated[k].Bubble.MyTransform.localPosition = Pos;
                        }
                        else
                        {
                            var Angle = Mathf.Lerp(Rotated[k].OldAngle, Rotated[k].NewAngle, Lerp);
                            Rotated[k].Bubble.MyTransform.localPosition = Angle2LocalPos(Angle);
                        }
                    }
                    
                    if (isUsualBubble) newBubble.MyTransform.localPosition = Vector2.Lerp(OutOfViewPos, newPos, Lerp);
                    
                    yield return Wait;
                }
                if (isUsualBubble)
                {
                    _bubblesInCircle.Insert(0, newBubble);
                    _bubbleCount++;
                }
                LastToFirst();
                PlaceBubblesAndRecolorTrajectory();
                _isBubblesOnReload = false;
                OnEnd?.Invoke();
            }
            
            void ProcessEndBubbleWay()
            {
                _movingBubbles.DisableBubbleParticle();
                flyTrajectory.StepLengthOnWay *= -1;
                
                if (isUsualBubble) ((Gameplay.Bubble)bubble).ActivateCollisions();
                Field.PlaceUserBubble(bubble, flyTrajectory, !isUsualBubble);
                if (!isUsualBubble) _multiBall.MyTransform.gameObject.SetActive(false);
                for(int i = 0; i < _bubblesInCircle.Count; i++)
                {
                    if (_bubblesInCircle[i] is Gameplay.Bubble bubble)
                    {
                        Field.TryFilterColor(ref bubble);
                    }
                }
            }
        }
    }
}