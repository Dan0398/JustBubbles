using System.Collections;
using UnityEngine;

namespace Services.Audio.Sounds
{
    [AddComponentMenu("Help/SoundService")]
    public class Service: MonoBehaviour, IService
    {
        private WaitForSecondsRealtime _wait;
        private Config _soundsConfig;
        private bool _soundsPrepared;
        private float _volume;
        
        public static Service CreateInstance()
        {
            var obj = new GameObject("Sounds");
            DontDestroyOnLoad(obj);
            return obj.AddComponent<Service>();
        }
        
        private void Start()
        {
            _wait = new WaitForSecondsRealtime(0.5f);
            StartCoroutine(LoadConfig());
            StartCoroutine(LoadSoundSettings());
        }
        
        private IEnumerator LoadConfig()
        {
            var ConfigRequest = Resources.LoadAsync<Config>("Config/SoundConfig");
            while(!ConfigRequest.isDone) yield return _wait;
            _soundsConfig = (Config)ConfigRequest.asset;
            
            var loader = DI.Single<Bundles.Agent>();
            var LoadRequest = loader.GiveMeContent(Application.streamingAssetsPath + '/' + _soundsConfig.BundlePath, this, Bundles.Request.Priority.Mid);
            while(!LoadRequest.IsReady) yield return _wait;
            var bundle = LoadRequest.BundleInMemory;
            
            foreach(var pair in _soundsConfig.SoundPairs)
            {
                var Load = bundle.LoadAssetAsync<AudioClip>(pair.NameInBundle);
                while(Load.isDone) yield return _wait;
                
                pair.LoadedData = (AudioClip) Load.asset;
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.clip = pair.LoadedData;
                source.loop = pair.Looped;
                source.volume = pair.DefaultVolume;
                pair.OnScene = source;
            }
            _soundsPrepared = true;
        }
        
        private IEnumerator LoadSoundSettings()
        {
            var Settings = DI.Single<Data.SettingsController>();
            while(!Settings.isDataLoaded) yield return _wait;
            while(!_soundsPrepared) yield return _wait;
            Refresh();
            Settings.Data.SoundLevel.Changed += Refresh;
            
            void Refresh()
            {
                _volume = Settings.Data.SoundLevel.Value;
                foreach(var pair in _soundsConfig.SoundPairs)
                {
                    pair.OnScene.volume = _volume * pair.DefaultVolume * pair.InGameMastering;
                }
            }
        }
        
        public void Play(SoundType type)
        {
            if (!_soundsPrepared) return;
            var pair = PairByType(type);
            if (pair.OnScene.isPlaying && pair.Looped) return;
            pair.OnScene.Play();
        }
        
        public void Play(SoundType type, float Pitch)
        {
            if (!_soundsPrepared) return;
            var Pair = PairByType(type);
            if (Pair == null) return;
            Pair.OnScene.pitch = Pitch;
            Pair.OnScene.Play();
        }
        
        public System.Action<float> PlayAndGiveVolumeChange(SoundType type)
        {
            if (!_soundsPrepared) return null;
            var pair = PairByType(type);
            if (pair == null) return null;
            pair.OnScene.Play();
            return (float a) => 
            {
                pair.InGameMastering = a;
                pair.OnScene.volume = _volume * pair.DefaultVolume * pair.InGameMastering;
            };
        }

        public void Stop(SoundType type)
        {
            if (!_soundsPrepared) return;
            PairByType(type).OnScene.Stop();
        }
        
        private Config.Pair PairByType(SoundType type)
        {
            if (!_soundsPrepared) return null;
            foreach(var pair in _soundsConfig.SoundPairs)
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