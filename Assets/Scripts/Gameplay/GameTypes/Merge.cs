#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif
using System.Collections;
using Gameplay.Merge;
using System.Linq;
using UnityEngine;
using UI.Settings;

namespace Gameplay.GameType
{
    public class Merge : BaseType
    {
        private MergeField _field;
        private User.MergeUser _user;
        private UI.Merge.MergeCanvas _canvas;
        
        private int _slotNumber;
        private bool _allWrapped, _wasInGame;
        
        private Data.MergeController _userData;
        private Content.Merge.Service _content;
        
        private Content.Merge.SizesList.Size _selectedSize;
        private Content.Merge.ThemesList.Theme _selectedTheme;
        private SaveModel _saveSlot;
        
        protected override string Settings_ExitLangKey => "Save&Menu";
        
        public Content.Merge.ThemesList Views => _content.ThemesConfig;

        public Merge(Gameplay.Controller Gameplay, Settings Settings, InGameParents InGameParts, MergeField field, User.MergeUser user, UI.Merge.MergeCanvas canvas) 
        : base(Gameplay, user, Settings, InGameParts)
        {
            _field = field;
            _user = user;
            _canvas = canvas;
            _userData = Services.DI.Single<Data.MergeController>();
            _content = Services.DI.Single<Content.Merge.Service>();
            _selectedTheme = null;
            Gameplay.StartCoroutine(Start());
            _allWrapped = false;
        }

        private IEnumerator Start()
        {
            while(!_userData.isDataLoaded) yield return new WaitForSeconds(0.5f);
            if (!_userData.Data.Tried)
            {
                _userData.Data.SaveSlots[0] = null;
                LoadSlot(0);
                _userData.Data.Tried = true;
                _userData.SaveData();
                yield break;
            }
            _canvas.ShowSlotSelector(this, _userData.Data);
        }
        
        public void DeleteSlot(int ID, UI.Merge.SlotViewOnScene Shown)
        {
            _canvas.RequestDeleteSlot(ID, ApplyDelete);
            
            void ApplyDelete()
            {
                _userData.Data.SaveSlots[ID] = null;
                _userData.SaveData();
                Shown.RefreshSource(_userData.Data.SaveSlots[ID] , ID, this);
            }
        }

        public void LoadSlot(int number)
        {
            _slotNumber = number;
            _saveSlot = _userData.Data.SaveSlots[_slotNumber];
            if (_saveSlot == null)
            {
                _selectedTheme = _content.ThemesConfig.Themes[0];
                _selectedSize = _content.SizesConfig.Orientations[0];
                _saveSlot = new SaveModel(SizeType.Slim, _selectedTheme.BundlePath);
                _userData.Data.SaveSlots[_slotNumber] = _saveSlot;
                _userData.SaveData();
            }
            else
            {
                _selectedSize = _content.SizesConfig.Orientations.FirstOrDefault(h => h.Data == _saveSlot.FieldSize);
                _selectedTheme = _content.ThemesConfig.Themes.FirstOrDefault(h => h.BundlePath == _saveSlot.BundlePath);
            }
            WaitLoadAndRun();
        }
        
        private void WaitLoadAndRun()
        {
            _content.Processor.LoadTheme(_selectedTheme, StartAnimated);
            
            void StartAnimated()
            {
                _field.ShowAnimated(_selectedTheme.Loaded, _saveSlot, 1.5f);
                _field.GameOverRelative.Changed += TryDetectGameOver;
                _user.StartGameplayAndAnimate(1.5f);
                _canvas.ShowIngame(_saveSlot, this, _field.GameOverRelative, _selectedSize.MinimalAspect, 1.5f);
                _allWrapped = false;
                _wasInGame = true;
            }
        }

