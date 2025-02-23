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
            
            _userHelp.ReactOnEnvironment(!User.UsingTouch);
            ReInitTrajectory();
            RefreshTrajectory();
            
            _userHelp.TryShowHelpAnimated(Duration);
            StartCoroutine(AnimateShow(Duration, OnAnimationEnds));
            
            void RefreshBubblesInCircle()
            {
                _bubblesInCircle??= new System.Collections.Generic.List<Gameplay.User.ICircleObject>(usualBubbleCount);
                while (_bubbleCount < usualBubbleCount)
                {
                    _bubblesInCircle.Add(GiveAndSetupBubble());
                    _bubbleCount ++;
                }
                PlaceBubblesAndRecolorTrajectory();
            }
            
            void OnAnimationEnds()
            {
                InstrumentShown = true;
            }
        }
        
        private Gameplay.Bubble GiveAndSetupBubble()
        {
            var Result = Field.GiveAndPrepareBubble();
            Result.MyTransform.SetParent(User.transform);
            Result.DeactivateCollisions();
            return Result;
        }
        
        private IEnumerator AnimateShow(float Duration, System.Action OnEnd, bool isInverted = false)
        {
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            float Lerp = 0;
            
            var SwitchCircleView = _bubbleSwitchButton.GetComponent<SpriteRenderer>();
            var SwitchCircleColor = Color.black * 0.8f;
            
            var StepAngle = 360 / _bubblesInCircle.Count;
            
            StartCoroutine(RecolorTrajectory(() => Color.clear, _bubblesInCircle[0].TrajectoryColor, Steps, !isInverted));
            
            for (int Step = 0; Step <= Steps; Step++)
            {
                Lerp = Mathf.Sin(Step/(float)Steps * 90 * Mathf.Deg2Rad);
                if(isInverted) Lerp = 1 - Lerp;
                
                SwitchCircleView.color = SwitchCircleColor * Lerp;
                
                for (int i = 0; i < _bubblesInCircle.Count; i++)
                {
                    var Angle = Mathf.Repeat(StepAngle * i, 360);
                    _bubblesInCircle[i].MyTransform.localPosition = Angle2LocalPos(Angle);
                }
                yield return Wait;
            }
            OnEnd?.Invoke();
        }

        public override void HideAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            InstrumentShown = false;
            _userHelp.HideNonSwitched(Duration);
            StartCoroutine(AnimateShow(Duration, OnAnimationEnd, true));
            
            void OnAnimationEnd()
            {
                for (int i = 0; i < _bubblesInCircle.Count; i++)
                {
                    if (_bubblesInCircle[i] is Gameplay.Bubble bubble)
                    {
                        _bubblePool.Hide(bubble);
                        _bubblesInCircle.RemoveAt(i);
                        _bubbleCount--;
                        i--;
                    }
                }
                OnEnd?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}