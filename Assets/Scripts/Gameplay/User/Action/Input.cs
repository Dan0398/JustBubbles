using UnityEngine;

namespace Gameplay.User
{
    public partial class Action: BaseUser<Field.BubbleField>
    {
        protected override void ReactPointerMove()
        {
            if (_selectedInstrument != null) _selectedInstrument.ProcessAimVector(MouseWorldPos - (Vector2)transform.position);
        }
        
        protected override void BindInputs()
        {
            Inputs.BaseMap.Clicked.performed += (s) => ReactClickStart();
            Inputs.BaseMap.Clicked.canceled += (s) => ReactClickEnd();
            Inputs.BaseMap.SwitchBubbles.performed += (s) => ReactOnAdditional();
            
            void ReactClickStart()
            {
                if (_selectedInstrument != null) _selectedInstrument.ReactOnClickDown();
            }
            
            void ReactClickEnd()
            {
                if (_selectedInstrument != null) _selectedInstrument.ReactOnClickUp();
            }

            void ReactOnAdditional()
            {
                if (_selectedInstrument != null) _selectedInstrument.ReactOnAdditional();
            }
        }
    }
}