using BrakelessGames.Localization;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Endless
{
    [System.Serializable]
    public class RequestByAds
    {
        [SerializeField] GameObject Turnable;
        [SerializeField] Animator Animator;
        [SerializeField] TextTMPLocalized Name, Description;
        [SerializeField] TMPro.TMP_Text BonusCount;
        [SerializeField] Image Icon;
        [SerializeField] Button TryActivateButton;
        
        public void TurnOn(Content.Instrument.Config.InstrumentView View, System.Action OnActivate)
        {
            var str = BrakelessGames.Localization.Controller.GetValueByKey("Instrument_Formatted");
            var name = BrakelessGames.Localization.Controller.GetValueByKey(View.NameLangKey);
            Name.SetTextNoTranslate(string.Format(str, name));
            Description.SetNewKey(View.DescriptionLangKey);
            Icon.sprite = View.Sprite;
            BonusCount.text = '+' + View.IncreaseCount.ToString();
            TryActivateButton.onClick.RemoveAllListeners();
            TryActivateButton.onClick.AddListener(OnActivate.Invoke);
            Turnable.SetActive(true);
        }

        internal void StartTurnOff() => Animator.SetTrigger("Hide");

        internal void TurnOff() => Turnable.SetActive(false);
    }
}