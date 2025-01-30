using System.Collections.Generic;
using Gameplay.Field;
using Gameplay.User;
using UI.Settings;

namespace Gameplay.GameType
{
    public abstract class BubbleBaseType : BaseType
    {
        protected Action user                   { get; private set; }
        protected Field.BubbleField field       { get; private set; }
        protected virtual float UserDistance => 8f;
        
        protected BubbleBaseType(Gameplay.Controller Gameplay, Settings Settings, InGameParents InGameParts, BubbleField Field, Action User) 
        : base(Gameplay, User, Settings, InGameParts)
        {
            field = Field;
            user = User;
            user.ChangeRayDistance(UserDistance);
            ProcessEnterToType();
        }
        
        public abstract void ReactOnUserBubbleSet(List<Field.Place> PopByUser, List<Field.Place> Fallen, System.Type InstrumentType);
        
        void ProcessEnterToType()
        {
            field.ReceiveConfig(new Field.BubbleField.Config()
            {
                ReactOnBubbleSet = ReactOnUserBubbleSet,
                IsFieldSizeDynamic = this.IsFieldAspectDynamic,
                MaxAspectRatio = MaxFieldAspect,
                RelativeOutstand = FieldUpperOutstand
            });
            if (!IsFieldAspectDynamic)
            {
                field.SetAspect(MaxFieldAspect);
            }
            else 
            {
                field.ResetAspect();
            }
            settings.Skins.SetTurnedOnStatus(SkinChangeAvailable);
        }
        
        protected override void ReactOn(InGameParents InGameParts)
        {
            InGameParts.Bubble.SetActive(true);
            InGameParts.Merge.SetActive(false);
        }
    }
}