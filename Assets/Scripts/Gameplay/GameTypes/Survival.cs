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
        private const string LeaderBoardName = "SurvivalBestScore";
        private const int RewardForFallenBubble = 50;
        private const int RewardForPoppedBubble = 20;
        
        private bool _firstBubbleSet;
        private float _distance;
        private int _lives, _points, _combo;
        private float _pointsMultiplier;
        private int _currentGameStageID;
        private SurvivalStage[] _gameStages;
        private UI.Survival.SurvivalCanvas _viewCanvas;
        private Data.UserController _userData;
        private Config _instrumentsConfig;
        private Counts _InstrumentsCount;

        private SurvivalStage CurrentStage => _gameStages[_currentGameStageID];

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
            _gameStages = new SurvivalStage[] 
            {
                new (3, 60, 6, 0.1f, 0.25f),
                new (4, 180, 5, 0.12f, 0.2f),
                new (5, 240, 4, 0.1f, 0.15f),
                new (5, 300, 4, 0.1f, 0.16f),
                new (6, 600, 3, 0.1f, 0.12f),
                new (6, 600, 3, 0.1f, 0.15f),
                new (6, 600, 3, 0.1f, 0.15f),
                new (6, 600, 3, 0.1f, 0.2f),
                new (6, 600, 3, 0.1f, 0.3f),
                new (6, float.MaxValue, 2, 0.1f, 0.3f),
            };
            _viewCanvas = Canvas;
            _userData = Services.DI.Single<Data.UserController>();
            CustomEnterToType();
        }

        private void CustomEnterToType()
        {
            Field.Difficulty = 0.9f;
            StartNewGame();
            Field.ShowViews();
            
            User.StartGameplayAndAnimate(1f);
        }
        
        private void StartNewGame()
        {
            _lives = 1;
            
            _currentGameStageID = 0;
            Field.SetColorConfig(CurrentStage.SceneColors, false);
            
            _viewCanvas.Show(this, 1f);
            _viewCanvas.SwitchFunctionalButtons(true);
            _viewCanvas.RefreshGameStageData(CurrentStage);
            _viewCanvas.ReceiveNewGameStage(CurrentStage);
            
            _viewCanvas.Show(this, 1f);
            
            _points = 0;
            _viewCanvas.Score.Refresh(_points, 0);
            
            _combo = 0;
            _pointsMultiplier = 1;
            _viewCanvas.Combo.Refresh(_combo, _pointsMultiplier);
            
            _firstBubbleSet = false;
            Field.AppendLinesAndAnimate(6, 1f, ProcessUnpause);
            GetAndProcessInstruments();
        }
        
        private async void GetAndProcessInstruments()
        {
            if (_instrumentsConfig == null)
            {
                var Service = Services.DI.Single<Content.Instrument.Service>();
                while (Service.Config == null) if (await Utilities.IsWaitEndsFailure()) return;
                _instrumentsConfig = Service.Config;
            }
            
            _InstrumentsCount = new Instruments.Counts(_instrumentsConfig);
            User.ActivateInstruments(_InstrumentsCount);
        }
        
        public override void ProcessGameplayUpdate() 
        {
            if (Paused) return;
            if (!_firstBubbleSet) return;
            _distance = Field.GetRelativeDistanceToFieldEdge();
            if (_distance < 0)
            {
                GameOver();
                return;
            }
            var WeightedSpeed = _viewCanvas.MoveDynamic.Evaluate(_distance);
            CurrentStage.RegisterTime(Time.fixedDeltaTime * WeightedSpeed);
            _viewCanvas.RefreshGameStageData(CurrentStage);
            Field.ShiftLinesDown(WeightedSpeed * CurrentStage.Speed);
            if (Field.IsPlaceOnTopEmpty)
            {
                Field.AppendOneLine();
            }
            if (CurrentStage.RequireChangeStage && _currentGameStageID < _gameStages.Length - 1)
            {
                _currentGameStageID++;
                CurrentStage.RegisterTime(_gameStages[_currentGameStageID-1].TimeOutOfDuration);
                Field.SetColorConfig(CurrentStage.SceneColors, false);
                _viewCanvas.ReceiveNewGameStage(CurrentStage);
            }
        }
        
        private async void GameOver()
        {
            ProcessPause();
            
            await Utilities.Wait(500);
            
            var bestResult = _userData.Data.SurvivalBestScore.Value;
            var IsRecord = _points > bestResult;
            var MaxPoints = IsRecord? _points : bestResult;
            
            _viewCanvas.SwitchFunctionalButtons(false);
            _viewCanvas.GameOver.OnGiveUp = Clean;
            _viewCanvas.GameOver.ProcessEnd(new UI.Strategy.Result()
            {
                SessionResult = _points,
                BestResult = MaxPoints,
                IsNewHighScore = IsRecord,
                LeaderBoardName = Survival.LeaderBoardName,
                OnRetry = ProcessRetry,
                OnEnd = ProcessGoToMenu
            }, 
            _lives, Revive);
            
            void Clean()
            {
                if (IsRecord) _userData.Data.SurvivalBestScore.Value = Mathf.Max(_userData.Data.SurvivalBestScore.Value, _points);
                _viewCanvas.Hide(1f, false);
                Field.FullCleanupAnimated(1f);
#if UNITY_WEBGL
                if (IsRecord)
                {
                    Services.Web.Catcher.SetNewScoreInLeaderboards(new Services.Leaderboards.ScoreWrite(LeaderBoardName, MaxPoints));
                }
#endif
            }
            
            void Revive()
            {
                _firstBubbleSet = false;
                _lives--;
                Field.FullCleanupAnimated(0.5f, () =>
                Field.AppendLinesAndAnimate(5, 0.5f, ProcessUnpause));
                _viewCanvas.SwitchFunctionalButtons(true);
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
            _firstBubbleSet = true;
            ProcessCombo();
            if (PopByUser.Count < 3) return;
            RefreshLinesInfo();
            TryShowFallenText();
            
            void ProcessCombo()
            {
                if (InstrumentType != typeof(Instruments.Bubble.Circle)) return;
                if (PopByUser.Count < 3)
                {
                    _combo = 0;
                    ProcessMultiplier(0);
                    _viewCanvas.Combo.Refresh(_combo, _pointsMultiplier);
                    return;
                }
                _combo ++;
                if (_combo % CurrentStage.RewardByComboCount == 0)
                {
                    var RewardCount = _combo / CurrentStage.RewardByComboCount;
                    if (RewardCount > 4) RewardCount = 4;
                    ProcessMultiplier(RewardCount);
                }
                _viewCanvas.Combo.Refresh(_combo, _pointsMultiplier);
                
                void ProcessMultiplier(int ComboStage)
                {
                         if (ComboStage == 0)   //0
                    {
                        _pointsMultiplier = 1;
                    }
                    else if (ComboStage == 1)   //5
                    {
                        _pointsMultiplier = 1.25f;
                    }
                    else if (ComboStage == 2)   //10
                    {
                        _pointsMultiplier = 1.75f;
                        ReceiveReward(1);
                    }
                    else if (ComboStage == 3)   //15
                    {
                        _pointsMultiplier = 2.5f;
                        ReceiveReward(1);
                    }
                    else if (ComboStage == 4)   //20
                    {
                        _pointsMultiplier = 3.2f;
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
                    _viewCanvas.ReceivedBonus.ShowBonuses(Reward, _instrumentsConfig, ApplyReward);
                }
                
                void ApplyReward(WorkType workType)
                {
                    _InstrumentsCount.GetPair(workType).Count.Value ++;
                }
            }
            
            void RefreshLinesInfo()
            {
                int PointsByUserPop = PopByUser.Count * RewardForPoppedBubble;
                int PointsByFallen = Fallen.Count * RewardForFallenBubble;
                int Increment = Mathf.RoundToInt(PointsByFallen * _pointsMultiplier) + Mathf.RoundToInt(PointsByUserPop * _pointsMultiplier); 
                _points += Increment;
                _viewCanvas.Score.Refresh(_points, Increment);
            }
            
            void TryShowFallenText()
            {
                if (Fallen.Count == 0) return;
                Vector3 MidPoint = Field.PlaceToPos(Fallen[0]);
                for (int i = 1; i < Fallen.Count; i++)
                {
                    MidPoint += Field.PlaceToPos(Fallen[i]);
                }
                MidPoint /= Fallen.Count;
                _viewCanvas.FallenBubblesView.ShowAnimated(Fallen.Count, Mathf.RoundToInt(RewardForFallenBubble * _pointsMultiplier), MidPoint, User.WorldToScreen);
            }
        }

        public override async Task Dispose()
        {
            ProcessPause();
            Field.HideViews();
            bool FieldReady = false;
            Field.FullCleanupAnimated(1f, () => FieldReady = true);
            bool UserReady = false;
            User.DeactivateInstruments();
            User.StopGameplayAndAnimate(1f, () => UserReady = true);
            _viewCanvas.Hide(1f);
            while(!(FieldReady && UserReady)) await Utilities.Wait();
        }
    }
}