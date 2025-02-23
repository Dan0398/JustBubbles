using BrakelessGames.Localization;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Endless
{
    [System.Serializable]
    public class RequestByAds
    {
        [SerializeField] private GameObject _turnable;
        [SerializeField] private Animator _animator;
        [SerializeField] private TextTMPLocalized _name;
        [SerializeField] private TextTMPLocalized _description;
        [SerializeField] private TMPro.TMP_Text _bonusCount;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _tryActivateButton;
        
        public void TurnOn(Content.Instrument.Config.InstrumentView View, System.Action OnActivate)
        {
            var str = BrakelessGames.Localization.Controller.GetValueByKey("Instrument_Formatted");
            var name = BrakelessGames.Localization.Controller.GetValueByKey(View.NameLangKey);
            _name.SetTextNoTranslate(string.Format(str, name));
            _description.SetNewKey(View.DescriptionLangKey);
            _icon.sprite = View.Sprite;
            _bonusCount.text = '+' + View.IncreaseCount.ToString();
            _tryActivateButton.onClick.RemoveAllListeners();
            _tryActivateButton.onClick.AddListener(OnActivate.Invoke);
            _turnable.SetActive(true);
        }

        public void StartTurnOff()
        {
            _animator.SetTrigger("Hide");
        }

        public void TurnOff()
        {
            _turnable.SetActive(false);
        }
    }
}