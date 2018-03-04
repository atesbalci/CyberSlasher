using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Utility;

namespace Game
{
    public class SlashPlane : MonoBehaviour
    {
        [SerializeField] private Transform _slash;
        private TrailRenderer _slashTrail;
        private bool _active;
        private Vector3? _slashStart;

        public bool Active
        {
            get { return _active; }
            set
            {
                if (value == Active)
                    return;
                _slashStart = value ? _slashStart : null;
                if (!value)
                {
                    ApplyHits();
                }
                _active = value;
            }
        }

        private void Start()
        {
            _slashTrail = _slash.GetComponentInChildren<TrailRenderer>();
        }

        private void Update()
        {
            if (Active)
            {
                var cam = Camera.main;
                var plane = new Plane(transform.forward, transform.position);
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                float hitDist;
                if (plane.Raycast(ray, out hitDist))
                {
                    _slash.position = ray.GetPoint(hitDist);
                    if (_slashStart == null)
                    {
                        _slashStart = _slash.position;
                    }
                }
            }
        }

        //TODO: (Haram) This function incorporates trail renderer vertexes with the slashing logic, that shouldn't be the case. It could be fixed by having these vertexes be kept in a separate place.
        public void ApplyHits()
        {
            var slashees = new List<Tuple<ISlashable, float>>();
            var cam = Camera.main;
            foreach (var slashable in GameRegistry.Slashables)
            {
                var bounds = Utils.ToScreenRectFromWorldBounds(slashable.Collider.bounds);
                Vector3? enterPos = null;
                Vector3? exitPos = null;
                for (var i = 1; i < _slashTrail.positionCount; i++)
                {
                    var p1 = cam.WorldToScreenPoint(_slashTrail.GetPosition(i - 1));
                    p1.z = 0f;
                    var p2 = cam.WorldToScreenPoint(_slashTrail.GetPosition(i));
                    p2.z = 0f;
                    if (enterPos == null)
                    {
                        if (bounds.Intersects(new Bounds((p1 + p2) / 2f,
                            new Vector3(Mathf.Abs(p1.x - p2.x), Mathf.Abs(p1.y - p2.y)))))
                        {
                            enterPos = bounds.ClosestPoint(p1);
                        }
                    }
                    else if(!bounds.Contains(p2))
                    {
                        exitPos = bounds.ClosestPoint(p2);
                    }
                }

                //Act if a TECHNICALLY successful slash was executed (let the game logic decide if it was logic-wise actually successful)
                if (enterPos != null)
                {
                    if (exitPos == null) //Set exit pos to last position if not set already
                    {
                        var ex = cam.WorldToScreenPoint(_slashTrail.GetPosition(_slashTrail.positionCount - 1));
                        ex.z = 0f;
                        exitPos = ex;
                    }
                    var slashablePos = slashable.Collider.transform.position;
                    slashablePos.y = 0f;
                    var pos = transform.parent.position;
                    pos.y = 0f;
                    var plane = new Plane((slashablePos - pos).normalized, slashablePos);
                    var ray1 = cam.ScreenPointToRay(enterPos.Value);
                    var ray2 = cam.ScreenPointToRay(exitPos.Value);
                    float hit1, hit2;
                    plane.Raycast(ray1, out hit1);
                    plane.Raycast(ray2, out hit2);
                    var p1 = ray1.GetPoint(hit1);
                    var p2 = ray2.GetPoint(hit2);
                    Debug.DrawLine(p1, p2, Color.red, 1f, false);
                    slashees.Add(new Tuple<ISlashable, float>(slashable, Vector3.Distance(p1, p2)));
                }
            }

            //Direction calculation
            var totalVec = Vector3.zero;
            var totalDist = 0f;
            for (var i = 1; i < _slashTrail.positionCount; i++)
            {
                var p1 = cam.WorldToScreenPoint(_slashTrail.GetPosition(i - 1));
                p1.z = 0f;
                var p2 = cam.WorldToScreenPoint(_slashTrail.GetPosition(i));
                p2.z = 0f;
                totalVec += p2 - p1;
                totalDist += Vector3.Distance(p1, p2);
            }
            var dir = (totalVec / totalDist).normalized;

            //Send out event
            MessageManager.SendEvent(new SlashCompletedEvent
            {
                Slashees = slashees,
                AverageDirection = dir
            });
        }
    }

    public class SlashCompletedEvent : GameEvent
    {
        public List<Tuple<ISlashable, float>> Slashees { get; set; }
        public Vector3 AverageDirection { get; set; }
    }
}