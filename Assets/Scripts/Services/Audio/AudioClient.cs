using System.Collections;
using UnityEngine;

namespace Services.Audio
{
    public class AudioClient : MonoBehaviour
    {
        [SerializeField] private AudioType _myAudioType;
        private Data.SettingsController _reference;
        private SoundPair[] _pairs;
        private bool _subscribed;
        private bool _isStarted;
        
        private void Start()
        {
            FindStarts();
            if (!_subscribed) StartCoroutine(Subscribe());
            _isStarted = true;
        }
        
        private void FindStarts()
        {
            var Sources = gameObject.GetComponents<AudioSource>();
            if (Sources == null || Sources.Length == 0) return;
            _pairs = new SoundPair[Sources.Length];
            for (int i=0; i < Sources.Length; i++)
            {
                _pairs[i] = new SoundPair(Sources[i]);
            }
        }
        
        private void OnEnable()
        {
            if (!_isStarted) return;
            if (!_subscribed) StartCoroutine(Subscribe());
        }
        
        private IEnumerator Subscribe()
        {
            var Wait = new WaitForFixedUpdate();
            _subscribed = true;
            _reference ??= Services.DI.Single<Data.SettingsController>();
            while (!_reference.isDataLoaded) yield return Wait;
            System.Action RefreshScale = null;
            if (_myAudioType == AudioType.Music)
            {
                RefreshScale = () => RefreshVolumes(_reference.Data.MusicLevel.Value);
                _reference.Data.MusicLevel.Changed += RefreshScale;
            }
            else 
            {
                RefreshScale = () => RefreshVolumes(_reference.Data.SoundLevel.Value);
                _reference.Data.SoundLevel.Changed += RefreshScale;
            }
            RefreshScale?.Invoke();
        }
        
        private void RefreshVolumes(float Multiplier)
        {
            foreach(var Pair in _pairs)
            {
                Pair.SetVolumeMultiplier(Multiplier);
            }
        }
        
        private class SoundPair
        {
            private AudioSource _mySource;
            private float _defaultVolume;
            private bool _playedBeforePause, _isPausedByMenu;
            
            public SoundPair(AudioSource Source)
            {
                _mySource = Source;
                _defaultVolume = _mySource.volume;
            }
            
            public void SetVolumeMultiplier(float Multiplier)
            {
                _mySource.volume = _defaultVolume * Multiplier;
            }
            
            public void PauseByMenu()
            {
                if (_isPausedByMenu) return;
                _isPausedByMenu = true;
                _playedBeforePause = _mySource.isPlaying;
                _mySource.Pause();
            }
            
            public void RestoreFromPause()
            {
                if (!_isPausedByMenu) return;
                _isPausedByMenu = false;
                if (_playedBeforePause)
                {
                    _mySource.Play();
                }
            }
        }
    }
}