using System.Collections;
using Utils.Observables;
using Gameplay.Merge;
using UnityEngine;
using System;

namespace UI.Merge
{    
    public class MergeCanvas : MonoBehaviour
    {
        [SerializeField] Header Header;
        [SerializeField] MergeMainWindow MergeWindow;
        [SerializeField] Shop InGameShop;
        [SerializeField] Help HelpWindow;
        [SerializeField] DeleteModal Delete;
        [SerializeField] SizeSeeker CanvasSize;
        [SerializeField] GameObject SaveOverlay;
        [SerializeField] Interstitial Interstitial;
        [field:SerializeField] public Endgame EndgameCanvas { get; private set; }

        internal void ShowSlotSelector(Gameplay.GameType.Merge merge, Data.Merge data)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            if (Header.Shown) Header.HideAnimated(1f);
            CanvasSize.Deactivate();
            MergeWindow.ShowSlotSelector(merge, data);
            Interstitial.Hide();
        }
        
        public void ShowIngame(SaveModel selectedSaveSlot, Gameplay.GameType.Merge merge, ObsFloat GameOver, float MinimalAspect, float Duration)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            
            if (MergeWindow.Shown) MergeWindow.Hide(Duration);
            InGameShop.Bind(selectedSaveSlot, merge);
            HelpWindow.RegisterPauser(merge);
            CanvasSize.Activate(merge, MinimalAspect);
            Header.BindAndShowAnimated(selectedSaveSlot, merge, GameOver, Duration);
            Interstitial.Show(merge);
        }
        
        public void ShowConfigurator(Content.Merge.Selector.Request request, System.Action subloadTheme)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            if (Header.Shown) Header.HideAnimated(1f);
            CanvasSize.Deactivate();
            MergeWindow.ShowConfigurator(request, subloadTheme);
            Interstitial.Hide();
        }
        
        public void ShowSaveAndTurnOff(float Duration, float SaveDuration, System.Action OnEnd)
        {
            if (Header.Shown) Header.HideAnimated(Duration);
            if (MergeWindow.Shown) MergeWindow.Hide(Duration);
            Interstitial.Hide();
            CanvasSize.Deactivate();
            StartCoroutine(ShowSave());
            
            IEnumerator ShowSave()
            {
                SaveOverlay.gameObject.SetActive(true);
                yield return new WaitForSeconds(SaveDuration);
                SaveOverlay.gameObject.SetActive(false);
                OnEnd?.Invoke();
                gameObject.SetActive(false);
            }
        }
        
        public void Hide(float Duration = 1f, bool RequireTurnOff = true, System.Action OnEnd = null)
        {
            if (RequireTurnOff && !Header.Shown && !MergeWindow.Shown)
            {
                gameObject.SetActive(false);
                OnEnd?.Invoke();
                return;
            }
            if (Header.Shown) Header.HideAnimated(Duration);
            if (MergeWindow.Shown) MergeWindow.Hide(Duration);
            Interstitial.Hide();
            CanvasSize.Deactivate();
            if (RequireTurnOff) StartCoroutine(TurnOffDelayed(Duration, OnEnd));
            //SlotSelector.SetActive(false);
        }
        
        IEnumerator TurnOffDelayed(float Duration, System.Action OnEnd = null)
        {
            yield return new WaitForSeconds(Duration);
            OnEnd?.Invoke();
            gameObject.SetActive(false);
        }

        internal void RequestDeleteSlot(int iD, Action applyDelete)
        {
            Delete.ShowWindow(iD, applyDelete);
        }
    }
}