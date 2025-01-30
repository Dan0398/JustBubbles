#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif
using System.Collections.Generic;
using Gameplay.Field;
using Gameplay.User;
using UnityEngine;

namespace Gameplay.GameType
{
    [System.Serializable]
    public class Idle : BaseType
    {
        [SerializeField] UI.Menu.MainMenu MenuCanvas;

        protected override bool GoToMenuAvailable => false;
        protected override bool IsFieldAspectDynamic => true;

        public Idle(Gameplay.Controller Gameplay, UI.Settings.Settings Settings, InGameParents InGameParts, UI.Menu.MainMenu MenuCanvas) 
                : base(Gameplay, null, Settings, InGameParts)
        {
            this.MenuCanvas = MenuCanvas;
            CustomEnterToType();
        }

        void CustomEnterToType()
        {
            MenuCanvas.OnCallPause = CallSettings;
            MenuCanvas.Show();
        }

        public override void ProcessGameplayUpdate() { }
        
        public override Task Dispose()
        {
            return Task.CompletedTask;
        }

        protected override void ReactOn(InGameParents InGameParts)
        {
            InGameParts.Bubble.SetActive(false);
            InGameParts.Merge.SetActive(false);
        }
    }
}