        public void ConfigureSlot(int number)
        {
            Content.Merge.Selector.Request request = null;
            request = new Content.Merge.Selector.Request(number, _content.SizesConfig, _content.ThemesConfig, ApplyRequest);
            _canvas.ShowConfigurator(request, SubloadTheme);
            
            void ApplyRequest()
            {
                _slotNumber = request.SlotID;
                _saveSlot = new SaveModel(request.SelectedOrientation, request.SelectedTheme);
                _userData.Data.SaveSlots[_slotNumber] = _saveSlot;
                _userData.SaveData();
                _selectedTheme = _content.ThemesConfig.Themes.FirstOrDefault(h => h.BundlePath == _saveSlot.BundlePath);
                _selectedSize = _content.SizesConfig.Orientations.FirstOrDefault(h => h.Data == _saveSlot.FieldSize);
                WaitLoadAndRun();
            }
            
            void SubloadTheme()
            {
                _content.Processor.LoadTheme(_content.ThemesConfig.Themes.FirstOrDefault(h => h.BundlePath == request.SelectedTheme), null);
            }
        }
        
        private void TryDetectGameOver()
        {
            if (_field.GameOverRelative.Value < 1) return;
            ProcessPause();
            _canvas.Hide(1f, false);
            _field.Deactivate(1f);
            _field.RequireSave = false;
            _user.StopGameplayAndAnimate(1f);
            _allWrapped = true;
            _canvas.EndgameCanvas.Show(_saveSlot, Retry, Exit);
            _field.GameOverRelative.Changed -= TryDetectGameOver;
            
            async void Retry()
            {
                _userData.Data.SaveSlots[_slotNumber] = null;
                _userData.SaveData();
                await Services.DI.Single<Services.Advertisements.Controller>().ShowInterstitial();
                LoadSlot(_slotNumber);
            }
            
            void Exit()
            {
                _userData.Data.SaveSlots[_slotNumber] = null;
                _userData.SaveData();
                gameplay.StopGameplay();
            }
        }
        
        public void SaveSelectedSlot()
        {
            _field.SyncUnits(_saveSlot);
            _userData.Data.SaveSlots[_slotNumber] = _saveSlot;
            _userData.SaveData();
            _field.RequireSave = false;
        }

        public override void ProcessGameplayUpdate() { }

        public void CloseByUser()
        {
            gameplay.StopGameplay();
        }

        public override async Task Dispose()
        {
            ProcessPause();
            if (_allWrapped) return;
            bool UserInAnimate = true;
            _user.StopGameplayAndAnimate(1f, () => UserInAnimate = false);
            
            bool CanvasInAnimate = true;
            System.Action CanvasDeactivate = () => CanvasInAnimate = false;
            if (_field.RequireSave)
            {
                SaveSelectedSlot();
                _canvas.ShowSaveAndTurnOff(1f, 3f, CanvasDeactivate);
            }
            else
            {
                _canvas.Hide(1f, true, CanvasDeactivate);
            }
            
            _field.GameOverRelative.Changed -= TryDetectGameOver;
            _field.Deactivate(1f);
            
            if (_wasInGame) await Services.DI.Single<Services.Advertisements.Controller>().ShowInterstitial();
            while (UserInAnimate || CanvasInAnimate) await Utilities.Wait();
        }
        
        public System.Action IsBuyBombSuccess(int bombCost)
        {
            if (_saveSlot.Money.Value < bombCost) return null;
            _saveSlot.Money.Value -= bombCost;
            return _user.UseBomb;
        }

        public System.Action IsBuyShakerSuccess(int shakerCost)
        {
            if (_saveSlot.Money.Value < shakerCost) return null;
            _saveSlot.Money.Value -= shakerCost;
            return ProcessShaker;
            
            void ProcessShaker()
            {
                _user.FastTurnOff();
                _field.RunShaker(_user.FastTurnOn);
            }
        }

        protected override void ReactOn(InGameParents InGameParts)
        {
            InGameParts.Bubble.SetActive(false);
            InGameParts.Merge.SetActive(true);
        }
    }
}