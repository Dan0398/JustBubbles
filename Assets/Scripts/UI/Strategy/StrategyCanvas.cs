using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Strategy
{
    [RequireComponent(typeof(Animator))]
    public class StrategyCanvas : MonoBehaviour
    {
        [SerializeField] float ShowAnimationSpeed;
        [Header("In-game data In Header")]
        [SerializeField] Image PopComboFill;
        [SerializeField] TMPro.TMP_Text BeforeAppendCountLabel, AppendLinesCountLabel, ClicksCountLabel;
        [Header("Removed overlay")]
        [SerializeField] GameObject RemovedColorParent;
        [SerializeField] RemoveColorContainer[] RemovedColors;
        [Space()]
        [SerializeField] Animator HelpAnimator;
        [Space()]
        [SerializeField] Endgame EndgameView;
        Coroutine PopComboRoutine;
        Gameplay.GameType.Strategy parent;
        System.Action AfterPopEnd;
        bool HeaderShown;
        bool turnOffLocked;

        public void Show(Gameplay.GameType.Strategy strategy, float Duration = 1f)
        {
            parent = strategy;
            gameObject.SetActive(true);
            turnOffLocked= false;
            if (HeaderShown) return;
            var animator = GetComponent<Animator>();
            animator.SetFloat("Speed", ShowAnimationSpeed / Duration);
            ShowHeader();
        }
        
        void ShowHeader()
        {
            GetComponent<Animator>().SetTrigger("Show");
            HeaderShown = true;
        }
        
        public void RegisterHideFromAnimator()
        {
            if (turnOffLocked) return;
            gameObject.SetActive(false);
        }

        public void ReactOnColorRemove(List<Gameplay.Bubble.BubbleColor> obj)
        {
            for (int i = 0; i < RemovedColors.Length; i++)
            {
                bool required = i < obj.Count;
                RemovedColors[i].Turnable.SetActive(required);
                if (required)
                {
                    RemovedColors[i].Shown.color = Gameplay.ColorPicker.GetColorByEnum(obj[i]);
                }
            }
            RemovedColorParent.SetActive(true);
        }
        
        public void RemovedCallHide()
        {
            RemovedColorParent.SetActive(false);
        }

        internal void RefreshAppendLinesCount(int appendLinesCount)
        {
            AppendLinesCountLabel.text = GetStringMultiplier(appendLinesCount);
        }
        
        public void RefreshCountUntilAppend(int countUntilAppend)
        {
            BeforeAppendCountLabel.text = GetStringMultiplier(countUntilAppend);
        }
        
        string GetStringMultiplier(int Value) => string.Concat('X', Value);

        public void RefreshClicks(int clicksCount)
        {
            ClicksCountLabel.text = clicksCount.ToString();
        }

        public void RefreshPopCombo(float v, System.Action appendLine = null)
        {
            if (PopComboRoutine != null) StopCoroutine(PopComboRoutine);
            AfterPopEnd?.Invoke();
            AfterPopEnd = appendLine;
            
            if (v > PopComboFill.fillAmount)
            {
                System.Action AfterEnd = v == 1? PlayIncreasePop : null;
                PopComboRoutine = StartCoroutine(AnimatePopCombo(v, AfterEnd));
            }
            else
            {
                PopComboFill.fillAmount = v;
                AfterPopEnd?.Invoke();
                AfterPopEnd = null;
            }
            
            void PlayIncreasePop()
            {
                StopCoroutine(PopComboRoutine);
                PopComboRoutine = StartCoroutine(AnimateIncrease(PopComboFill.rectTransform, () => RefreshPopCombo(0)));
            }
        }
        
        IEnumerator AnimatePopCombo(float newAmount, System.Action OnEnd)
        {
            float oldAmount = PopComboFill.fillAmount;
            var Wait = new WaitForFixedUpdate();
            PopComboFill.rectTransform.localScale = Vector3.one;
            for (int i = 1; i <= 10; i++)
            {
                PopComboFill.fillAmount = Mathf.Lerp(oldAmount, newAmount, Mathf.Sin(i/10f * 90 * Mathf.Deg2Rad));
                yield return Wait;
            }
            if (OnEnd == null)
            {
                AfterPopEnd?.Invoke();
                AfterPopEnd = null;
            }
            else OnEnd.Invoke();
        }
        
        IEnumerator AnimateIncrease(RectTransform Rect, System.Action OnMiddle)
        {
            var Wait = new WaitForFixedUpdate();
            for (int i = 1; i <= 10; i++)
            {
                Rect.localScale = (1 + 0.3f * Mathf.Sin(i/10f * 180 * Mathf.Deg2Rad)) * Vector3.one;
                yield return Wait;
            }
            OnMiddle.Invoke();
        }
        
        public void CallSettings()
        {
            parent.CallSettings();
        }
        
        public void CallInfo()
        {
            parent.ProcessPause();
            HelpAnimator.gameObject.SetActive(true);
        }
        
        public void HideInfo() => HelpAnimator.SetTrigger("Hide"); 
        
        public void FinalizeHideInfo()
        {
            parent.ProcessUnpause();
            HelpAnimator.gameObject.SetActive(false);
        }
        
        public void Hide(float Duration = 1f)
        {
            if (!HeaderShown) return;
            HeaderShown = false;
            var animator = GetComponent<Animator>();
            animator.SetFloat("Speed", ShowAnimationSpeed / Duration);
            animator.SetTrigger("Hide");
        }
        
        public void ShowEndgame(Result result)
        {
            turnOffLocked = true;
            Hide();
            result.OnRetry += ShowHeader;
            result.OnEnd += () =>
            {
                EndgameView.BeforeTurnOff += () => gameObject.SetActive(false);
            };
            EndgameView.ShowEndgame(result);
        }
        
        public void ReceiveEndgameUnwrapped()
        {
            EndgameView.ReceiveEndgameUnwrapped();
        }
    }
}