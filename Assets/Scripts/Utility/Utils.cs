using UnityEngine;

namespace Utility
{
    public static class Utils
    {
        public static Bounds ToScreenRectFromWorldBounds(Bounds bounds)
        {
            var cam = Camera.main;
            Vector3 cen = bounds.center;
            Vector3 ext = bounds.extents;
            Vector2[] extentPoints = {
                cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
                cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
                cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
                cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
                cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
                cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
                cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
                cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
            };
            Vector2 min = extentPoints[0];
            Vector2 max = extentPoints[0];
            foreach (Vector2 v in extentPoints)
            {
                min = Vector2.Min(min, v);
                max = Vector2.Max(max, v);
            }
            var size = new Vector3(max.x - min.x, max.y - min.y, 0f);
            return new Bounds(new Vector3(min.x + size.x / 2f, min.y + size.y / 2f, 0f), size);
        }

        public static Vector3? RaycastPlane(this Plane plane, Ray ray)
        {
            float dist;
            if (plane.Raycast(ray, out dist))
                return ray.GetPoint(dist);
            return null;
        }
    }
}
