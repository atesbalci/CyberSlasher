using System;
using System.Linq;
using DG.Tweening;
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
        private Sequence _sequence;
        private Material _bladeMaterial;
        private int _streak;

        private void Start()
        {
            _bladeMaterial = _bladeRenderer.material;
        }

        private void Update()
        {
            var cutoff = _bladeMaterial.GetFloat("_Cutoff");
            cutoff = Mathf.MoveTowards(cutoff, _streak >= StreakRequirement ? 1f : 0f, Time.deltaTime * 5f);
            _bladeMaterial.SetFloat("_Cutoff", cutoff);

            if (Active != null && Active())
                return;
            float dist;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var tipPlane = new Plane(_tipPlane.forward, _tipPlane.position);
            var pt = RaycastPlane(tipPlane, ray);
            if (pt != null)
            {
                var point = pt.Value;
                point.y = Mathf.Max(_minHeight, point.y);
                var center = RaycastPlane(tipPlane, new Ray(transform.position, _tipPlane.forward)) ?? Vector3.zero;
                var finalPoint = center + (point - center).normalized * _radius;
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(finalPoint - transform.position), Time.deltaTime * 400f);
                Debug.DrawLine(finalPoint, finalPoint + Vector3.up * 0.2f);
            }
        }

        private static Vector3? RaycastPlane(Plane plane, Ray ray)
        {
            float dist;
            if (plane.Raycast(ray, out dist))
                return ray.GetPoint(dist);
            return null;
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