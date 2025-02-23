#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif
using System.Collections.Generic;
using Gameplay.Field;
using Gameplay.User;
using UnityEngine;

namespace Gameplay.GameType
{
    [System.Serializable]
    public class Endless : BubbleBaseType
    {
        private const int MinimumLines = 7;
        [SerializeField] private float _rewardRelative;
        [SerializeField] private int _fallenToRewardMultiplier;
        private UI.Endless.EndlessCanvas _canvas;
        private float _appendLinesTimer;
        private Content.Instrument.Config _instrumentsConfig;
        private Instruments.Counts _instrumentsCount;
        
        protected override bool IsFieldAspectDynamic => true;
        protected override float UserDistance => 25f;

        public Endless(Gameplay.Controller Gameplay, UI.Settings.Settings Settings, InGameParents InGameParts, BubbleField Field, Action User, UI.Endless.EndlessCanvas  canvas) 
                : base(Gameplay, Settings, InGameParts, Field, User) 
        {
            _appendLinesTimer = 0;
            _rewardRelative = 0;
            #if UNITY_EDITOR
            _fallenToRewardMultiplier = 8;
            #else
            FallenToRewardMultiplier = 16;
            #endif
            this._canvas = canvas;
            CustomEnterToType();
        }

        void CustomEnterToType()
        {
            Field.Difficulty = 0.8f;
            Field.SetColorConfig(4, false);
            Field.ShowViews();
            Field.AppendLinesAndAnimate(MinimumLines + 1, 1f, ProcessUnpause);
            
            _canvas.Show(this);
            
            GetAndProcessInstruments();
            
            User.StartGameplayAndAnimate(1f);
        }
        
        private async void GetAndProcessInstruments()
        {
            var Service = Services.DI.Single<Content.Instrument.Service>();
            while (Service.Config == null) if (await Utilities.IsWaitEndsFailure()) return;
            _instrumentsConfig = Service.Config;
            
            _instrumentsCount = new Instruments.Counts(_instrumentsConfig);
            _instrumentsCount.GetPair(Content.Instrument.WorkType.Bomb).Count.Value = 5;
            _instrumentsCount.OnFailUseInstrument += ReactOnFailUseInstrument;
            User.ActivateInstruments(_instrumentsCount);
        }
        
        private void ReactOnFailUseInstrument(Content.Instrument.WorkType type)
        {
            Content.Instrument.Config.InstrumentView Data = null;
            foreach(var instrument in _instrumentsConfig.Instruments)
            {
                if (instrument.Type == type)
                {
                    Data = instrument;
                    break;
                }
            }
            var Pair = _instrumentsCount.GetPair(type);
            _canvas.ReactOnFailUseInstrument(Data, OnIncrease);
            
            async void OnIncrease()
            {
                if (await Services.DI.Single<Services.Advertisements.Controller>().IsRewardAdSuccess())
                {
                    Pair.Count.Value += Data.IncreaseCount;
                    _canvas.HideAdsRequest();
                    User.PeekInstrument(type);
                }
            }
        }
        
        public override void ProcessGameplayUpdate() 
        {
            if (Paused) return;
            Field.TryCheckAspectChange();
            
            if (_appendLinesTimer <= 0) return;
            _appendLinesTimer -= Time.fixedDeltaTime;
            TryRefreshFieldIfOutOfEdge();
        }
        
        private void TryRefreshFieldIfOutOfEdge()
        {
            if (!Field.IsLowerLineUnderFieldEdge()) return;
            ProcessPause();
            Field.FullCleanupAnimated(1f, () =>
            Field.AppendLinesAndAnimate(MinimumLines + 1, 0.5f, ProcessUnpause));
        }

        public override void ReactOnUserBubbleSet(List<Place> PopByUser, List<Place> Fallen, System.Type InstrumentType)
        {
            TryRegisterFallen();
            TryAppendLine();
            
            void TryRegisterFallen()
            {
                if (Fallen.Count == 0) return;
                _rewardRelative += Fallen.Count / (float)(Field.BubblesCountPerLine * _fallenToRewardMultiplier);
                System.Func<bool> AfterCircleFill = null;
                if (_rewardRelative >= 1)
                {
                    AfterCircleFill = () =>
                    {
                        _rewardRelative -= 1;
                        return IsClaimRewardFilled();
                    };
                }
                _canvas.ShowRewardFill(Mathf.Clamp01(_rewardRelative), AfterCircleFill, _rewardRelative % 1);
            }
            
            void TryAppendLine()
            {
                var MinimumBubblesCount = Field.BubblesCountPerLine * MinimumLines;
                if (Field.BubblesCountOnScene < MinimumBubblesCount)
                {
                    int Count = Mathf.CeilToInt((MinimumBubblesCount - Field.BubblesCountOnScene) / (float) Field.BubblesCountPerLine);
                    _appendLinesTimer = 0.3f * Count;
                    Field.AppendLinesAndAnimate(Count, _appendLinesTimer);
                }
                else 
                {
                    TryRefreshFieldIfOutOfEdge();
                }
            }
            
            bool IsClaimRewardFilled()
            {
                var RewardTypes = System.Enum.GetValues(typeof(Content.Instrument.WorkType));
                var RewardType = (Content.Instrument.WorkType) RewardTypes.GetValue(Random.Range(1, RewardTypes.Length));
                Content.Instrument.Config.InstrumentView Shown = null;
                foreach(var instrument in _instrumentsConfig.Instruments)
                {
                    if (instrument.Type != RewardType) continue;
                    Shown = instrument;
                    break;
                }
                _canvas.ShowClaimedReward(Shown);
                
                var Pair = _instrumentsCount.GetPair(RewardType);
                if (Pair == null) return false;
                Pair.Count.Value += 1;
                if (Pair.Count.Value <= 9) return false;
                Pair.Count.Value = 9;
                return true;
            }
        }

        public override async Task Dispose()
        {
            _instrumentsCount.OnFailUseInstrument -= ReactOnFailUseInstrument;
            ProcessPause();
            Field.HideViews();
            User.DeactivateInstruments();
            User.StopGameplayAndAnimate(0.5f);
            
            _canvas.Dispose();
            _canvas.Hide();
            
            var RequireToWait = true;
            Field.FullCleanupAnimated(0.5f, () => RequireToWait = false);
            while(RequireToWait) await Utilities.Wait();
        }
    }
}