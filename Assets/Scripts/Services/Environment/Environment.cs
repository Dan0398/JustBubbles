#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif
using Utils.Observables;
using UnityEngine;

namespace Services 
{
    public class Environment : IService
    {
        public ObsBool IsUsingTouch                 { get; private set; }
        public ObsBool LoginAvailable               { get; private set; }
        public ObsBool IsLogon                      { get; private set; }
        public ObsBool MoreGamesTabAvailable        { get; private set; }
        public ObsBool FeedbackAvailable            { get; private set; }
        public ObsBool RequireSendGameplayStatus    { get; private set; }
        public bool LanguageOverwritten;
        
        public Environment()
        {
            LanguageOverwritten = false;
            LoginAvailable = false;
            IsLogon = false;
            MoreGamesTabAvailable = false;
            FeedbackAvailable = false;
            RequireSendGameplayStatus = false;
            ApplyTouchStatus();
            SetupLanguage();
#if UNITY_WEBGL
            Services.Web.Catcher.RequestOuterEnvironment();
#endif
            SetupApplication();
        }
        
        void ApplyTouchStatus()
        {
#if UNITY_EDITOR
            IsUsingTouch = false;
#else
    #if (UNITY_ANDROID || UNITY_IOS)
                IsUsingTouch = true;
    #else
                IsUsingTouch = false;
    #endif
#endif
        }
        
        async void SetupLanguage()
        {
            Data.SettingsController Settings = Services.DI.Single<Data.SettingsController>();
            while(Settings == null)
            {
                await Task.Delay(300);
                Settings = Services.DI.Single<Data.SettingsController>();
            }
            while(!Settings.isDataLoaded) await Task.Delay(300);
            
            System.Action OnLangChanged = () =>
                BrakelessGames.Localization.Controller.SetLanguageByCode(Settings.Data.UserLanguage.Value);
            OnLangChanged.Invoke();
            Settings.Data.UserLanguage.Changed += OnLangChanged;
            
        }
        
        void SetupApplication()
        {
            Application.targetFrameRate = 61;
        }
    }
}