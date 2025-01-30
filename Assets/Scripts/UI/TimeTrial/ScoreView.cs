using UnityEngine;

namespace UI.Survival
{
    [System.Serializable]
    public class ScoreView
    {
        [SerializeField] TMPro.TMP_Text ScoreLabel;
        /*
        [SerializeField] TMPro.TMP_Text DeltaScoreLabel;
        [SerializeField] Animation Anim;
        */
        
        public void Refresh(int NewScore, int Delta)
        {
            ScoreLabel.text = NewScore.ToString();
            /*
            DeltaScoreLabel.text = "+" + Delta;
            if (Delta > 0)
            {
                Anim.Play();
            }
            else 
            {
                DeltaScoreLabel.color = new Color(1,1,1,0);
                ScoreLabel.rectTransform.localScale = Vector3.one;
            }
            */
        }
    }
}