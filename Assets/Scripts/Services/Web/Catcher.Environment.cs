#pragma warning disable CS0618 
#if UNITY_WEBGL
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        [System.Serializable]
        public class OuterEnvironment
        {
            public bool IsUsingTouch;
            public bool LoginAvailable;
            public bool IsLogOn;
            public bool LanguageOverwriteByEnv;
            public string LanguageCode;
            public bool MoreGamesTabAvailable;
            public bool FeedbackAvailable;
            public bool RequireSendGameplayStatus;
        }
        
        public async void ReceiveEnvironment(string OuterEnvJSON)
        {
            OuterEnvironment Outer = Newtonsoft.Json.JsonConvert.DeserializeObject<OuterEnvironment>(OuterEnvJSON);
            var Env = await GimmeEnvironment();
            Env.IsUsingTouch.Value = Outer.IsUsingTouch;
            Env.LoginAvailable.Value = Outer.LoginAvailable;
            Env.IsLogon.Value = Outer.IsLogOn;
            Env.MoreGamesTabAvailable.Value = Outer.MoreGamesTabAvailable;
            if (Outer.LanguageOverwriteByEnv)
            {
                if (!Env.LanguageOverwritten)
                {
                    var Settings = Services.DI.Single<Data.SettingsController>();
                    while(!Settings.isDataLoaded) await Utilities.Wait();
                    Settings.Data.UserLanguage.Value = Outer.LanguageCode;
                    Env.LanguageOverwritten = true;
                }
            }
            Env.FeedbackAvailable.Value = Outer.FeedbackAvailable;
            Env.RequireSendGameplayStatus.Value = Outer.RequireSendGameplayStatus;
            
            //SendLoadingEnds();
        }
        
        public static void RequestOuterEnvironment()
        {
            Application.ExternalCall("SendEnvironmentToEngine");
        }
        
        async UniTask<Services.Environment> GimmeEnvironment()
        {
            var Env = Services.DI.Single<Services.Environment>();
            while (Env == null)
            {
                await Utilities.Wait();
                Env = Services.DI.Single<Services.Environment>();
            }
            return Env;
        }
        
        public static void ShowMoreGames()
        {
            Application.ExternalCall("ShowMoreGames");
        }
    }
}
#endif