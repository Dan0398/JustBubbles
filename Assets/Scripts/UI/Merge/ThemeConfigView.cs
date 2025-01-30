using BrakelessGames.Localization;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Merge
{
    [System.Serializable]
    public class ThemeConfigView
    {
        [SerializeField] GameObject Parent;
        [SerializeField] Image Preview;
        [SerializeField] TextTMPLocalized NameLocalized;
        [SerializeField] Button Prev, Next;
        [SerializeField] Button Select;
        [SerializeField] GameObject UsualSelect, ForAdsSelect;
        bool adsAllowed;
        System.Action onClick;
        Content.Merge.Selector.ThemeSelector Selector;
        
        public void Init()
        {
            Next.onClick.AddListener(() =>
            {
                if (Selector == null) return;
                Selector.GoToNext();
                Refresh();
            });
            Prev.onClick.AddListener(()=>
            {
                if (Selector == null) return;
                Selector.GoToPrev();
                Refresh();
            });
            Select.onClick.AddListener( () =>
            {
                Selector.Select();
                onClick.Invoke();
            });
        }

        internal void Show(Content.Merge.Selector.ThemeSelector theme, bool AdsAllowed, System.Action OnSelect)
        {
            Selector = theme;
            adsAllowed = AdsAllowed;
            onClick = OnSelect;
            Refresh();
            Parent.SetActive(true);
        }

        void Refresh()
        {
            var Target = Selector.Selected;
            Preview.sprite = Target.Sprite;
            NameLocalized.SetNewKey(Target.NameLangKey);
            UsualSelect.SetActive(!(Target.AdsRequired && adsAllowed));
            ForAdsSelect.SetActive(Target.AdsRequired && adsAllowed);
        }
        
        public void Hide() => Parent.SetActive(false);
    }
}