using UnityEngine;

namespace Game
{
    public interface ISlashable
    {
        Collider Collider { get; }
        void Hit(HitType hitType);
    }
}
