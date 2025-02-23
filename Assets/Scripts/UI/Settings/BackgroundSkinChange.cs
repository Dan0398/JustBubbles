using UnityEngine.UI;
using UnityEngine;

namespace UI.Settings
{
    [System.Serializable]
    public class BackgroundSkinChange: IInitiable
    {
        [SerializeField] private Content.Background _window;
        [SerializeField] private Button _clickable;
        
        public void Init(Settings parent)
        {
            _clickable.onClick.RemoveAllListeners();
            var ParentCanvas = parent.GetComponent<Canvas>();
            _clickable.onClick.AddListener(() => 
            {
                ParentCanvas.enabled = false; 
                _window.GoToSelector(() => ParentCanvas.enabled = true);
            });
        }
    }
}