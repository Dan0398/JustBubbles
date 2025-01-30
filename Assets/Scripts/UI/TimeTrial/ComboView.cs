using BrakelessGames.Localization;
using UnityEngine;

namespace UI.Survival
{
    [System.Serializable]
    public class ComboView
    {
        [SerializeField] string ComboLangKey;
        [SerializeField] TextTMPLocalized ComboLabel;
        [SerializeField] TMPro.TMP_Text PointsMultiplierLabel;
        float oldPointsMultiplier;
        
        public void Refresh(int ComboCount, float PointsMultiplier)
        {
            ComboLabel.SetNewKeyFormatted(ComboLangKey, new string[]{ComboCount.ToString()});
            if (PointsMultiplier != oldPointsMultiplier)
            {
                oldPointsMultiplier = PointsMultiplier;
                PointsMultiplierLabel.text = oldPointsMultiplier.ToString();
            }
        }
    }
}