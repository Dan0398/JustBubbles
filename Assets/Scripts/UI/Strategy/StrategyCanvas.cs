using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Strategy
{
    [RequireComponent(typeof(Animator))]
    public class StrategyCanvas : MonoBehaviour
    {
        [SerializeField] private float _showAnimationSpeed;
        [Header("In-game data In Header")]
        [SerializeField] private Image _popComboFill;
        [SerializeField] private TMPro.TMP_Text _beforeAppendCountLabel;
        [SerializeField] private TMPro.TMP_Text _appendLinesCountLabel;
        [SerializeField] private TMPro.TMP_Text _clicksCountLabel;
        [Header("Removed overlay")]
        [SerializeField] private GameObject _removedColorParent;
        [SerializeField] private RemoveColorContainer[] _removedColors;
        [Space()]
        [SerializeField] private Animator _helpAnimator;
        [Space()]
        [SerializeField] private Endgame _endgameView;
        private Coroutine _popComboRoutine;
        private Gameplay.GameType.Strategy _parent;
        private System.Action _afterPopEnd;
        private bool _headerShown;
        private bool _turnOffLocked;

        public void Show(Gameplay.GameType.Strategy strategy, float Duration = 1f)
        {
            _parent = strategy;
            gameObject.SetActive(true);
            _turnOffLocked= false;
            if (_headerShown) return;
            var animator = GetComponent<Animator>();
            animator.SetFloat("Speed", _showAnimationSpeed / Duration);
            ShowHeader();
        }
        
        private void ShowHeader()
        {
            GetComponent<Animator>().SetTrigger("Show");
            _headerShown = true;
        }
        
        public void RegisterHideFromAnimator()
        {
            if (_turnOffLocked) return;
            gameObject.SetActive(false);
        }

        public void ReactOnColorRemove(List<Gameplay.Bubble.BubbleColor> obj)
        {
            for (int i = 0; i < _removedColors.Length; i++)
            {
                bool required = i < obj.Count;
                _removedColors[i].Turnable.SetActive(required);
                if (required)
                {
                    _removedColors[i].Shown.color = Gameplay.ColorPicker.GetColorByEnum(obj[i]);
                }
            }
            _removedColorParent.SetActive(true);
        }
        
        public void RemovedCallHide()
        {
            _removedColorParent.SetActive(false);
        }

        public void RefreshAppendLinesCount(int appendLinesCount)
        {
            _appendLinesCountLabel.text = GetStringMultiplier(appendLinesCount);
        }
        
        public void RefreshCountUntilAppend(int countUntilAppend)
        {
            _beforeAppendCountLabel.text = GetStringMultiplier(countUntilAppend);
        }
        
        private string GetStringMultiplier(int Value)
        {
            return string.Concat('X', Value);
        }

        public void RefreshClicks(int clicksCount)
        {
            _clicksCountLabel.text = clicksCount.ToString();
        }

        public void RefreshPopCombo(float v, System.Action appendLine = null)
        {
            if (_popComboRoutine != null) StopCoroutine(_popComboRoutine);
            _afterPopEnd?.Invoke();
            _afterPopEnd = appendLine;
            
            if (v > _popComboFill.fillAmount)
            {
                System.Action AfterEnd = v == 1? PlayIncreasePop : null;
                _popComboRoutine = StartCoroutine(AnimatePopCombo(v, AfterEnd));
            }
            else
            {
                _popComboFill.fillAmount = v;
                _afterPopEnd?.Invoke();
                _afterPopEnd = null;
            }
            
            void PlayIncreasePop()
            {
                StopCoroutine(_popComboRoutine);
                _popComboRoutine = StartCoroutine(AnimateIncrease(_popComboFill.rectTransform, () => RefreshPopCombo(0)));
            }
        }
        
        private IEnumerator AnimatePopCombo(float newAmount, System.Action OnEnd)
        {
            float oldAmount = _popComboFill.fillAmount;
            var Wait = new WaitForFixedUpdate();
            _popComboFill.rectTransform.localScale = Vector3.one;
            for (int i = 1; i <= 10; i++)
            {
                _popComboFill.fillAmount = Mathf.Lerp(oldAmount, newAmount, Mathf.Sin(i/10f * 90 * Mathf.Deg2Rad));
                yield return Wait;
            }
            if (OnEnd == null)
            {
                _afterPopEnd?.Invoke();
                _afterPopEnd = null;
            }
            else OnEnd.Invoke();
        }
        
        private IEnumerator AnimateIncrease(RectTransform Rect, System.Action OnMiddle)
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
            _parent.CallSettings();
        }
        
        public void CallInfo()
        {
            _parent.ProcessPause();
            _helpAnimator.gameObject.SetActive(true);
        }
        
        public void HideInfo() => _helpAnimator.SetTrigger("Hide"); 
        
        public void FinalizeHideInfo()
        {
            _parent.ProcessUnpause();
            _helpAnimator.gameObject.SetActive(false);
        }
        
        public void Hide(float Duration = 1f)
        {
            if (!_headerShown) return;
            _headerShown = false;
            var animator = GetComponent<Animator>();
            animator.SetFloat("Speed", _showAnimationSpeed / Duration);
            animator.SetTrigger("Hide");
        }
        
        public void ShowEndgame(Result result)
        {
            _turnOffLocked = true;
            Hide();
            result.OnRetry += ShowHeader;
            result.OnEnd += () =>
            {
                _endgameView.BeforeTurnOff += () => gameObject.SetActive(false);
            };
            _endgameView.ShowEndgame(result);
        }
        
        public void ReceiveEndgameUnwrapped()
        {
            _endgameView.ReceiveEndgameUnwrapped();
        }
    }
}