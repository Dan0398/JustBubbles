using System.Collections;
using UnityEngine;

namespace Services.Audio.Sounds
{
    [AddComponentMenu("Help/SoundService")]
    public class Service: MonoBehaviour, IService
    {
        WaitForSecondsRealtime Wait;
        Config soundsConfig;
        bool soundsPrepared = false;
        float volume;
        
        public static Service CreateInstance()
        {
            var obj = new GameObject("Sounds");
            DontDestroyOnLoad(obj);
            return obj.AddComponent<Service>();
        }
        
        void Start()
        {
            Wait = new WaitForSecondsRealtime(0.5f);
            StartCoroutine(LoadConfig());
            StartCoroutine(LoadSoundSettings());
        }
        
        IEnumerator LoadConfig()
        {
            var ConfigRequest = Resources.LoadAsync<Config>("Config/SoundConfig");
            while(!ConfigRequest.isDone) yield return Wait;
            soundsConfig = (Config)ConfigRequest.asset;
            
            var loader = DI.Single<Bundles.Agent>();
            var LoadRequest = loader.GiveMeContent(Application.streamingAssetsPath + '/' + soundsConfig.BundlePath, this, Bundles.Request.Priority.Mid);
            while(!LoadRequest.IsReady) yield return Wait;
            var bundle = LoadRequest.BundleInMemory;
            
            foreach(var pair in soundsConfig.SoundPairs)
            {
                var Load = bundle.LoadAssetAsync<AudioClip>(pair.NameInBundle);
                while(Load.isDone) yield return Wait;
                
                pair.LoadedData = (AudioClip) Load.asset;
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.clip = pair.LoadedData;
                source.loop = pair.Looped;
                source.volume = pair.DefaultVolume;
                pair.OnScene = source;
            }
            
            soundsPrepared = true;
        }
        
        IEnumerator LoadSoundSettings()
        {
            var Settings = DI.Single<Data.SettingsController>();
            while(!Settings.isDataLoaded) yield return Wait;
            while(!soundsPrepared) yield return Wait;
            Refresh();
            Settings.Data.SoundLevel.Changed += Refresh;
            
            void Refresh()
            {
                volume = Settings.Data.SoundLevel.Value;
                foreach(var pair in soundsConfig.SoundPairs)
                {
                    pair.OnScene.volume = volume * pair.DefaultVolume * pair.InGameMastering;
                }
            }
        }
        
        public void Play(SoundType type)
        {
            if (!soundsPrepared) return;
            var pair = PairByType(type);
            //if (pair == null) return;
            if (pair.OnScene.isPlaying && pair.Looped) return;
            pair.OnScene.Play();
        }
        
        public void Play(SoundType type, float Pitch)
        {
            if (!soundsPrepared) return;
            var Pair = PairByType(type);
            if (Pair == null) return;
            Pair.OnScene.pitch = Pitch;
            Pair.OnScene.Play();
        }
        
        public System.Action<float> PlayAndGiveVolumeChange(SoundType type)
        {
            if (!soundsPrepared) return null;
            var pair = PairByType(type);
            if (pair == null) return null;
            pair.OnScene.Play();
            return (float a) => 
            {
                pair.InGameMastering = a;
                pair.OnScene.volume = volume * pair.DefaultVolume * pair.InGameMastering;
            };
        }

        public void Stop(SoundType type)
        {
            if (!soundsPrepared) return;
            PairByType(type).OnScene.Stop();
        }
        
        Config.Pair PairByType(SoundType type)
        {
            if (!soundsPrepared) return null;
            foreach(var pair in soundsConfig.SoundPairs)
            {
                if (pair.Type == type)
                {
                    return pair;
                }
            }
            return null;
        }
    }
}