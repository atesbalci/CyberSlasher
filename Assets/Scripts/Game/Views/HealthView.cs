using Game.Engine;
using UnityEngine;

namespace Game.Views
{
    [RequireComponent(typeof(Player))]
    public class HealthView : MonoBehaviour
    {
        private static readonly Color FullHealthColor = Color.green;
        private static readonly Color NoHealthColor = Color.red;

        [SerializeField] private Renderer _renderer;
        [SerializeField] private int _materialIndex;

        private Player _player;
        private Material _material;

        private void Start()
        {
            _player = GetComponent<Player>();
            _material = _renderer.materials[_materialIndex];
        }

        private void Update()
        {
            _material.color = Color.Lerp(NoHealthColor, FullHealthColor, _player.Health);
        }
    }
}
