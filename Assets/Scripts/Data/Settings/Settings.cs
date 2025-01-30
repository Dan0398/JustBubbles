using Utils.Observables;

namespace Data
{
    [System.Serializable]
    public class Settings: IAbstractData
    {
        public ObsFloat SoundLevel, MusicLevel;
        public ObsString UserLanguage;
        
        public Settings()
        {
            SoundLevel = 0.4f;
            MusicLevel = 0.4f;
            #if UNITY_ANDROID || UNITY_IOS
            UserLanguage = BrakelessGames.Localization.Utilities.LangCodeFromSystemLanguage();
            #else
            UserLanguage = "en";
            #endif
        }

        public void SetValuesAsFromStart()
        {
            SoundLevel = 0.4f;
            MusicLevel = 0.4f;
            #if UNITY_ANDROID || UNITY_IOS
            UserLanguage = BrakelessGames.Localization.Utilities.LangCodeFromSystemLanguage();
            #else
            UserLanguage = "en";
            #endif
        }
    }
}