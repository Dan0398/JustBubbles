using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Services.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class Music : MonoBehaviour, Services.IService
    {
        private const int MusicCount = 13;
        public bool UserPaused = false;
        
        [SerializeField] private MusicModel[] _musics;
        [SerializeField] private bool _musicAllowed = true;
        [SerializeField] private bool _contentDelivering;
        [SerializeField] private bool _platformAvailable = true;
        [SerializeField] private bool _musicPaused;
        [SerializeField] private bool _turnedOn;
        private AudioSource _source;
        private int _currentCompositionNumber;
        private int[] _shuffledNumbers;
        private float _timeOnUnfocus;
        private bool _safetyDelayNow, _appInFocus;
        private WaitForFixedUpdate _wait;
        
        private int NextCompositionsNumber => (_currentCompositionNumber == (_musics.Length - 1))? 0 : (_currentCompositionNumber+1);
        
        private bool PauseRequired => UserPaused || !_musicAllowed || !_appInFocus;
        
        private string GimmePathByNumber(int ID)
        {
            return UnityEngine.Application.streamingAssetsPath + "/music/" + ID;
        }
        
        public static Music CreateInstance()
        {
            var obj = new GameObject("Music");
            DontDestroyOnLoad(obj);
            return obj.AddComponent<Music>();
        }
        
        private void Start()
        {
            _wait = new WaitForFixedUpdate();
            _source = GetComponent<AudioSource>();
            _source.reverbZoneMix = 0;
            PrepareShuffle();
            PrepareCompositions();
            StartCoroutine(BindWithSettings());
            BindAppFocus();
            _turnedOn = true;
        }
        
        private void PrepareShuffle()
        {
            List<int> AllNumbers = new List<int>(MusicCount);
            for (int i = 0; i < MusicCount; i++) 
            {
                int number = i;
                AllNumbers.Add(number);
            }
            List<int> ShuffledList = new List<int>(MusicCount);
            while (AllNumbers.Count > 0)
            {
                var ID = Random.Range(0, AllNumbers.Count);
                ShuffledList.Add(AllNumbers[ID]);
                AllNumbers.RemoveAt(ID);
            }
            _shuffledNumbers = ShuffledList.ToArray();
        }
        
        private void PrepareCompositions()
        {
            _currentCompositionNumber = 0;
            _musics = new MusicModel[MusicCount];
            for (int i=0; i< MusicCount; i++)
            {
                _musics[i] = new MusicModel();
            }
        }
        
        private IEnumerator BindWithSettings()
        {
            ApplyNewSoundScale(0);
            var RefData = Services.DI.Single<Data.SettingsController>();
            while(!RefData.isDataLoaded) yield return _wait;
            ApplyNewSoundScale(RefData.Data.MusicLevel.Value);
            RefData.Data.MusicLevel.Changed += () => ApplyNewSoundScale(RefData.Data.MusicLevel.Value);
        }
        
        private void ApplyNewSoundScale(float NewScale)
        {
            _source.volume = NewScale;
        }
        
        private void BindAppFocus()
        {
            ReceiveFocus(Application.isFocused);
            Application.focusChanged += ReceiveFocus;
        }
        
        private void ReceiveFocus(bool inFocus)
        {
            if (!Application.isPlaying || _source == null)
            {
                UnbindAppFocus();
                return;
            }
            _appInFocus = inFocus;
            if (inFocus) _source.time = _timeOnUnfocus;
            else _timeOnUnfocus = _source.time;
            ProcessPauseState();
        }
        
        private void UnbindAppFocus()
        {
            Application.focusChanged -= ReceiveFocus;
        }
        
        public void SetEnvironmentStatus(bool MusicAllowed)
        {
            this._musicAllowed = MusicAllowed;
        }
        
        private void Update()
        {
            if (!_turnedOn) return;
            if (!_platformAvailable) return;
            ProcessPauseState();
            CheckEndOfComposition();
        }
        
        private void ProcessPauseState()
        {
            if (_musicPaused != PauseRequired)
            {
                _musicPaused = PauseRequired;
                if (_musicPaused)
                {
                    _source.Pause();
                }
                else 
                {
                    StartCoroutine(PlaySourceSafety());
                }
            }
        }
        
        private IEnumerator PlaySourceSafety()
        {
            _safetyDelayNow = true;
            _source.Play();
            yield return _wait;
            while(PauseRequired) yield return _wait;
            if (!_source.isPlaying)
            {
                Debug.Log("Environment didnt support music");
                _platformAvailable = false;
                FullDispose();
            }
            _safetyDelayNow =  false;
        }
        
        private void FullDispose()
        {
            _source.Stop();
            for (int i = _musics.Length-1; i>=0; i--)
            {
                if (_musics[i] == null) continue;
                if (_musics[i].Clip != null)
                {
                    _musics[i].Clip.UnloadAudioData();
                    _musics[i].Clip = null;
                }
                if (_musics[i].Request != null)
                {
                    Services.DI.Single<Services.Bundles.Agent>().ReleaseContentUsage(_musics[i].Request, this);
                }
                _musics[i] = null;
            }
            _musics = null;
            _source.clip = null;
        }
        
        private void CheckEndOfComposition()
        {
            if (_contentDelivering) return;
            if (_safetyDelayNow) return;
            if (_source == null) return;
            if (_source.isPlaying) return;
            if (_musicPaused) return;
            if (!_appInFocus) return;
            SetNextCompositionAndLoad();
        }
        
        private void SetNextCompositionAndLoad()
        {
            _source.Stop();
            _source.clip  = null;
            _source.time = 0;
            _currentCompositionNumber = NextCompositionsNumber;
            _contentDelivering = true;
            if (!_platformAvailable)  return; 
            CheckCompositions(PlayCurrent);
            
            void PlayCurrent()
            {
                _source.clip = _musics[_currentCompositionNumber].Clip;
                if (!_musicPaused)
                {
                    StartCoroutine(PlaySourceSafety());
                }
            }
        }
        
        private void CheckCompositions(System.Action OnCurrentLoad)
        {
            _contentDelivering = true;
            System.Action OnCurrentEnd = () => 
            {
                OnCurrentLoad?.Invoke();
                _contentDelivering = false;
                StartCoroutine(CheckContentExistence(NextCompositionsNumber));
            };
            StartCoroutine(CheckContentExistence(_currentCompositionNumber, OnCurrentEnd));
            
        }
        
        private IEnumerator CheckContentExistence(int Number, System.Action OnEnds = null)
        {
            if (_musics[Number].Clip == null)
            {
                if (_musics[Number].Request == null)
                {
                    var bundles = Services.DI.Single<Services.Bundles.Agent>();
                    _musics[Number].Request = bundles.GiveMeContent(GimmePathByNumber(_shuffledNumbers[Number]), this);
                }
                if (_musics[Number].Request == null) yield break;
                while(_musics[Number].Request != null && !_musics[Number].Request.IsReady) yield return _wait;
                var unpackRequest = _musics[Number].Request.BundleInMemory.LoadAllAssetsAsync<AudioClip>();
                while(!unpackRequest.isDone) yield return _wait;
                _musics[Number].Clip = (AudioClip)unpackRequest.allAssets[0];
            }
            OnEnds?.Invoke();
        }
    }
}