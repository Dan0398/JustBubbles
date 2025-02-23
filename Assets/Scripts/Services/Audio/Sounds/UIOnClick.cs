using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

namespace Services.Audio.Sounds
{
    [AddComponentMenu("Help/Sounds On UI Click")]
    public class UIOnClick : BaseRequest, IPointerDownHandler
    {
        [SerializeField] private UnityEvent _additionalMoves;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            TryPlaySound(SoundType.ButtonClick);
            _additionalMoves?.Invoke();
        }
    }
}