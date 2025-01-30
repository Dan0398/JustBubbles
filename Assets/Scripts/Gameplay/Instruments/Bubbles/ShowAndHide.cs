using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    public partial class Circle : BaseInstrument
    {
        public override void ShowAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            gameObject.SetActive(true);
            RefreshBubblesInCircle();
            
            UserHelp.ReactOnEnvironment(!User.UsingTouch);
            ReInitTrajectory();
            RefreshTrajectory();
            
            UserHelp.TryShowHelpAnimated(Duration);
            StartCoroutine(AnimateShow(Duration, OnAnimationEnds));
            
            void RefreshBubblesInCircle()
            {
                bubblesInCircle??= new System.Collections.Generic.List<Gameplay.User.ICircleObject>(usualBubbleCount);
                while (bubbleCount < usualBubbleCount)
                {
                    bubblesInCircle.Add(GiveAndSetupBubble());
                    bubbleCount ++;
                }
                PlaceBubblesAndRecolorTrajectory();
            }
            
            void OnAnimationEnds()
            {
                instrumentShown = true;
            }
        }
        
        Gameplay.Bubble GiveAndSetupBubble()
        {
            var Result = Field.GiveAndPrepareBubble();
            Result.MyTransform.SetParent(User.transform);
            Result.DeactivateCollisions();
            return Result;
        }
        
        IEnumerator AnimateShow(float Duration, System.Action OnEnd, bool isInverted = false)
        {
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            float Lerp = 0;
            
            var SwitchCircleView = BubbleSwitchButton.GetComponent<SpriteRenderer>();
            var SwitchCircleColor = Color.black * 0.8f;
            
            var StepAngle = 360 / bubblesInCircle.Count;
            
            StartCoroutine(RecolorTrajectory(() => Color.clear, bubblesInCircle[0].TrajectoryColor, Steps, !isInverted));
            
            for (int Step = 0; Step <= Steps; Step++)
            {
                Lerp = Mathf.Sin(Step/(float)Steps * 90 * Mathf.Deg2Rad);
                if(isInverted) Lerp = 1 - Lerp;
                
                SwitchCircleView.color = SwitchCircleColor * Lerp;
                
                for (int i = 0; i < bubblesInCircle.Count; i++)
                {
                    var Angle = Mathf.Repeat(StepAngle * i, 360);
                    bubblesInCircle[i].MyTransform.localPosition = Angle2LocalPos(Angle);
                }
                yield return Wait;
            }
            OnEnd?.Invoke();
        }

        public override void HideAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            instrumentShown = false;
            UserHelp.HideNonSwitched(Duration);
            StartCoroutine(AnimateShow(Duration, OnAnimationEnd, true));
            
            void OnAnimationEnd()
            {
                for (int i = 0; i < bubblesInCircle.Count; i++)
                {
                    if (bubblesInCircle[i] is Gameplay.Bubble bubble)
                    {
                        BubblePool.Hide(bubble);
                        bubblesInCircle.RemoveAt(i);
                        bubbleCount--;
                        i--;
                    }
                }
                OnEnd?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}