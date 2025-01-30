using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments
{
    public partial class Laser : BaseInstrument
    {
        public override void ReactOnClickDown()
        {
            //processRoutine = 
            if (!instrumentShown) return;
            if (!User.IsClickedInGameField()) return;
            StartCoroutine(ProcessSlice());
        }

        public override void ReactOnClickUp()
        {
            //if (processRoutine != null) User.StopCoroutine(processRoutine);
            IsSlicing = false;
            burnLine.enabled = false;
            Sounds.Stop(Services.Audio.Sounds.SoundType.LaserUse);
            Sounds.Play(Services.Audio.Sounds.SoundType.LaserIdle);
            /*
            burnSound.Stop();
            idleSound.Play();
            */
            if (burnSmoke != null) burnSmoke.Stop();
            if (laserSmoke != null) laserSmoke.Stop();
            if (burnEdge != null) burnEdge.Stop();
        }
        
        IEnumerator ProcessSlice()
        {
            RefreshTargetBubble();
            ChangeView();
            
            IsSlicing = true;
            burnLine.enabled = true;
            Sounds.Play(Services.Audio.Sounds.SoundType.LaserUse);
            Sounds.Stop(Services.Audio.Sounds.SoundType.LaserIdle);
            
            /*
            idleSound.Stop();
            burnSound.Play();
            */
            if (laserSmoke != null) laserSmoke.Play();
            if (burnSmoke != null) burnSmoke.Play();
            if (burnEdge != null) burnEdge.Play();
            
            bool RefreshOnNextStep = true;
            while(IsSlicing && Energy > 0)
            {
                if (RefreshOnNextStep)
                {
                    RefreshTargetBubble();
                    ChangeView();
                    RotateRifle();
                    RefreshOnNextStep = false;
                }
                Energy --;
                if (TargetBubble != null && TargetBubble.IsDamageCauseDeath())
                {
                    underAttack.Remove(TargetBubble);
                    Field.CleanDamagedBubble(ref TargetBubble);
                    TargetBubble = null;
                    RefreshOnNextStep = true;
                }
                yield return Wait;
            }
            if (Energy <= 0)
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
        
        Vector3[] LinePoints;
        void ChangeView()
        {
            if (LinePoints == null)
            {
                LinePoints = new Vector3[3] {laserOnScene.parent.position, Vector3.zero, Vector3.zero};
            }
            LinePoints[2] = burnWorldPos;
            LinePoints[1] = Vector3.Lerp(LinePoints[0], LinePoints[2], 0.1f);
            burnLine.SetPositions(LinePoints);
            
            burnPoint.position = burnWorldPos;
        }
        
        void RefreshTargetBubble()
        {
            if (traj == null)
            {
                //Debug.Log("Не создалось((");
                traj = new User.Trajectory(CollisionRadius, Field.TryResponseCollision, 1, trajectoryCollisionsCount);
            }
            traj.CalculateFullWayClean(laserOnScene.parent.position, mouseClampedDirection);
            var corner = traj.Corners[0];
            burnWorldPos = corner.Endpoint;
            if (!IsSlicing) return;
            if (corner.CollisionReason != Gameplay.User.CollisionType.IntoBunch) return;
            var place = Field.GiveBubbleByDirection(ref burnWorldPos, corner.Direction);
            if (!place.Valid || !place.Busy)
            {
                TargetBubble = null;
                return;
            }
            foreach(var damaged in underAttack)
            {
                if (damaged.fieldPlace.Line == place.Line && damaged.fieldPlace.Column == place.Column)
                {
                    TargetBubble = damaged;
                    return;
                }
            }
            TargetBubble = new DamagedBubble(place, this.BubbleResistFrames);
            underAttack.Add(TargetBubble);
        }
        
        void RotateRifle()
        {
            if (!laserSpite) laserSpite = laserOnScene.GetComponent<SpriteRenderer>();
            laserOnScene.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(mouseClampedDirection.x, mouseClampedDirection.y));
        }
        
        public override void ReactOnFieldMove()
        {
            base.ReactOnFieldMove();
            var newLinesCount = Field.LineCount;
            if (oldLinesCount != newLinesCount)
            {
                Field.TryChangeLinesPosInDamaged(ref underAttack, oldLinesCount);
                oldLinesCount = newLinesCount;
            }
            
        }
    }
}