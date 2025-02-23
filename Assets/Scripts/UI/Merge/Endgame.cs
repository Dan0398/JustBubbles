using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Merge
{    
    public class Endgame : BaseAnimatedWindow
    {
        [SerializeField] private TMPro.TMP_Text _score;
        [SerializeField] private Button _retry;
        [SerializeField] private Button _exit;
        private WaitForSecondsRealtime _wait;
        
        protected override bool AddHalfHeader => false;
        
        public void Show(Gameplay.Merge.SaveModel model, System.Action OnRetry, System.Action OnExit)
        {
            Header.SetNewKey("GameOver");
            _score.text = string.Empty;
            
            _retry.gameObject.SetActive(false);
            _exit.gameObject.SetActive(false);
            
            _exit.onClick.RemoveAllListeners();
            _retry.onClick.RemoveAllListeners();
            
            _retry.onClick.AddListener(StartRetry);
            _exit.onClick.AddListener(StartExit);
            
            gameObject.SetActive(true);
            StartCoroutine(ShowAnimated());
            
            void StartRetry()
            {
                _exit.onClick.RemoveAllListeners();
                _retry.onClick.RemoveAllListeners();
                StartCoroutine(HideAnimated("NewGame", OnRetry));
            }
            
            void StartExit()
            {
                _exit.onClick.RemoveAllListeners();
                _retry.onClick.RemoveAllListeners();
                StartCoroutine(HideAnimated("Close", OnExit));
            }
            
            IEnumerator ShowAnimated()
            {
                StartCoroutine(AnimateFade(0.7f));
                StartCoroutine(AnimateHeaderColor(0.2f));
                yield return base.AnimateHeader(.7f);
                yield return AnimateUnwrapWindow(1f);
                yield return AnimateScore();
                _retry.gameObject.SetActive(true);
                _exit.gameObject.SetActive(true);
            }
            
            IEnumerator AnimateScore()
            {
                _wait ??= new(0.1f);
                var Sounds = Services.DI.Single<Services.Audio.Sounds.Service>();
                Sounds.Play(Services.Audio.Sounds.SoundType.Counter);
                for (int i = 1; i <= 13; i++)
                {
                    _score.text = Mathf.RoundToInt(model.Points.Value * i/13f).ToString();
                    yield return _wait;
                }
                Sounds.Stop(Services.Audio.Sounds.SoundType.Counter);
            }
        }
        
        private IEnumerator HideAnimated(string HeaderKey, System.Action AfterEnd)
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