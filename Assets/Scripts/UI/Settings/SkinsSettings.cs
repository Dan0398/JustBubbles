using UnityEngine;

namespace UI.Settings
{
    [System.Serializable]
    public class SkinsSettings: IInitiable
    {
        [SerializeField] GameObject Blocker;
        [SerializeField] BubbleSkinChange BubbleSkin;
        [SerializeField] BackgroundSkinChange BackgroundSkin;
        
        public void Init(Settings Parent)
        {
            BubbleSkin.Init(Parent);
            BackgroundSkin.Init(Parent);
        }
        
        public void SetTurnedOnStatus(bool Activated)
        {
            Blocker.SetActive(!Activated);
        }
    }
}
