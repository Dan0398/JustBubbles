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
        protected override string Settings_ExitLangKey => "Save&Menu";
        MergeField Field;
        User.MergeUser User;
        UI.Merge.MergeCanvas Canvas;
        
        int SlotNumber;
        bool allWrapped, wasInGame;
        
        Data.MergeController UserData;
        Content.Merge.Service Content;
        
        Content.Merge.SizesList.Size SelectedSize;
        Content.Merge.ThemesList.Theme SelectedTheme;
        SaveModel SaveSlot;
        
        public Content.Merge.ThemesList Views => Content.ThemesConfig;

        public Merge(Gameplay.Controller Gameplay, Settings Settings, InGameParents InGameParts, MergeField field, User.MergeUser user, UI.Merge.MergeCanvas canvas) 
        : base(Gameplay, user, Settings, InGameParts)
        {
            Field = field;
            User = user;
            Canvas = canvas;
            UserData = Services.DI.Single<Data.MergeController>();
            Content = Services.DI.Single<Content.Merge.Service>();
            SelectedTheme = null;
            Gameplay.StartCoroutine(Start());
            allWrapped = false;
        }

        IEnumerator Start()
        {
            while(!UserData.isDataLoaded) yield return new WaitForSeconds(0.5f);
            if (!UserData.Data.Tried)
            {
                UserData.Data.SaveSlots[0] = null;
                LoadSlot(0);
                UserData.Data.Tried = true;
                UserData.SaveData();
                yield break;
            }
            Canvas.ShowSlotSelector(this, UserData.Data);
        }
        
        public void DeleteSlot(int ID, UI.Merge.SlotViewOnScene Shown)
        {
            Canvas.RequestDeleteSlot(ID, ApplyDelete);
            
            void ApplyDelete()
            {
                UserData.Data.SaveSlots[ID] = null;
                UserData.SaveData();
                Shown.RefreshSource(UserData.Data.SaveSlots[ID] , ID, this);
            }
        }

        public void LoadSlot(int number)
        {
            SlotNumber = number;
            SaveSlot = UserData.Data.SaveSlots[SlotNumber];
            if (SaveSlot == null)
            {
                SelectedTheme = Content.ThemesConfig.Themes[0];
                SelectedSize = Content.SizesConfig.Orientations[0];
                SaveSlot = new SaveModel(Barrier.SizeType.Slim, SelectedTheme.BundlePath);
                UserData.Data.SaveSlots[SlotNumber] = SaveSlot;
                UserData.SaveData();
            }
            else
            {
                SelectedSize = Content.SizesConfig.Orientations.FirstOrDefault(h => h.Data == SaveSlot.FieldSize);
                SelectedTheme = Content.ThemesConfig.Themes.FirstOrDefault(h => h.BundlePath == SaveSlot.BundlePath);
            }
            WaitLoadAndRun();
        }
        
        void WaitLoadAndRun()
        {
            Content.Processor.LoadTheme(SelectedTheme, StartAnimated);
            
            void StartAnimated()
            {
                Field.ShowAnimated(SelectedTheme.Loaded, SaveSlot, 1.5f);
                Field.GameOverRelative.Changed += TryDetectGameOver;
                User.StartGameplayAndAnimate(1.5f);
                Canvas.ShowIngame(SaveSlot, this, Field.GameOverRelative, SelectedSize.MinimalAspect, 1.5f);
                allWrapped = false;
                wasInGame = true;
            }
        }

        public void ConfigureSlot(int number)
        {
            Content.Merge.Selector.Request request = null;
            request = new Content.Merge.Selector.Request(number, Content.SizesConfig, Content.ThemesConfig, ApplyRequest);
            Canvas.ShowConfigurator(request, SubloadTheme);
            
            void ApplyRequest()
            {
                SlotNumber = request.slotID;
                SaveSlot = new SaveModel(request.selectedOrientation, request.selectedTheme);
                UserData.Data.SaveSlots[SlotNumber] = SaveSlot;
                UserData.SaveData();
                SelectedTheme = Content.ThemesConfig.Themes.FirstOrDefault(h => h.BundlePath == SaveSlot.BundlePath);
                SelectedSize = Content.SizesConfig.Orientations.FirstOrDefault(h => h.Data == SaveSlot.FieldSize);
                WaitLoadAndRun();
            }
            
            void SubloadTheme()
            {
                Content.Processor.LoadTheme(Content.ThemesConfig.Themes.FirstOrDefault(h => h.BundlePath == request.selectedTheme), null);
            }
        }
        
        void TryDetectGameOver()
        {
            if (Field.GameOverRelative.Value < 1) return;
            ProcessPause();
            Canvas.Hide(1f, false);
            Field.Deactivate(1f);
            Field.RequireSave = false;
            User.StopGameplayAndAnimate(1f);
            allWrapped = true;
            Canvas.EndgameCanvas.Show(SaveSlot, Retry, Exit);
            Field.GameOverRelative.Changed -= TryDetectGameOver;
            
            async void Retry()
            {
                UserData.Data.SaveSlots[SlotNumber] = null;
                UserData.SaveData();
                await Services.DI.Single<Services.Advertisements.Controller>().ShowInterstitial();
                LoadSlot(SlotNumber);
            }
            
            void Exit()
            {
                UserData.Data.SaveSlots[SlotNumber] = null;
                UserData.SaveData();
                gameplay.StopGameplay();
            }
        }
        
        public void SaveSelectedSlot()
        {
            Field.SyncUnits(SaveSlot);
            UserData.Data.SaveSlots[SlotNumber] = SaveSlot;
            UserData.SaveData();
            Field.RequireSave = false;
        }

        public override void ProcessGameplayUpdate()
        {
            /*if (Save)
            {
                Save = false;
                IsSaveSlotSuccess();
            }*/
        }

        internal void CloseByUser() => gameplay.StopGameplay();

        public override async Task Dispose()
        {
            ProcessPause();
            if (allWrapped) return;
            bool UserInAnimate = true;
            User.StopGameplayAndAnimate(1f, () => UserInAnimate = false);
            
            bool CanvasInAnimate = true;
            System.Action CanvasDeactivate = () => CanvasInAnimate = false;
            if (Field.RequireSave)
            {
                SaveSelectedSlot();
                Canvas.ShowSaveAndTurnOff(1f, 3f, CanvasDeactivate);
            }
            else
            {
                Canvas.Hide(1f, true, CanvasDeactivate);
            }
            
            Field.GameOverRelative.Changed -= TryDetectGameOver;
            Field.Deactivate(1f);
            
            if (wasInGame) await Services.DI.Single<Services.Advertisements.Controller>().ShowInterstitial();
            while (UserInAnimate || CanvasInAnimate) await Utilities.Wait();
        }
        
        public System.Action IsBuyBombSuccess(int bombCost)
        {
            if (SaveSlot.Money.Value < bombCost) return null;
            SaveSlot.Money.Value -= bombCost;
            return User.UseBomb;
        }

        public System.Action IsBuyShakerSuccess(int shakerCost)
        {
            if (SaveSlot.Money.Value < shakerCost) return null;
            SaveSlot.Money.Value -= shakerCost;
            return ProcessShaker;
            
            void ProcessShaker()
            {
                User.FastTurnOff();
                Field.RunShaker(User.FastTurnOn);
            }
        }

        protected override void ReactOn(InGameParents InGameParts)
        {
            InGameParts.Bubble.SetActive(false);
            InGameParts.Merge.SetActive(true);
        }
    }
}