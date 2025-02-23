using Gameplay.Instruments;
using Content.Instrument;
using UnityEngine;

namespace Gameplay.User
{
    public partial class Action: BaseUser<Field.BubbleField>
    {
        private Counts _instrumentsUseCount;
        private bool _instrumentInUse;
        
        public void PeekInstrument(WorkType type)
        {
                 if (type == WorkType.Bomb)         UseBomb();
            else if (type == WorkType.Sniper)       UseSniperShot();
            else if (type == WorkType.Laser)        UseLaser();
            else if (type == WorkType.MultiBall)    UseMultiBall();
        }
        
        public void UseBomb()
        {
            if (_instrumentInUse) return;
            if (IsDecrementPairFail(WorkType.Bomb)) return;
            WrapCircleAndUseInstrument(WorkType.Bomb);
        }
        
        public void UseSniperShot()
        {
            if (_instrumentInUse) return;
            if (IsDecrementPairFail(WorkType.Sniper)) return;
            WrapCircleAndUseInstrument(WorkType.Sniper);
        }
        
        public void UseLaser()
        {
            if (_instrumentInUse) return;
            if (IsDecrementPairFail(WorkType.Laser)) return;
            WrapCircleAndUseInstrument(WorkType.Laser);
        }
        
        public void UseMultiBall()
        {
            if (_instrumentInUse) return;
            if (_bubble.MultiBallUsed) return;
            if (IsDecrementPairFail(WorkType.MultiBall)) return;
            _bubble.UseMultiBall();
        }
        
        private bool IsDecrementPairFail(WorkType type)
        {
            var Pair = _instrumentsUseCount.GetPair(type);
            if(Pair.Count.Value < 1)
            {
                _instrumentsUseCount.OnFailUseInstrument?.Invoke(type);
                _inGameCanvas.AnimateFailUseInstrument(type);
                return true;
            }
            Pair.Count.Value --;
            return false;
        }
        
        private void WrapCircleAndUseInstrument(WorkType type)
        {
            _instrumentInUse = true;
            var OldSelected = _selectedInstrument;
            var newSelected = GiveInstrumentByType(type);
            SwitchToNew(newSelected);
            newSelected.AfterUse = () => 
            {
                SwitchToNew(OldSelected);
                _instrumentInUse = false;
            };
        }
        
        private BaseInstrument GiveInstrumentByType(WorkType type)
        {
            if (type == WorkType.Bomb) return _bomb;
            else if (type == WorkType.Sniper) return _sniperShot;
            else if (type == WorkType.Laser) return _laser;
            else return null;
        }
        
        private void SwitchToNew(BaseInstrument instrument)
        {
            _selectedInstrument.HideAnimated(0.4f, ()=>
            {
                _selectedInstrument = instrument;
                _selectedInstrument.ProcessAimVector(MouseWorldPos - (Vector2)transform.position);
                _selectedInstrument.ShowAnimated(0.4f);
            });
        }

        public void ActivateInstruments(Counts instrumentsCount)
        {
            _instrumentsUseCount = instrumentsCount;
            _inGameCanvas.gameObject.SetActive(true);
            _inGameCanvas.BindWithCounts(instrumentsCount);
        }

        public void DeactivateInstruments()
        {
            _inGameCanvas.Hide();
        }

        public void ChangeRayDistance(float userDistance)
        {
            if (_rayBaseDistance == null)
            {
                _rayBaseDistance = userDistance;
            }
            else
            {
                _rayBaseDistance.Value = userDistance;
            }
        }
    }
}