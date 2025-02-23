using System.Collections;
using UnityEngine;

namespace UI.Merge
{   
    public class Help : BaseAnimatedWindow
    {
        private Gameplay.GameType.Merge _gameType;
        
        public void RegisterPauser(Gameplay.GameType.Merge GameType)
        {
            _gameType = GameType;
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
            _gameType.ProcessPause();
            StartCoroutine(AnimateShow());
        } 
        
        private IEnumerator AnimateShow()
        {
            SetTurnOffStatus(false);
            StartCoroutine(AnimateFade(.5f));
            StartCoroutine(AnimateHeaderColor(.2f));
            yield return AnimateHeader(.7f);
            yield return new WaitForSecondsRealtime(.2f);
            yield return AnimateUnwrapWindow(.8f);
            SetTurnOffStatus(true);
        }
        
        public void Hide()
        {
            StartCoroutine(AnimateHide());
        }
        
        private IEnumerator AnimateHide()
        {
            SetTurnOffStatus(false);
            yield return AnimateUnwrapWindow(.8f, true);
            yield return new WaitForSecondsRealtime(.2f);
            StartCoroutine(AnimateHeaderColor(.7f, true));
            StartCoroutine(AnimateFade(.7f, true));
            yield return AnimateHeader(.7f, true);
            _gameType.ProcessUnpause();
            gameObject.SetActive(false);
        }
    }
}