#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif
using Gameplay.User;

namespace Gameplay.GameType
{
    [System.Serializable]
    public abstract class BaseType
    {
        protected UI.Settings.Settings settings { get; private set; }
        protected Gameplay.Controller gameplay  { get; private set; }
        protected bool Paused                   { get; private set; }
        public    bool InSettings               { get; private set; }
        
        protected virtual bool GoToMenuAvailable    => true;
        protected virtual bool SkinChangeAvailable  => true;
        protected virtual bool IsFieldAspectDynamic => false;
        protected virtual float MaxFieldAspect      => 16/9f;
        protected virtual float FieldUpperOutstand  => 0f;
        protected virtual string Settings_ExitLangKey => "Menu";
        IPausableUser pausableUser;
        
        public BaseType(Gameplay.Controller Gameplay, IPausableUser User, UI.Settings.Settings Settings, InGameParents InGameParts)
        {
            gameplay = Gameplay;
            pausableUser = User;
            settings = Settings;
            settings.GoToMenuAvailable = GoToMenuAvailable;
            settings.RefreshExitLabel(Settings_ExitLangKey);
            ReactOn(InGameParts);
        }
        
        protected abstract void ReactOn(InGameParents InGameParts);
        
        public abstract void ProcessGameplayUpdate();
        
        public void CallGoToMenu()
        {
            gameplay.StopGameplay();
        }
        
        public void CallSettings()
        {
            ProcessPause();
            InSettings = true;
            settings.ShowSettings(() =>
            {
                ProcessUnpause();
                InSettings = false;
            });
        }
        
        public void ProcessPause()
        {
            Paused = true;
            pausableUser?.Pause();
        }
        
        public void ProcessUnpause()
        {
            Paused = false;
            pausableUser?.Unpause();
        }
        
        public abstract Task Dispose();
    }
}