using UnityEngine;

namespace Game.Engine
{
    public interface ISlashable
    {
        Collider Collider { get; }
        void Hit(HitType hitType);
    }
}
