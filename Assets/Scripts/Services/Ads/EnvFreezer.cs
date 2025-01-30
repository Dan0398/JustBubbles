using UnityEngine;

namespace Services.Advertisements 
{
    public class EnvFreezer : Services.IService
    {
        CursorLockMode OldLockMode;
        bool OldCursorVisible;
        Services.Audio.Music Music;
        /*
        Services.Pause PauseSystem;
        Pause.PauseType OldPauseType;
        */
        
        
        public EnvFreezer()
        {
            //GetPauseAsync();
        }
        
            /*
        async void GetPauseAsync()
        {
            PauseSystem = Services.DI.Single<Services.Pause>();
            while (PauseSystem == null)
            {
                if (await Utilities.IsWaitEndsFailure()) return;
                PauseSystem = Services.DI.Single<Services.Pause>();
            }
        }
            */
        
        public void RememberAndFreezeEnvironment()
        {
            /*
            OldPauseType = PauseSystem.CurrentPauseType;
            PauseSystem.CallHardPause();
            */
            if (Music == null)
            {
                Music = Services.DI.Single<Services.Audio.Music>();
            }
            if (Music != null) Music.UserPaused = true;
            OldLockMode = Cursor.lockState;
            OldCursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } 
        
        public void RestoreEnvironment()
        {
            Cursor.lockState = OldLockMode;
            Cursor.visible = OldCursorVisible;
            if (Music != null) Music.UserPaused = false;
            /*
            if (OldPauseType == Pause.PauseType.None) PauseSystem.ReleaseFromPause();
            if (OldPauseType == Pause.PauseType.Soft) PauseSystem.CallInShopPause();
            */
        }
    }
}