using System.Collections.Generic;
using BrakelessGames.Localization;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Survival
{
    [RequireComponent(typeof(Animator))]
    public class BonusView : MonoBehaviour
    {
        [SerializeField] Image InstrumentIcon;
        [SerializeField] TextTMPLocalized InstrumentName;
        Animator animator;
        Queue<ShowQuery> QueryForShow;
        Services.Audio.Sounds.Service sounds;
        bool started;
        
        void Start()
        {
            if (QueryForShow != null) return;
            sounds = Services.DI.Single<Services.Audio.Sounds.Service>();
            animator = GetComponent<Animator>();
            QueryForShow = new Queue<ShowQuery>(2);
        }
        
        public void ShowBonuses(IEnumerable<Content.Instrument.WorkType> Types, Content.Instrument.Config ViewData, System.Action<Content.Instrument.WorkType> OnReceive)
        {
            if (QueryForShow == null) Start();
            foreach(var Type in Types)
            {
                var Pair = GetPair(Type);
                QueryForShow.Enqueue(new ShowQuery()
                {
                    Icon = Pair.Sprite,
                    Name = Pair.NameLangKey,
                    OnReceive = () => OnReceive.Invoke(Pair.Type)
                });
            }
            if (!gameObject.activeSelf)
            {
                RefreshView(QueryForShow.Peek());
                gameObject.SetActive(true);
            }
            
            Content.Instrument.Config.InstrumentView GetPair(Content.Instrument.WorkType type)
            {
                foreach(var inst in ViewData.Instruments)
                {
                    if (inst.Type == type) return inst;
                }
                return null;
            }
        }
        
        public void ReadyToShowNew()
        {
            if (QueryForShow.TryDequeue(out var result))
            {
                RefreshView(result);
                result.OnReceive.Invoke();
                animator.SetTrigger("ShowNew");
                sounds.Play(Services.Audio.Sounds.SoundType.InstrumentPicked);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        
        void RefreshView(ShowQuery shown)
        {
            InstrumentIcon.sprite = shown.Icon;
            InstrumentName.SetNewKey(shown.Name);
        }
        
        class ShowQuery
        {
            public Sprite Icon;
            public string Name;
            public System.Action OnReceive;
        }
    }
}