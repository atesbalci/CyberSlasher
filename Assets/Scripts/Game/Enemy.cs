﻿using UnityEngine;
using Utility;

namespace Game
{
    public abstract class Enemy : MonoBehaviour, ISlashable
    {
        public Collider Collider { get; private set; }
        public EnemyPath Path { get; set; }

        public bool IsDead { get; protected set; }
        public bool IsPastPlayer { get; protected set; }

        private Vector3 _prevPos;
        
        protected abstract float Damage { get; }
        public abstract void Hit(HitType hitType);

        protected virtual void Start()
        {
            Collider = GetComponent<Collider>();
            GameRegistry.Spawn(this);
            _prevPos = transform.position;
            IsPastPlayer = false;
        }

        protected virtual void Update()
        {
            var direction = transform.position - _prevPos;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.LookAt(transform.position + direction, Vector3.up);
                _prevPos = transform.position;
            }

            if (transform.position.z < 0.5f && !IsPastPlayer)
            {
                var pl = FindObjectOfType<Player>();
                var directionSign = Path == EnemyPath.Left ? -1f : 1f;
                if (!(pl.LeanState > 0f != directionSign > 0f && Mathf.Abs(pl.LeanState) > 0.5f))
                {
                    MessageManager.SendEvent(new PlayerDamagedEvent { Damage = Damage });
                    Destroy(gameObject);
                }
                IsPastPlayer = true;
                GameRegistry.Remove(this);
            }
        }

        private void OnDestroy()
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
    }
}
