using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Merge
{
    [System.Serializable]
    public class ContactSounds: MonoBehaviour
    {
        [SerializeField, Range(.0f, 1.0f)] private float _lowerClip;
        [SerializeField, Range(1, 100)] private int _maxSourcesCount;
        private List<WrappedSource> _sources;
        private Data.SettingsController _settings;
        private AudioClip _actualClip;
        
        private void Start()
        {
            _settings = Services.DI.Single<Data.SettingsController>();
        }
        
        public void Play(float Volume)
        {
            if (Volume < _lowerClip) return;
            foreach(var Source in _sources)
            {
                if (Source.Busy) continue;
                Source.Play(Volume);
                return;
            }
            if (_sources.Count == _maxSourcesCount) return;
            var aud = gameObject.AddComponent<AudioSource>();
            aud.playOnAwake = false;
            aud.clip = _actualClip;
            var newSource = new WrappedSource(aud);
            
            System.Action Refresh = () => newSource.ChangeSettingsVolume(_settings.Data.SoundLevel.Value);
            Refresh.Invoke();
            _settings.Data.SoundLevel.Changed += Refresh;
            newSource.OnDestroy = () => 
            {
                _settings.Data.SoundLevel.Changed -= Refresh;
                newSource.OnDestroy = null;
            };
            
            _sources.Add(newSource);
            newSource.Play(Volume);
        }
        
        public void Setup(AudioClip source)
        {
            _sources ??= new List<WrappedSource>(10);
            for (int i = _sources.Count -1; i >= 0; i--)
            {
                _sources[i].SelfDestroy();
                _sources.RemoveAt(i);
            }
            _actualClip = source;
        }
        
        private void FixedUpdate()
        {
            _sources ??= new List<WrappedSource>(10);
            foreach(var Source in _sources) Source.TryDecrementFixedTime();
        }
    }
}