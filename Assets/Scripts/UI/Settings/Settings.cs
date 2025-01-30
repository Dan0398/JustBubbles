using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using BrakelessGames.Localization;

namespace UI.Settings
{
    [RequireComponent(typeof(Canvas))]
    public class Settings : MonoBehaviour, Services.IService
    {
        public bool GoToMenuAvailable;
        public SkinsSettings Skins;
        [SerializeField] GameObject MenuButton;
        [SerializeField] Gameplay.Controller Gameplay;
        [SerializeField] Slider SoundsSlider, MusicSlider;
        [SerializeField] GameObject MusicInfoButton;
        [SerializeField] TextTMPLocalized GoToMenuLabel;
        //[SerializeField] AudioSource EffectsSource;
        System.Action AfterHide;
        
        public void ShowSettings(System.Action afterHide = null)
        {
            AfterHide = afterHide;
            RefreshViews();
            gameObject.SetActive(true);
        }
        
        void Start()
        {
            Skins.Init(this);
            StartCoroutine(BindSounds());
            #if UNITY_WEBGL
            MusicInfoButton?.SetActive(false);
            #endif
        }
        
        IEnumerator BindSounds()
        {
            var Wait = new WaitForFixedUpdate();
            var Settings = Services.DI.Single<Data.SettingsController>();
            while (!Settings.isDataLoaded) 
            {
                yield return Wait;
            }
            SubscribeToAudio(Settings);
        }
        
        void SubscribeToAudio(Data.SettingsController Settings)
        {
            SoundsSlider.value = Settings.Data.SoundLevel.Value;
            SoundsSlider.onValueChanged.AddListener((s) => Settings.Data.SoundLevel.Value = s);
            var Sound = Services.DI.Single<Services.Audio.Sounds.Service>();
            var SoundHelp = SoundsSlider.GetComponent<PointerHelp>();
            SoundHelp.PointerDown += () =>
            {
                Sound.Play(Services.Audio.Sounds.SoundType.Settings_SoundsTest);
            };
            SoundHelp.PointerUp += () =>
            {
                Sound.Stop(Services.Audio.Sounds.SoundType.Settings_SoundsTest);
                //EffectsSource.Stop();
                Settings.SaveData();
            };
            
            MusicSlider.value = Settings.Data.MusicLevel.Value;
            MusicSlider.onValueChanged.AddListener((s) => Settings.Data.MusicLevel.Value = s);
            MusicSlider.GetComponent<PointerHelp>().PointerUp += () =>
            {
                Settings.SaveData();
            };
        }
        
        
        void RefreshViews()
        {
            MenuButton.SetActive(GoToMenuAvailable);
        }
        
        public void GoToMenu()
        {
            Gameplay.StopGameplay();
            HideSettings();
        }
        
        public void HideSettings()
        {
            GetComponent<Animator>().SetTrigger("Hide");
        }
        
        public void RefreshExitLabel(string LangKey) => GoToMenuLabel.SetNewKey(LangKey);
        
        public void FinalizeHideWindow()
        {
            gameObject.SetActive(false);
            AfterHide?.Invoke();
            AfterHide = null;
        }
    }
}