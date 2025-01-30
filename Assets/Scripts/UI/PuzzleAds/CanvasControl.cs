using UnityEngine;

namespace UI.Settings
{
    public class CanvasControl : MonoBehaviour
    {
        public void FinalizeHideWindow() => gameObject.SetActive(false);
        
        public void GoToGame() => Application.ExternalCall("GoToWorldOfPuzzles");
    }
}