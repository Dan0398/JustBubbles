using Gameplay.Instruments;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.InGame
{
    [System.Serializable]
    public class Icon
    {
        [SerializeField] private string _name;
        [field:SerializeField] public Content.Instrument.WorkType Type { get; private set; }
        [SerializeField] private Image _shown;
        [SerializeField] private AspectRatioFitter _target;
        [SerializeField] private TMPro.TMP_Text _useCount;
        [SerializeField] private InstrumentPlace _horizontal;
        [SerializeField] private InstrumentPlace _vertical;
        private WaitForFixedUpdate _wait;
        private bool _isUseHorizontal;
        
        public void ApplyFieldStat(bool isHorizontal)
        {
            if (_isUseHorizontal == isHorizontal) return;
            _isUseHorizontal = isHorizontal;
            if (isHorizontal)
            {
                _horizontal.Apply(_target);
            }
            else 
            {
                _vertical.Apply(_target);
            }
        }
        
        public void ApplyConfig(Content.Instrument.Config config)
        {
            foreach(var item in config.Instruments)
            {
                if (item.Type != Type) continue;
                _shown.sprite = item.Sprite;
                return;
            }
        }

        public System.Action BindWithCount(Counts.Pair pair)
        {
            if (pair == null) return null; 
            else if (pair.Count == null)
            {
                Debug.Log("Pair count is null");
            }
            System.Action Refresh = () => 
            {
                _useCount.text = pair.Count.Value.ToString();
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
        
        private IEnumerator MoveIconHorizontal(float Duration = 1f)
        {
            var Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            var Rect = _shown.rectTransform;
            for (int i = 1; i <= Steps; i++)
            {
                float Shift = Mathf.Sin(i/(float)Steps * 1440 * Mathf.Deg2Rad) * 0.1f;
                Rect.anchorMin = Vector2.right * Shift;
                Rect.anchorMax = new Vector2(1 + Shift, 1);
                yield return _wait;
            }
            
        }
        
        private IEnumerator AnimateIconColor(float Duration = 1f)
        {
            var Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            var StartInd = Steps / 4;
            var EndInd = Steps * 3 / 4;
            for (int i = 1; i <= Steps; i++)
            {
                        if (i <= StartInd)
                {
                    _shown.color = Color.Lerp(Color.white, Color.red, i / (float)StartInd);
                }
                else    if (i >= EndInd)
                {
                    _shown.color = Color.Lerp(Color.white, Color.red, 1 - i / (float)EndInd);
                }
                yield return _wait;
            }
        }
    }
}