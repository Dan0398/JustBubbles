using System.Collections;
using UnityEngine;

namespace UI.Survival
{
    [System.Serializable]
    public class FallenBubbles
    {
        [SerializeField] TMPro.TMP_Text FallenText;
        SurvivalCanvas parent;
        Coroutine FallenAnimation;
        WaitForFixedUpdate Wait;
        
        public void Init(SurvivalCanvas Parent)
        {
            parent = Parent;
            Wait = new WaitForFixedUpdate();
        }
        
        public void ShowAnimated(int Count, int ScorePer, Vector3 MidPosOfAll, System.Func<Vector3, Vector3> ConvertSpaces)
        {
            FallenText.text = $"{Count}x{ScorePer}\n={Count*ScorePer}";
            if (FallenAnimation!= null)
            {
                parent.StopCoroutine(FallenAnimation);
            }
            FallenAnimation = parent.StartCoroutine(AnimateText(FallenText, MidPosOfAll, ConvertSpaces));
        }
        
        IEnumerator AnimateText(TMPro.TMP_Text Target, Vector3 StartPos, System.Func<Vector3, Vector3> ConvertSpaces)
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
                yield return Wait;
            }
        }
    }
}