using BrakelessGames.Localization;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Merge
{
    [System.Serializable]
    public class SizeConfigView
    {
        [SerializeField] private GameObject _parent;
        [SerializeField] private Image _preview;
        [SerializeField] private TextTMPLocalized _nameLocalized;
        [SerializeField] private Button _prev;
        [SerializeField] private Button _next;
        [SerializeField] private TextTMPLocalized _phoneBlockerLabel;
        [SerializeField] private Image _phoneBlockerView;
        [SerializeField] private Sprite _phoneBlocked;
        [SerializeField] private Sprite _phoneAvailable;
        [SerializeField] private Button _select;
        [SerializeField] private Image _select_View;
        [SerializeField] private TextTMPLocalized _selectLabel;
        private Content.Merge.Selector.SizeSelector _selector;
        private System.Action _onClick;
        private Services.Environment _env;
        private bool _selectAvailable;
        
        public void Init()
        {
            _next.onClick.AddListener(() =>
            {
                if (_selector == null) return;
                _selector.GoToNext();
                Refresh();
            });
            _prev.onClick.AddListener(()=>
            {
                if (_selector == null) return;
                _selector.GoToPrev();
                Refresh();
            });
            _select.onClick.AddListener( () =>
            {
                if (!_selectAvailable)
                {
                    Services.DI.Single<Services.Audio.Sounds.Service>().Play(Services.Audio.Sounds.SoundType.InstrumentFail);
                    return;
                }
                _selector.Select();
                _onClick.Invoke();
            });
            _env = Services.DI.Single<Services.Environment>();
        }
        
        public void Show(Content.Merge.Selector.SizeSelector selector, System.Action OnSelect)
        {
            _parent.SetActive(true);
            _selector = selector;
            _onClick = OnSelect;
            Refresh();
        }
        
        private void Refresh()
        {
            _preview.sprite = _selector.Selected.Preview;
            _nameLocalized.SetNewKey(_selector.Selected.NameLangKey);
            
            var OKForTouch = _selector.Selected.MobileAvailable;
            _phoneBlockerView.sprite = OKForTouch? _phoneAvailable : _phoneBlocked;
            _phoneBlockerLabel.SetNewKey(OKForTouch? "AvailabeForTouch" : "NotAvailabeForTouch");
            
            _selectAvailable = OKForTouch == _env.IsUsingTouch.Value || !_env.IsUsingTouch.Value;
            _selectLabel.SetNewKey(_selectAvailable? "Select": "Blocked");
            _select_View.color = _selectAvailable? Color.white : new Color(1, 0.8f, 0.8f, 1);
        }
        
        public void Hide() => _parent.SetActive(false);
    }
}