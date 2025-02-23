using UnityEngine.EventSystems;
using UnityEngine;

namespace UI.Settings
{
    public class PointerHelp : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public System.Action PointerDown, PointerUp;

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp?.Invoke();
        }
    }
}