using UnityEngine;

namespace Game
{
    public class Enemy : MonoBehaviour, ISlashable
    {
        public Collider Collider { get; set; }

        private void Start()
        {
            Collider = GetComponent<Collider>();
            GameRegistry.Slashables.Add(this);
        }
    }
}
