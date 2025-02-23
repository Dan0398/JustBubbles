using UnityEngine;

namespace Gameplay
{
    [AddComponentMenu("Help/Gameplay Controller")]
    public class Controller : MonoBehaviour, Services.IService
    {
        [SerializeField] private UI.Menu.MainMenu _mainMenuCanvas;
        [SerializeField] private UI.Settings.Settings _settings;
        [Header("Game modes")]
        [SerializeField] private UI.Endless.EndlessCanvas _endless;
        [SerializeField] private UI.Survival.SurvivalCanvas _timeTrial;
        [SerializeField] private UI.Strategy.StrategyCanvas _strategy;
        [SerializeField] private UI.Merge.MergeCanvas _merge;
        [Header("Bubble Game Processors")]
        [SerializeField] private Field.BubbleField _bubbleField;
        [SerializeField] private User.Action _actions;
        [Header("Merge Game Processors")]
        [SerializeField] private Merge.MergeField _mergeField;
        [SerializeField] private User.MergeUser _mergeUser;
        [SerializeReference] private GameType.BaseType _actualType;
        [SerializeField] private InGameParents _inGameParts;
        
        private void Start()
        {
            StopGameplay();
        }
        
        private void FixedUpdate()
        {
            _actualType?.ProcessGameplayUpdate();
        }
        
        public void StopGameplay() => ChangeGameType(() => new GameType.Idle       (this, _settings, _inGameParts, _mainMenuCanvas));
        
        public void StartEndless() => ChangeGameType(() => new GameType.Endless     (this, _settings, _inGameParts, _bubbleField, _actions, _endless));
        
        public void StartTimeTrial()=> ChangeGameType(() => new GameType.Survival   (this, _settings, _inGameParts, _bubbleField, _actions, _timeTrial));
        
        public void StartStrategy() => ChangeGameType(() => new GameType.Strategy   (this, _settings, _inGameParts, _bubbleField, _actions, _strategy));
        
        public void StartMerge()    => ChangeGameType(() => new GameType.Merge       (this, _settings, _inGameParts, _mergeField, _mergeUser, _merge));
        
        private async void ChangeGameType(System.Func<GameType.BaseType> newType)
        {
            if (_actualType != null)
            {
                await _actualType.Dispose();
            }
            _actualType = newType.Invoke();
        }
    }
}