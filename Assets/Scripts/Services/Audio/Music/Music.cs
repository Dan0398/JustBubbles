using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Services.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class Music : MonoBehaviour, Services.IService
    {
        const int MusicCount = 13;
        [SerializeField] MusicModel[] Musics;
        AudioSource Source;
        int CurrentCompositionNumber;
        public bool UserPaused = false;
        [SerializeField] bool MusicAllowed = true, ContentDelivering, PlatformAvailable = true;
        [SerializeField] bool MusicPaused, turnedOn;
        int[] ShuffledNumbers;
        float TimeOnUnfocus;
        bool SafetyDelayNow, AppInFocus;
        WaitForFixedUpdate Wait;
        
        int NextCompositionsNumber => (CurrentCompositionNumber == (Musics.Length - 1))? 0 : (CurrentCompositionNumber+1);
        
        bool PauseRequired => UserPaused || !MusicAllowed || !AppInFocus;
        
        string GimmePathByNumber(int ID) => UnityEngine.Application.streamingAssetsPath + "/music/" + ID;
        
        public static Music CreateInstance()
        {
            var obj = new GameObject("Music");
            DontDestroyOnLoad(obj);
            return obj.AddComponent<Music>();
        }
        
        void Start()
        {
            Wait = new WaitForFixedUpdate();
            Source = GetComponent<AudioSource>();
            Source.reverbZoneMix = 0;
            PrepareShuffle();
            PrepareCompositions();
            StartCoroutine(BindWithSettings());
            BindAppFocus();
            turnedOn = true;
            //StartCoroutine(TurnOnDelayed());
        }
        
        void PrepareShuffle()
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
            ShuffledNumbers = ShuffledList.ToArray();
        }
        
        void PrepareCompositions()
        {
            CurrentCompositionNumber = 0;
            Musics = new MusicModel[MusicCount];
            for (int i=0; i< MusicCount; i++)
            {
                Musics[i] = new MusicModel();
            }
        }
        
        IEnumerator BindWithSettings()
        {
            ApplyNewSoundScale(0);
            var RefData = Services.DI.Single<Data.SettingsController>();
            while(!RefData.isDataLoaded) yield return Wait;
            ApplyNewSoundScale(RefData.Data.MusicLevel.Value);
            RefData.Data.MusicLevel.Changed += () => ApplyNewSoundScale(RefData.Data.MusicLevel.Value);
        }
        
        void ApplyNewSoundScale(float NewScale)
        {
            Source.volume = NewScale;
        }
        
        void BindAppFocus()
        {
            ReceiveFocus(Application.isFocused);
            Application.focusChanged += ReceiveFocus;
        }
        
        void ReceiveFocus(bool inFocus)
        {
            if (!Application.isPlaying || Source == null)
            {
                UnbindAppFocus();
                return;
            }
            AppInFocus = inFocus;
            if (inFocus) Source.time = TimeOnUnfocus;
            else TimeOnUnfocus = Source.time;
            ProcessPauseState();
        }
        
        void UnbindAppFocus()
        {
            Application.focusChanged -= ReceiveFocus;
        }
        
        IEnumerator TurnOnDelayed()
        {
            yield return new WaitForSeconds(3f);
            turnedOn = true;
        }
        
        public void SetEnvironmentStatus(bool MusicAllowed)
        {
            this.MusicAllowed = MusicAllowed;
        }
        
        void Update()
        {
            if (!turnedOn) return;
            if (!PlatformAvailable) return;
            ProcessPauseState();
            CheckEndOfComposition();
        }
        
        void ProcessPauseState()
        {
            if (MusicPaused != PauseRequired)
            {
                MusicPaused = PauseRequired;
                if (MusicPaused)
                {
                    Source.Pause();
                }
                else 
                {
                    StartCoroutine(PlaySourceSafety());
                }
            }
        }
        
        IEnumerator PlaySourceSafety()
        {
            SafetyDelayNow = true;
            Source.Play();
            yield return Wait;
            while(PauseRequired) yield return Wait;
            if (!Source.isPlaying)
            {
                Debug.Log("Environment didnt support music");
                PlatformAvailable = false;
                FullDispose();
            }
            SafetyDelayNow =  false;
        }
        
        void FullDispose()
        {
            Source.Stop();
            for (int i = Musics.Length-1; i>=0; i--)
            {
                if (Musics[i] == null) continue;
                if (Musics[i].Clip != null)
                {
                    Musics[i].Clip.UnloadAudioData();
                    Musics[i].Clip = null;
                }
                if (Musics[i].Request != null)
                {
                    Services.DI.Single<Services.Bundles.Agent>().ReleaseContentUsage(Musics[i].Request, this);
                }
                Musics[i] = null;
            }
            Musics = null;
            Source.clip = null;
        }
        
        void CheckEndOfComposition()
        {
            if (ContentDelivering) return;
            if (SafetyDelayNow) return;
            if (Source == null) return;
            if (Source.isPlaying) return;
            if (MusicPaused) return;
            if (!AppInFocus) return;
            SetNextCompositionAndLoad();
        }
        
        void SetNextCompositionAndLoad()
        {
            Source.Stop();
            Source.clip  = null;
            Source.time = 0;
            CurrentCompositionNumber = NextCompositionsNumber;
            ContentDelivering = true;
            if (!PlatformAvailable)  return; 
            CheckCompositions(PlayCurrent);
            
            void PlayCurrent()
            {
                Source.clip = Musics[CurrentCompositionNumber].Clip;
                if (!MusicPaused)
                {
                    StartCoroutine(PlaySourceSafety());
                }
            }
        }
        
        void CheckCompositions(System.Action OnCurrentLoad)
        {
            ContentDelivering = true;
            System.Action OnCurrentEnd = () => 
            {
                OnCurrentLoad?.Invoke();
                ContentDelivering = false;
                StartCoroutine(CheckContentExistence(NextCompositionsNumber));
            };
            StartCoroutine(CheckContentExistence(CurrentCompositionNumber, OnCurrentEnd));
            
        }
        
        IEnumerator CheckContentExistence(int Number, System.Action OnEnds = null)
        {
            if (Musics[Number].Clip == null)
            {
                if (Musics[Number].Request == null)
                {
                    var bundles = Services.DI.Single<Services.Bundles.Agent>();
                    Musics[Number].Request = bundles.GiveMeContent(GimmePathByNumber(ShuffledNumbers[Number]), this);
                }
                if (Musics[Number].Request == null) yield break;
                while(Musics[Number].Request != null && !Musics[Number].Request.IsReady) yield return Wait;
                var unpackRequest = Musics[Number].Request.BundleInMemory.LoadAllAssetsAsync<AudioClip>();
                while(!unpackRequest.isDone) yield return Wait;
                Musics[Number].Clip = (AudioClip)unpackRequest.allAssets[0];
            }
            OnEnds?.Invoke();
        }
    }
}