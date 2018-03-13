using UnityEngine;
using Utility;

namespace Game
{
    public class SaberPlane
    {
        private readonly Transform _transform;
        private readonly float _radius;
        private readonly float _minHeight;

        public SaberPlane(Transform transform, float radius, float minHeight)
        {
            _transform = transform;
            _radius = radius;
            _minHeight = minHeight;
        }

        public Vector3? GetPointOnRadius(Ray ray)
        {
            var tipPlane = new Plane(_transform.forward, _transform.position);
            var pt = tipPlane.RaycastPlane(ray);
            if (pt == null)
                return null;
            var point = pt.Value;
            point.y = Mathf.Max(_minHeight, point.y);
            var center = tipPlane.RaycastPlane(new Ray(_transform.position - _transform.forward, _transform.forward)) ?? Vector3.zero;
            return center + (point - center).normalized * _radius;
        }

        public Vector3 GetPointOnRadius(Vector3 direction)
        {
            var tipPlane = new Plane(_transform.forward, _transform.position);
            var center = tipPlane.RaycastPlane(new Ray(_transform.position - _transform.forward, _transform.forward)) ?? Vector3.zero;
            return center + direction.normalized * _radius;
        }
    }
}
