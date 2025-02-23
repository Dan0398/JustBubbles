using UnityEngine.UI;
using UnityEngine;
using Content.Instrument;

namespace UI.InGame
{
    public class InGameCanvas : MonoBehaviour
    {
        [SerializeField] private Icon[] _icons;
        [SerializeField] private GameObject _horizontalParent;
        [SerializeField] private GameObject _verticalParent;
        [SerializeField] private AspectRatioFitter _verticalAspect;
        private System.Action _onInstrumentCountUnbind;
        
        private void Start()
        {
            ActivateViewsFromConfig();
        }
        
        public async void ActivateViewsFromConfig()
        {
            var service = Services.DI.Single<Content.Instrument.Service>();
            while(service.Config == null) if (await Utilities.IsWaitEndsFailure()) return;
            foreach(var icon in _icons) icon.ApplyConfig(service.Config);
        }
        
        public void BindWithCounts(Gameplay.Instruments.Counts counts)
        {
            _onInstrumentCountUnbind?.Invoke();
            _onInstrumentCountUnbind = null;
            if (counts == null) return;
            foreach(var icon in _icons)
            {
                var NewUnbindLogic = icon.BindWithCount(counts.GetPair(icon.Type));;
                if (NewUnbindLogic == null) continue;
                _onInstrumentCountUnbind += NewUnbindLogic;
            }
        }
        
        public void ReactOnFieldResize(float Aspect)
        {
            bool isHorizontal = Aspect >= 1;
            foreach(var icon in _icons) icon.ApplyFieldStat(isHorizontal);
            _verticalAspect.aspectRatio = Aspect;
            if (isHorizontal)
            {
                _horizontalParent.SetActive(true);
                var ForRebuild =  _horizontalParent.GetComponentInChildren<HorizontalLayoutGroup>().GetComponent<RectTransform>();
                LayoutRebuilder.ForceRebuildLayoutImmediate(ForRebuild);
                _verticalParent.SetActive(false);
            }
            else
            {
                _horizontalParent.SetActive(false);
                _verticalParent.SetActive(true);
            }
        }

        public void AnimateFailUseInstrument(WorkType type)
        {
            foreach(var icon in _icons)
            {
                if (icon.Type != type) continue;
                icon.AnimateFailActivate(this, 0.5f);
                Services.DI.Single<Services.Audio.Sounds.Service>().Play(Services.Audio.Sounds.SoundType.InstrumentFail);
                return;
            }
        }
        
        public void Hide()
        {
            GetComponent<Animator>().SetTrigger("Hide");
        }
        
        public void FinalizeHide()
        {
            gameObject.SetActive(false);
        }
    }
}