using UnityEngine;
using TMPro;

namespace BrakelessGames.Localization
{
    [RequireComponent(typeof(TMP_Text)), AddComponentMenu("UI/Localization/TextMeshPro Localized")]
    public class TextTMPLocalized: LocalizedBehaviour
    {
        TMP_Text MyTextComponent;
        
        protected override void AfterStart()
        {
            MyTextComponent = GetComponent<TMP_Text>();
        }
        
        protected override void UpdateContent()
        {
            if (MyTextComponent == null)
            {
                MyTextComponent = GetComponent<TMP_Text>();
            }
			MyTextComponent.text = ResultLocalized.Replace(@"\n", System.Environment.NewLine);
        }
    }
}