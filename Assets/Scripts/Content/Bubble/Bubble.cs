using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Content
{
    public class Bubble: BaseView<Material>
    {
        [SerializeField] RawImage BubbleOnScene;
        
        protected override void ApplyAvailable()
        {
            if (BubbleOnScene == null) return;
            BubbleOnScene.material = SelectedContent.Value;
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