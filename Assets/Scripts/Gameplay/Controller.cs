using UnityEngine;

namespace Gameplay
{
    [AddComponentMenu("Help/Gameplay Controller")]
    public class Controller : MonoBehaviour, Services.IService
    {
        [SerializeField] UI.Menu.MainMenu MainMenuCanvas;
        [SerializeField] UI.Settings.Settings Settings;
        [Header("Game modes")]
        [SerializeField] UI.Endless.EndlessCanvas Endless;
        [SerializeField] UI.Survival.SurvivalCanvas TimeTrial;
        [SerializeField] UI.Strategy.StrategyCanvas Strategy;
        [SerializeField] UI.Merge.MergeCanvas Merge;
        [Header("Bubble Game Processors")]
        [SerializeField] Field.BubbleField bubbleField;
        [SerializeField] User.Action Actions;
        [Header("Merge Game Processors")]
        [SerializeField] Merge.MergeField mergeField;
        [SerializeField] User.MergeUser mergeUser;
        [SerializeReference] GameType.BaseType ActualType;
        [SerializeField] InGameParents InGameParts;
        
        void Start()
        {
            StopGameplay();
        }
        
        void FixedUpdate()
        {
            ActualType?.ProcessGameplayUpdate();
        }
        
        public void StopGameplay() => ChangeGameType(() => new GameType.Idle       (this, Settings, InGameParts, MainMenuCanvas));
        
        public void StartEndless() => ChangeGameType(() => new GameType.Endless     (this, Settings, InGameParts, bubbleField, Actions, Endless));
        
        public void StartTimeTrial()=> ChangeGameType(() => new GameType.Survival   (this, Settings, InGameParts, bubbleField, Actions, TimeTrial));
        
        public void StartStrategy() => ChangeGameType(() => new GameType.Strategy   (this, Settings, InGameParts, bubbleField, Actions, Strategy));
        
        internal void StartMerge() => ChangeGameType(() => new GameType.Merge       (this, Settings, InGameParts, mergeField, mergeUser, Merge));
        
        async void ChangeGameType(System.Func<GameType.BaseType> newType)
        {
            if (ActualType != null)
            {
                await ActualType.Dispose();
            }
            ActualType = newType.Invoke();
        }
    }
}