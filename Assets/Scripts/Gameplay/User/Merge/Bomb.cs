using UnityEngine;

namespace Gameplay.User
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bomb : MonoBehaviour
    {
        [field:SerializeField] public float PhysicalSize   { get; private set; }
        [field:SerializeField] public float ExplodeRadius  { get; private set; }
        private Rigidbody2D _myRigid;
        private System.Action<Vector3> _reactor;
        
        public void Drop(Vector3 fallSpeed, System.Action<Vector3> reactOnDrop)
        {
            _reactor = reactOnDrop;
            _myRigid.bodyType = RigidbodyType2D.Dynamic;
            _myRigid.velocity = fallSpeed;
        }

        public void WakeUpAndFreeze()
        {
            gameObject.SetActive(true);
            if (_myRigid == null) _myRigid = GetComponent<Rigidbody2D>();
            _myRigid.bodyType = RigidbodyType2D.Kinematic;
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (_reactor == null) return;
            _reactor.Invoke(transform.position);
            gameObject.SetActive(false);
            _reactor = null;
        }
    }
}