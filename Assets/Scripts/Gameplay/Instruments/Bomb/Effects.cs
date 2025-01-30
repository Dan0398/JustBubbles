using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments.Bomb
{
    [System.Serializable]
    public class Effects : MonoBehaviour
    {
        //[HideInInspector] public bool RequireOff;
        //[Header("Explosions"), SerializeField] Services.Audio.AudioWrapper idleSound;
        /*
        [SerializeField] AudioSource flySound, explosionSound;
        */
        [SerializeField] ParticleSystem explosionEffect;
        [SerializeField] Gameplay.Effects.CameraShaker Shaker;
        //WaitForSeconds LongWait;
        WaitForFixedUpdate Wait = new();
        //Coroutine BombIdleSoundRoutine, TurnOffRoutine;
        
        /*
        public void PlayBombIdleSound(float VolumeIncreaseDuration)
        {
            idleSound.ChangeVolume(0);
            idleSound.Play();
            if (BombIdleSoundRoutine != null)
            {
                StopCoroutine(BombIdleSoundRoutine);
            }
            BombIdleSoundRoutine = StartCoroutine(IncreaseIdleSound(VolumeIncreaseDuration));
        }
        */
        /*
        IEnumerator IncreaseIdleSound(float VolumeIncreaseDuration, bool Reversed = false)
        {
            if (!Reversed) idleSound.Play();
            int Steps = Mathf.FloorToInt(VolumeIncreaseDuration / Time.fixedDeltaTime);
            for (int i = 0; i <= Steps; i++)
            {
                float Lerp = i/(float)Steps;
                if (Reversed) Lerp = 1 - Lerp;
                idleSound.ChangeVolume(Lerp);
                yield return Wait;
            }
            if (Reversed) idleSound.Stop();
            //BombIdleSoundRoutine = null;
        }
        */
        
        /*
        public void StopBombIdleSound(float Duration = 0)
        {
            if (BombIdleSoundRoutine != null)
            {
                StopCoroutine(BombIdleSoundRoutine);
            }
            if (Duration == 0)
            {
                idleSound.Stop();
                return;
            }
            BombIdleSoundRoutine = StartCoroutine(IncreaseIdleSound(Duration, true));
        }
        */
        //public void PlayBombFlySound() => flySound.Play();
        
        public void PlayExplosionEffects(Vector3 ExplosionWorldPos)
        {
            /*
            StopBombIdleSound();
            flySound.Stop();
            explosionSound.Play();
            */
            //TurnOffRoutine = StartCoroutine(WaitExplosionEndAndTryOff());
            
            explosionEffect.transform.position = new Vector3(ExplosionWorldPos.x, ExplosionWorldPos.y, -0.5f);
            explosionEffect.Emit(1);
            //StartCoroutine(ApplyShake(Random.insideUnitCircle.normalized, 1.5f));
            Shaker.ApplyShake(Random.insideUnitCircle.normalized, 1.5f);
        }
        /*
        IEnumerator WaitExplosionEndAndTryOff()
        {
            if (LongWait == null) LongWait = new WaitForSeconds(explosionSound.clip.length);
            yield return LongWait;
            if (RequireOff)
            {
                RequireOff = false;
                gameObject.SetActive(false);
                StopCoroutine(TurnOffRoutine);
                TurnOffRoutine = null;
            }
        }
        
        public void TryStopTurnOff()
        {
            if (TurnOffRoutine != null) 
            {
                StopCoroutine(TurnOffRoutine);
                RequireOff = false;
            }
        }
        */
    }
}