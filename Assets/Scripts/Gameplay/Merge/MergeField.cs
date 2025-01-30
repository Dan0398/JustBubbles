using System.Collections.Generic;
using System.Collections;
using Utils.Observables;
using Gameplay.Pools;
using UnityEngine;
using Gameplay.Effects;

namespace Gameplay.Merge
{
    public class MergeField : MonoBehaviour, IField
    {
        public bool RequireSave;
        [SerializeField] Barrier barriers;
        [SerializeField] ContactSounds contactSounds;
        [SerializeField] End GameOverPlace;
        [SerializeField] ParticleSystem MergeEffects;
        [SerializeField] CameraShaker CamShaker;
        Pools.MergePool[] Pools;
        List<Unit> unitsOnScene;
        SaveModel Model;
        
        public ObsFloat GameOverRelative => GameOverPlace.RelativeFillChanged;
        Services.Audio.Sounds.Service sounds;
        WaitForFixedUpdate Wait;
        
        public float HalfXSize { get; private set; }
        
        public void ShowAnimated(Content.Merge.ViewConfig actual, SaveModel saveModel, float Duration = 1f)
        {
            Model = saveModel;
            sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
            contactSounds.Setup(actual.CollisionSound);
            GameOverPlace.Clean();
            PreparePools(actual);
            StartCoroutine(Animate(saveModel, Duration));
        }
        
        IEnumerator Animate(SaveModel saveModel, float Duration = 1f)
        {
            yield return barriers.ShowAndResizeAnimated(saveModel.FieldSize, GameOverPlace, Duration / 2f);
            HalfXSize = barriers.Width * 0.5f;
            LoadOldUnits(saveModel);
            foreach(var unit in unitsOnScene)
            {
                StartCoroutine(AnimateUnit(unit, Duration/2f, true));
            }
        }
        
        IEnumerator AnimateUnit(Unit target, float Duration = 1f, bool SleepRequired = false)
        {
            Wait ??= new();
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            if (SleepRequired) target.Sleep();
            for (int i = 1; i <= Steps; i++)
            {
                var Lerp = EasingFunction.EaseOutElastic(0,1, i/(float)Steps);
                target.transform.localScale = Lerp * target.OriginalScale;
                yield return Wait;
            }
            if (SleepRequired) target.WakeUp();
        }
        
        void PreparePools(Content.Merge.ViewConfig Config)
        {
            if (Pools == null)
            {
                Pools = new Pools.MergePool[Config.Items.Length];
            }
            else if (Pools.Length != Config.Items.Length)
            {
                System.Array.Resize(ref Pools, Config.Items.Length);
            }
            for (int i = 0; i < Config.Items.Length; i++)
            {
                var unit = Config.Items[i].Sample.GetComponent<Unit>();
                if (Pools[i] == null)
                {
                    var poolObj = new GameObject("Pool #" + unit.Point);
                    poolObj.transform.SetParent(transform);
                    Pools[i] = poolObj.AddComponent<MergePool>();
                }
                Pools[i].Init(unit);
            }
        }
        
        void LoadOldUnits(SaveModel saveModel)
        {
            unitsOnScene ??= new List<Unit>(saveModel.Units.Length);
            foreach(var unitData in saveModel.Units)
            {
                var newUnit = GiveUnit(unitData.ID);
                newUnit.PrepareForScene(unitData.Pos.ToVector(), unitData.Ang, unitData.Vel.ToVector(), unitData.AngVel);
                newUnit.SwitchGravityTo(true);
                unitsOnScene.Add(newUnit);
            }
        }
        
        public Unit GiveUnit(int Point)
        {
            foreach(var pool in Pools)
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
            unitsOnScene.Add(dropped);
        } 

