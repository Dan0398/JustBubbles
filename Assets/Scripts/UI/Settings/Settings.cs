using BrakelessGames.Localization;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Settings
{
    [RequireComponent(typeof(Canvas))]
    public class Settings : MonoBehaviour, Services.IService
    {
        public bool GoToMenuAvailable;
        public SkinsSettings Skins;
        [SerializeField] private string[] _languageCodes;
        [SerializeField] private GameObject _menuButton;
        [SerializeField] private Gameplay.Controller _gameplay;
        [SerializeField] private Slider _soundsSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private GameObject _musicInfoButton;
        [SerializeField] private TextTMPLocalized _goToMenuLabel;
        private System.Action _afterHide;
        private Data.SettingsController _settings;
        
        public void ShowSettings(System.Action afterHide = null)
        {
            _afterHide = afterHide;
            RefreshViews();
            gameObject.SetActive(true);
        }
        
        private void Start()
        {
            Skins.Init(this);
            StartCoroutine(BindSounds());
            #if UNITY_WEBGL
            MusicInfoButton?.SetActive(false);
            #endif
        }
        
        private IEnumerator BindSounds()
        {
            var Wait = new WaitForFixedUpdate();
            _settings = Services.DI.Single<Data.SettingsController>();
            while (!_settings.isDataLoaded) 
            {
                yield return Wait;
            }
            SubscribeToAudio(_settings);
        }
        
        private void SubscribeToAudio(Data.SettingsController Settings)
        {
            _soundsSlider.value = Settings.Data.SoundLevel.Value;
            _soundsSlider.onValueChanged.AddListener((s) => Settings.Data.SoundLevel.Value = s);
            var Sound = Services.DI.Single<Services.Audio.Sounds.Service>();
            var SoundHelp = _soundsSlider.GetComponent<PointerHelp>();
            SoundHelp.PointerDown += () =>
            {
                Sound.Play(Services.Audio.Sounds.SoundType.Settings_SoundsTest);
            };
            SoundHelp.PointerUp += () =>
            {
                Sound.Stop(Services.Audio.Sounds.SoundType.Settings_SoundsTest);
                Settings.SaveData();
            };
            
            _musicSlider.value = Settings.Data.MusicLevel.Value;
            _musicSlider.onValueChanged.AddListener((s) => Settings.Data.MusicLevel.Value = s);
            _musicSlider.GetComponent<PointerHelp>().PointerUp += () =>
            {
                Settings.SaveData();
            };
        }
        
        private void RefreshViews()
        {
            _menuButton.SetActive(GoToMenuAvailable);
        }
        
        public void GoToMenu()
        {
            _gameplay.StopGameplay();
            HideSettings();
        }
        
        public void HideSettings()
        {
            GetComponent<Animator>().SetTrigger("Hide");
        }
        
        public void RefreshExitLabel(string LangKey)
        {
            _goToMenuLabel.SetNewKey(LangKey);
        }
        
        public void FinalizeHideWindow()
        {
            gameObject.SetActive(false);
            _afterHide?.Invoke();
            _afterHide = null;
        }
        
        public void IncrementLanguage()
        {
            var codeIndex = System.Array.IndexOf(_languageCodes, _settings.Data.UserLanguage.Value);
            if (codeIndex < 0) throw new System.Exception("Code index not found.");
            codeIndex++;
            if (codeIndex >= _languageCodes.Length) codeIndex = 0;
            _settings.Data.UserLanguage.Value = _languageCodes[codeIndex];
            _settings.SaveData();
            BrakelessGames.Localization.Controller.SetLanguageByCode(_languageCodes[codeIndex]);
        }
    }
}