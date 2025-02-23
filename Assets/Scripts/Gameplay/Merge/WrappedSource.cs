using UnityEngine;

namespace Gameplay.Merge
{
    public class WrappedSource
    {
        public System.Action OnDestroy;
        private AudioSource _source;
        private float _settingsVolume, _collisionVolume;
        private float _playTimer;
        
        public bool Busy => _playTimer > 0;
        
        public WrappedSource(AudioSource Source)
        {
            _source = Source;
            _playTimer = 0;
        }
        
        public void ChangeSettingsVolume(float newVolume)
        {
            _settingsVolume = newVolume;
            ApplyVolume();
        }
        
        public void Play(float Volume)
        {
            if (Busy) throw new System.Exception("Попытка включить занятый источник");
            _collisionVolume = Volume;
            ApplyVolume();
            _playTimer = _source.clip.length;
            _source.Play();
        }
        
        private void ApplyVolume()
        {
            _source.volume = _settingsVolume * _collisionVolume;
        }
        
        public void TryDecrementFixedTime()
        {
            if (!Busy) return;
            _playTimer -= Time.fixedDeltaTime;
        }
        
        public void SelfDestroy()
        {
            Object.Destroy(_source);
            OnDestroy.Invoke();
            OnDestroy = null;
        }
    }
}