        internal void ReactOnCollision(Unit unit, Collision2D col)
        {
            if (col.gameObject.TryGetComponent<Unit>(out Unit side) && unit.Point == side.Point && !side.MergeBlocked)
            {
                side.MergeBlocked = true;
                
                var midPoint = (unit.transform.position + side.transform.position) * 0.5f;
                var newPoints = unit.Point + side.Point;
                var midSpeed = (unit.velocity + side.velocity) * 0.5f;
                var midAngular = (unit.angularSpeed + side.angularSpeed) * 0.5f;
                
                unitsOnScene.Remove(unit);
                Hide(unit);
                
                unitsOnScene.Remove(side);
                Hide(side);
                
                var newUnit = GiveUnit(newPoints);
                if (newUnit != null)
                {
                    newUnit.WakeUp();
                    newUnit.PrepareForScene(midPoint, 0, midSpeed, midAngular);
                    newUnit.SwitchGravityTo(true);
                    unitsOnScene.Add(newUnit);
                    StartCoroutine(AnimateUnit(newUnit, 1f));
                }
                Model.Points.Value += newPoints / 2;
                
                var Money = FastLog2(newPoints / 2) - 2;
                if (Money > 0) Model.Money.Value += Money;
                
                MergeEffects.transform.position = midPoint;
                MergeEffects.Emit(30);
                sounds.Play(Services.Audio.Sounds.SoundType.Merge);
                
                RequireSave = true;
            }
            else
            {
                float Volume = Mathf.Clamp01(col.relativeVelocity.magnitude / 16f);
                contactSounds.Play(Volume);
            }
            
            int FastLog2(int Value)
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
            foreach(var pool in Pools)
            {
                if (pool.PointOfSample != onScene.Point) continue;
                pool.Hide(onScene);
                return;
            }
        }

        internal void SyncUnits(SaveModel slot)
        {
            SaveModel.UnitStatus[] Units = new SaveModel.UnitStatus[unitsOnScene.Count];
            for (int i = 0; i < unitsOnScene.Count; i++)
            {
                Units[i] = unitsOnScene[i].CaptureStatus();
            }
            slot.Units = Units;
        }

        public bool IsPositionInsideField(Vector2 Input) => barriers.IsPosInside(Input);
        
        public void Deactivate(float Duration = 1f)
        {
            if (unitsOnScene != null)
            {
                for (int i = 0; i < unitsOnScene.Count; i++)
                {
                    Hide(unitsOnScene[i]);
                    unitsOnScene[i] = null;
                }
                unitsOnScene.Clear();
            }
            barriers.Hide();
            GameOverPlace.Hide(Duration);
        }
        
        public void RunShaker(System.Action AfterEnd)
        {
            StartCoroutine(AnimateShaker(AfterEnd));
        }
        
        IEnumerator AnimateShaker(System.Action AfterEnd)
        {
            var Sounds = Services.DI.Single<Services.Audio.Sounds.Service>();
            GameOverPlace.Hide(0.2f);
            yield return AnimateScale();
            Sounds.Play(Services.Audio.Sounds.SoundType.Merge_Shaker);
            yield return barriers.Shake();
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
                    yield return Wait;
                }
            }
            
            IEnumerator ActivateGameOverDelayed()
            {
                yield return new WaitForSeconds(1f);
                GameOverPlace.ResetAndShow(0.2f);
            }
        }

        internal void ProcessExplosion(Vector3 fallPosition, float explodeRadius)
        {
            var powDistance = explodeRadius * explodeRadius;
            List<ExplodedUnit> exploded = new List<ExplodedUnit>();
            CamShaker.ApplyShake(Vector2.right, 1);
            for (int i = 0; i < unitsOnScene.Count; i++)
            {
                var Dist = (unitsOnScene[i].transform.position - fallPosition).magnitude;
                Dist -= unitsOnScene[i].PhysicalSize*0.5f;
                if (Dist >= explodeRadius) continue;
                exploded.Add(new ExplodedUnit(unitsOnScene[i], Dist));
                //Hide(unitsOnScene[i]);
                unitsOnScene.RemoveAt(i);
                i--;
            }
            if (exploded.Count == 0) return;
            exploded.Sort();
            StartCoroutine(AnimateExplosion(exploded));
        }
        
        IEnumerator AnimateExplosion(List<ExplodedUnit> targetList)
        {
            //var WaitStep = new WaitForSeconds(0.15f);
            for (int i = 0; i < targetList.Count; i++)
            {
                StartCoroutine(targetList[i].AnimateExplode(Wait, Hide));
                yield return Wait;
                yield return Wait;
            }
        }
    }
}