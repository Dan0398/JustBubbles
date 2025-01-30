using BrakelessGames.Localization;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Merge
{
    [System.Serializable]
    public class SizeConfigView
    {
        [SerializeField] GameObject Parent;
        [SerializeField] Image Preview;
        [SerializeField] TextTMPLocalized NameLocalized;
        [SerializeField] Button Prev, Next;
        [SerializeField] TextTMPLocalized PhoneBlockerLabel;
        [SerializeField] Image PhoneBlockerView;
        [SerializeField] Sprite PhoneBlocked, PhoneAvailable;
        [SerializeField] Button Select;
        [SerializeField] Image Select_View;
        [SerializeField] TextTMPLocalized SelectLabel;
        Content.Merge.Selector.SizeSelector Selector;
        System.Action onClick;
        Services.Environment Env;
        bool selectAvailable;
        
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
                if (!selectAvailable)
                {
                    Services.DI.Single<Services.Audio.Sounds.Service>().Play(Services.Audio.Sounds.SoundType.InstrumentFail);
                    return;
                }
                Selector.Select();
                onClick.Invoke();
            });
            Env = Services.DI.Single<Services.Environment>();
        }
        
        public void Show(Content.Merge.Selector.SizeSelector selector, System.Action OnSelect)
        {
            Parent.SetActive(true);
            Selector = selector;
            onClick = OnSelect;
            Refresh();
        }
        
        void Refresh()
        {
            Preview.sprite = Selector.Selected.Preview;
            NameLocalized.SetNewKey(Selector.Selected.NameLangKey);
            
            var OKForTouch = Selector.Selected.MobileAvailable;
            PhoneBlockerView.sprite = OKForTouch? PhoneAvailable : PhoneBlocked;
            PhoneBlockerLabel.SetNewKey(OKForTouch? "AvailabeForTouch" : "NotAvailabeForTouch");
            
            selectAvailable = OKForTouch == Env.IsUsingTouch.Value || !Env.IsUsingTouch.Value;
            SelectLabel.SetNewKey(selectAvailable? "Select": "Blocked");
            Select_View.color = selectAvailable? Color.white : new Color(1, 0.8f, 0.8f, 1);
        }
        
        public void Hide() => Parent.SetActive(false);
    }
}