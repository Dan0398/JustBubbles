using BrakelessGames.Localization;
using UnityEngine;

namespace UI.Survival
{
    [System.Serializable]
    public class ComboView
    {
        [SerializeField] private string _comboLangKey;
        [SerializeField] private TextTMPLocalized _comboLabel;
        [SerializeField] private TMPro.TMP_Text _pointsMultiplierLabel;
        private float _oldPointsMultiplier;
        
        public void Refresh(int ComboCount, float PointsMultiplier)
        {
            _comboLabel.SetNewKeyFormatted(_comboLangKey, new string[]{ComboCount.ToString()});
            if (PointsMultiplier != _oldPointsMultiplier)
            {
                _oldPointsMultiplier = PointsMultiplier;
                _pointsMultiplierLabel.text = _oldPointsMultiplier.ToString();
            }
        }
    }
}