using BrakelessGames.Localization;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Merge
{
    [System.Serializable]
    public class ThemeConfigView
    {
        [SerializeField] private GameObject _parent;
        [SerializeField] private Image _preview;
        [SerializeField] private TextTMPLocalized _nameLocalized;
        [SerializeField] private Button _prev;
        [SerializeField] private Button _next;
        [SerializeField] private Button _select;
        [SerializeField] private GameObject _usualSelect;
        [SerializeField] private GameObject _forAdsSelect;
        private bool _adsAllowed;
        private System.Action _onClick;
        private Content.Merge.Selector.ThemeSelector _selector;
        
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
                _selector.Select();
                _onClick.Invoke();
            });
        }

        public void Show(Content.Merge.Selector.ThemeSelector theme, bool AdsAllowed, System.Action OnSelect)
        {
            _selector = theme;
            _adsAllowed = AdsAllowed;
            _onClick = OnSelect;
            Refresh();
            _parent.SetActive(true);
        }

        private void Refresh()
        {
            var Target = _selector.Selected;
            _preview.sprite = Target.Sprite;
            _nameLocalized.SetNewKey(Target.NameLangKey);
            _usualSelect.SetActive(!(Target.AdsRequired && _adsAllowed));
            _forAdsSelect.SetActive(Target.AdsRequired && _adsAllowed);
        }
        
        public void Hide()
        {
            _parent.SetActive(false);
        }
    }
}