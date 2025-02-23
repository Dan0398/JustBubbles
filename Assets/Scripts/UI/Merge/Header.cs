using System.Collections;
using Utils.Observables;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Merge
{    
    [System.Serializable]
    public class Header: MonoBehaviour
    {
        public bool Shown   { get; private set; }
        [SerializeField] private TMPro.TMP_Text _score;
        [SerializeField] private TMPro.TMP_Text _money;
        [SerializeField] private Button _save;
        [SerializeField] private Button _pause;
        [SerializeField] private Image _saveFill;
        [SerializeField] private Transform _saveMark;
        [SerializeField] private AnimationCurve _saveMarkSizeDynamic;
        [Header("Game Over")]
        [SerializeField] private Image _gameOverFill;
        [SerializeField] private Slider _gameOverValue;
        private Gameplay.GameType.Merge _parent;
        private RectTransform _myRect;
        private System.Action _onUnbind;
        private ObsFloat _gameOver;
        private bool _gameOverUnWrapped;
        private Coroutine _animationRoutine, _saveRoutine, _gameOverUnwrapRoutine;
        private WaitForFixedUpdate _wait;
        
        public void BindAndShowAnimated(Gameplay.Merge.SaveModel slotModel, Gameplay.GameType.Merge Parent, ObsFloat GameOver, float AnimDuration = 1f)
        {
            Shown = true;
            _onUnbind?.Invoke();
            _onUnbind = null;
            gameObject.SetActive(true);
            _parent = Parent;
            BindScore();
            BindMoney();
            BindPause();
            BindSave();
            BindGameOver();
            if (_animationRoutine != null) StopCoroutine(_animationRoutine);
            _animationRoutine = StartCoroutine(AnimateShow(AnimDuration));
            
            void BindScore()
            {
                System.Action RefreshScore = () => _score.text = slotModel.Points.Value.ToString();
                RefreshScore.Invoke();
                slotModel.Points.Changed += RefreshScore;
                _onUnbind += () => slotModel.Points.Changed -= RefreshScore;
            }
            
            void BindMoney()
            {
                System.Action RefreshMoney = () => _money.text = string.Concat(slotModel.Money.Value, '$');
                RefreshMoney.Invoke();
                slotModel.Money.Changed += RefreshMoney;
                _onUnbind += () => slotModel.Money.Changed -= RefreshMoney;
            }
            
            void BindPause()
            {
                _pause.onClick.AddListener(Parent.CallSettings);
                _onUnbind += () => _pause.onClick.RemoveAllListeners();
            }
            
            void BindSave()
            {
                _save.onClick.AddListener(TrySave);
                _onUnbind += () => _save.onClick.RemoveAllListeners();
            }
            
            void BindGameOver()
            {
                if (_gameOverUnwrapRoutine != null) StopCoroutine(_gameOverUnwrapRoutine);
                _gameOverFill.fillAmount = 0;
                
                _gameOver = GameOver;
                _gameOver.Changed += ReactOnStatus;
                ReactOnStatus();
                _onUnbind += () => 
                {
                    _gameOver.Changed -= ReactOnStatus;
                    _gameOver = null;
                };
                
                void ReactOnStatus()
                {
                    if (_gameOver == null) return;
                    _gameOverValue.value = _gameOver.Value;
                    if (!_gameOverUnWrapped && _gameOver.Value > 0)
                    {
                        _gameOverUnWrapped = true;
                        if (_gameOverUnwrapRoutine != null) StopCoroutine(_gameOverUnwrapRoutine);
                        _gameOverUnwrapRoutine = StartCoroutine(UnwrapGameOver());
                    }
                    else if (_gameOverUnWrapped && _gameOver.Value == 0)
                    {
                        _gameOverUnWrapped = false;
                        if (_gameOverUnwrapRoutine != null) StopCoroutine(_gameOverUnwrapRoutine);
                        _gameOverUnwrapRoutine = StartCoroutine(UnwrapGameOver(true));
                    }
                }
                
            }
        }
        
        private IEnumerator UnwrapGameOver(bool Reversed = false, float Duration = 1f)
        {
            _wait ??= new WaitForFixedUpdate();
            
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            
            if (!Reversed) _gameOverFill.gameObject.SetActive(true);
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = EasingFunction.EaseInSine(0, 1, i/(float)Steps);
                if (Reversed) Lerp = 1 - Lerp;
                _gameOverFill.fillAmount = Lerp;
                yield return _wait;
            }
            if (Reversed) _gameOverFill.gameObject.SetActive(false);
        }
        
        private IEnumerator AnimateShow(float Duration, System.Action OnEnd = null, bool IsHide = false)
        {
            const float Height = 0.07f;
            _myRect ??= GetComponent<RectTransform>();
            _wait ??= new WaitForFixedUpdate();
            
            int MaxSteps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            
            int Step = Mathf.RoundToInt((_myRect.anchorMax.y - 1)/Height * MaxSteps);
            
            int FinalStep = IsHide? MaxSteps : 0;
            int Dir = IsHide? 1: -1;
            
            while(Step != FinalStep + Dir)
            {
                float Lerp = Mathf.Cos(Step/(float)MaxSteps * 90 * Mathf.Deg2Rad);
                _myRect.anchorMin = Vector2.up * (1 - Height* Lerp);
                _myRect.anchorMax = new Vector2(1, 1 + Height * (1-Lerp));
                Step += Dir;
                yield return _wait;
            }
            OnEnd?.Invoke();
        }
        
        public void HideAnimated(float Duration = 1f, System.Action OnEnd = null)
        {
            Shown = false;
            _onUnbind?.Invoke();
            _onUnbind = null;
            if (!gameObject.activeSelf)
            {
                OnEnd?.Invoke();
                return;
            }
            if (_animationRoutine != null) StopCoroutine(_animationRoutine);
            _animationRoutine = StartCoroutine(AnimateShow(Duration, AfterEnd, true));
            
            if (_gameOverUnwrapRoutine != null) StopCoroutine(_gameOverUnwrapRoutine);
            _gameOverUnwrapRoutine = StartCoroutine(UnwrapGameOver(true, Duration));
            
            void AfterEnd()
            {
                OnEnd?.Invoke();
                gameObject.SetActive(false);
            }
        }
        
        private void TrySave()
        {
            if (_saveRoutine != null) 
            {
                Debug.Log("Error sound");
                return;
            }
            _parent.SaveSelectedSlot();
            _saveRoutine = StartCoroutine(AnimateSave());
        }
        
        private IEnumerator AnimateSave()
        {
            _wait ??= new WaitForFixedUpdate();
            int Steps = Mathf.RoundToInt(2.5f / Time.fixedDeltaTime);
            for (int i = 1; i <= Steps; i++)
            {
                _saveFill.fillAmount = i/(float)Steps;
                yield return _wait;
            }
            _saveMark.gameObject.SetActive(true);
            Steps = Mathf.RoundToInt(0.7f / Time.fixedDeltaTime);
            Services.DI.Single<Services.Audio.Sounds.Service>().Play(Services.Audio.Sounds.SoundType.Merge_Saved);
            for (int i = 1; i <= Steps; i++)
            {
                _saveMark.localScale = _saveMarkSizeDynamic.Evaluate(i/(float)Steps) * Vector3.one;
                yield return _wait;
            }
            _saveMark.gameObject.SetActive(false);
            _saveRoutine = null;
        }
    }
}