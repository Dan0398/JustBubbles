using System.Collections.Generic;
using System.Collections;
using Utils.Observables;
using Gameplay.Effects;
using Gameplay.Pools;
using UnityEngine;

namespace Gameplay.Merge
{
    public class MergeField : MonoBehaviour, IField
    {
        public float HalfXSize { get; private set; }
        public bool RequireSave;
        [SerializeField] private Barrier _barriers;
        [SerializeField] private ContactSounds _contactSounds;
        [SerializeField] private End _gameOverPlace;
        [SerializeField] private ParticleSystem _mergeEffects;
        [SerializeField] private CameraShaker _camShaker;
        private Pools.MergePool[] _pools;
        private List<Unit> _unitsOnScene;
        private SaveModel _model;
        private Services.Audio.Sounds.Service _sounds;
        private WaitForFixedUpdate _wait;
        
        public ObsFloat GameOverRelative => _gameOverPlace.RelativeFillChanged;
        
        public void ShowAnimated(Content.Merge.ViewConfig actual, SaveModel saveModel, float Duration = 1f)
        {
            _model = saveModel;
            _sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
            _contactSounds.Setup(actual.CollisionSound);
            _gameOverPlace.Clean();
            PreparePools(actual);
            StartCoroutine(Animate(saveModel, Duration));
        }
        
        private IEnumerator Animate(SaveModel saveModel, float Duration = 1f)
        {
            yield return _barriers.ShowAndResizeAnimated(saveModel.FieldSize, _gameOverPlace, Duration / 2f);
            HalfXSize = _barriers.Width * 0.5f;
            LoadOldUnits(saveModel);
            foreach(var unit in _unitsOnScene)
            {
                StartCoroutine(AnimateUnit(unit, Duration/2f, true));
            }
        }
        
        private IEnumerator AnimateUnit(Unit target, float Duration = 1f, bool SleepRequired = false)
        {
            _wait ??= new();
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            if (SleepRequired) target.Sleep();
            for (int i = 1; i <= Steps; i++)
            {
                var Lerp = EasingFunction.EaseOutElastic(0,1, i/(float)Steps);
                target.transform.localScale = Lerp * target.OriginalScale;
                yield return _wait;
            }
            if (SleepRequired) target.WakeUp();
        }
        
        private void PreparePools(Content.Merge.ViewConfig Config)
        {
            if (_pools == null)
            {
                _pools = new Pools.MergePool[Config.Items.Length];
            }
            else if (_pools.Length != Config.Items.Length)
            {
                System.Array.Resize(ref _pools, Config.Items.Length);
            }
            for (int i = 0; i < Config.Items.Length; i++)
            {
                var unit = Config.Items[i].Sample.GetComponent<Unit>();
                if (_pools[i] == null)
                {
                    var poolObj = new GameObject("Pool #" + unit.Point);
                    poolObj.transform.SetParent(transform);
                    _pools[i] = poolObj.AddComponent<MergePool>();
                }
                _pools[i].Init(unit);
            }
        }
        
        private void LoadOldUnits(SaveModel saveModel)
        {
            _unitsOnScene ??= new List<Unit>(saveModel.Units.Length);
            foreach(var unitData in saveModel.Units)
            {
                var newUnit = GiveUnit(unitData.ID);
                newUnit.PrepareForScene(unitData.Pos.ToVector(), unitData.Ang, unitData.Vel.ToVector(), unitData.AngVel);
                newUnit.SwitchGravityTo(true);
                _unitsOnScene.Add(newUnit);
            }
        }
        
        public Unit GiveUnit(int Point)
        {
            foreach(var pool in _pools)
            {
                if (pool.PointOfSample != Point) continue;
                var Unit = pool.GiveItem();
                Unit.transform.SetParent(transform);
                Unit.Field = this;
                return Unit;
            }
            return null;
        }
        
        public void RegisterByUserDrop(Unit dropped)
        {
            RequireSave = true;
            _unitsOnScene.Add(dropped);
        } 

