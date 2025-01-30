#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif
using System.Collections.Generic;
using Gameplay.Field;
using Gameplay.User;
using UI.Settings;
using UI.Strategy;
using UnityEngine;

namespace Gameplay.GameType
{
    [System.Serializable]
    public class Strategy : BubbleBaseType
    {
        const string LeaderBoardName = "StrategyBestScore";
        const int PopMaxCount = 5;
        [SerializeField] int SetCounts = 2;
        [SerializeField] int CountUntilAppend;
        [SerializeField] int userClicks;
        [SerializeField] int AppendLinesCount;
        [SerializeField] int PopCombo;
        [SerializeField] float ttt;
        bool GameInProcess;
        #if UNITY_EDITOR
        [SerializeField] bool ForceEnd;
        #endif
        StrategyCanvas strategyCanvas;
        float checkTimer;

        protected override bool IsFieldAspectDynamic => false;
        protected override float FieldUpperOutstand => 0.07f;

#if UNITY_WEBGL
        protected override float MaxFieldAspect => 0.63f;
        #else 
        protected override float MaxFieldAspect => 9/16f;
        #endif
        
        public Strategy(Gameplay.Controller gameplay, Settings Settings, InGameParents InGameParts, BubbleField Field, Action User, StrategyCanvas Canvas)
                :base(gameplay, Settings, InGameParts, Field, User)
        {
            strategyCanvas = Canvas;
            CustomEnterToType();
        }
        
        void CustomEnterToType()
        {
            field.Difficulty = 1;
            field.ShowViews();
            StartNewGame();
            
            strategyCanvas.Show(this, 1f);
            
            field.ColorStats.ColorsCount.Changed += CalculateAppendLinesCount; 
            field.ColorStats.OnColorRemove += ReactOnColorRemove;
            user.StartGameplayAndAnimate();
        }
        
        void ReactOnColorRemove(List<Bubble.BubbleColor> s)
        {
            if (!GameInProcess) return;
            strategyCanvas.ReactOnColorRemove(s);
        }
        
        void StartNewGame()
        {
            GameInProcess = true;
            field.SetColorConfig(5, true);
            CalculateAppendLinesCount();
            CountUntilAppend = SetCounts;
            strategyCanvas.RefreshCountUntilAppend(CountUntilAppend);
            
            userClicks = 0;
            strategyCanvas.RefreshClicks(userClicks);
            
            PopCombo = 0;
            strategyCanvas.RefreshPopCombo(PopCombo);
#if UNITY_EDITOR
            field.AppendLinesAndAnimate(3, 1f, ProcessUnpause);
#else
            field.AppendLinesAndAnimate(8, 1f, ProcessUnpause);
#endif
        }
        
        void CalculateAppendLinesCount()
        {
#if UNITY_EDITOR
            AppendLinesCount = 1;
            SetCounts = 2;
            strategyCanvas.RefreshAppendLinesCount(AppendLinesCount);
#else
            var count = field.ColorStats.ColorsCount.Value;
            if (count == 5) 
            {
                AppendLinesCount = 1;
                SetCounts = 2;
            }
            else if (count == 4)
            {
                AppendLinesCount = 2;
                SetCounts = 2;
            }
            else if (count == 3)
            {
                AppendLinesCount = 2;
                SetCounts = 1;
            }
            else if (count == 2) 
            {
                AppendLinesCount = 7;
                SetCounts = 1;
            }
            else if (count == 1) 
            {
                AppendLinesCount = 1;
                SetCounts = 1;
            }
            strategyCanvas.RefreshAppendLinesCount(AppendLinesCount);
#endif
        }

        public override void ProcessGameplayUpdate()
        {
            if (Paused || !GameInProcess) return;
#if UNITY_EDITOR
            if (ForceEnd)
            {
                FinalizeSession(true);
                ForceEnd = false;
            }
#endif
            if (checkTimer <= 0) return;
            checkTimer -= Time.fixedDeltaTime;
            ttt = field.GetDistanceToFieldEdge();
            if (field.IsLowerLineUnderFieldEdge())
            {
                FinalizeSession(false);
                return;
            }
        }

        public override void ReactOnUserBubbleSet(List<Place> PopByUser, List<Place> Fallen, System.Type InstrumentType)
        {
            if (field.BubblesCountOnScene == 0)
            {
                FinalizeSession(true);
                return;
            }
            userClicks++;
            strategyCanvas.RefreshClicks(userClicks);
            if (field.IsLowerLineUnderFieldEdge())
            {
                FinalizeSession(false);
            }
            if (PopByUser.Count >= 3)
            {
                PopCombo++;
                if (PopCombo == PopMaxCount)
                {
                    strategyCanvas.RefreshPopCombo(1, DecrementUntilAppend);
                    PopCombo = 0;
                }
                else
                {
                    strategyCanvas.RefreshPopCombo(PopCombo / (float) PopMaxCount);
                }
            }
            else
            {
                PopCombo = 0;
                strategyCanvas.RefreshPopCombo(0);
                DecrementUntilAppend();
            }
            
            void DecrementUntilAppend()
            {
                CountUntilAppend--;
                if (CountUntilAppend == 0)
                {
                    field.AppendLinesAndAnimate(AppendLinesCount, 0.5f);
                    checkTimer = 0.5f;
                    CountUntilAppend = SetCounts;
                }
                strategyCanvas.RefreshCountUntilAppend(CountUntilAppend);
            }
        }
        
        void FinalizeSession(bool isSuccess)
        {
            GameInProcess = false;
            field.FullCleanupAnimated(1f);
            ProcessPause();
            
            var Saved = Services.DI.Single<Data.UserController>();
            var oldHigh = Saved.Data.StrategyBestScore;
            var isHigh = isSuccess && (oldHigh == 0 || userClicks < oldHigh);
            if (isHigh)
            {
                Saved.Data.StrategyBestScore = userClicks;
                Saved.SaveData();
#if UNITY_WEBGL
                var LeaderboardWrite = new Services.Leaderboards.ScoreWrite(LeaderBoardName, userClicks);
                Services.Web.Catcher.SetNewScoreInLeaderboards(LeaderboardWrite);
#endif
            }
            var Result = new Result()
            {
                SessionResult = userClicks,
                BestResult = isHigh? userClicks : Saved.Data.StrategyBestScore,
                IsNewHighScore = isHigh,
                IsSuccess = isSuccess,
                LeaderBoardName = Strategy.LeaderBoardName,
                HintLangKey = isSuccess? string.Empty : "StrategyFailInfo",
                OnEnd =     () => gameplay.StopGameplay(),
                OnRetry =   () => StartNewGame()
            };
            strategyCanvas.ShowEndgame(Result);
        }

        public override async Task Dispose()
        {
            ProcessPause();
            
            field.ColorStats.FullCount.Changed -= CalculateAppendLinesCount; 
            field.ColorStats.OnColorRemove -= ReactOnColorRemove;
            field.HideViews();
            
            bool FieldReady = false;
            field.FullCleanupAnimated(1f, () => FieldReady = true);
            
            bool UserReady = false;
            user.StopGameplayAndAnimate(1f, () => UserReady = true);
            strategyCanvas.Hide(1f);
            
            while(!FieldReady || !UserReady) await Utilities.Wait();
        }
    }
}