using UnityEngine;

namespace UI.Settings
{
    [System.Serializable]
    public class SkinsSettings: IInitiable
    {
        [SerializeField] private GameObject _blocker;
        [SerializeField] private BubbleSkinChange _bubbleSkin;
        [SerializeField] private BackgroundSkinChange _backgroundSkin;
        
        public void Init(Settings Parent)
        {
            _bubbleSkin.Init(Parent);
            _backgroundSkin.Init(Parent);
        }
        
        public void SetTurnedOnStatus(bool Activated)
        {
            _blocker.SetActive(!Activated);
        }
    }
}
