#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif
using System.Collections.Generic;
using Gameplay.Instruments;
using Content.Instrument;
using Gameplay.Field;
using Gameplay.User;
using UnityEngine;

namespace Gameplay.GameType
{
    [System.Serializable]
    public class Survival : BubbleBaseType
    {
        const string LeaderBoardName = "SurvivalBestScore";
        const int RewardForFallenBubble = 50;
        const int RewardForPoppedBubble = 20;
        
        bool FirstBubbleSet;
        float Distance;
        int Lives,Points,Combo;
        float pointsMultiplier;
        int currentGameStageID;
        SurvivalStage[] GameStages;
        UI.Survival.SurvivalCanvas ViewCanvas;
        Data.UserController UserData;
        Config instrumentsConfig;
        Counts InstrumentsCount;

        SurvivalStage CurrentStage => GameStages[currentGameStageID];

        protected override bool SkinChangeAvailable => false;
        protected override bool IsFieldAspectDynamic => false;
        #if UNITY_WEBGL
        protected override float MaxFieldAspect => 0.63f;
        #else 
        protected override float MaxFieldAspect => 9/16f;
        #endif

        public Survival(Gameplay.Controller Gameplay, UI.Settings.Settings Settings, InGameParents InGameParts, BubbleField Field, Action User, UI.Survival.SurvivalCanvas Canvas) 
                : base(Gameplay, Settings, InGameParts, Field, User) 
        {
            GameStages = new SurvivalStage[] 
            {
                new SurvivalStage(3, 60, 6, 0.1f, 0.25f),
                new SurvivalStage(4, 180, 5, 0.12f, 0.2f),
                new SurvivalStage(5, 240, 4, 0.1f, 0.15f),
                new SurvivalStage(5, 300, 4, 0.1f, 0.16f),
                new SurvivalStage(6, 600, 3, 0.1f, 0.12f),
                new SurvivalStage(6, 600, 3, 0.1f, 0.15f),
                new SurvivalStage(6, 600, 3, 0.1f, 0.15f),
                new SurvivalStage(6, 600, 3, 0.1f, 0.2f),
                new SurvivalStage(6, 600, 3, 0.1f, 0.3f),
                new SurvivalStage(6, float.MaxValue, 2, 0.1f, 0.3f),
            };
            ViewCanvas = Canvas;
            UserData = Services.DI.Single<Data.UserController>();
            CustomEnterToType();
        }

        void CustomEnterToType()
        {
            field.Difficulty = 0.9f;
            StartNewGame();
            field.ShowViews();
            
            user.StartGameplayAndAnimate(1f);
        }
        
        void StartNewGame()
        {
            Lives = 1;
            
            currentGameStageID = 0;
            field.SetColorConfig(CurrentStage.SceneColors, false);
            
            ViewCanvas.Show(this, 1f);
            ViewCanvas.SwitchFunctionalButtons(true);
            ViewCanvas.RefreshGameStageData(CurrentStage);
            ViewCanvas.ReceiveNewGameStage(CurrentStage);
            
            ViewCanvas.Show(this, 1f);
            
            Points = 0;
            ViewCanvas.Score.Refresh(Points, 0);
            
            Combo = 0;
            pointsMultiplier = 1;
            ViewCanvas.Combo.Refresh(Combo, pointsMultiplier);
            
            FirstBubbleSet = false;
            field.AppendLinesAndAnimate(6, 1f, ProcessUnpause);
            GetAndProcessInstruments();
        }
        
        async void GetAndProcessInstruments()
        {
            if (instrumentsConfig == null)
            {
                var Service = Services.DI.Single<Content.Instrument.Service>();
                while (Service.Config == null) if (await Utilities.IsWaitEndsFailure()) return;
                instrumentsConfig = Service.Config;
            }
            
            InstrumentsCount = new Instruments.Counts(instrumentsConfig);
            user.ActivateInstruments(InstrumentsCount);
        }
        
        public override void ProcessGameplayUpdate() 
        {
            if (Paused) return;
            if (!FirstBubbleSet) return;
            Distance = field.GetRelativeDistanceToFieldEdge();
            if (Distance < 0)
            {
                GameOver();
                return;
            }
            var WeightedSpeed = ViewCanvas.MoveDynamic.Evaluate(Distance);
            CurrentStage.RegisterTime(Time.fixedDeltaTime * WeightedSpeed);
            ViewCanvas.RefreshGameStageData(CurrentStage);
            field.ShiftLinesDown(WeightedSpeed * CurrentStage.Speed);
            if (field.IsPlaceOnTopEmpty)
            {
                field.AppendOneLine();
            }
            if (CurrentStage.RequireChangeStage && currentGameStageID < GameStages.Length - 1)
            {
                currentGameStageID++;
                CurrentStage.RegisterTime(GameStages[currentGameStageID-1].TimeOutOfDuration);
                field.SetColorConfig(CurrentStage.SceneColors, false);
                ViewCanvas.ReceiveNewGameStage(CurrentStage);
            }
        }
        
