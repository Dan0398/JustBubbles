using System.Collections;
using UnityEngine;

namespace UI.Survival
{
    [System.Serializable]
    public class FallenBubbles
    {
        [SerializeField] private TMPro.TMP_Text _fallenText;
        private SurvivalCanvas _parent;
        private Coroutine _fallenAnimation;
        private WaitForFixedUpdate _wait;
        
        public void Init(SurvivalCanvas Parent)
        {
            _parent = Parent;
            _wait = new WaitForFixedUpdate();
        }
        
        public void ShowAnimated(int Count, int ScorePer, Vector3 MidPosOfAll, System.Func<Vector3, Vector3> ConvertSpaces)
        {
            _fallenText.text = $"{Count}x{ScorePer}\n={Count*ScorePer}";
            if (_fallenAnimation!= null)
            {
                _parent.StopCoroutine(_fallenAnimation);
            }
            _fallenAnimation = _parent.StartCoroutine(AnimateText(_fallenText, MidPosOfAll, ConvertSpaces));
        }
        
        private IEnumerator AnimateText(TMPro.TMP_Text Target, Vector3 StartPos, System.Func<Vector3, Vector3> ConvertSpaces)
        {
            StartPos += Vector3.down;
            RectTransform Rect = Target.rectTransform;
            for (int i = 1; i <= 40; i ++)
            {
                Rect.position = ConvertSpaces.Invoke(StartPos + Vector3.down * 0.15f * (i/40f));
                if (i<=5)
                {
                    Target.color = new Color(1,1,1, i/5f);
                }
                else if (i>=35)
                {
                    Target.color = new Color(1,1,1, 1 - (i-35)/5f);
                }
                yield return _wait;
            }
        }
    }
}