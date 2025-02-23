using System.Collections;
using Gameplay.Merge;
using UnityEngine;

namespace Gameplay.User
{
    public class MergeUser : BaseUser<MergeField>
    {
        [SerializeField] private Vector3 _dropSpeed;
        [SerializeField] private float _rotateSpeed;
        [SerializeField] private End _gameOverPlace;
        [SerializeField] private MergeTrajectory _trajectory;
        [SerializeField] private Bomb _mergeBomb;
        [SerializeField] private ParticleSystem _bombEffects;
        private Transform _replacable;
        private Vector2 _worldClampedPos;
        private Unit _holded;
        private float _heightOnScene, _outstandRadius;
        private bool _replacableInAnimation, _usedBomb, _clicked, _worked;
        private Services.Audio.Sounds.Service _sounds;
        private WaitForFixedUpdate _wait;
        
        protected override void Start()
        {
            base.Start();
            _heightOnScene = transform.position.y;
        }

        protected override void ReactPointerMove()
        {
            _worldClampedPos = new Vector2(Mathf.Clamp(MouseWorldPos.x, -Field.HalfXSize + _outstandRadius, Field.HalfXSize - _outstandRadius), _heightOnScene);
            _trajectory.Replace(_worldClampedPos.x);
            if (Paused) return;
            if (_replacableInAnimation) return;
            if (_replacable != null) _replacable.position = _worldClampedPos;
        }

        protected override void BindInputs()
        {
            Inputs.BaseMap.Clicked.performed += (s) => ReactClickStart();
            Inputs.BaseMap.Clicked.canceled += (s) => ReactClickEnd();
        }
        
        private void ReactClickStart()
        {
            if (!IsClickedInGameField()) return;
            _clicked = true;
            if (!_worked) return;
            if (Paused) return;
            if (_replacableInAnimation) return;
            _trajectory.Show();
        }
        
        private void ReactClickEnd()
        {
            if (!_clicked) return;
            _clicked = false;
            if (!_worked) return;
            if (Paused) return;
            _trajectory.Hide();
            if (!IsClickedInGameField()) return;
            if (_replacableInAnimation) return;
            StartCoroutine(TryDropAndSwitch());
        }
        
        private IEnumerator TryDropAndSwitch()
        {
            if (_usedBomb)
            {
                _sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
                _sounds.Stop(Services.Audio.Sounds.SoundType.BombIdle);
                _sounds.Play(Services.Audio.Sounds.SoundType.BombFly);
                _mergeBomb.Drop(_dropSpeed, ReactOnDropBomb);
                _trajectory.StopBombReplacing();
                _replacable = _holded.transform;
                _holded.gameObject.SetActive(true);
                _trajectory.ChangeWidth(_holded.PhysicalSize);
                _usedBomb = false;
            }
            else
            {    
                if (_holded != null)
                {
                    _holded.WakeUp();
                    _holded.PrepareForScene(_worldClampedPos, 0, _dropSpeed, _rotateSpeed);
                    _holded.SwitchGravityTo(true);
                    Field.RegisterByUserDrop(_holded);
                    _sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
                    _sounds.Play(Services.Audio.Sounds.SoundType.Merge_Drop);
                    StartCoroutine(RegisterForGameOver(_holded.GetComponent<Collider2D>()));
                    _holded = null;
                }
                TakeNewUnit();
                ReactPointerMove();
            }
            yield return AnimateReplacablePos(0.5f);

            void ReactOnDropBomb(Vector3 FallPosition)
            {
                _bombEffects.transform.position = FallPosition;
                _bombEffects.Play();
                _sounds.Stop(Services.Audio.Sounds.SoundType.BombFly);
                _sounds.Play(Services.Audio.Sounds.SoundType.BombExplode);
                _trajectory.HideBombView();
                Field.ProcessExplosion(FallPosition, _mergeBomb.ExplodeRadius);
            }
        }
        
        private IEnumerator RegisterForGameOver(Collider2D col)
        {
            yield return new WaitForSecondsRealtime(1f);
            _gameOverPlace.RemoveAsIgnore(col);
        }
        
        private void TakeNewUnit()
        {
            _holded = Field.GiveUnit(FastPow(Random.Range(0, 3)));
            _holded.SwitchGravityTo(false);
            _holded.Sleep();
            _replacable = _holded.transform;
            _replacable.position = _worldClampedPos;
            _replacable.rotation = Quaternion.identity;
            _trajectory.ChangeWidth(_holded.PhysicalSize);
            _gameOverPlace.RegisterAsIgnore(_holded.GetComponent<Collider2D>()); 
            _outstandRadius = _holded.PhysicalSize * 0.5f;

            static int FastPow(int i)
            {
                if (i == 0) return 1;
                if (i == 1) return 2;
                return 4;
            }
        }
        
        private IEnumerator AnimateReplacablePos(float Duration = 0.5f, bool Inverted = false, System.Action OnEnd = null)
        {
            _wait ??= new ();
            _replacableInAnimation = true;
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = EasingFunction.EaseInSine(0,1, i/(float)Steps);
                if (Inverted) Lerp = 1 - Lerp;
                _replacable.position = Vector3.Lerp(new Vector3(_worldClampedPos.x, 5 + _outstandRadius), _worldClampedPos, Lerp);
                yield return _wait;
            }
            _replacableInAnimation = false;
            OnEnd?.Invoke();
            if (_clicked && !Inverted) 
            {
                _trajectory.Show();
            }
        }

        public override void StartGameplayAndAnimate(float Duration = 1)
        {
            var pt = UnityEngine.InputSystem.Pointer.current;
            ApplyPointerPos(pt.position.ReadValue());
            _worked = true;
            TakeNewUnit();
            StartCoroutine(AnimateReplacablePos(Duration, false, Unpause));
        }

        public override void StopGameplayAndAnimate(float Duration = 1, System.Action OnEnd = null)
        {
            _worked = false;
            _trajectory.Hide();
            if (_holded == null)
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
                if (_holded == null) return;
                Field.Hide(_holded);
                _holded = null;
            }
        }

        public void UseBomb()
        {
            StartCoroutine(AnimateReplacablePos(0.5f, true, ActivateUseBomb));
            
            void ActivateUseBomb()
            {
                _usedBomb = true;
                _holded.gameObject.SetActive(false);
                _replacable = _mergeBomb.transform;
                _mergeBomb.WakeUpAndFreeze();
                StartCoroutine(AnimateReplacablePos(0.5f));
                StartCoroutine(IncreaseBombIdleSound(0.5f));
                _trajectory.ActivateBombView();
                _trajectory.ChangeWidth(_mergeBomb.PhysicalSize*0.5f);
            }
        }
        
        private IEnumerator IncreaseBombIdleSound(float Duration)
        {
            _sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
            var ChangeBombIdleVolume = _sounds.PlayAndGiveVolumeChange(Services.Audio.Sounds.SoundType.BombIdle);
            
            _wait ??= new();
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            for (int i = 0; i < Steps; i++)
            {
                ChangeBombIdleVolume.Invoke(i/(float)Steps);
                yield return _wait;
            }
        }
        
        public void FastTurnOff()
        {
            Pause();
            _replacable.gameObject.SetActive(false);
        }
        
        public void FastTurnOn()
        {
            _replacable.gameObject.SetActive(true);
            Unpause();
        }
    }
}