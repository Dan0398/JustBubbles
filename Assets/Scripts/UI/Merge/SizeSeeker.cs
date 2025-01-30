using UnityEngine;

namespace UI.Merge
{
    public class SizeSeeker : MonoBehaviour
    {
        const int MaxStep = 20;
        [SerializeField] GameObject Turnable;
        float minimalAspect;
        Gameplay.GameType.Merge pauser;
        int step;
        int oldWidth, oldHeight;
        
        public void Activate(Gameplay.GameType.Merge merge, float MinimalAspect)
        {
            pauser = merge;
            enabled = true;
            minimalAspect = MinimalAspect;
        }
        
        public void Deactivate()
        {
            pauser = null;
            enabled = false;
        }
        
        void Update()
        {
            step ++;
            if (step != MaxStep) return;
            step = 0;
            if (Screen.width == oldWidth && Screen.height == oldHeight) return;
            oldWidth = Screen.width;
            oldHeight = Screen.height;
            float Aspect = oldWidth / (float) oldHeight;
            bool turnedOn = Aspect < minimalAspect;
            if (Turnable.activeSelf != turnedOn)
            {
                Turnable.SetActive(turnedOn);
                if (turnedOn) pauser.ProcessPause();
                else pauser.ProcessUnpause();
            }
        }
    }
}