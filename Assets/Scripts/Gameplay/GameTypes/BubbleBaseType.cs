using System.Collections.Generic;
using Gameplay.Field;
using Gameplay.User;
using UI.Settings;

namespace Gameplay.GameType
{
    public abstract class BubbleBaseType : BaseType
    {
        protected Action User                   { get; private set; }
        protected Field.BubbleField Field       { get; private set; }
        protected virtual float UserDistance => 8f;
        
        protected BubbleBaseType(Gameplay.Controller Gameplay, Settings Settings, InGameParents InGameParts, BubbleField Field, Action User) 
        : base(Gameplay, User, Settings, InGameParts)
        {
            this.Field = Field;
            this.User = User;
            this.User.ChangeRayDistance(UserDistance);
            ProcessEnterToType();
        }
        
        public abstract void ReactOnUserBubbleSet(List<Field.Place> PopByUser, List<Field.Place> Fallen, System.Type InstrumentType);
        
        private void ProcessEnterToType()
        {
            Field.ReceiveConfig(new Field.BubbleField.Config()
            {
                ReactOnBubbleSet = ReactOnUserBubbleSet,
                IsFieldSizeDynamic = this.IsFieldAspectDynamic,
                MaxAspectRatio = MaxFieldAspect,
                RelativeOutstand = FieldUpperOutstand
            });
            if (!IsFieldAspectDynamic)
            {
                Field.SetAspect(MaxFieldAspect);
            }
            else 
            {
                Field.ResetAspect();
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