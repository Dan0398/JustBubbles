using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Pools;
using UnityEngine;

namespace Gameplay.Merge
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Unit : MonoBehaviour, IWithTransform
    {
        public Vector3 OriginalScale;
        [field:SerializeField] public int Point             { get; private set; }
        [field:SerializeField] public float PhysicalSize    { get; private set; }
        [NonSerialized] public MergeField Field;
        public bool MergeBlocked;
        Rigidbody2D myRigid;
        SaveModel.UnitStatus BeforeSleep;
        bool sleeping;

        public Transform MyTransform => transform;
        
        public Vector2 velocity => myRigid.velocity;
        public float angularSpeed => myRigid.angularVelocity;

        public void PrepareForScene(Vector2 worldPos, float YRot = 0, Vector2 speed = default, float angularSpeed = 0)
        {
            gameObject.SetActive(true);
            myRigid ??= GetComponent<Rigidbody2D>();
            if (!sleeping)
            {
                myRigid.velocity = speed;
                myRigid.angularVelocity = angularSpeed;
            }
            Replace(worldPos, YRot);
        }
        
        public void Replace(Vector2 worldPos, float YPos = 0)
        {
            transform.position = worldPos;
            transform.eulerAngles = Vector3.forward * YPos;
        }
        
        void FixedUpdate()
        {
            if (sleeping) myRigid.Sleep();
            MergeBlocked = false;
        }
        
        internal void SwitchGravityTo(bool Falling)
        {
            myRigid ??= GetComponent<Rigidbody2D>();
            myRigid.gravityScale = Falling? 1 : 0;
        }
        
        public void Sleep()
        {
            if (sleeping) return;
            myRigid ??= GetComponent<Rigidbody2D>();
            BeforeSleep = CaptureStatus();
            myRigid.bodyType = RigidbodyType2D.Static;
            GetComponent<Collider2D>().enabled = false;
            sleeping = true;
        }
        
        public void WakeUp()
        {
            if (!sleeping) return;
            GetComponent<Collider2D>().enabled = true;
            myRigid ??= GetComponent<Rigidbody2D>();
            myRigid.bodyType = RigidbodyType2D.Dynamic;
            PrepareForScene(BeforeSleep.Pos.ToVector(), BeforeSleep.Ang, BeforeSleep.Vel.ToVector(), BeforeSleep.AngVel);
            sleeping = false;
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (MergeBlocked) return;
            if (!col.gameObject.activeSelf) return;
            if (!gameObject.activeSelf) return;
            Field.ReactOnCollision(this, col);
        }

        internal SaveModel.UnitStatus CaptureStatus() => new SaveModel.UnitStatus()
        {
            ID = Point,
            Pos = new SaveModel.SerVector2(transform.position),
            Ang = Mathf.RoundToInt(transform.eulerAngles.z),
            Vel = new SaveModel.SerVector2(myRigid.velocity),
            AngVel = Mathf.RoundToInt(myRigid.angularVelocity)
        };
    }
}