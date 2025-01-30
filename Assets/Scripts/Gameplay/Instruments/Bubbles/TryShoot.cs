using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    public partial class Circle : BaseInstrument
    {
        void TryShoot()
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
                return Hit.collider == BubbleSwitchButton;
            }
        }
        
        void ShootBubble()
        {
            if (!User.IsClickedInGameField()) return;
            if (isBubblesOnReload) return;
            if (!instrumentShown) return;
            
            TryBreakSwitchAnimation();
            var flyTrajectory = new User.Trajectory(CollisionRadius, Field.TryResponseCollision, 1, trajectoryCollisionsCount);
            
            Gameplay.User.ICircleObject bubble = bubblesInCircle[0];
            bubblesInCircle.RemoveAt(0);
            
            bool isUsualBubble = bubble is Gameplay.Bubble;
            if (isUsualBubble)
            {
                bubbleCount--;
            }
            else
            {
                MultiBallUsed = false;
            }
            MovingBubbles.ApplyParticleToMovingBubble(bubble);
            Sounds.Play(Services.Audio.Sounds.SoundType.BubbleShoot);
            
            StartCoroutine(AnimateBubbleFly(bubble.MyTransform, ProcessEndBubbleWay));
            StartCoroutine(AnimateBubblesShift(bubble.TrajectoryColor));
        
            void TryBreakSwitchAnimation()
            {
                if (CircleRotateAnimation == null) return;
                User.StopCoroutine(CircleRotateAnimation);
                PlaceBubblesAndRecolorTrajectory();
            }
        
            IEnumerator AnimateBubbleFly(Transform Target, System.Action OnEnd)
            {
                User.CollisionType Col;
                flyTrajectory.PrepareFirst(User.transform.position, mouseClampedDirection);
                Target.position = flyTrajectory.PosOnWay;
                flyTrajectory.StepLengthOnWay = BubbleSpeed;
                while (!flyTrajectory.Completed)
                {
                    Col = flyTrajectory.TryStepAndCheckCollisions();
                    if (Col != Gameplay.User.CollisionType.None)
                    {
                        Collisions.ReactOnCollision(Col, flyTrajectory.CurrentCornerPointOnWay);
                    }
                    Target.position = flyTrajectory.PosOnWay;
                    yield return Wait;
                }
                OnEnd?.Invoke();
            }
            
            IEnumerator AnimateBubblesShift(System.Func<Color> OldColor, System.Action OnEnd = null)
            {
                isBubblesOnReload = true;
                var Increment = isUsualBubble? 1 : 0;
                var AngleStep = 360 / (bubblesInCircle.Count + Increment);
                
                bool Linear = bubblesInCircle.Count + Increment == 2;
                
                BubblePack[] Rotated = new BubblePack[bubblesInCircle.Count];
                for (int i = 0; i < Rotated.Length; i++)
                {
                    
                    Rotated[i] = new BubblePack(bubblesInCircle[i], ObjToAngle(bubblesInCircle[i]), (i + 1 + Increment) * AngleStep);
                    //Rotated[i] = new BubblePack(bubblesInCircle[i], (i+1) * AngleStep, (i+2) * AngleStep);
                }
                
                Vector2 newPos = Vector2.zero;
                Gameplay.Bubble newBubble = null;
                if (isUsualBubble)
                {
                    newPos = Angle2LocalPos(AngleStep);
                    newBubble = GiveAndSetupBubble();
                }
                
                StartCoroutine(RecolorTrajectory(OldColor, bubblesInCircle[bubblesInCircle.Count - 1].TrajectoryColor, 11));
                
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
                    bubblesInCircle.Insert(0, newBubble);
                    bubbleCount++;
                }
                LastToFirst();
                PlaceBubblesAndRecolorTrajectory();
                isBubblesOnReload = false;
                OnEnd?.Invoke();
            }
            
            void ProcessEndBubbleWay()
            {
                MovingBubbles.DisableBubbleParticle();
                flyTrajectory.StepLengthOnWay *= -1;
                
                if (isUsualBubble) ((Gameplay.Bubble)bubble).ActivateCollisions();
                Field.PlaceUserBubble(bubble, flyTrajectory, !isUsualBubble);
                if (!isUsualBubble) multiball.MyTransform.gameObject.SetActive(false);
                for(int i = 0; i < bubblesInCircle.Count; i++)
                {
                    if (bubblesInCircle[i] is Gameplay.Bubble bubble)
                    {
                        Field.TryFilterColor(ref bubble);
                    }
                }
            }
        }
    }
}