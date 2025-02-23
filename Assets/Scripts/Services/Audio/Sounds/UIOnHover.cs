using UnityEngine.EventSystems;
using UnityEngine;

namespace Services.Audio.Sounds
{
    [AddComponentMenu("Help/Sounds On UI Hover")]
    public class UIOnHover : MonoBehaviour, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Play Hover Sound");
        }
    }
}