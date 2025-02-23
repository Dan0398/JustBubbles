using Gameplay.Pools;
using UnityEngine;
using System;

namespace Gameplay.Merge
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Unit : MonoBehaviour, IWithTransform
    {
        public Vector3 OriginalScale;
        [field:SerializeField] public int Point             { get; private set; }
        [field:SerializeField] public float PhysicalSize    { get; private set; }
        [NonSerialized] public MergeField Field;
        public bool _mergeBlocked;
        private Rigidbody2D _myRigid;
        private SaveModel.UnitStatus _beforeSleep;
        private bool _sleeping;

        public Transform MyTransform => transform;
        
        public Vector2 Velocity => _myRigid.velocity;
        
        public float AngularSpeed => _myRigid.angularVelocity;

        public void PrepareForScene(Vector2 worldPos, float YRot = 0, Vector2 speed = default, float angularSpeed = 0)
        {
            gameObject.SetActive(true);
            _myRigid ??= GetComponent<Rigidbody2D>();
            if (!_sleeping)
            {
                _myRigid.velocity = speed;
                _myRigid.angularVelocity = angularSpeed;
            }
            Replace(worldPos, YRot);
        }
        
        public void Replace(Vector2 worldPos, float YPos = 0)
        {
            transform.position = worldPos;
            transform.eulerAngles = Vector3.forward * YPos;
        }
        
        private void FixedUpdate()
        {
            if (_sleeping) _myRigid.Sleep();
            _mergeBlocked = false;
        }
        
        public void SwitchGravityTo(bool Falling)
        {
            _myRigid ??= GetComponent<Rigidbody2D>();
            _myRigid.gravityScale = Falling? 1 : 0;
        }
        
        public void Sleep()
        {
            if (_sleeping) return;
            _myRigid ??= GetComponent<Rigidbody2D>();
            _beforeSleep = CaptureStatus();
            _myRigid.bodyType = RigidbodyType2D.Static;
            GetComponent<Collider2D>().enabled = false;
            _sleeping = true;
        }
        
        public void WakeUp()
        {
            if (!_sleeping) return;
            GetComponent<Collider2D>().enabled = true;
            _myRigid ??= GetComponent<Rigidbody2D>();
            _myRigid.bodyType = RigidbodyType2D.Dynamic;
            PrepareForScene(_beforeSleep.Pos.ToVector(), _beforeSleep.Ang, _beforeSleep.Vel.ToVector(), _beforeSleep.AngVel);
            _sleeping = false;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (_mergeBlocked) return;
            if (!col.gameObject.activeSelf) return;
            if (!gameObject.activeSelf) return;
            Field.ReactOnCollision(this, col);
        }

        public SaveModel.UnitStatus CaptureStatus() => new SaveModel.UnitStatus()
        {
            ID = Point,
            Pos = new SaveModel.SerVector2(transform.position),
            Ang = Mathf.RoundToInt(transform.eulerAngles.z),
            Vel = new SaveModel.SerVector2(_myRigid.velocity),
            AngVel = Mathf.RoundToInt(_myRigid.angularVelocity)
        };
    }
}