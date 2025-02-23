using System.Collections.Generic;
using BrakelessGames.Localization;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Survival
{
    [RequireComponent(typeof(Animator))]
    public class BonusView : MonoBehaviour
    {
        [SerializeField] Image _instrumentIcon;
        [SerializeField] TextTMPLocalized _instrumentName;
        private Animator _animator;
        private Queue<ShowQuery> _queryForShow;
        private Services.Audio.Sounds.Service _sounds;
        
        private void Start()
        {
            if (_queryForShow != null) return;
            _sounds = Services.DI.Single<Services.Audio.Sounds.Service>();
            _animator = GetComponent<Animator>();
            _queryForShow = new Queue<ShowQuery>(2);
        }
        
        public void ShowBonuses(IEnumerable<Content.Instrument.WorkType> Types, Content.Instrument.Config ViewData, System.Action<Content.Instrument.WorkType> OnReceive)
        {
            if (_queryForShow == null) Start();
            foreach(var Type in Types)
            {
                var Pair = GetPair(Type);
                _queryForShow.Enqueue(new ShowQuery()
                {
                    Icon = Pair.Sprite,
                    Name = Pair.NameLangKey,
                    OnReceive = () => OnReceive.Invoke(Pair.Type)
                });
            }
            if (!gameObject.activeSelf)
            {
                RefreshView(_queryForShow.Peek());
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
            if (_queryForShow.TryDequeue(out var result))
            {
                RefreshView(result);
                result.OnReceive.Invoke();
                _animator.SetTrigger("ShowNew");
                _sounds.Play(Services.Audio.Sounds.SoundType.InstrumentPicked);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        
        private void RefreshView(ShowQuery shown)
        {
            _instrumentIcon.sprite = shown.Icon;
            _instrumentName.SetNewKey(shown.Name);
        }
        
        private class ShowQuery
        {
            public Sprite Icon;
            public string Name;
            public System.Action OnReceive;
        }
    }
}