using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments
{
    public partial class Laser : BaseInstrument
    {
        private Vector3[] _linePoints;
        
        public override void ReactOnClickDown()
        {
            if (!InstrumentShown) return;
            if (!User.IsClickedInGameField()) return;
            StartCoroutine(ProcessSlice());
        }

        public override void ReactOnClickUp()
        {
            _isSlicing = false;
            _burnLine.enabled = false;
            Sounds.Stop(Services.Audio.Sounds.SoundType.LaserUse);
            Sounds.Play(Services.Audio.Sounds.SoundType.LaserIdle);
            if (_burnSmoke != null) _burnSmoke.Stop();
            if (_laserSmoke != null) _laserSmoke.Stop();
            if (_burnEdge != null) _burnEdge.Stop();
        }
        
        private IEnumerator ProcessSlice()
        {
            RefreshTargetBubble();
            ChangeView();
            
            _isSlicing = true;
            _burnLine.enabled = true;
            Sounds.Play(Services.Audio.Sounds.SoundType.LaserUse);
            Sounds.Stop(Services.Audio.Sounds.SoundType.LaserIdle);
            
            if (_laserSmoke != null) _laserSmoke.Play();
            if (_burnSmoke != null) _burnSmoke.Play();
            if (_burnEdge != null) _burnEdge.Play();
            
            bool RefreshOnNextStep = true;
            while(_isSlicing && _energy > 0)
            {
                if (RefreshOnNextStep)
                {
                    RefreshTargetBubble();
                    ChangeView();
                    RotateRifle();
                    RefreshOnNextStep = false;
                }
                _energy --;
                if (_targetBubble != null && _targetBubble.IsDamageCauseDeath())
                {
                    _underAttack.Remove(_targetBubble);
                    Field.CleanDamagedBubble(ref _targetBubble);
                    _targetBubble = null;
                    RefreshOnNextStep = true;
                }
                yield return Wait;
            }
            if (_energy <= 0)
            {
                AfterUse?.Invoke();
                ReactOnClickUp();
            }
        }

        public override void ProcessAimVector(Vector2 Vector)
        {
            base.ProcessAimVector(Vector);
            RefreshTargetBubble();
            ChangeView();
            RotateRifle();
        }
        
        private void ChangeView()
        {
            _linePoints ??= new Vector3[3] {_laserOnScene.parent.position, Vector3.zero, Vector3.zero};
            _linePoints[2] = _burnWorldPos;
            _linePoints[1] = Vector3.Lerp(_linePoints[0], _linePoints[2], 0.1f);
            _burnLine.SetPositions(_linePoints);
            
            _burnPoint.position = _burnWorldPos;
        }
        
        private void RefreshTargetBubble()
        {
            _trajectory ??= new User.Trajectory(CollisionRadius, Field.TryResponseCollision, 1, TrajectoryCollisionsCount);
            _trajectory.CalculateFullWayClean(_laserOnScene.parent.position, MouseClampedDirection);
            var corner = _trajectory.Corners[0];
            _burnWorldPos = corner.Endpoint;
            if (!_isSlicing) return;
            if (corner.CollisionReason != Gameplay.User.CollisionType.IntoBunch) return;
            var place = Field.GiveBubbleByDirection(ref _burnWorldPos, corner.Direction);
            if (!place.Valid || !place.Busy)
            {
                _targetBubble = null;
                return;
            }
            foreach(var damaged in _underAttack)
            {
                if (damaged.FieldPlace.Line == place.Line && damaged.FieldPlace.Column == place.Column)
                {
                    _targetBubble = damaged;
                    return;
                }
            }
            _targetBubble = new DamagedBubble(place, this._bubbleResistFrames);
            _underAttack.Add(_targetBubble);
        }
        
        private void RotateRifle()
        {
            if (!_laserSpite) _laserSpite = _laserOnScene.GetComponent<SpriteRenderer>();
            _laserOnScene.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(MouseClampedDirection.x, MouseClampedDirection.y));
        }
        
        public override void ReactOnFieldMove()
        {
            base.ReactOnFieldMove();
            var newLinesCount = Field.LineCount;
            if (_oldLinesCount != newLinesCount)
            {
                Field.TryChangeLinesPosInDamaged(ref _underAttack, _oldLinesCount);
                _oldLinesCount = newLinesCount;
            }
        }
    }
}