#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif
using UnityEngine;

namespace UI.Survival
{
    public class Final : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text _currentScoreLabel;
        [SerializeField] private TMPro.TMP_Text _oldBestScoreLabel;
        [SerializeField] private Animator _myAnimator;
        [SerializeField] private Animator[] _buttonsAnimations;
        [SerializeField] private Fade _fade;
        
        public void Show(int CurrentScore)
        {
            _currentScoreLabel.text = string.Format(Formatted("CurrentResult_Formatted"), CurrentScore).Replace(@"\n", System.Environment.NewLine);
            var User = Services.DI.Single<Data.UserController>();
            bool IsNewHighScore = User.Data.SurvivalBestScore.Value < CurrentScore;
            if (!IsNewHighScore)
            {
                _oldBestScoreLabel.text = string.Format(Formatted("BestResult_Formatted"), User.Data.SurvivalBestScore.Value);
            }
            else 
            {
                User.Data.SurvivalBestScore.Value = CurrentScore;
            }
            SwitchAnimatorsState(true);
            gameObject.SetActive(true);
            _myAnimator.SetBool("IsNewHighScore", IsNewHighScore);

            static string Formatted(string Key) => BrakelessGames.Localization.Controller.GetValueByKey(Key);
        }
        
        private void SwitchAnimatorsState(bool MyIsPrimary)
        {
            _myAnimator.enabled = MyIsPrimary;
            foreach(var Anim in _buttonsAnimations)
            {
                Anim.enabled = !MyIsPrimary;
            }
        }
        
        public void FinalizeShow()
        {
            SwitchAnimatorsState(false);
        }
        
        public async void Hide()
        {
            _fade.Hide();
            SwitchAnimatorsState(true);
            _myAnimator.SetTrigger("Hide");
            await Task.Delay(1000);
            gameObject.SetActive(false);
        }
        
        public void FinalizeHide(){ }
        
        public void PlayNewRecordAudio()
        {
            GetComponent<AudioSource>().Play();
        }
    }
}