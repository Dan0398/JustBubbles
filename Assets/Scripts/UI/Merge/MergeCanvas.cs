using System.Collections;
using Utils.Observables;
using Gameplay.Merge;
using UnityEngine;
using System;

namespace UI.Merge
{    
    public class MergeCanvas : MonoBehaviour
    {
        [field:SerializeField] public Endgame EndgameCanvas { get; private set; }
        [SerializeField] private Header _header;
        [SerializeField] private MergeMainWindow _mergeWindow;
        [SerializeField] private Shop _inGameShop;
        [SerializeField] private Help _helpWindow;
        [SerializeField] private DeleteModal _delete;
        [SerializeField] private SizeSeeker _canvasSize;
        [SerializeField] private GameObject _saveOverlay;
        [SerializeField] private Interstitial _interstitial;

        public void ShowSlotSelector(Gameplay.GameType.Merge merge, Data.Merge data)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            if (_header.Shown) _header.HideAnimated(1f);
            _canvasSize.Deactivate();
            _mergeWindow.ShowSlotSelector(merge, data);
            _interstitial.Hide();
        }
        
        public void ShowIngame(SaveModel selectedSaveSlot, Gameplay.GameType.Merge merge, ObsFloat GameOver, float MinimalAspect, float Duration)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            
            if (_mergeWindow.Shown) _mergeWindow.Hide(Duration);
            _inGameShop.Bind(selectedSaveSlot, merge);
            _helpWindow.RegisterPauser(merge);
            _canvasSize.Activate(merge, MinimalAspect);
            _header.BindAndShowAnimated(selectedSaveSlot, merge, GameOver, Duration);
            _interstitial.Show(merge);
        }
        
        public void ShowConfigurator(Content.Merge.Selector.Request request, System.Action subloadTheme)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            if (_header.Shown) _header.HideAnimated(1f);
            _canvasSize.Deactivate();
            _mergeWindow.ShowConfigurator(request, subloadTheme);
            _interstitial.Hide();
        }
        
        public void ShowSaveAndTurnOff(float Duration, float SaveDuration, System.Action OnEnd)
        {
            if (_header.Shown) _header.HideAnimated(Duration);
            if (_mergeWindow.Shown) _mergeWindow.Hide(Duration);
            _interstitial.Hide();
            _canvasSize.Deactivate();
            StartCoroutine(ShowSave());
            
            IEnumerator ShowSave()
            {
                _saveOverlay.gameObject.SetActive(true);
                yield return new WaitForSeconds(SaveDuration);
                _saveOverlay.gameObject.SetActive(false);
                OnEnd?.Invoke();
                gameObject.SetActive(false);
            }
        }
        
        public void Hide(float Duration = 1f, bool RequireTurnOff = true, System.Action OnEnd = null)
        {
            if (RequireTurnOff && !_header.Shown && !_mergeWindow.Shown)
            {
                gameObject.SetActive(false);
                OnEnd?.Invoke();
                return;
            }
            if (_header.Shown) _header.HideAnimated(Duration);
            if (_mergeWindow.Shown) _mergeWindow.Hide(Duration);
            _interstitial.Hide();
            _canvasSize.Deactivate();
            if (RequireTurnOff) StartCoroutine(TurnOffDelayed(Duration, OnEnd));
        }
        
        private IEnumerator TurnOffDelayed(float Duration, System.Action OnEnd = null)
        {
            yield return new WaitForSeconds(Duration);
            OnEnd?.Invoke();
            gameObject.SetActive(false);
        }

        public void RequestDeleteSlot(int iD, Action applyDelete)
        {
            _delete.ShowWindow(iD, applyDelete);
        }
    }
}