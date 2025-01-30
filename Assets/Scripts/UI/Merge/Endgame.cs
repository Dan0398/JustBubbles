using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Merge
{    
    public class Endgame : BaseAnimatedWindow
    {
        [SerializeField] TMPro.TMP_Text Score;
        [SerializeField] Button Retry, Exit;
        WaitForSecondsRealtime wait;
        
        protected override bool AddHalfHeader => false;
        
        public void Show(Gameplay.Merge.SaveModel model, System.Action OnRetry, System.Action OnExit)
        {
            Header.SetNewKey("GameOver");
            Score.text = string.Empty;
            
            Retry.gameObject.SetActive(false);
            Exit.gameObject.SetActive(false);
            
            Exit.onClick.RemoveAllListeners();
            Retry.onClick.RemoveAllListeners();
            
            Retry.onClick.AddListener(StartRetry);
            Exit.onClick.AddListener(StartExit);
            
            gameObject.SetActive(true);
            StartCoroutine(ShowAnimated());
            
            void StartRetry()
            {
                Exit.onClick.RemoveAllListeners();
                Retry.onClick.RemoveAllListeners();
                StartCoroutine(HideAnimated("NewGame", OnRetry));
            }
            
            void StartExit()
            {
                Exit.onClick.RemoveAllListeners();
                Retry.onClick.RemoveAllListeners();
                StartCoroutine(HideAnimated("Close", OnExit));
            }
            
            IEnumerator ShowAnimated()
            {
                StartCoroutine(AnimateFade(0.7f));
                StartCoroutine(AnimateHeaderColor(0.2f));
                yield return base.AnimateHeader(.7f);
                yield return AnimateUnwrapWindow(1f);
                yield return AnimateScore();
                Retry.gameObject.SetActive(true);
                Exit.gameObject.SetActive(true);
            }
            
            IEnumerator AnimateScore()
            {
                wait ??= new(0.1f);
                var Sounds = Services.DI.Single<Services.Audio.Sounds.Service>();
                Sounds.Play(Services.Audio.Sounds.SoundType.Counter);
                for (int i = 1; i <= 13; i++)
                {
                    Score.text = Mathf.RoundToInt(model.Points.Value * i/13f).ToString();
                    yield return wait;
                }
                Sounds.Stop(Services.Audio.Sounds.SoundType.Counter);
            }
        }
        
        IEnumerator HideAnimated(string HeaderKey, System.Action AfterEnd)
        {
            StartCoroutine(Header());
            yield return AnimateUnwrapWindow(1f, true);
            StartCoroutine(AnimateFade(.7f, true));
            yield return AnimateHeader(.7f, true);
            AfterEnd?.Invoke();
            gameObject.SetActive(false);
            
            IEnumerator Header()
            {
                yield return AnimateHeaderColor(0.3f, true);
                base.Header.SetNewKey(HeaderKey);
                yield return AnimateHeaderColor(0.3f, false);
            }
        }
    }
}