        async void GameOver()
        {
            ProcessPause();
            
            await Utilities.Wait(500);
            
            var bestResult = UserData.Data.SurvivalBestScore.Value;
            var IsRecord = Points > bestResult;
            var MaxPoints = IsRecord? Points : bestResult;
            
            ViewCanvas.SwitchFunctionalButtons(false);
            ViewCanvas.GameOver.OnGiveUp = Clean;
            ViewCanvas.GameOver.ProcessEnd(new UI.Strategy.Result()
            {
                SessionResult = Points,
                BestResult = MaxPoints,
                IsNewHighScore = IsRecord,
                LeaderBoardName = Survival.LeaderBoardName,
                OnRetry = ProcessRetry,
                OnEnd = ProcessGoToMenu
            }, 
            Lives, Revive);
            
            void Clean()
            {
                if (IsRecord) UserData.Data.SurvivalBestScore.Value = Mathf.Max(UserData.Data.SurvivalBestScore.Value, Points);
                ViewCanvas.Hide(1f, false);
                field.FullCleanupAnimated(1f);
#if UNITY_WEBGL
                if (IsRecord)
                {
                    Services.Web.Catcher.SetNewScoreInLeaderboards(new Services.Leaderboards.ScoreWrite(LeaderBoardName, MaxPoints));
                }
#endif
            }
            
            void Revive()
            {
                FirstBubbleSet = false;
                Lives--;
                field.FullCleanupAnimated(0.5f, () =>
                field.AppendLinesAndAnimate(5, 0.5f, ProcessUnpause));
                ViewCanvas.SwitchFunctionalButtons(true);
            }
            
            void ProcessGoToMenu()
            {
                gameplay.StopGameplay();
            }
            
            void ProcessRetry()
            {
                StartNewGame();
            }
        }
        
        public override void ReactOnUserBubbleSet(List<Place> PopByUser, List<Place> Fallen, System.Type InstrumentType)
        {
            FirstBubbleSet = true;
            ProcessCombo();
            if (PopByUser.Count < 3) return;
            RefreshLinesInfo();
            TryShowFallenText();
            
            void ProcessCombo()
            {
                if (InstrumentType != typeof(Instruments.Bubble.Circle)) return;
                if (PopByUser.Count < 3)
                {
                    Combo = 0;
                    ProcessMultiplier(0);
                    ViewCanvas.Combo.Refresh(Combo, pointsMultiplier);
                    return;
                }
                Combo ++;
                if (Combo % CurrentStage.RewardByComboCount == 0)
                {
                    var RewardCount = Combo / CurrentStage.RewardByComboCount;
                    if (RewardCount > 4) RewardCount = 4;
                    ProcessMultiplier(RewardCount);
                }
                ViewCanvas.Combo.Refresh(Combo, pointsMultiplier);
                
                void ProcessMultiplier(int ComboStage)
                {
                         if (ComboStage == 0)   //0
                    {
                        pointsMultiplier = 1;
                    }
                    else if (ComboStage == 1)   //5
                    {
                        pointsMultiplier = 1.25f;
                    }
                    else if (ComboStage == 2)   //10
                    {
                        pointsMultiplier = 1.75f;
                        ReceiveReward(1);
                    }
                    else if (ComboStage == 3)   //15
                    {
                        pointsMultiplier = 2.5f;
                        ReceiveReward(1);
                    }
                    else if (ComboStage == 4)   //20
                    {
                        pointsMultiplier = 3.2f;
                        ReceiveReward(1);
                    } 
                }
                
                void ReceiveReward(int Count)
                {
                    var Reward = new WorkType[Count];
                    var RewardArray = System.Enum.GetValues(typeof(WorkType));
                    for (int i = 0; i < Count; i++)
                    {
                        Reward[i] = (WorkType) RewardArray.GetValue(Random.Range(1, RewardArray.Length));
                    }
                    ViewCanvas.ReceivedBonus.ShowBonuses(Reward, instrumentsConfig, ApplyReward);
                }
                
                void ApplyReward(WorkType workType)
                {
                    InstrumentsCount.GetPair(workType).Count.Value ++;
                }
            }
            
            void RefreshLinesInfo()
            {
                int PointsByUserPop = PopByUser.Count * RewardForPoppedBubble;
                int PointsByFallen = Fallen.Count * RewardForFallenBubble;
                int Increment = Mathf.RoundToInt(PointsByFallen * pointsMultiplier) + Mathf.RoundToInt(PointsByUserPop * pointsMultiplier); 
                Points += Increment;
                ViewCanvas.Score.Refresh(Points, Increment);
            }
            
            void TryShowFallenText()
            {
                if (Fallen.Count == 0) return;
                Vector3 MidPoint = field.PlaceToPos(Fallen[0]);
                for (int i = 1; i < Fallen.Count; i++)
                {
                    MidPoint += field.PlaceToPos(Fallen[i]);
                }
                MidPoint /= Fallen.Count;
                ViewCanvas.FallenBubblesView.ShowAnimated(Fallen.Count, Mathf.RoundToInt(RewardForFallenBubble * pointsMultiplier), MidPoint, user.WorldToScreen);
            }
        }

        public override async Task Dispose()
        {
            ProcessPause();
            field.HideViews();
            bool FieldReady = false;
            field.FullCleanupAnimated(1f, () => FieldReady = true);
            bool UserReady = false;
            user.DeactivateInstruments();
            user.StopGameplayAndAnimate(1f, () => UserReady = true);
            ViewCanvas.Hide(1f);
            while(!(FieldReady && UserReady)) await Utilities.Wait();
        }
    }
}