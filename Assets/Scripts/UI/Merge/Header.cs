using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils.Observables;

namespace UI.Merge
{    
    [System.Serializable]
    public class Header: MonoBehaviour
    {
        public bool Shown   { get; private set; }
        [SerializeField] TMPro.TMP_Text Score, Money;
        [SerializeField] Button Save, Pause;
        [SerializeField] Image SaveFill;
        [SerializeField] Transform SaveMark;
        [SerializeField] AnimationCurve SaveMarkSizeDynamic;
        [Header("Game Over")]
        [SerializeField] Image GameOverFill;
        [SerializeField] Slider GameOverValue;
        Gameplay.GameType.Merge parent;
        RectTransform MyRect;
        System.Action OnUnbind;
        ObsFloat gameOver;
        bool gameOverUnWrapped;
        Coroutine AnimationRoutine, SaveRoutine, GameOverUnwrapRoutine;
        WaitForFixedUpdate Wait;
        
        public void BindAndShowAnimated(Gameplay.Merge.SaveModel slotModel, Gameplay.GameType.Merge Parent, ObsFloat GameOver, float AnimDuration = 1f)
        {
            Shown = true;
            OnUnbind?.Invoke();
            OnUnbind = null;
            gameObject.SetActive(true);
            parent = Parent;
            BindScore();
            BindMoney();
            BindPause();
            BindSave();
            BindGameOver();
            if (AnimationRoutine != null) StopCoroutine(AnimationRoutine);
            AnimationRoutine = StartCoroutine(AnimateShow(AnimDuration));
            
            void BindScore()
            {
                System.Action RefreshScore = () => Score.text = slotModel.Points.Value.ToString();
                RefreshScore.Invoke();
                slotModel.Points.Changed += RefreshScore;
                OnUnbind += () => slotModel.Points.Changed -= RefreshScore;
            }
            
            void BindMoney()
            {
                System.Action RefreshMoney = () => Money.text = string.Concat(slotModel.Money.Value, '$');
                RefreshMoney.Invoke();
                slotModel.Money.Changed += RefreshMoney;
                OnUnbind += () => slotModel.Money.Changed -= RefreshMoney;
            }
            
            void BindPause()
            {
                Pause.onClick.AddListener(Parent.CallSettings);
                OnUnbind += () => Pause.onClick.RemoveAllListeners();
            }
            
            void BindSave()
            {
                Save.onClick.AddListener(TrySave);
                OnUnbind += () => Save.onClick.RemoveAllListeners();
            }
            
            void BindGameOver()
            {
                if (GameOverUnwrapRoutine != null) StopCoroutine(GameOverUnwrapRoutine);
                GameOverFill.fillAmount = 0;
                
                gameOver = GameOver;
                gameOver.Changed += ReactOnStatus;
                ReactOnStatus();
                OnUnbind += () => 
                {
                    gameOver.Changed -= ReactOnStatus;
                    gameOver = null;
                };
                
                void ReactOnStatus()
                {
                    if (gameOver == null) return;
                    GameOverValue.value = gameOver.Value;
                    if (!gameOverUnWrapped && gameOver.Value > 0)
                    {
                        gameOverUnWrapped = true;
                        if (GameOverUnwrapRoutine != null) StopCoroutine(GameOverUnwrapRoutine);
                        GameOverUnwrapRoutine = StartCoroutine(UnwrapGameOver());
                    }
                    else if (gameOverUnWrapped && gameOver.Value == 0)
                    {
                        gameOverUnWrapped = false;
                        if (GameOverUnwrapRoutine != null) StopCoroutine(GameOverUnwrapRoutine);
                        GameOverUnwrapRoutine = StartCoroutine(UnwrapGameOver(true));
                    }
                }
                
            }
        }
        
        IEnumerator UnwrapGameOver(bool Reversed = false, float Duration = 1f)
        {
            Wait ??= new WaitForFixedUpdate();
            
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            
            if (!Reversed) GameOverFill.gameObject.SetActive(true);
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = EasingFunction.EaseInSine(0, 1, i/(float)Steps);
                if (Reversed) Lerp = 1 - Lerp;
                GameOverFill.fillAmount = Lerp;
                yield return Wait;
            }
            if (Reversed) GameOverFill.gameObject.SetActive(false);
        }
        
        IEnumerator AnimateShow(float Duration, System.Action OnEnd = null, bool IsHide = false)
        {
            const float Height = 0.07f;
            MyRect ??= GetComponent<RectTransform>();
            Wait ??= new WaitForFixedUpdate();
            
            int MaxSteps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            
            int Step = Mathf.RoundToInt((MyRect.anchorMax.y - 1)/Height * MaxSteps);
            
            int FinalStep = IsHide? MaxSteps : 0;
            int Dir = IsHide? 1: -1;
            
            while(Step != FinalStep + Dir)
            {
                float Lerp = Mathf.Cos(Step/(float)MaxSteps * 90 * Mathf.Deg2Rad);
                MyRect.anchorMin = Vector2.up * (1 - Height* Lerp);
                MyRect.anchorMax = new Vector2(1, 1 + Height * (1-Lerp));
                Step += Dir;
                yield return Wait;
            }
            OnEnd?.Invoke();
        }
        
        public void HideAnimated(float Duration = 1f, System.Action OnEnd = null)
        {
            Shown = false;
            OnUnbind?.Invoke();
            OnUnbind = null;
            if (!gameObject.activeSelf)
            {
                OnEnd?.Invoke();
                return;
            }
            if (AnimationRoutine != null) StopCoroutine(AnimationRoutine);
            AnimationRoutine = StartCoroutine(AnimateShow(Duration, AfterEnd, true));
            
            if (GameOverUnwrapRoutine != null) StopCoroutine(GameOverUnwrapRoutine);
            GameOverUnwrapRoutine = StartCoroutine(UnwrapGameOver(true, Duration));
            
            void AfterEnd()
            {
                OnEnd?.Invoke();
                gameObject.SetActive(false);
            }
        }
        
        void TrySave()
        {
            if (SaveRoutine != null) 
            {
                Debug.Log("Error sound");
                return;
            }
            parent.SaveSelectedSlot();
            SaveRoutine = StartCoroutine(AnimateSave());
        }
        
        IEnumerator AnimateSave()
        {
            Wait ??= new WaitForFixedUpdate();
            int Steps = Mathf.RoundToInt(2.5f / Time.fixedDeltaTime);
            for (int i = 1; i <= Steps; i++)
            {
                SaveFill.fillAmount = i/(float)Steps;
                yield return Wait;
            }
            //yield return new WaitForSecondsRealtime(0.5f);
            SaveMark.gameObject.SetActive(true);
            Steps = Mathf.RoundToInt(0.7f / Time.fixedDeltaTime);
            Services.DI.Single<Services.Audio.Sounds.Service>().Play(Services.Audio.Sounds.SoundType.Merge_Saved);
            for (int i = 1; i <= Steps; i++)
            {
                SaveMark.localScale = SaveMarkSizeDynamic.Evaluate(i/(float)Steps) * Vector3.one;
                yield return Wait;
            }
            SaveMark.gameObject.SetActive(false);
            SaveRoutine = null;
        }
    }
}