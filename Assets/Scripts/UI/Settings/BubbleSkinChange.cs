using UnityEngine.UI;
using UnityEngine;

namespace UI.Settings
{
    [System.Serializable]
    public class BubbleSkinChange: IInitiable
    {
        [SerializeField] private Content.Bubble _window;
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