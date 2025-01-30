using UnityEngine;

namespace Gameplay.Instruments
{
    [System.Serializable]
    public partial class SniperShot : BaseInstrument
    {
        public override bool RequireDrawTrajectory => false;
        [SerializeField] Transform rifleOnScene;
        [SerializeField] float BulletSpeed;
        [SerializeField] Transform BulletOnScene;
        [SerializeField] GameObject AimLine;
        SpriteRenderer rifleSprite;
        Transform rifleParent;
        bool ShotProcessed;
        bool userPressed;
        Coroutine turnOffRoutine, showRoutine;
    }
}