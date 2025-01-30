using UnityEngine;

namespace Gameplay.User
{
    public abstract partial class BaseUser<TField> : MonoBehaviour, IPausableUser where TField:IField
    {
        [SerializeField] protected TField Field;
        public bool MouseInsideField        { get; private set; }
        protected Controls Inputs           { get; private set; }
        protected Vector2 MouseScreenPos    { get; private set; }
        protected Vector2 MouseWorldPos     { get; private set; }
        
        protected void ApplyInputs()
        {
            Inputs = new Controls();
            Inputs.BaseMap.ClickPosition.performed += (s) => ApplyPointerPos(s.ReadValue<Vector2>());
            BindInputs();
        }
        
        protected void ApplyPointerPos(Vector2 NewPos)
        {
            if (MouseScreenPos == NewPos) return;
            MouseScreenPos = NewPos;
            MouseWorldPos = ScreenToWorld(MouseScreenPos);
            MouseInsideField = Field.IsPositionInsideField(MouseWorldPos);
            ReactPointerMove();
        }
        
        protected virtual void ReactPointerMove() {}
        
        protected abstract void BindInputs();
    }
}