using UnityEngine;

namespace Gameplay.User
{
    public abstract partial class BaseUser<TField> : MonoBehaviour, IPausableUser where TField:IField
    {
        [SerializeField] Camera Cam;
        
        public Ray GetScreenRayAtCursor() => Cam.ScreenPointToRay(MouseScreenPos);
        public Vector3 WorldToScreen(Vector3 source) => Cam.WorldToScreenPoint(source);
        protected Vector3 ScreenToWorld(Vector3 input) => Cam.ScreenToWorldPoint(input, Camera.MonoOrStereoscopicEye.Mono);
        
        protected virtual void Start()
        {
            ApplyInputs();
        }
        
        public abstract void StartGameplayAndAnimate(float Duration = 1f);
        
        public abstract void StopGameplayAndAnimate(float Duration = 1f, System.Action OnEnd = null);
    }
}