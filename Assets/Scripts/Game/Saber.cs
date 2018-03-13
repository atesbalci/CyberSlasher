using System;
using UnityEngine;

namespace Game
{
    public class Saber : MonoBehaviour
    {
        private const int StreakRequirement = 2;

        public event Func<bool> Active; 

        [SerializeField] private Renderer _bladeRenderer;
        [SerializeField] private Transform _tipPlane;
        [SerializeField] private float _minHeight;
        [SerializeField] private float _radius;
        private Material _bladeMaterial;
        private int _streak;
        private SaberPlane _saberPlane;

        private void Start()
        {
            _bladeMaterial = _bladeRenderer.material;
            _saberPlane = new SaberPlane(_tipPlane, _radius, _minHeight);
        }

        private void Update()
        {
            var cutoff = _bladeMaterial.GetFloat("_Cutoff");
            cutoff = Mathf.MoveTowards(cutoff, _streak >= StreakRequirement ? 1f : 0f, Time.deltaTime * 5f);
            _bladeMaterial.SetFloat("_Cutoff", cutoff);

            if (Active != null && Active())
                return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var finalPoint = _saberPlane.GetPointOnRadius(ray);
            if (finalPoint != null)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(finalPoint.Value - transform.position), Time.deltaTime * 400f);
                Debug.DrawLine(finalPoint.Value, finalPoint.Value + Vector3.up * 0.2f);
            }
        }

        public HitType Swing(Vector2 direction, bool hit)
        {
            return HitType.Miss;
        }
    }

    public enum HitType
    {
        Miss,
        Regular,
        Super
    }
}