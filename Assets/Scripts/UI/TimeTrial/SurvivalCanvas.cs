using System.Threading.Tasks;
using Gameplay.GameType;
using UnityEngine;

namespace UI.Survival
{
    public class SurvivalCanvas : MonoBehaviour
    {
        public AnimationCurve MoveDynamic;
        [SerializeField] float ShowAnimationSpeed;
        [field: SerializeField] public ScoreView Score                  { get; private set; }
        [field: SerializeField] public ComboView Combo                  { get; private set; }
        [field: SerializeField] public Progress Progress                { get; private set; }
        [field: SerializeField] public FallenBubbles FallenBubblesView  { get; private set; }
        [field: SerializeField] public GameOver GameOver                { get; private set; }
        [field: SerializeField] public BonusView ReceivedBonus          { get; private set; }
        [Header("Stage Data")]
        [SerializeField] TMPro.TMP_Text ColorsCount;
        [SerializeField] TMPro.TMP_Text LevelAnchor;
        [Space(19)]
        [SerializeField] Animator HelpWindow;
        [SerializeField] GameObject EndgameObj, HeaderObj;
        [SerializeField] GameObject[] FunctionalButtonsInHeader;
        Gameplay.GameType.Survival GameType;
        bool started, turnedOn, requireDisableGameObject;
        
        void Start()
        {
            if (started) return;
            started =  true;
            Progress.Init(this);
            FallenBubblesView.Init(this);
        }
        
        #region  Window
        public void Show(Gameplay.GameType.Survival parent, float Duration = 1f)
        {
            if (!started) Start();
            GameType = parent;
            gameObject.SetActive(true);
            HeaderObj.SetActive(true);
            turnedOn = true;
            GetComponent<Animator>().SetFloat("Speed", ShowAnimationSpeed / Duration);
            GetComponent<Animator>().SetTrigger("Show");
        }
        
        public void Hide(float Duration = 1f, bool RequireTurnOff = true)
        {
            requireDisableGameObject = RequireTurnOff;
            if (turnedOn)
            {
                GetComponent<Animator>().SetFloat("Speed", ShowAnimationSpeed / Duration);
                GetComponent<Animator>().SetTrigger("Hide");
            }
            else
            {
                Invoke("RegisterHideFromAnimator", Duration);
            }
            turnedOn = false;
        }
        
        public void RegisterHideFromAnimator()
        {
            if (requireDisableGameObject)
            {
                gameObject.SetActive(false);
                EndgameObj.SetActive(false);
            }
            HeaderObj.SetActive(false);
        }
        #endregion
        
        #region Help
        public void ShowHelp()
        {
            GameType?.ProcessPause();
            HelpWindow.gameObject.SetActive(true);
        }
        
        public void HideHelp() => HelpWindow.SetTrigger("Hide");
        
        public void FinalizeHideInfo()
        {
            HelpWindow.gameObject.SetActive(false);
            GameType?.ProcessUnpause();
        }
        #endregion
        
        public void RefreshGameStageData(SurvivalStage stage)
        {
            Progress.RefreshProgress(stage.TimeOfStageRelative);
        }
        
        public void ReceiveNewGameStage(SurvivalStage stage)
        {
            ColorsCount.text = stage.SceneColors.ToString();
            LevelAnchor.text = stage.RewardByComboCount.ToString();
            Progress.ResetToZero();
        }
        
        public void CallPause() => GameType.CallSettings();
        
        public void SwitchFunctionalButtons(bool enabled)
        {
            foreach(var button in FunctionalButtonsInHeader)
            {
                button.SetActive(enabled);
            }
        }
    }
}