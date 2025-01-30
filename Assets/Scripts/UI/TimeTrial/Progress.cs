using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace UI.Survival
{
    [System.Serializable]
    public class Progress
    {
        [SerializeField] Slider progressSlider;
        SurvivalCanvas parent;
        float lastRegisteredValue;
        Coroutine SliderAnimation;
        WaitForFixedUpdate Wait;
        
        public void Init(SurvivalCanvas Parent)
        {
            parent = Parent;
            progressSlider.value = 0;
        }
        
        public void RefreshProgress(float newValue)
        {
            lastRegisteredValue = newValue;
            if (SliderAnimation == null)
            {
                progressSlider.value = newValue;
            }
        }
        
        public void ResetToZero()
        {
            parent.StartCoroutine(AnimateSlider());
        }
        
        IEnumerator AnimateSlider()
        {
            float OldValue = lastRegisteredValue;
            lastRegisteredValue = 0;
            for (int i = 1; i <= 40; i++)
            {
                progressSlider.value = Mathf.Lerp(OldValue, lastRegisteredValue, Mathf.Sin(i/40f * Mathf.Deg2Rad * 90));
                yield return Wait;
            }
            SliderAnimation = null;
        }
    }
}