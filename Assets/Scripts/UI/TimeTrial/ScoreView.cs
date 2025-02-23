using UnityEngine;

namespace UI.Survival
{
    [System.Serializable]
    public class ScoreView
    {
        [SerializeField] TMPro.TMP_Text _scoreLabel;
        
        public void Refresh(int NewScore, int Delta)
        {
            _scoreLabel.text = NewScore.ToString();
        }
    }
}