using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Survival
{
    public class GameOver : MonoBehaviour
    {
        public System.Action OnGiveUp;
        [SerializeField] private Fade _fade;
        [SerializeField] private TMPro.TMP_Text _livesCountLabel;
        [SerializeField] private GameObject _reviveParent;
        [SerializeField] private Image _countdownMask;
        [SerializeField] private GameObject _skipCountdownButton;
        [SerializeField] private UI.Strategy.Endgame _final;
        private WaitForEndOfFrame _wait;
        private int _waitStepsCount;
        private bool _revivedSuccessfully;
        private bool _countdownInProcess, _watchAdsInProcess;
        private UI.Strategy.Result _result;
        private Services.Audio.Sounds.Service _sound;
        
        private void Start()
        {
            _wait = new WaitForEndOfFrame();
            _waitStepsCount = Mathf.RoundToInt(1f / Time.fixedDeltaTime);
            _sound = Services.DI.Single<Services.Audio.Sounds.Service>();
        }
        
        public void ProcessEnd(UI.Strategy.Result result, int LivesCount, System.Action OnRevive = null)
        {
            _result = result;
            _fade.Show();
            result.OnEnd += () => _fade.Hide();
            result.OnRetry += () => _fade.Hide();
            _livesCountLabel.text = "x" + LivesCount;
            _revivedSuccessfully = false;
            if (LivesCount > 0)
            {
                TurnToCountdown(OnRevive);
            }
            else 
            {
                TurnToOver();
            }
        }
        
        private void TurnToCountdown(System.Action OnRevive)
        {
            _countdownMask.fillAmount = 1;
            _watchAdsInProcess = false;
            _skipCountdownButton.SetActive(false);
            var But = _countdownMask.GetComponentInChildren<Button>();
            But.onClick.RemoveAllListeners();
            But.onClick.AddListener(() => ShowRewarded(OnRevive));
            _reviveParent.SetActive(true);
        }
        
        public async void ShowRewarded(System.Action OnRevive)
        {
            _sound.Stop(Services.Audio.Sounds.SoundType.Revive_TickTack);
            _watchAdsInProcess = true;
            var Ads = Services.DI.Single<Services.Advertisements.Controller>();
            _revivedSuccessfully = await Ads.IsRewardAdSuccess();
            _watchAdsInProcess = false;
            if (_revivedSuccessfully)
            {
                SkipCountDown();
                _fade.Hide();
                OnRevive?.Invoke();
            }
            else 
            {
                _sound.Play(Services.Audio.Sounds.SoundType.Revive_TickTack);
            }
        } 
        
        public void ProcessAdsCountDown()
        {
            StartCoroutine(AnimateCountDown()); 
        }
        
        public void SkipCountDown()
        {
            _countdownInProcess = false;
        }
        
        private IEnumerator AnimateCountDown()
        {
            _countdownInProcess = true;
            for (int i = 0; i < _waitStepsCount; i++)
            {
                while(_watchAdsInProcess) yield return _wait;
                if (!_countdownInProcess) break;
                yield return _wait;
            }
            if (_countdownInProcess)
            {
                _sound.Play(Services.Audio.Sounds.SoundType.Revive_TickTack);
                for (int i=300; i>0; i--)
                {
                    while(_watchAdsInProcess) yield return _wait;
                    if (!_countdownInProcess) break;
                    _countdownMask.fillAmount = i/300f;
                    if (i == 220)
                    {
                        _skipCountdownButton.SetActive(true);
                    }
                    yield return _wait;
                }
                _sound.Stop(Services.Audio.Sounds.SoundType.Revive_TickTack);
                _countdownMask.fillAmount = 0;
            }
            _reviveParent.GetComponent<Animator>().SetTrigger("Hide");
        }
        
        public void TurnToOver()
        {
            _reviveParent.SetActive(false);
            if (!_revivedSuccessfully)
            {
                OnGiveUp?.Invoke();
                _final.ShowEndgame(_result);   
            }
        }
    }
}