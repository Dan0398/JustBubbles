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
        private const string LeaderBoardName = "StrategyBestScore";
        private const int PopMaxCount = 5;
        
        [SerializeField] private int _setCounts = 2;
        [SerializeField] private int _countUntilAppend;
        [SerializeField] private int _userClicks;
        [SerializeField] private int _appendLinesCount;
        [SerializeField] private int _popCombo;
        private bool _gameInProcess;
        #if UNITY_EDITOR
        [SerializeField] private bool _forceEnd;
        #endif
        private StrategyCanvas _strategyCanvas;
        private float _checkTimer;

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
            _strategyCanvas = Canvas;
            CustomEnterToType();
        }
        
        private void CustomEnterToType()
        {
            Field.Difficulty = 1;
            Field.ShowViews();
            StartNewGame();
            
            _strategyCanvas.Show(this, 1f);
            
            Field.ColorStats.ColorsCount.Changed += CalculateAppendLinesCount; 
            Field.ColorStats.OnColorRemove += ReactOnColorRemove;
            User.StartGameplayAndAnimate();
        }
        
        private void ReactOnColorRemove(List<Bubble.BubbleColor> s)
        {
            if (!_gameInProcess) return;
            _strategyCanvas.ReactOnColorRemove(s);
        }
        
        private void StartNewGame()
        {
            _gameInProcess = true;
            Field.SetColorConfig(5, true);
            CalculateAppendLinesCount();
            _countUntilAppend = _setCounts;
            _strategyCanvas.RefreshCountUntilAppend(_countUntilAppend);
            
            _userClicks = 0;
            _strategyCanvas.RefreshClicks(_userClicks);
            
            _popCombo = 0;
            _strategyCanvas.RefreshPopCombo(_popCombo);
#if UNITY_EDITOR
            Field.AppendLinesAndAnimate(3, 1f, ProcessUnpause);
#else
            Field.AppendLinesAndAnimate(8, 1f, ProcessUnpause);
#endif
        }
        
        private void CalculateAppendLinesCount()
        {
#if UNITY_EDITOR
            _appendLinesCount = 1;
            _setCounts = 2;
            _strategyCanvas.RefreshAppendLinesCount(_appendLinesCount);
#else
            var count = Field.ColorStats.ColorsCount.Value;
            if (count == 5) 
            {
                _appendLinesCount = 1;
                _setCounts = 2;
            }
            else if (count == 4)
            {
                _appendLinesCount = 2;
                _setCounts = 2;
            }
            else if (count == 3)
            {
                _appendLinesCount = 2;
                _setCounts = 1;
            }
            else if (count == 2) 
            {
                _appendLinesCount = 7;
                _setCounts = 1;
            }
            else if (count == 1) 
            {
                _appendLinesCount = 1;
                _setCounts = 1;
            }
            _strategyCanvas.RefreshAppendLinesCount(_appendLinesCount);
#endif
        }

        public override void ProcessGameplayUpdate()
        {
            if (Paused || !_gameInProcess) return;
#if UNITY_EDITOR
            if (_forceEnd)
            {
                FinalizeSession(true);
                _forceEnd = false;
            }
#endif
            if (_checkTimer <= 0) return;
            _checkTimer -= Time.fixedDeltaTime;
            if (Field.IsLowerLineUnderFieldEdge())
            {
                FinalizeSession(false);
                return;
            }
        }

        public override void ReactOnUserBubbleSet(List<Place> PopByUser, List<Place> Fallen, System.Type InstrumentType)
        {
            if (Field.BubblesCountOnScene == 0)
            {
                FinalizeSession(true);
                return;
            }
            _userClicks++;
            _strategyCanvas.RefreshClicks(_userClicks);
            if (Field.IsLowerLineUnderFieldEdge())
            {
                FinalizeSession(false);
            }
            if (PopByUser.Count >= 3)
            {
                _popCombo++;
                if (_popCombo == PopMaxCount)
                {
                    _strategyCanvas.RefreshPopCombo(1, DecrementUntilAppend);
                    _popCombo = 0;
                }
                else
                {
                    _strategyCanvas.RefreshPopCombo(_popCombo / (float) PopMaxCount);
                }
            }
            else
            {
                _popCombo = 0;
                _strategyCanvas.RefreshPopCombo(0);
                DecrementUntilAppend();
            }
            
            void DecrementUntilAppend()
            {
                _countUntilAppend--;
                if (_countUntilAppend == 0)
                {
                    Field.AppendLinesAndAnimate(_appendLinesCount, 0.5f);
                    _checkTimer = 0.5f;
                    _countUntilAppend = _setCounts;
                }
                _strategyCanvas.RefreshCountUntilAppend(_countUntilAppend);
            }
        }
        
        private void FinalizeSession(bool isSuccess)
        {
            _gameInProcess = false;
            Field.FullCleanupAnimated(1f);
            ProcessPause();
            
            var Saved = Services.DI.Single<Data.UserController>();
            var oldHigh = Saved.Data.StrategyBestScore;
            var isHigh = isSuccess && (oldHigh == 0 || _userClicks < oldHigh);
            if (isHigh)
            {
                Saved.Data.StrategyBestScore = _userClicks;
                Saved.SaveData();
#if UNITY_WEBGL
                var LeaderboardWrite = new Services.Leaderboards.ScoreWrite(LeaderBoardName, _userClicks);
                Services.Web.Catcher.SetNewScoreInLeaderboards(LeaderboardWrite);
#endif
            }
            var Result = new Result()
            {
                SessionResult = _userClicks,
                BestResult = isHigh? _userClicks : Saved.Data.StrategyBestScore,
                IsNewHighScore = isHigh,
                IsSuccess = isSuccess,
                LeaderBoardName = Strategy.LeaderBoardName,
                HintLangKey = isSuccess? string.Empty : "StrategyFailInfo",
                OnEnd =     () => gameplay.StopGameplay(),
                OnRetry =   () => StartNewGame()
            };
            _strategyCanvas.ShowEndgame(Result);
        }

        public override async Task Dispose()
        {
            ProcessPause();
            
            Field.ColorStats.FullCount.Changed -= CalculateAppendLinesCount; 
            Field.ColorStats.OnColorRemove -= ReactOnColorRemove;
            Field.HideViews();
            
            bool FieldReady = false;
            Field.FullCleanupAnimated(1f, () => FieldReady = true);
            
            bool UserReady = false;
            User.StopGameplayAndAnimate(1f, () => UserReady = true);
            _strategyCanvas.Hide(1f);
            
            while(!FieldReady || !UserReady) await Utilities.Wait();
        }
    }
}