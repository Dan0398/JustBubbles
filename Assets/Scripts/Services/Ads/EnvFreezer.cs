using UnityEngine;

namespace Services.Advertisements 
{
    public class EnvFreezer : Services.IService
    {
        private CursorLockMode _oldLockMode;
        private bool _oldCursorVisible;
        private Services.Audio.Music _music;
        
        public EnvFreezer() { }
        
        public void RememberAndFreezeEnvironment()
        {
            if (_music == null)
            {
                _music = Services.DI.Single<Services.Audio.Music>();
            }
            if (_music != null) _music.UserPaused = true;
            _oldLockMode = Cursor.lockState;
            _oldCursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } 
        
        public void RestoreEnvironment()
        {
            Cursor.lockState = _oldLockMode;
            Cursor.visible = _oldCursorVisible;
            if (_music != null) _music.UserPaused = false;
        }
    }
}