using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Settings
{
    public class PointerHelp : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public System.Action PointerDown, PointerUp;

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            PointerUp?.Invoke();
        }
    }
}