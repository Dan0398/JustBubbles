using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Gameplay.Effects;

namespace UI.Survival
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] Fade fade;
        [SerializeField] TMPro.TMP_Text LivesCountLabel;
        [SerializeField] GameObject ReviveParent;
        [SerializeField] Image CountdownMask;
        [SerializeField] GameObject SkipCountdownButton;
        [SerializeField] UI.Strategy.Endgame Final;
        public System.Action OnGiveUp;
        WaitForEndOfFrame Wait;
        int WaitStepsCount;
        bool RevivedSuccessfully;
        bool CountdownInProcess, WatchAdsInProcess;
        UI.Strategy.Result Result;
        Services.Audio.Sounds.Service Sound;
        
        void Start()
        {
            Wait = new WaitForEndOfFrame();
            WaitStepsCount = Mathf.RoundToInt(1f / Time.fixedDeltaTime);
            Sound = Services.DI.Single<Services.Audio.Sounds.Service>();
        }
        
        public void ProcessEnd(UI.Strategy.Result result, int LivesCount, System.Action OnRevive = null)
        {
            Result = result;
            fade.Show();
            result.OnEnd += () => fade.Hide();
            result.OnRetry += () => fade.Hide();
            LivesCountLabel.text = "x" + LivesCount;
            RevivedSuccessfully = false;
            if (LivesCount > 0)
            {
                TurnToCountdown(OnRevive);
            }
            else 
            {
                TurnToOver();
            }
        }
        
        void TurnToCountdown(System.Action OnRevive)
        {
            CountdownMask.fillAmount = 1;
            WatchAdsInProcess = false;
            SkipCountdownButton.SetActive(false);
            var But = CountdownMask.GetComponentInChildren<Button>();
            But.onClick.RemoveAllListeners();
            But.onClick.AddListener(() => ShowRewarded(OnRevive));
            ReviveParent.SetActive(true);
        }
        
        public async void ShowRewarded(System.Action OnRevive)
        {
            Sound.Stop(Services.Audio.Sounds.SoundType.Revive_TickTack);
            WatchAdsInProcess = true;
            var Ads = Services.DI.Single<Services.Advertisements.Controller>();
            RevivedSuccessfully = await Ads.IsRewardAdSuccess();
            WatchAdsInProcess = false;
            if (RevivedSuccessfully)
            {
                SkipCountDown();
                fade.Hide();
                OnRevive?.Invoke();
            }
            else 
            {
                Sound.Play(Services.Audio.Sounds.SoundType.Revive_TickTack);
            }
        } 
        
        public void ProcessAdsCountDown() => StartCoroutine(AnimateCountDown()); 
        
        public void SkipCountDown() => CountdownInProcess = false;
        
        IEnumerator AnimateCountDown()
        {
            CountdownInProcess = true;
            for (int i = 0; i < WaitStepsCount; i++)
            {
                while(WatchAdsInProcess) yield return Wait;
                if (!CountdownInProcess) break;
                yield return Wait;
            }
            //yield return WaitSeconds;
            if (CountdownInProcess)
            {
                Sound.Play(Services.Audio.Sounds.SoundType.Revive_TickTack);
                for (int i=300; i>0; i--)
                {
                    while(WatchAdsInProcess) yield return Wait;
                    if (!CountdownInProcess) break;
                    CountdownMask.fillAmount = i/300f;
                    if (i == 220)
                    {
                        SkipCountdownButton.SetActive(true);
                    }
                    yield return Wait;
                }
                Sound.Stop(Services.Audio.Sounds.SoundType.Revive_TickTack);
                CountdownMask.fillAmount = 0;
            }
            ReviveParent.GetComponent<Animator>().SetTrigger("Hide");
        }
        
        public void TurnToOver()
        {
            ReviveParent.SetActive(false);
            if (!RevivedSuccessfully)
            {
                OnGiveUp?.Invoke();
                Final.ShowEndgame(Result);   
            }
        }
    }
}