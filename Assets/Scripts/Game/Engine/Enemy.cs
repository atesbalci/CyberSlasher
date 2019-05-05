using Helpers;
using UnityEngine;
using Zenject;

namespace Game.Engine
{
    public abstract class Enemy : MonoBehaviour, ISlashable
    {
        public Collider Collider { get; private set; }
        public EnemyPath Path { get; set; }

        public bool IsDead
        {
            get { return _isDead; }
            protected set
            {
                if (_isDead != value && value)
                {
                    MessageManager.SendEvent(new EnemyDefeatedEvent { Enemy = this });
                }
                _isDead = value;
            }
        }

        public bool IsPastPlayer { get; protected set; }
        public EnemyType Type { get; set; }

        private Vector3 _prevPos;
        private Player _player;
        
        protected abstract float Damage { get; }
        public abstract void Hit(HitType hitType);

        [Inject]
        public void Initialize(Player player)
        {
            _player = player;
            Collider = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            GameRegistry.Spawn(this);
            _prevPos = transform.position;
            IsDead = false;
            IsPastPlayer = false;
        }

        private void Update()
        {
            var pos = transform.position;
            var direction = pos - _prevPos;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.LookAt(transform.position + direction, Vector3.up);
                _prevPos = pos;
            }

            if (transform.position.z < 0.5f && !IsPastPlayer)
            {
                var directionSign = Path == EnemyPath.Left ? -1f : 1f;
                if (!(_player.LeanState > 0f != directionSign > 0f && Mathf.Abs(_player.LeanState) > 0.5f))
                {
                    MessageManager.SendEvent(new PlayerDamagedEvent { Damage = Damage, Source = this });
                }
                IsPastPlayer = true;
                GameRegistry.Remove(this);
            }
        }

        private void OnDisable()
        {
            GameRegistry.Remove(this);
        }
    }

    public enum EnemyPath
    {
        Left, Right
    }

    public class PlayerDamagedEvent : GameEvent
    {
        public float Damage { get; set; }
        public Enemy Source { get; set; }
    }

    public enum EnemyType
    {
        None, Minion, Boss
    }

    public class EnemyDefeatedEvent : GameEvent
    {
        public Enemy Enemy { get; set; }
    }
}
