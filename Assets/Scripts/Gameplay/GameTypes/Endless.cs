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
        const int MinimumLines = 7;
        UI.Endless.EndlessCanvas canvas;
        float appendLinesTimer;
        [SerializeField] float RewardRelative;
        [SerializeField] int FallenToRewardMultiplier;
        Content.Instrument.Config instrumentsConfig;
        Instruments.Counts InstrumentsCount;
        
        protected override bool IsFieldAspectDynamic => true;
        protected override float UserDistance => 25f;

        public Endless(Gameplay.Controller Gameplay, UI.Settings.Settings Settings, InGameParents InGameParts, BubbleField Field, Action User, UI.Endless.EndlessCanvas  canvas) 
                : base(Gameplay, Settings, InGameParts, Field, User) 
        {
            appendLinesTimer = 0;
            RewardRelative = 0;
            #if UNITY_EDITOR
            FallenToRewardMultiplier = 8;
            #else
            FallenToRewardMultiplier = 16;
            #endif
            this.canvas = canvas;
            CustomEnterToType();
        }

        void CustomEnterToType()
        {
            field.Difficulty = 0.8f;
            field.SetColorConfig(4, false);
            field.ShowViews();
            field.AppendLinesAndAnimate(MinimumLines + 1, 1f, ProcessUnpause);
            
            canvas.Show(this);
            
            GetAndProcessInstruments();
            //SetupMiddleInterstitial();
            
            user.StartGameplayAndAnimate(1f);
        }
        
        async void GetAndProcessInstruments()
        {
            var Service = Services.DI.Single<Content.Instrument.Service>();
            while (Service.Config == null) if (await Utilities.IsWaitEndsFailure()) return;
            instrumentsConfig = Service.Config;
            
            InstrumentsCount = new Instruments.Counts(instrumentsConfig);
            InstrumentsCount.GetPair(Content.Instrument.WorkType.Bomb).Count.Value = 5;
            InstrumentsCount.OnFailUseInstrument += ReactOnFailUseInstrument;
            user.ActivateInstruments(InstrumentsCount);
        }
        
        /*
        void SetupMiddleInterstitial()
        {
            #if UNITY_EDITOR
            ads = new EndlessAds(this, 20f);
            canvas.BindWithAds(ads);
            #elif UNITY_WEBGL
            Services.Web.Catcher.RequestInterstitialDelay(ReceiveTimer);
            
            void ReceiveTimer(int Time)
            {
                if (Time <= 0) return;
                ads = new EndlessAds(this, Time);
                canvas.BindWithAds(ads);
            }
            #endif
        }
        */
        
        void ReactOnFailUseInstrument(Content.Instrument.WorkType type)
        {
            Content.Instrument.Config.InstrumentView Data = null;
            foreach(var instrument in instrumentsConfig.Instruments)
            {
                if (instrument.Type == type)
                {
                    Data = instrument;
                    break;
                }
            }
            var Pair = InstrumentsCount.GetPair(type);
            canvas.ReactOnFailUseInstrument(Data, OnIncrease);
            
            async void OnIncrease()
            {
                if (await Services.DI.Single<Services.Advertisements.Controller>().IsRewardAdSuccess())
                {
                    Pair.Count.Value += Data.IncreaseCount;
                    canvas.HideAdsRequest();
                    user.PeekInstrument(type);
                }
            }
        }
        
        public override void ProcessGameplayUpdate() 
        {
            if (Paused) return;
            field.TryCheckAspectChange();
            //ads?.ProcessFixedUpdate();
            
            if (appendLinesTimer <= 0) return;
            appendLinesTimer -= Time.fixedDeltaTime;
            TryRefreshFieldIfOutOfEdge();
        }
        
        void TryRefreshFieldIfOutOfEdge()
        {
            if (!field.IsLowerLineUnderFieldEdge()) return;
            ProcessPause();
            field.FullCleanupAnimated(1f, () =>
            field.AppendLinesAndAnimate(MinimumLines + 1, 0.5f, ProcessUnpause));
        }

        public override void ReactOnUserBubbleSet(List<Place> PopByUser, List<Place> Fallen, System.Type InstrumentType)
        {
            TryRegisterFallen();
            TryAppendLine();
            
            void TryRegisterFallen()
            {
                if (Fallen.Count == 0) return;
                RewardRelative += Fallen.Count / (float)(field.BubblesCountPerLine * FallenToRewardMultiplier);
                System.Func<bool> AfterCircleFill = null;
                if (RewardRelative >= 1)
                {
                    AfterCircleFill = () =>
                    {
                        RewardRelative -= 1;
                        return IsClaimRewardFilled();
                    };
                }
                canvas.ShowRewardFill(Mathf.Clamp01(RewardRelative), AfterCircleFill, RewardRelative % 1);
            }
            
            void TryAppendLine()
            {
                var MinimumBubblesCount = field.BubblesCountPerLine * MinimumLines;
                if (field.BubblesCountOnScene < MinimumBubblesCount)
                {
                    int Count = Mathf.CeilToInt((MinimumBubblesCount - field.BubblesCountOnScene) / (float) field.BubblesCountPerLine);
                    appendLinesTimer = 0.3f * Count;
                    field.AppendLinesAndAnimate(Count, appendLinesTimer);
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
                foreach(var instrument in instrumentsConfig.Instruments)
                {
                    if (instrument.Type != RewardType) continue;
                    Shown = instrument;
                    break;
                }
                canvas.ShowClaimedReward(Shown);
                
                var Pair = InstrumentsCount.GetPair(RewardType);
                if (Pair == null) return false;
                Pair.Count.Value += 1;
                if (Pair.Count.Value <= 9) return false;
                Pair.Count.Value = 9;
                return true;
            }
        }

        public override async Task Dispose()
        {
            InstrumentsCount.OnFailUseInstrument -= ReactOnFailUseInstrument;
            ProcessPause();
            field.HideViews();
            user.DeactivateInstruments();
            user.StopGameplayAndAnimate(0.5f);
            
            canvas.Dispose();
            //ads?.Dispose();
            canvas.Hide();
            
            var RequireToWait = true;
            field.FullCleanupAnimated(0.5f, () => RequireToWait = false);
            while(RequireToWait) await Utilities.Wait();
        }
    }
}