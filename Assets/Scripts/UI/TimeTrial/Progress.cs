using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace UI.Survival
{
    [System.Serializable]
    public class Progress
    {
        [SerializeField] private Slider _progressSlider;
        private SurvivalCanvas parent;
        private float lastRegisteredValue;
        private Coroutine SliderAnimation;
        private WaitForFixedUpdate Wait;
        
        public void Init(SurvivalCanvas Parent)
        {
            parent = Parent;
            _progressSlider.value = 0;
        }
        
        public void RefreshProgress(float newValue)
        {
            lastRegisteredValue = newValue;
            if (SliderAnimation == null)
            {
                _progressSlider.value = newValue;
            }
        }
        
        public void ResetToZero()
        {
            parent.StartCoroutine(AnimateSlider());
        }
        
        private IEnumerator AnimateSlider()
        {
            float OldValue = lastRegisteredValue;
            lastRegisteredValue = 0;
            for (int i = 1; i <= 40; i++)
            {
                _progressSlider.value = Mathf.Lerp(OldValue, lastRegisteredValue, Mathf.Sin(i/40f * Mathf.Deg2Rad * 90));
                yield return Wait;
            }
            SliderAnimation = null;
        }
    }
}