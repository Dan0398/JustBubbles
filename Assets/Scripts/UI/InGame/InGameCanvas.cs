using UnityEngine.UI;
using UnityEngine;
using Content.Instrument;

namespace UI.InGame
{
    public class InGameCanvas : MonoBehaviour
    {
        [SerializeField] Icon[] icons;
        [SerializeField] GameObject HorizontalParent, VerticalParent;
        [SerializeField] AspectRatioFitter VerticalAspect;
        System.Action OnInstrumentCountUnbind;
        
        void Start()
        {
            ActivateViewsFromConfig();
        }
        
        public async void ActivateViewsFromConfig()
        {
            var service = Services.DI.Single<Content.Instrument.Service>();
            while(service.Config == null) if (await Utilities.IsWaitEndsFailure()) return;
            foreach(var icon in icons) icon.ApplyConfig(service.Config);
        }
        
        public void BindWithCounts(Gameplay.Instruments.Counts counts)
        {
            OnInstrumentCountUnbind?.Invoke();
            OnInstrumentCountUnbind = null;
            if (counts == null) return;
            foreach(var icon in icons)
            {
                var NewUnbindLogic = icon.BindWithCount(counts.GetPair(icon.Type));;
                if (NewUnbindLogic == null) continue;
                OnInstrumentCountUnbind += NewUnbindLogic;
            }
        }
        
        public void ReactOnFieldResize(float Aspect)
        {
            bool isHorizontal = Aspect >= 1;
            foreach(var icon in icons) icon.ApplyFieldStat(isHorizontal);
            VerticalAspect.aspectRatio = Aspect;
            if (isHorizontal)
            {
                HorizontalParent.SetActive(true);
                var ForRebuild =  HorizontalParent.GetComponentInChildren<HorizontalLayoutGroup>().GetComponent<RectTransform>();
                LayoutRebuilder.ForceRebuildLayoutImmediate(ForRebuild);
                VerticalParent.SetActive(false);
            }
            else
            {
                HorizontalParent.SetActive(false);
                VerticalParent.SetActive(true);
            }
        }

        internal void AnimateFailUseInstrument(WorkType type)
        {
            foreach(var icon in icons)
            {
                if (icon.Type != type) continue;
                icon.AnimateFailActivate(this, 0.5f);
                Services.DI.Single<Services.Audio.Sounds.Service>().Play(Services.Audio.Sounds.SoundType.InstrumentFail);
                return;
            }
        }
        
        public void Hide() => GetComponent<Animator>().SetTrigger("Hide");
        
        public void FinalizeHide() => gameObject.SetActive(false);
    }
}