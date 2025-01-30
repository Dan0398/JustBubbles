using Gameplay.Instruments;
using Content.Instrument;
using UnityEngine;

namespace Gameplay.User
{
    public partial class Action: BaseUser<Field.BubbleField>
    {
        Counts instrumentsUseCount;
        bool instrumentInUse = false;
        
        public void PeekInstrument(WorkType type)
        {
                 if (type == WorkType.Bomb)         UseBomb();
            else if (type == WorkType.Sniper)       UseSniperShot();
            else if (type == WorkType.Laser)        UseLaser();
            else if (type == WorkType.MultiBall)    UseMultiBall();
        }
        
        public void UseBomb()
        {
            if (instrumentInUse) return;
            if (IsDecrementPairFail(WorkType.Bomb)) return;
            WrapCircleAndUseInstrument(WorkType.Bomb);
        }
        
        public void UseSniperShot()
        {
            if (instrumentInUse) return;
            if (IsDecrementPairFail(WorkType.Sniper)) return;
            WrapCircleAndUseInstrument(WorkType.Sniper);
        }
        
        public void UseLaser()
        {
            if (instrumentInUse) return;
            if (IsDecrementPairFail(WorkType.Laser)) return;
            WrapCircleAndUseInstrument(WorkType.Laser);
        }
        
        public void UseMultiBall()
        {
            if (instrumentInUse) return;
            if (bubble.MultiBallUsed) return;
            if (IsDecrementPairFail(Content.Instrument.WorkType.MultiBall)) return;
            bubble.UseMultiBall();
        }
        
        bool IsDecrementPairFail(Content.Instrument.WorkType type)
        {
            var Pair = instrumentsUseCount.GetPair(type);
            if(Pair.Count.Value < 1)
            {
                instrumentsUseCount.OnFailUseInstrument?.Invoke(type);
                InGameCanvas.AnimateFailUseInstrument(type);
                return true;
            }
            Pair.Count.Value --;
            return false;
        }
        
        void WrapCircleAndUseInstrument(Content.Instrument.WorkType type)
        {
            instrumentInUse = true;
            var OldSelected = SelectedInstrument;
            var newSelected = GiveInstrumentByType(type);
            SwitchToNew(newSelected);
            newSelected.AfterUse = () => 
            {
                SwitchToNew(OldSelected);
                instrumentInUse = false;
            };
        }
        
        BaseInstrument GiveInstrumentByType(Content.Instrument.WorkType type)
        {
            if (type == Content.Instrument.WorkType.Bomb) return bomb;
            else if (type == Content.Instrument.WorkType.Sniper) return sniperShot;
            else if (type == Content.Instrument.WorkType.Laser) return laser;
            else return null;
        }
        
        void SwitchToNew(BaseInstrument instrument)
        {
            SelectedInstrument.HideAnimated(0.4f, ()=>
            {
                SelectedInstrument = instrument;
                SelectedInstrument.ProcessAimVector(MouseWorldPos - (Vector2)transform.position);
                SelectedInstrument.ShowAnimated(0.4f);
            });
        }

        public void ActivateInstruments(Counts instrumentsCount)
        {
            instrumentsUseCount = instrumentsCount;
            InGameCanvas.gameObject.SetActive(true);
            InGameCanvas.BindWithCounts(instrumentsCount);
        }

        internal void DeactivateInstruments()
        {
            InGameCanvas.Hide();
        }

        internal void ChangeRayDistance(float userDistance)
        {
            if (RayBaseDistance == null)
            {
                RayBaseDistance = userDistance;
            }
            else
            {
                RayBaseDistance.Value = userDistance;
            }
        }
    }
}