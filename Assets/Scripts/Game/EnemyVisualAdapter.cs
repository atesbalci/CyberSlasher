using UnityEngine;

namespace Game
{
    public class EnemyVisualAdapter : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        private Enemy _enemy;
        private Material _material;
        private Color _color;
        private Color _inactiveColor;

        private void Start()
        {
            _enemy = GetComponent<Enemy>();
            _material = _renderer.material;
            _color = _material.color;
            _inactiveColor = Color.Lerp(_color, Color.black, 0.5f);
        }

        private void Update()
        {
            _material.color = Vector4.MoveTowards(_material.color, !_enemy.IsPastPlayer && transform.position.z < Player.SwordRange ? _color : _inactiveColor, Time.deltaTime * 10f);
            if (_enemy.IsDead)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * 10f);
                if (transform.localScale.sqrMagnitude < 0.001f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
