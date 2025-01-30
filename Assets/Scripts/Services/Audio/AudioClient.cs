using System.Collections;
using UnityEngine;

namespace Services.Audio
{
    public class AudioClient : MonoBehaviour
    {
        [System.Serializable]
        enum AudioType
        {
            Sound,
            Music
        }
        [SerializeField] AudioType MyAudioType;
        bool isStarted = false;
        Data.SettingsController Reference;
        bool Subscribed = false;
        SoundPair[] Pairs;
        System.Action onDestroy;
        
        void Start()
        {
            FindStarts();
            if (!Subscribed) StartCoroutine(Subscribe());
            isStarted = true;
        }
        
        void FindStarts()
        {
            var Sources = gameObject.GetComponents<AudioSource>();
            if (Sources == null || Sources.Length == 0) return;
            Pairs = new SoundPair[Sources.Length];
            for (int i=0; i < Sources.Length; i++)
            {
                Pairs[i] = new SoundPair(Sources[i]);
            }
        }
        
        void OnEnable()
        {
            if (!isStarted) return;
            if (!Subscribed) StartCoroutine(Subscribe());
        }
        
        IEnumerator Subscribe()
        {
            var Wait = new WaitForFixedUpdate();
            Subscribed = true;
            if (Reference == null)
            {
                Reference = Services.DI.Single<Data.SettingsController>();    
            }
            while (!Reference.isDataLoaded) yield return Wait;
            System.Action RefreshScale = null;
            if (MyAudioType == AudioType.Music)
            {
                RefreshScale = () => RefreshVolumes(Reference.Data.MusicLevel.Value);
                Reference.Data.MusicLevel.Changed += RefreshScale;
                onDestroy += () => Reference.Data.MusicLevel.Changed -= RefreshScale;
            }
            else 
            {
                RefreshScale = () => RefreshVolumes(Reference.Data.SoundLevel.Value);
                Reference.Data.SoundLevel.Changed += RefreshScale;
                onDestroy += () => Reference.Data.SoundLevel.Changed -= RefreshScale;
            }
            RefreshScale?.Invoke();
            //Services.DI.Single<Services.Pause>().OnPauseChanges += ApplyPauseState;
        }
        
        void RefreshVolumes(float Multiplier)
        {
            foreach(var Pair in Pairs)
            {
                Pair.SetVolumeMultiplier(Multiplier);
            }
        }
        
        /*
        void ApplyPauseState(Services.Pause.PauseType Type)
        {
            foreach(var Sound in Pairs)
                {
                if (Type == Services.Pause.PauseType.Hard)
                {
                    Sound.PauseByMenu();
                }
                else
                {
                    Sound.RestoreFromPause();
                }
            }
        }
        */

        void OnDisable()
        {
            if (gameObject == null) return;
            //if (Subscribed) StartCoroutine(UnSubscribe());
        }
        
        void OnDestroy()
        {
            if (gameObject == null) return;
            //if (Subscribed) StartCoroutine(UnSubscribe());
        }
        
        IEnumerator UnSubscribe()
        {
            var Wait = new WaitForFixedUpdate();
            if (Reference == null)
            {
                Reference = Services.DI.Single<Data.SettingsController>();    
            }
            while (!Reference.isDataLoaded) yield return Wait;
            onDestroy?.Invoke();
            onDestroy = null;
            //Services.DI.Single<Services.Pause>().OnPauseChanges -= ApplyPauseState;
            Subscribed = false;
        }
        
        class SoundPair
        {
            AudioSource MySource;
            float DefaultVolume;
            bool PlayedBeforePause, isPausedByMenu;
            
            public SoundPair(AudioSource Source)
            {
                MySource = Source;
                DefaultVolume = MySource.volume;
            }
            
            public void SetVolumeMultiplier(float Multiplier)
            {
                MySource.volume = DefaultVolume * Multiplier;
            }
            
            public void PauseByMenu()
            {
                if (isPausedByMenu) return;
                isPausedByMenu = true;
                PlayedBeforePause = MySource.isPlaying;
                MySource.Pause();
            }
            
            public void RestoreFromPause()
            {
                if (!isPausedByMenu) return;
                isPausedByMenu = false;
                if (PlayedBeforePause)
                {
                    MySource.Play();
                }
            }
        }
    }
}