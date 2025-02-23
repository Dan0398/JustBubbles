using UnityEngine;

namespace UI.Settings
{
    public class CanvasControl : MonoBehaviour
    {
        public void FinalizeHideWindow() => gameObject.SetActive(false);
        
        #pragma warning disable CS0618 
        public void GoToGame() => Application.ExternalCall("GoToWorldOfPuzzles");
        #pragma warning restore CS0618 
    }
}