using UnityEngine;

namespace Gameplay.Instruments
{
    [System.Serializable]
    public partial class SniperShot : BaseInstrument
    {
        [SerializeField] private Transform _rifleOnScene;
        [SerializeField] private float _bulletSpeed;
        [SerializeField] private Transform _bulletOnScene;
        [SerializeField] private GameObject _aimLine;
        private SpriteRenderer _rifleSprite;
        private Transform _rifleParent;
        private bool _shotProcessed;
        private bool _userPressed;
        private Coroutine _turnOffRoutine, _showRoutine;
        
        public override bool RequireDrawTrajectory => false;
    }
}