using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    [System.Serializable]
    public class BackgroundSkinChange: IInitiable
    {
        [SerializeField] Content.Background Window;
        [SerializeField] Button Clickable;
        
        public void Init(Settings parent)
        {
            Clickable.onClick.RemoveAllListeners();
            var ParentCanvas = parent.GetComponent<Canvas>();
            Clickable.onClick.AddListener(() => 
            {
                ParentCanvas.enabled = false; 
                Window.GoToSelector(() => ParentCanvas.enabled = true);
            });
        }
    }
}