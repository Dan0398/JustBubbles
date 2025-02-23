using BrakelessGames.Localization;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Merge
{
    public class SlotViewOnScene : MonoBehaviour
    {
        [SerializeField] private GameObject _emptyParent;
        [SerializeField] private GameObject _dataParent;
        [SerializeField] private Image _themePreview;
        [SerializeField] private TextTMPLocalized _fieldSizeLabel;
        [SerializeField] private TextTMPLocalized _themeNameLabel;
        [SerializeField] private TextTMPLocalized _scoreLabel;
        [SerializeField] private TextTMPLocalized _moneyLabel;
        [SerializeField] private GameObject _phoneBlocker;
        private Content.Merge.ThemesList.Theme _actualView;
        private Gameplay.Merge.SaveModel _data;
        private int _number;
        private bool _envTouchSubscribed, _langSubscribed, _usingTouch;
        private Gameplay.GameType.Merge _parent;
        private Coroutine _phoneBlockerRoutine;
        private WaitForFixedUpdate _wait;
        
        private bool PhoneBlocked => _usingTouch && _data.FieldSize != Gameplay.Merge.SizeType.Slim;
        
        public void RefreshSource(Gameplay.Merge.SaveModel Data, int ID, Gameplay.GameType.Merge Parent)
        {
            _data = Data;
            _number = ID;
            _parent = Parent;
            TrySubscribeTouch();
            TrySubscribeLangs();
            RefreshViewItem(_parent.Views);
            RefreshView();
        }

        private void TrySubscribeTouch()
        {
            if (_envTouchSubscribed) return;
            var Env = Services.DI.Single<Services.Environment>();
            System.Action Refresh = () => 
            {
                _usingTouch = Env.IsUsingTouch.Value;
                RefreshView();
            };
            Refresh.Invoke();
            Env.IsUsingTouch.Changed += Refresh;
            _envTouchSubscribed = true;
        }
        
        private void TrySubscribeLangs()
        {
            if (_langSubscribed) return;
            BrakelessGames.Localization.Controller.OnLanguageChange += RefreshView;
            _langSubscribed = true;
        }

        private void RefreshViewItem(Content.Merge.ThemesList Views)
        {
            _actualView = null;
            if (_data == null) return;
            foreach(var item in Views.Themes)
            {
                if (_data.BundlePath == item.BundlePath)
                {
                    _actualView = item;
                    return;
                }
            }
        }
        
        private void RefreshView()
        {
            _emptyParent.SetActive(_data == null);
            _dataParent.SetActive(_data != null);
            if (_data != null && _actualView != null)
            {
                _phoneBlocker.SetActive(PhoneBlocked);
                _themePreview.sprite = _actualView.Sprite;
                var SizeLocalized = BrakelessGames.Localization.Controller.GetValueByKey(System.Enum.GetName(typeof(Gameplay.Merge.SizeType), _data.FieldSize));
                _fieldSizeLabel.SetNewKeyFormatted("Merge_SizeName_Formatted", new string[] { SizeLocalized });
                
                var ThemeLocalized = BrakelessGames.Localization.Controller.GetValueByKey(_actualView.NameLangKey);
                _themeNameLabel.SetNewKeyFormatted("Merge_ThemeName_Formatted", new string[] { ThemeLocalized });
                _scoreLabel.SetNewKeyFormatted("Score_Formatted", new string[] { _data.Points.Value.ToString() });
                _moneyLabel.SetNewKeyFormatted("Money_Formatted", new string[] { _data.Money.Value.ToString()});
            }
        }     

        public void DeleteSlot()
        {
            _parent.DeleteSlot(_number, this);
        }
        
        public void StartNewGameFast()
        {
            _parent.LoadSlot(_number); 
        }
        
        public void ConfigureAndStartGame()
        {
            _parent.ConfigureSlot(_number);
        }
        
        public void SelectSlot()
        {
            if (PhoneBlocked)
            {
                if (_phoneBlockerRoutine != null) StopCoroutine(_phoneBlockerRoutine);
                _phoneBlockerRoutine = StartCoroutine(AnimatePhoneBlocker());
                Services.DI.Single<Services.Audio.Sounds.Service>().Play(Services.Audio.Sounds.SoundType.InstrumentFail);
                return;
            }
            _parent.LoadSlot(_number);
        } 
        
        private IEnumerator AnimatePhoneBlocker()
        {
            _wait ??= new();
            var Target = _phoneBlocker.GetComponent<RectTransform>();
            var offsetMin = Target.offsetMin;
            var offsetMax = Target.offsetMax;
            for (int i = 0; i <= 25; i++)
            {
                var Lerp = Mathf.Sin(i/25f * 1440 * Mathf.Deg2Rad) * 10 * Vector2.right;
                Target.offsetMin = offsetMin + Lerp;
                Target.offsetMax = offsetMax + Lerp;
                yield return _wait;
            }
        }
    }
}