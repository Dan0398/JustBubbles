using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Gameplay.User
{
    public abstract partial class BaseUser<TField> : MonoBehaviour, IPausableUser where TField:IField
    {
        [SerializeField] EventSystem Events;
        [SerializeField] UnityEngine.UI.GraphicRaycaster[] CanvasesForIgnore;
        List<RaycastResult> CanvasResults;
        PointerEventData m_PointerEventData;
        
        public bool IsClickedInGameField()
        {
            if (Paused) return false;
            if (!MouseInsideField) return false;
            if (IsClickedToCanvas()) return false;
            return true;
            
            bool IsClickedToCanvas()
            {
                foreach(var Canvas in CanvasesForIgnore)
                {
                    if (!Canvas.enabled) continue;
                    if (!Canvas.gameObject.activeInHierarchy) continue;
                    if (isClickedToCanvas(Canvas)) return true;
                }
                return false;
            }
            
            bool isClickedToCanvas(UnityEngine.UI.GraphicRaycaster Target)
            {
                m_PointerEventData = new PointerEventData(Events)
                {
                    position = MouseScreenPos
                };
                CanvasResults ??= new List<RaycastResult>();
                CanvasResults.Clear();
                Target.Raycast(m_PointerEventData, CanvasResults);
                return CanvasResults.Count > 0;
            }
        }
    }
}
