using Gameplay.GameType;
using UnityEngine;

namespace UI.Survival
{
    public class SurvivalCanvas : MonoBehaviour
    {
        public AnimationCurve MoveDynamic;
        [SerializeField] private float _showAnimationSpeed;
        [field: SerializeField] public ScoreView Score                  { get; private set; }
        [field: SerializeField] public ComboView Combo                  { get; private set; }
        [field: SerializeField] public Progress Progress                { get; private set; }
        [field: SerializeField] public FallenBubbles FallenBubblesView  { get; private set; }
        [field: SerializeField] public GameOver GameOver                { get; private set; }
        [field: SerializeField] public BonusView ReceivedBonus          { get; private set; }
        [Header("Stage Data")]
        [SerializeField] private TMPro.TMP_Text _colorsCount;
        [SerializeField] private TMPro.TMP_Text _levelAnchor;
        [Space(19)]
        [SerializeField] private Animator _helpWindow;
        [SerializeField] private GameObject _endgameObj;
        [SerializeField] private GameObject _headerObj;
        [SerializeField] private GameObject[] _functionalButtonsInHeader;
        private Gameplay.GameType.Survival _gameType;
        private bool _started, _turnedOn, _requireDisableGameObject;
        
        private void Start()
        {
            if (_started) return;
            _started =  true;
            Progress.Init(this);
            FallenBubblesView.Init(this);
        }
        
        #region  Window
        public void Show(Gameplay.GameType.Survival parent, float Duration = 1f)
        {
            if (!_started) Start();
            _gameType = parent;
            gameObject.SetActive(true);
            _headerObj.SetActive(true);
            _turnedOn = true;
            GetComponent<Animator>().SetFloat("Speed", _showAnimationSpeed / Duration);
            GetComponent<Animator>().SetTrigger("Show");
        }
        
        public void Hide(float Duration = 1f, bool RequireTurnOff = true)
        {
            _requireDisableGameObject = RequireTurnOff;
            if (_turnedOn)
            {
                GetComponent<Animator>().SetFloat("Speed", _showAnimationSpeed / Duration);
                GetComponent<Animator>().SetTrigger("Hide");
            }
            else
            {
                Invoke("RegisterHideFromAnimator", Duration);
            }
            _turnedOn = false;
        }
        
        public void RegisterHideFromAnimator()
        {
            if (_requireDisableGameObject)
            {
                gameObject.SetActive(false);
                _endgameObj.SetActive(false);
            }
            _headerObj.SetActive(false);
        }
        #endregion
        
        #region Help
        public void ShowHelp()
        {
            _gameType?.ProcessPause();
            _helpWindow.gameObject.SetActive(true);
        }
        
        public void HideHelp()
        {
            _helpWindow.SetTrigger("Hide");
        }
        
        public void FinalizeHideInfo()
        {
            _helpWindow.gameObject.SetActive(false);
            _gameType?.ProcessUnpause();
        }
        #endregion
        
        public void RefreshGameStageData(SurvivalStage stage)
        {
            Progress.RefreshProgress(stage.TimeOfStageRelative);
        }
        
        public void ReceiveNewGameStage(SurvivalStage stage)
        {
            _colorsCount.text = stage.SceneColors.ToString();
            _levelAnchor.text = stage.RewardByComboCount.ToString();
            Progress.ResetToZero();
        }
        
        public void CallPause()
        {
            _gameType.CallSettings();
        }
        
        public void SwitchFunctionalButtons(bool enabled)
        {
            foreach(var button in _functionalButtonsInHeader)
            {
                button.SetActive(enabled);
            }
        }
    }
}