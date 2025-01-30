using UnityEngine;

namespace Content
{
    public class Background : BaseView<GameObject>
    {
        protected override void ApplyAvailable()
        {
            SelectedContent.Value.SetActive(true);
        }

        protected override void SaveSelected()
        {
            User.Data.SelectedBackgroundID = ViewedID;
            User.SaveData();
        }
        
        protected override void ReactOnDeselect(int id)
        {
            AvailableContent[id].gameObject.SetActive(false);
        }

        protected override void SubscribeToUser()
        {
            SelectedID = User.Data.SelectedBackgroundID;
            ViewedID = SelectedID;
            SelectedContent.Changed += ApplyAvailable;
        }
    }
}