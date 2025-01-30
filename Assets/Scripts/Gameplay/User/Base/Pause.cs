using UnityEngine;

namespace Gameplay.User
{
    public abstract partial class BaseUser<TField> : MonoBehaviour, IPausableUser where TField:IField
    {
        protected bool Paused { get; private set; }
        
        public void Pause()
        {
            Paused = true;
            Inputs.Disable();
        }
        
        public void Unpause()
        {
            Paused = false;
            Inputs.Enable();
        }
    }
}
