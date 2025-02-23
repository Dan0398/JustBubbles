using System.Collections;
using UnityEngine;

namespace Services.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioWrapper : MonoBehaviour
    {
        [SerializeField, Range(0, 1.0f)] private float _valueFromMastering;
        [SerializeField] private AudioSource _audio;
        private float _valueFromSettings = 1;
        private float _valueFromScript;
        private Data.SettingsController _reference;
        private bool _subscribed = false;
        private bool _started = false;
        private System.Action _onDestroy;
        
        public void Play()
        {
            _audio.Play();
        }
        
        public void Stop()
        {
            _audio.Stop();
        }
        
        public void ChangeVolume(float NewValue)
        {
            _valueFromScript = NewValue;
            RefreshVolumes();
        }
        
        private void Start()
        {
            if (_started) return;
            _audio ??= GetComponent<AudioSource>();
            if (!_subscribed) StartCoroutine(Subscribe());
            _started = true;
        }
        
        private void OnEnable()
        {
            if (!_started) return;
            if (!_subscribed) StartCoroutine(Subscribe());
        }
        
        private void OnValidate()
        {
            RefreshVolumes();
        }
        
        private IEnumerator Subscribe()
        {
            var Wait = new WaitForFixedUpdate();
            _reference ??= Services.DI.Single<Data.SettingsController>();
            while (!_reference.isDataLoaded) yield return Wait;
            RefreshScale();
            _reference.Data.SoundLevel.Changed += RefreshScale;
            _onDestroy += () => _reference.Data.SoundLevel.Changed -= RefreshScale;
            _subscribed = true;
            
            void RefreshScale()
            {
                _valueFromSettings = _reference.Data.SoundLevel.Value;
                RefreshVolumes();
            }
        }
        
        private void RefreshVolumes()
        {
            _audio.volume = _valueFromSettings * _valueFromScript * _valueFromMastering;
        }
        
        private void OnDisable()
        {
            if (gameObject == null) return;
            if (_subscribed) UnSubscribe();
        }
        
        private void OnDestroy()
        {
            if (gameObject == null) return;
            if (_subscribed) UnSubscribe();
        }
        
        private void UnSubscribe()
        {
            _onDestroy?.Invoke();
            _onDestroy = null;
            _subscribed = false;
        }
    }
}