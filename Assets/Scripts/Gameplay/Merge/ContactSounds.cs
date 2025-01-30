using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Merge
{
    [System.Serializable]
    public class ContactSounds: MonoBehaviour
    {
        [SerializeField, Range(.0f, 1.0f)] float LowerClip;
        [SerializeField, Range(1, 100)] int MaxSourcesCount;
        List<WrappedSource> Sources;
        Data.SettingsController settings;
        AudioClip ActualClip;
        
        void Start()
        {
            settings = Services.DI.Single<Data.SettingsController>();
        }
        
        public void Play(float Volume)
        {
            if (Volume < LowerClip) return;
            foreach(var Source in Sources)
            {
                if (Source.Busy) continue;
                Source.Play(Volume);
                return;
            }
            if (Sources.Count == MaxSourcesCount) return;
            var aud = gameObject.AddComponent<AudioSource>();
            aud.playOnAwake = false;
            aud.clip = ActualClip;
            var newSource = new WrappedSource(aud);
            
            System.Action Refresh = () => newSource.ChangeSettingsVolume(settings.Data.SoundLevel.Value);
            Refresh.Invoke();
            settings.Data.SoundLevel.Changed += Refresh;
            newSource.OnDestroy = () => 
            {
                settings.Data.SoundLevel.Changed -= Refresh;
                newSource.OnDestroy = null;
            };
            
            Sources.Add(newSource);
            newSource.Play(Volume);
        }
        
        public void Setup(AudioClip source)
        {
            Sources ??= new List<WrappedSource>(10);
            for (int i = Sources.Count -1; i >= 0; i--)
            {
                Sources[i].SelfDestroy();
                Sources.RemoveAt(i);
            }
            ActualClip = source;
        }
        
        void FixedUpdate()
        {
            Sources ??= new List<WrappedSource>(10);
            foreach(var Source in Sources) Source.TryDecrementFixedTime();
        }
        
        public class WrappedSource
        {
            public System.Action OnDestroy;
            AudioSource source;
            float settingsVolume, CollisionVolume;
            float playTimer;
            
            public bool Busy => playTimer > 0;
            
            public WrappedSource(AudioSource Source)
            {
                source = Source;
                playTimer = 0;
            }
            
            public void ChangeSettingsVolume(float newVolume)
            {
                settingsVolume = newVolume;
                ApplyVolume();
            }
            
            public void Play(float Volume)
            {
                if (Busy) throw new System.Exception("Попытка включить занятый источник");
                CollisionVolume = Volume;
                ApplyVolume();
                playTimer = source.clip.length;
                source.Play();
            }
            
            void ApplyVolume()
            {
                source.volume = settingsVolume * CollisionVolume;
            }
            
            public void TryDecrementFixedTime()
            {
                if (!Busy) return;
                playTimer -= Time.fixedDeltaTime;
            }
            
            public void SelfDestroy()
            {
                Object.Destroy(source);
                OnDestroy.Invoke();
                OnDestroy = null;
            }
        }
    }
}