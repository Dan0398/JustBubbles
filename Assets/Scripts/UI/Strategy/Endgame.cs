using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Strategy
{
    public class Endgame: MonoBehaviour
    {
        const int Steps = 17;
        public System.Action BeforeTurnOff;
        [SerializeField] BrakelessGames.Localization.TextTMPLocalized WindowHeader;
        [SerializeField] TMPro.TMP_Text UserClick;
        [SerializeField] BrakelessGames.Localization.TextTMPLocalized BestScore;
        [SerializeField] Hint Hints;
        [SerializeField] GameObject buttonsParent;
        [Header("Header Langs"), SerializeField] string GameEnded;
        [SerializeField] string CloseLangKey, RetryLangKey;
        [SerializeField] Leaderboards leaderboards;
        Services.Audio.Sounds.Service sound;
        Result actualResult;
        WaitForSecondsRealtime Wait;
        WaitForSecondsRealtime LongWait;
        System.Action afterHide;
        string headerLangKey;
        bool turnOffAllowed;
        
        void Start()
        {
            Wait = new WaitForSecondsRealtime(0.05f);
            LongWait = new WaitForSecondsRealtime(0.7f);
            leaderboards.Init(this);
        }
        
        public void ShowEndgame(Result result)
        {
            actualResult = result;
            ResetViewsToDefaults();
            leaderboards.PrepareView(result);
            gameObject.SetActive(true);
        }
        
        void ResetViewsToDefaults()
        {
            Hints.Hide();
            WindowHeader.SetNewKey(GameEnded);
            buttonsParent.SetActive(false);
            UserClick.text = string.Empty;
            BestScore.SetTextNoTranslate(string.Empty);
        }
        
        public void ReceiveEndgameUnwrapped()
        {
            StartCoroutine(AnimateValues());
        }
        
        IEnumerator AnimateValues()
        {
            yield return AnimateScore();
            yield return LongWait;
            Hints.Show(actualResult.HintLangKey);
            yield return AnimateBest();
            yield return LongWait;
            leaderboards.Show();
            buttonsParent.SetActive(true);
            turnOffAllowed = true;
        }
        
        IEnumerator AnimateScore()
        {
            if (sound == null) sound = Services.DI.Single<Services.Audio.Sounds.Service>();
            sound.Play(Services.Audio.Sounds.SoundType.Counter);
            for (int i = 0; i < Steps; i++)
            {
                UserClick.text = (actualResult.SessionResult * i / Steps).ToString();
                yield return Wait;
            }
            UserClick.text = actualResult.SessionResult.ToString();
            sound.Stop(Services.Audio.Sounds.SoundType.Counter);
        }
        
        IEnumerator AnimateBest()
        {
            string ShownResult = "-";
            if (actualResult.BestResult > 0)
            {
                ShownResult = actualResult.BestResult.ToString();
            }
            if (!actualResult.IsNewHighScore)
            {
                BestScore.SetTextNoTranslate(ShownResult);
                yield break;
            }
            BestScore.SetNewKey("NewRecordMark");
            Services.DI.Single<Services.Audio.Sounds.Service>().Play(Services.Audio.Sounds.SoundType.Record);
            var Rect = BestScore.GetComponent<RectTransform>();
            for (int i = 1; i <= Steps; i++)
            {
                Rect.localScale = (1 + 0.2f * Mathf.Sin(i/(float)Steps * 180 * Mathf.Deg2Rad)) * Vector3.one;
                yield return Wait;
            }
            Rect.localScale = Vector3.one;
            yield return LongWait;
            var Maskable = BestScore.GetComponent<MaskableGraphic>();
            for (int i = 1; i <= Steps; i++)
            {
                Maskable.color = Color.white - Color.black * Mathf.Sin(i/(float)Steps * 180 * Mathf.Deg2Rad);
                if (i == Steps/2) BestScore.SetTextNoTranslate(ShownResult);
                yield return Wait;
            }
        }
        
        public async void GoToMenu()
        {
            if (!turnOffAllowed) return;
            await Services.DI.Single<Services.Advertisements.Controller>().ShowInterstitial();
            StartHide(actualResult.OnEnd, CloseLangKey);
        }
        
        public async void GoRetry()
        {
            if (!turnOffAllowed) return;
            await Services.DI.Single<Services.Advertisements.Controller>().ShowInterstitial();
            StartHide(actualResult.OnRetry, RetryLangKey);
        }
        
        void StartHide(System.Action BeforeHeaderHide, string HeaderLangKey)
        {
            buttonsParent.SetActive(false);
            turnOffAllowed = false;
            afterHide = BeforeHeaderHide;
            GetComponent<Animator>().SetTrigger("Hide");
            headerLangKey = HeaderLangKey;
        }
        
        public void CallHeaderFullAlpha()
        {
            WindowHeader.SetNewKey(headerLangKey);
            afterHide.Invoke();
            afterHide = null;
        }
        
        public void FinalizeHide()
        {
            BeforeTurnOff?.Invoke();
            BeforeTurnOff = null;
            gameObject.SetActive(false);
        }
    }
}