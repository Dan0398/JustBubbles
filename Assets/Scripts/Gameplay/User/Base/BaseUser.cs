using UnityEngine;

namespace Gameplay.User
{
    public abstract partial class BaseUser<TField> : MonoBehaviour, IPausableUser where TField:IField
    {
        [SerializeField] private Camera _camera;
        
        public Ray GetScreenRayAtCursor()
        {
            return _camera.ScreenPointToRay(MouseScreenPos);
        }
        
        public Vector3 WorldToScreen(Vector3 source)
        {
            return _camera.WorldToScreenPoint(source);
        }
        
        protected Vector3 ScreenToWorld(Vector3 input)
        {
            return _camera.ScreenToWorldPoint(input, Camera.MonoOrStereoscopicEye.Mono);
        }
        
        protected virtual void Start()
        {
            ApplyInputs();
        }
        
        public abstract void StartGameplayAndAnimate(float Duration = 1f);
        
        public abstract void StopGameplayAndAnimate(float Duration = 1f, System.Action OnEnd = null);
    }
}