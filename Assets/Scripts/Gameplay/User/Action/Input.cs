using UnityEngine;

namespace Gameplay.User
{
    public partial class Action: BaseUser<Field.BubbleField>
    {
        protected override void ReactPointerMove()
        {
            SelectedInstrument?.ProcessAimVector(MouseWorldPos - (Vector2)transform.position);
        }
        protected override void BindInputs()
        {
            Inputs.BaseMap.Clicked.performed += (s) => ReactClickStart();
            Inputs.BaseMap.Clicked.canceled += (s) => ReactClickEnd();
            Inputs.BaseMap.SwitchBubbles.performed += (s) => ReactOnAdditional();
            
            void ReactClickStart()
            {
                SelectedInstrument?.ReactOnClickDown();
            }
            
            void ReactClickEnd()
            {
                SelectedInstrument?.ReactOnClickUp();
            }

            void ReactOnAdditional()
            {
                SelectedInstrument?.ReactOnAdditional();
            }
        }
    }
}