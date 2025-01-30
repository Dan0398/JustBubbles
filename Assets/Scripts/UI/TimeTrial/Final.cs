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
        [ SerializeField] TMPro.TMP_Text CurrentScoreLabel;
        [SerializeField] TMPro.TMP_Text OldBestScoreLabel;
        [SerializeField] GameObject NewHighScoreLabel;
        [SerializeField] Animator MyAnimator;
        [SerializeField] Animator[] ButtonsAnimations;
        [SerializeField] Fade fade;
        
        public void Show(int CurrentScore)
        {
            CurrentScoreLabel.text = string.Format(Formatted("CurrentResult_Formatted"), CurrentScore).Replace(@"\n", System.Environment.NewLine);
            var User = Services.DI.Single<Data.UserController>();
            bool IsNewHighScore = User.Data.SurvivalBestScore.Value < CurrentScore;
            if (!IsNewHighScore)
            {
                OldBestScoreLabel.text = string.Format(Formatted("BestResult_Formatted"), User.Data.SurvivalBestScore.Value);
            }
            else 
            {
                User.Data.SurvivalBestScore.Value = CurrentScore;
            }
            SwitchAnimatorsState(true);
            gameObject.SetActive(true);
            MyAnimator.SetBool("IsNewHighScore", IsNewHighScore);
            
            string Formatted(string Key) => BrakelessGames.Localization.Controller.GetValueByKey(Key);
        }
        
        void SwitchAnimatorsState(bool MyIsPrimary)
        {
            MyAnimator.enabled = MyIsPrimary;
            foreach(var Anim in ButtonsAnimations)
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
            fade.Hide();
            SwitchAnimatorsState(true);
            MyAnimator.SetTrigger("Hide");
            await Task.Delay(1000);
            gameObject.SetActive(false);
        }
        
        public void FinalizeHide()
        {
        }
        
        public void PlayNewRecordAudio() => GetComponent<AudioSource>().Play();
    }
}