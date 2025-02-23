using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Strategy
{
    public class Endgame: MonoBehaviour
    {
        private const int Steps = 17;
        
        public System.Action BeforeTurnOff;
        [SerializeField] private BrakelessGames.Localization.TextTMPLocalized _windowHeader;
        [SerializeField] private TMPro.TMP_Text _userClick;
        [SerializeField] private BrakelessGames.Localization.TextTMPLocalized _bestScore;
        [SerializeField] private Hint _hints;
        [SerializeField] private GameObject _buttonsParent;
        [Header("Header Langs")]
        [SerializeField] private string _gameEnded;
        [SerializeField] private string _closeLangKey;
        [SerializeField] private string _retryLangKey;
        [SerializeField] private Leaderboards _leaderboards;
        private Services.Audio.Sounds.Service _sound;
        private Result _actualResult;
        private WaitForSecondsRealtime _wait;
        private WaitForSecondsRealtime _longWait;
        private System.Action _afterHide;
        private string _headerLangKey;
        private bool _turnOffAllowed;
        
        private void Start()
        {
            _wait = new WaitForSecondsRealtime(0.05f);
            _longWait = new WaitForSecondsRealtime(0.7f);
        }
        
        public void ShowEndgame(Result result)
        {
            _actualResult = result;
            ResetViewsToDefaults();
            _leaderboards.PrepareView(result);
            gameObject.SetActive(true);
        }
        
        private void ResetViewsToDefaults()
        {
            _hints.Hide();
            _windowHeader.SetNewKey(_gameEnded);
            _buttonsParent.SetActive(false);
            _userClick.text = string.Empty;
            _bestScore.SetTextNoTranslate(string.Empty);
        }
        
        public void ReceiveEndgameUnwrapped()
        {
            StartCoroutine(AnimateValues());
        }
        
        private IEnumerator AnimateValues()
        {
            yield return AnimateScore();
            yield return _longWait;
            _hints.Show(_actualResult.HintLangKey);
            yield return AnimateBest();
            yield return _longWait;
            _leaderboards.Show();
            _buttonsParent.SetActive(true);
            _turnOffAllowed = true;
        }
        
        private IEnumerator AnimateScore()
        {
            if (_sound == null) _sound = Services.DI.Single<Services.Audio.Sounds.Service>();
            _sound.Play(Services.Audio.Sounds.SoundType.Counter);
            for (int i = 0; i < Steps; i++)
            {
                _userClick.text = (_actualResult.SessionResult * i / Steps).ToString();
                yield return _wait;
            }
            _userClick.text = _actualResult.SessionResult.ToString();
            _sound.Stop(Services.Audio.Sounds.SoundType.Counter);
        }
        
        private IEnumerator AnimateBest()
        {
            string ShownResult = "-";
            if (_actualResult.BestResult > 0)
            {
                ShownResult = _actualResult.BestResult.ToString();
            }
            if (!_actualResult.IsNewHighScore)
            {
                _bestScore.SetTextNoTranslate(ShownResult);
                yield break;
            }
            _bestScore.SetNewKey("NewRecordMark");
            Services.DI.Single<Services.Audio.Sounds.Service>().Play(Services.Audio.Sounds.SoundType.Record);
            var Rect = _bestScore.GetComponent<RectTransform>();
            for (int i = 1; i <= Steps; i++)
            {
                Rect.localScale = (1 + 0.2f * Mathf.Sin(i/(float)Steps * 180 * Mathf.Deg2Rad)) * Vector3.one;
                yield return _wait;
            }
            Rect.localScale = Vector3.one;
            yield return _longWait;
            var Maskable = _bestScore.GetComponent<MaskableGraphic>();
            for (int i = 1; i <= Steps; i++)
            {
                Maskable.color = Color.white - Color.black * Mathf.Sin(i/(float)Steps * 180 * Mathf.Deg2Rad);
                if (i == Steps/2) _bestScore.SetTextNoTranslate(ShownResult);
                yield return _wait;
            }
        }
        
        public async void GoToMenu()
        {
            if (!_turnOffAllowed) return;
            await Services.DI.Single<Services.Advertisements.Controller>().ShowInterstitial();
            StartHide(_actualResult.OnEnd, _closeLangKey);
        }
        
        public async void GoRetry()
        {
            if (!_turnOffAllowed) return;
            await Services.DI.Single<Services.Advertisements.Controller>().ShowInterstitial();
            StartHide(_actualResult.OnRetry, _retryLangKey);
        }
        
        private void StartHide(System.Action BeforeHeaderHide, string HeaderLangKey)
        {
            _buttonsParent.SetActive(false);
            _turnOffAllowed = false;
            _afterHide = BeforeHeaderHide;
            GetComponent<Animator>().SetTrigger("Hide");
            _headerLangKey = HeaderLangKey;
        }
        
        public void CallHeaderFullAlpha()
        {
            _windowHeader.SetNewKey(_headerLangKey);
            _afterHide.Invoke();
            _afterHide = null;
        }
        
        public void FinalizeHide()
        {
            BeforeTurnOff?.Invoke();
            BeforeTurnOff = null;
            gameObject.SetActive(false);
        }
    }
}