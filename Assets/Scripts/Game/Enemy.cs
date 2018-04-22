using UnityEngine;

namespace Game
{
    public abstract class Enemy : MonoBehaviour, ISlashable
    {
        public Collider Collider { get; private set; }

        private Vector3 _prevPos;

        public abstract void Hit(HitType hitType);

        protected virtual void Start()
        {
            Collider = GetComponent<Collider>();
            GameRegistry.Slashables.Add(this);
            _prevPos = transform.position;
        }

        protected virtual void Update()
        {
            var direction = transform.position - _prevPos;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.LookAt(transform.position + direction, Vector3.up);
                _prevPos = transform.position;
            }
        }
    }
}
