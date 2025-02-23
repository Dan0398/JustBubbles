using UnityEngine.UI;
using UnityEngine;

namespace Content
{
    public class Bubble: BaseView<Material>
    {
        [SerializeField] private RawImage _bubbleOnScene;
        
        protected override void ApplyAvailable()
        {
            if (_bubbleOnScene == null) return;
            _bubbleOnScene.material = SelectedContent.Value;
        }
        
        protected override void SaveSelected()
        {
            User.Data.SelectedBubbleID = ViewedID;
            User.SaveData();
        }

        protected override void SubscribeToUser()
        {
            SelectedID = User.Data.SelectedBubbleID;
            ViewedID = SelectedID;
            SelectedContent.Changed += ApplyAvailable;
        }
    }
}