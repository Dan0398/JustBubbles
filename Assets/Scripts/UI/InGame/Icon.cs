using Gameplay.Instruments;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace UI.InGame
{
    [System.Serializable]
    public class Icon
    {
        [SerializeField] string Name;
        [field:SerializeField] public Content.Instrument.WorkType Type { get; private set; }
        [SerializeField] Image Shown;
        [SerializeField] AspectRatioFitter target;
        [SerializeField] TMPro.TMP_Text UseCount;
        [SerializeField] InstrumentPlace horizontal, vertical;
        WaitForFixedUpdate Wait;
        bool isUseHorizontal;
        
        public void ApplyFieldStat(bool isHorizontal)
        {
            if (isUseHorizontal == isHorizontal) return;
            isUseHorizontal = isHorizontal;
            if (isHorizontal)
            {
                horizontal.Apply(target);
            }
            else 
            {
                vertical.Apply(target);
            }
        }
        
        public void ApplyConfig(Content.Instrument.Config config)
        {
            foreach(var item in config.Instruments)
            {
                if (item.Type != Type) continue;
                Shown.sprite = item.Sprite;
                return;
            }
        }

        internal System.Action BindWithCount(Counts.Pair pair)
        {
            if (pair == null) return null; 
            else if (pair.Count == null)
            {
                Debug.Log("Pair count is null");
            }
            System.Action Refresh = () => 
            {
                UseCount.text = pair.Count.Value.ToString();
            };
            Refresh.Invoke();
            pair.Count.Changed += Refresh;
            return () => pair.Count.Changed -= Refresh;
        }
        
        public void AnimateFailActivate(MonoBehaviour Coroutines, float Duration = 1)
        {
            Coroutines.StartCoroutine(MoveIconHorizontal(Duration));
            Coroutines.StartCoroutine(AnimateIconColor(Duration));
        }
        
        IEnumerator MoveIconHorizontal(float Duration = 1f)
        {
            var Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            var Rect = Shown.rectTransform;
            for (int i = 1; i <= Steps; i++)
            {
                float Shift = Mathf.Sin(i/(float)Steps * 1440 * Mathf.Deg2Rad) * 0.1f;
                Rect.anchorMin = Vector2.right * Shift;
                Rect.anchorMax = new Vector2(1 + Shift, 1);
                yield return Wait;
            }
            
        }
        
        IEnumerator AnimateIconColor(float Duration = 1f)
        {
            var Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            var StartInd = Steps / 4;
            var EndInd = Steps * 3 / 4;
            for (int i = 1; i <= Steps; i++)
            {
                        if (i <= StartInd)
                {
                    Shown.color = Color.Lerp(Color.white, Color.red, i / (float)StartInd);
                }
                else    if (i >= EndInd)
                {
                    Shown.color = Color.Lerp(Color.white, Color.red, 1 - i / (float)EndInd);
                }
                yield return Wait;
            }
        }
    }
}