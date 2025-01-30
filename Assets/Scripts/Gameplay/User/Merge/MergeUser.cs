using System.Collections;
using Gameplay.Merge;
using UnityEngine;

namespace Gameplay.User
{
    public class MergeUser : BaseUser<MergeField>
    {
        [SerializeField] Vector3 DropSpeed;
        [SerializeField] float RotateSpeed;
        [SerializeField] End GameOverPlace;
        [SerializeField] MergeTrajectory Trajectory;
        [SerializeField] Bomb MergeBomb;
        [SerializeField] ParticleSystem BombEffects;
        Transform Replacable;
        Vector2 WorldClampedPos;
        Unit Holded;
        float HeightOnScene, outstandRadius;
        bool ReplacableInAnimation, usedBomb, clicked, worked;
        Services.Audio.Sounds.Service Sounds;
        WaitForFixedUpdate Wait;
        
        protected override void Start()
        {
            base.Start();
            HeightOnScene = transform.position.y;
        }

        protected override void ReactPointerMove()
        {
            WorldClampedPos = new Vector2(Mathf.Clamp(MouseWorldPos.x, -Field.HalfXSize + outstandRadius, Field.HalfXSize - outstandRadius), HeightOnScene);
            Trajectory.Replace(WorldClampedPos.x);
            if (Paused) return;
            if (ReplacableInAnimation) return;
            if (Replacable != null) Replacable.position = WorldClampedPos;
        }

        protected override void BindInputs()
        {
            Inputs.BaseMap.Clicked.performed += (s) => ReactClickStart();
            Inputs.BaseMap.Clicked.canceled += (s) => ReactClickEnd();
        }
        
        private void ReactClickStart()
        {
            if (!IsClickedInGameField()) return;
            clicked = true;
            if (!worked) return;
            if (Paused) return;
            if (ReplacableInAnimation) return;
            Trajectory.Show();
        }
        
        private void ReactClickEnd()
        {
            if (!clicked) return;
            clicked = false;
            if (!worked) return;
            if (Paused) return;
            Trajectory.Hide();
            if (!IsClickedInGameField()) return;
            if (ReplacableInAnimation) return;
            //if (Holded == null) return;
            StartCoroutine(TryDropAndSwitch());
        }
        
        IEnumerator TryDropAndSwitch()
        {
            if (usedBomb)
            {
                Sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
                Sounds.Stop(Services.Audio.Sounds.SoundType.BombIdle);
                Sounds.Play(Services.Audio.Sounds.SoundType.BombFly);
                MergeBomb.Drop(DropSpeed, ReactOnDropBomb);
                Trajectory.StopBombReplacing();
                Replacable = Holded.transform;
                Holded.gameObject.SetActive(true);
                Trajectory.ChangeWidth(Holded.PhysicalSize);
                usedBomb = false;
            }
            else
            {    
                if (Holded != null)
                {
                    Holded.WakeUp();
                    Holded.PrepareForScene(WorldClampedPos, 0, DropSpeed, RotateSpeed);
                    Holded.SwitchGravityTo(true);
                    Field.RegisterByUserDrop(Holded);
                    Sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
                    Sounds.Play(Services.Audio.Sounds.SoundType.Merge_Drop);
                    StartCoroutine(RegisterForGameOver(Holded.GetComponent<Collider2D>()));
                    Holded = null;
                }
                TakeNewUnit();
                ReactPointerMove();
            }
            yield return AnimateReplacablePos(0.5f);

            void ReactOnDropBomb(Vector3 FallPosition)
            {
                BombEffects.transform.position = FallPosition;
                BombEffects.Play();
                Sounds.Stop(Services.Audio.Sounds.SoundType.BombFly);
                Sounds.Play(Services.Audio.Sounds.SoundType.BombExplode);
                Trajectory.HideBombView();
                Field.ProcessExplosion(FallPosition, MergeBomb.ExplodeRadius);
            }
        }
        
        IEnumerator RegisterForGameOver(Collider2D col)
        {
            yield return new WaitForSecondsRealtime(1f);
            GameOverPlace.RemoveAsIgnore(col);
        }
        
        void TakeNewUnit()
        {
            Holded = Field.GiveUnit(FastPow(Random.Range(0, 3)));
            Holded.SwitchGravityTo(false);
            Holded.Sleep();
            Replacable = Holded.transform;
            Replacable.position = WorldClampedPos;
            Replacable.rotation = Quaternion.identity;
            Trajectory.ChangeWidth(Holded.PhysicalSize);
            GameOverPlace.RegisterAsIgnore(Holded.GetComponent<Collider2D>()); 
            outstandRadius = Holded.PhysicalSize * 0.5f;
            
            int FastPow(int i)
            {
                if (i == 0) return 1;
                if (i == 1) return 2;
                return 4;
            }
        }
        
        IEnumerator AnimateReplacablePos(float Duration = 0.5f, bool Inverted = false, System.Action OnEnd = null)
        {
            Wait ??= new ();
            ReplacableInAnimation = true;
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = EasingFunction.EaseInSine(0,1, i/(float)Steps);
                if (Inverted) Lerp = 1 - Lerp;
                Replacable.position = Vector3.Lerp(new Vector3(WorldClampedPos.x, 5 + outstandRadius), WorldClampedPos, Lerp);
                yield return Wait;
            }
            ReplacableInAnimation = false;
            OnEnd?.Invoke();
            if (clicked && !Inverted) 
            {
                Trajectory.Show();
            }
        }

        public override void StartGameplayAndAnimate(float Duration = 1)
        {
            var pt = UnityEngine.InputSystem.Pointer.current;
            ApplyPointerPos(pt.position.ReadValue());
            worked = true;
            TakeNewUnit();
            StartCoroutine(AnimateReplacablePos(Duration, false, Unpause));
        }

        public override void StopGameplayAndAnimate(float Duration = 1, System.Action OnEnd = null)
        {
            worked = false;
            Trajectory.Hide();
            if (Holded == null)
            {
                OnEnd?.Invoke();
                return;
            }
            StartCoroutine(AnimateReplacablePos(Duration, true, () =>
            {
                CleanDropper();
                Pause();
                OnEnd?.Invoke();
            }));
            
            void CleanDropper()
            {
                if (Holded == null) return;
                Field.Hide(Holded);
                Holded = null;
            }
        }

        internal void UseBomb()
        {
            StartCoroutine(AnimateReplacablePos(0.5f, true, ActivateUseBomb));
            
            void ActivateUseBomb()
            {
                usedBomb = true;
                Holded.gameObject.SetActive(false);
                Replacable = MergeBomb.transform;
                MergeBomb.WakeUpAndFreeze();
                StartCoroutine(AnimateReplacablePos(0.5f));
                StartCoroutine(IncreaseBombIdleSound(0.5f));
                Trajectory.ActivateBombView();
                Trajectory.ChangeWidth(MergeBomb.PhysicalSize*0.5f);
            }
        }
        
        IEnumerator IncreaseBombIdleSound(float Duration)
        {
            Sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
            var ChangeBombIdleVolume = Sounds.PlayAndGiveVolumeChange(Services.Audio.Sounds.SoundType.BombIdle);
            
            Wait ??= new();
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            for (int i = 0; i < Steps; i++)
            {
                ChangeBombIdleVolume.Invoke(i/(float)Steps);
                yield return Wait;
            }
        }
        
        public void FastTurnOff()
        {
            Pause();
            Replacable.gameObject.SetActive(false);
        }
        
        public void FastTurnOn()
        {
            Replacable.gameObject.SetActive(true);
            Unpause();
        }
    }
}