using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

namespace Services.Audio.Sounds
{
    [AddComponentMenu("Help/Sounds On UI Click")]
    public class UIOnClick : BaseRequest, IPointerDownHandler
    {
        [SerializeField] UnityEvent AdditionalMoves;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            TryPlaySound(SoundType.ButtonClick);
            AdditionalMoves?.Invoke();
        }
    }
}