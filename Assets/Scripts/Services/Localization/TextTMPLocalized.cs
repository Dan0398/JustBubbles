using UnityEngine;
using TMPro;

namespace BrakelessGames.Localization
{
    [RequireComponent(typeof(TMP_Text)), AddComponentMenu("UI/Localization/TextMeshPro Localized")]
    public class TextTMPLocalized: LocalizedBehaviour
    {
        private TMP_Text _myTextComponent;
        
        protected override void AfterStart()
        {
            _myTextComponent = GetComponent<TMP_Text>();
        }
        
        protected override void UpdateContent()
        {
            if (_myTextComponent == null)
            {
                _myTextComponent = GetComponent<TMP_Text>();
            }
			_myTextComponent.text = ResultLocalized.Replace(@"\n", System.Environment.NewLine);
        }
    }
}