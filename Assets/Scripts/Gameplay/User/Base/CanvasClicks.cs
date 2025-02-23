using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Gameplay.User
{
    public abstract partial class BaseUser<TField> : MonoBehaviour, IPausableUser where TField:IField
    {
        [SerializeField] private EventSystem _events;
        [SerializeField] private UnityEngine.UI.GraphicRaycaster[] _canvasesForIgnore;
        private List<RaycastResult> _canvasResults;
        private PointerEventData _pointerEventData;
        
        public bool IsClickedInGameField()
        {
            if (Paused) return false;
            if (!MouseInsideField) return false;
            if (IsClickedToCanvas()) return false;
            return true;
            
            bool IsClickedToCanvas()
            {
                foreach(var Canvas in _canvasesForIgnore)
                {
                    if (!Canvas.enabled) continue;
                    if (!Canvas.gameObject.activeInHierarchy) continue;
                    if (isClickedToCanvas(Canvas)) return true;
                }
                return false;
            }
            
            bool isClickedToCanvas(UnityEngine.UI.GraphicRaycaster Target)
            {
                _pointerEventData = new PointerEventData(_events)
                {
                    position = MouseScreenPos
                };
                _canvasResults ??= new List<RaycastResult>();
                _canvasResults.Clear();
                Target.Raycast(_pointerEventData, _canvasResults);
                return _canvasResults.Count > 0;
            }
        }
    }
}
