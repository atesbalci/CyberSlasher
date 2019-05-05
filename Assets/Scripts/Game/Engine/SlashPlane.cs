using System.Collections.Generic;
using Game.Models;
using Helpers;
using UnityEngine;
using Zenject;

namespace Game.Engine
{
    public class SlashPlane : MonoBehaviour
    {
        private Vector3? _slashStart;
        private Camera _camera;
        private SlashStateData _slashStateData;
        private IList<Vector3> _vertexes;

        public bool Active
        {
            get => _slashStateData.Type.Value != SlashType.Inactive;
            set
            {
                if (value == Active)
                    return;
                _slashStart = value ? _slashStart : null;
                if (!value)
                {
                    ApplyHits();
                }
                _slashStateData.Type.Value = value ? SlashType.Active : SlashType.Inactive;
                GameTime.Slashing = value;
            }
        }

        [Inject]
        public void Initialize(SlashStateData slashStateData)
        {
            _slashStateData = slashStateData;
            _camera = Camera.main;
            _vertexes = new List<Vector3>();
        }

        private void Update()
        {
            if (Active)
            {
                var trans = transform;
                var plane = new Plane(trans.forward, trans.position);
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out var hitDist))
                {
                    var pos = ray.GetPoint(hitDist);
                    if (_slashStart == null)
                    {
                        _slashStart = pos;
                        _vertexes.Clear();
                    }
                    _vertexes.Add(pos);
                    _slashStateData.Position.Value = pos;
                }
            }
        }

        private void ApplyHits()
        {
            var slashees = new List<SlashInfo>();
            foreach (var slashable in GameRegistry.Slashables)
            {
                var bounds = Utils.ToScreenRectFromWorldBounds(slashable.Collider.bounds);
                Vector3? enterPos = null;
                Vector3? exitPos = null;
                for (var i = 1; i < _vertexes.Count; i++)
                {
                    var p1 = _camera.WorldToScreenPoint(_vertexes[i - 1]);
                    p1.z = 0f;
                    var p2 = _camera.WorldToScreenPoint(_vertexes[i]);
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
                        var ex = _camera.WorldToScreenPoint(_vertexes[_vertexes.Count - 1]);
                        ex.z = 0f;
                        exitPos = ex;
                    }
                    var slashablePos = slashable.Collider.transform.position;
                    slashablePos.y = 0f;
                    var pos = transform.parent.position;
                    pos.y = 0f;
                    var plane = new Plane((slashablePos - pos).normalized, slashablePos);
                    var ray1 = _camera.ScreenPointToRay(enterPos.Value);
                    var ray2 = _camera.ScreenPointToRay(exitPos.Value);
                    plane.Raycast(ray1, out var hit1);
                    plane.Raycast(ray2, out var hit2);
                    var p1 = ray1.GetPoint(hit1);
                    var p2 = ray2.GetPoint(hit2);
                    slashees.Add(new SlashInfo
                    {
                        Slashee = slashable,
                        EnterPosition = p1,
                        ExitPosition = p2
                    });
                }
            }

            //Direction calculation
            var totalVec = Vector3.zero;
            var totalDist = 0f;
            for (var i = 1; i < _vertexes.Count; i++)
            {
                var p1 = _camera.WorldToScreenPoint(_vertexes[i - 1]);
                p1.z = 0f;
                var p2 = _camera.WorldToScreenPoint(_vertexes[i]);
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
        public List<SlashInfo> Slashees { get; set; }
        public Vector3 AverageDirection { get; set; }
    }

    public class SlashInfo
    {
        public ISlashable Slashee { get; set; }
        public Vector3 EnterPosition { get; set; }
        public Vector3 ExitPosition { get; set; }
    }
}