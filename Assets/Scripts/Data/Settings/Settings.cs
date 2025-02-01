using Utils.Observables;

namespace Data
{
    [System.Serializable]
    public class Settings: IAbstractData
    {
        public ObsFloat SoundLevel, MusicLevel;
        public ObsString UserLanguage;
        
        [UnityEngine.Scripting.Preserve]
        public Settings()
        {
            SoundLevel = 0.4f;
            MusicLevel = 0.4f;
            #if UNITY_ANDROID || UNITY_IOS
            UserLanguage = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            #else
            UserLanguage = "en";
            #endif
        }

        public void SetValuesAsFromStart()
        {
            SoundLevel = 0.4f;
            MusicLevel = 0.4f;
            #if UNITY_ANDROID || UNITY_IOS
            UserLanguage = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            #else
            UserLanguage = "en";
            #endif
        }
    }
}