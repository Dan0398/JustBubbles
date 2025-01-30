using System.Collections;
using UnityEngine;

namespace Services.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioWrapper : MonoBehaviour
    {
        [SerializeField, Range(0, 1.0f)] float valueFromMastering;
        float valueFromSettings = 1;
        float valueFromScript;
        [SerializeField] AudioSource audio;
        Data.SettingsController reference;
        bool subscribed = false;
        bool started = false;
        System.Action onDestroy;
        
        public void Play() => audio.Play();
        public void Stop() => audio.Stop();
        
        public void ChangeVolume(float NewValue)
        {
            valueFromScript = NewValue;
            RefreshVolumes();
        }
        
        void Start()
        {
            if (started) return;
            audio ??= GetComponent<AudioSource>();
            if (!subscribed) StartCoroutine(Subscribe());
            started = true;
        }
        
        void OnEnable()
        {
            if (!started) return;
            if (!subscribed) StartCoroutine(Subscribe());
        }
        
        void OnValidate()
        {
            RefreshVolumes();
        }
        
        IEnumerator Subscribe()
        {
            var Wait = new WaitForFixedUpdate();
            reference ??= Services.DI.Single<Data.SettingsController>();
            while (!reference.isDataLoaded) yield return Wait;
            RefreshScale();
            reference.Data.SoundLevel.Changed += RefreshScale;
            onDestroy += () => reference.Data.SoundLevel.Changed -= RefreshScale;
            subscribed = true;
            
            void RefreshScale()
            {
                valueFromSettings = reference.Data.SoundLevel.Value;
                RefreshVolumes();
            }
        }
        
        void RefreshVolumes()
        {
            this.audio.volume = valueFromSettings * valueFromScript * valueFromMastering;
        }
        
        void OnDisable()
        {
            if (gameObject == null) return;
            if (subscribed) UnSubscribe();
        }
        
        void OnDestroy()
        {
            if (gameObject == null) return;
            if (subscribed) UnSubscribe();
        }
        
        void UnSubscribe()
        {
            onDestroy?.Invoke();
            onDestroy = null;
            subscribed = false;
        }
    }
}