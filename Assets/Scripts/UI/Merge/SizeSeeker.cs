using UnityEngine;

namespace UI.Merge
{
    public class SizeSeeker : MonoBehaviour
    {
        private const int MaxStep = 20;
        [SerializeField] private GameObject _turnable;
        private float _minimalAspect;
        private Gameplay.GameType.Merge _pauser;
        private int _step;
        private int _oldWidth, _oldHeight;
        
        public void Activate(Gameplay.GameType.Merge merge, float MinimalAspect)
        {
            _pauser = merge;
            enabled = true;
            _minimalAspect = MinimalAspect;
        }
        
        public void Deactivate()
        {
            _pauser = null;
            enabled = false;
        }
        
        private void Update()
        {
            _step ++;
            if (_step != MaxStep) return;
            _step = 0;
            if (Screen.width == _oldWidth && Screen.height == _oldHeight) return;
            _oldWidth = Screen.width;
            _oldHeight = Screen.height;
            float Aspect = _oldWidth / (float) _oldHeight;
            bool turnedOn = Aspect < _minimalAspect;
            if (_turnable.activeSelf != turnedOn)
            {
                _turnable.SetActive(turnedOn);
                if (turnedOn) _pauser.ProcessPause();
                else _pauser.ProcessUnpause();
            }
        }
    }
}