        public void ReactOnCollision(Unit unit, Collision2D col)
        {
            if (col.gameObject.TryGetComponent<Unit>(out Unit side) && unit.Point == side.Point && !side._mergeBlocked)
            {
                side._mergeBlocked = true;
                
                var midPoint = (unit.transform.position + side.transform.position) * 0.5f;
                var newPoints = unit.Point + side.Point;
                var midSpeed = (unit.Velocity + side.Velocity) * 0.5f;
                var midAngular = (unit.AngularSpeed + side.AngularSpeed) * 0.5f;
                
                _unitsOnScene.Remove(unit);
                Hide(unit);
                
                _unitsOnScene.Remove(side);
                Hide(side);
                
                var newUnit = GiveUnit(newPoints);
                if (newUnit != null)
                {
                    newUnit.WakeUp();
                    newUnit.PrepareForScene(midPoint, 0, midSpeed, midAngular);
                    newUnit.SwitchGravityTo(true);
                    _unitsOnScene.Add(newUnit);
                    StartCoroutine(AnimateUnit(newUnit, 1f));
                }
                _model.Points.Value += newPoints / 2;
                
                var Money = FastLog2(newPoints / 2) - 2;
                if (Money > 0) _model.Money.Value += Money;
                
                _mergeEffects.transform.position = midPoint;
                _mergeEffects.Emit(30);
                _sounds.Play(Services.Audio.Sounds.SoundType.Merge);
                
                RequireSave = true;
            }
            else
            {
                float Volume = Mathf.Clamp01(col.relativeVelocity.magnitude / 16f);
                _contactSounds.Play(Volume);
            }

            static int FastLog2(int Value)
            {
                     if (Value == 2)    return 1;
                else if (Value == 4)    return 2;
                else if (Value == 8)    return 3;
                else if (Value == 16)   return 4;
                else if (Value == 32)   return 5;
                else if (Value == 64)   return 6;
                else if (Value == 128)  return 7;
                else if (Value == 256)  return 8;
                else if (Value == 512)  return 9;
                else if (Value == 1024) return 10;
                else if (Value == 2048) return 11;
                else if (Value == 4096) return 12;
                else if (Value == 8192) return 13;
                else if (Value == 16386)return 14;
                else if (Value == 32768)return 15;
                else if (Value == 65536)return 16;
                return 0;
            }
        }
        
        public void Hide(Unit onScene)
        {
            foreach(var pool in _pools)
            {
                if (pool.PointOfSample != onScene.Point) continue;
                pool.Hide(onScene);
                return;
            }
        }

        public void SyncUnits(SaveModel slot)
        {
            SaveModel.UnitStatus[] Units = new SaveModel.UnitStatus[_unitsOnScene.Count];
            for (int i = 0; i < _unitsOnScene.Count; i++)
            {
                Units[i] = _unitsOnScene[i].CaptureStatus();
            }
            slot.Units = Units;
        }

        public bool IsPositionInsideField(Vector2 Input) => _barriers.IsPosInside(Input);
        
        public void Deactivate(float Duration = 1f)
        {
            if (_unitsOnScene != null)
            {
                for (int i = 0; i < _unitsOnScene.Count; i++)
                {
                    Hide(_unitsOnScene[i]);
                    _unitsOnScene[i] = null;
                }
                _unitsOnScene.Clear();
            }
            _barriers.Hide();
            _gameOverPlace.Hide(Duration);
        }
        
        public void RunShaker(System.Action AfterEnd)
        {
            StartCoroutine(AnimateShaker(AfterEnd));
        }
        
        private IEnumerator AnimateShaker(System.Action AfterEnd)
        {
            var Sounds = Services.DI.Single<Services.Audio.Sounds.Service>();
            _gameOverPlace.Hide(0.2f);
            yield return AnimateScale();
            Sounds.Play(Services.Audio.Sounds.SoundType.Merge_Shaker);
            yield return _barriers.Shake();
            Sounds.Stop(Services.Audio.Sounds.SoundType.Merge_Shaker);
            yield return AnimateScale(true);
            StartCoroutine(ActivateGameOverDelayed());
            AfterEnd?.Invoke();
            
            IEnumerator AnimateScale(bool Inverted = false)
            {
                for (int i = 0; i <= 15; i++)
                {
                    float Lerp = EasingFunction.EaseInSine(0,1, i/15f);
                    if (!Inverted) Lerp = 1 - Lerp;
                    transform.localScale = (0.5f + 0.5f * Lerp) * Vector3.one;
                    yield return _wait;
                }
            }
            
            IEnumerator ActivateGameOverDelayed()
            {
                yield return new WaitForSeconds(1f);
                _gameOverPlace.ResetAndShow(0.2f);
            }
        }

        public void ProcessExplosion(Vector3 fallPosition, float explodeRadius)
        {
            List<ExplodedUnit> exploded = new();
            _camShaker.ApplyShake(Vector2.right, 1);
            for (int i = 0; i < _unitsOnScene.Count; i++)
            {
                var Dist = (_unitsOnScene[i].transform.position - fallPosition).magnitude;
                Dist -= _unitsOnScene[i].PhysicalSize*0.5f;
                if (Dist >= explodeRadius) continue;
                exploded.Add(new ExplodedUnit(_unitsOnScene[i], Dist));
                _unitsOnScene.RemoveAt(i);
                i--;
            }
            if (exploded.Count == 0) return;
            exploded.Sort();
            StartCoroutine(AnimateExplosion(exploded));
        }
        
        private IEnumerator AnimateExplosion(List<ExplodedUnit> targetList)
        {
            for (int i = 0; i < targetList.Count; i++)
            {
                StartCoroutine(targetList[i].AnimateExplode(_wait, Hide));
                yield return _wait;
                yield return _wait;
            }
        }
    }
}