using UnityEngine;

namespace Gameplay.User
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bomb : MonoBehaviour
    {
        [field:SerializeField] public float PhysicalSize   { get; private set; }
        [field:SerializeField] public float ExplodeRadius  { get; private set; }
        Rigidbody2D myRigid;
        System.Action<Vector3> reactor;
        
        internal void Drop(Vector3 fallSpeed, System.Action<Vector3> reactOnDrop)
        {
            reactor = reactOnDrop;
            myRigid.bodyType = RigidbodyType2D.Dynamic;
            myRigid.velocity = fallSpeed;
        }

        internal void WakeUpAndFreeze()
        {
            gameObject.SetActive(true);
            myRigid ??= GetComponent<Rigidbody2D>();
            myRigid.bodyType = RigidbodyType2D.Kinematic;
        }
        
        void OnCollisionEnter2D(Collision2D col)
        {
            if (reactor == null) return;
            reactor.Invoke(transform.position);
            gameObject.SetActive(false);
            reactor = null;
        }
    }
}