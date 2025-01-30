using DIService = Services.DI;
using UnityEngine;

namespace Services
{
    public class Registrator : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        static void RegisterAllSystems()
        {
            var Runner = CoroutineRunner.CreateCoroutineRunner();
            DIService.Register<CoroutineRunner>(Runner);
            DIService.Register<Bundles.Agent>(new Bundles.Agent());
            
            DIService.Register<Data.UserController>(new Data.UserController());
            DIService.Register<Data.SettingsController>(new Data.SettingsController());
            DIService.Register<Data.MergeController>(new Data.MergeController());
            
            DIService.Register<Audio.Sounds.Service>(Audio.Sounds.Service.CreateInstance());
            #if !UNITY_EDITOR
            #endif
            DIService.Register<Audio.Music>(Audio.Music.CreateInstance());
            DIService.Register<Content.Instrument.Service>(new Content.Instrument.Service());
            DIService.Register<Content.Merge.Service>(new Content.Merge.Service());
            
            DIService.Register<Advertisements.Controller>(new Advertisements.Controller());
            DIService.Register<Environment>(new Environment());
        }
    }
}