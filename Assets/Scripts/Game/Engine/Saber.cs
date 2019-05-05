using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Game.Engine
{
    public class Saber : MonoBehaviour
    {
        private const int StreakRequirement = 2;

        [SerializeField] private Transform[] _horizontalPath;
        [SerializeField] private Transform[] _verticalPath;
        [SerializeField] private Renderer _bladeRenderer;
        private Sequence _sequence;
        private Material _bladeMaterial;
        private int _streak;

        public SaberState State
        {
            get
            {
                if (_sequence != null && _sequence.IsActive())
                    return SaberState.Swinging;
                return Vector3.Distance(transform.localPosition, _horizontalPath[0].localPosition) < 0.01f
                    ? SaberState.Left
                    : SaberState.Right;
            }
        }

        private void Start()
        {
            _bladeMaterial = _bladeRenderer.material;
            transform.localPosition = _horizontalPath[0].localPosition;
            transform.localRotation = _horizontalPath[0].localRotation;
        }

        private void Update()
        {
            var cutoff = _bladeMaterial.GetFloat("_Cutoff");
            cutoff = Mathf.MoveTowards(cutoff, _streak >= StreakRequirement ? 1f : 0f, Time.deltaTime * 5f);
            _bladeMaterial.SetFloat("_Cutoff", cutoff);
        }

        public HitType Swing(Vector2 direction, bool hit)
        {
            var state = State;
            if (state == SaberState.Swinging)
            {
                _streak = 0;
                return HitType.Miss;
            }

            _sequence = DOTween.Sequence();

            direction = direction.normalized;
            bool swingingLeft;
            if (_streak >= StreakRequirement && direction.y < -0.5f)
            {
                _streak = 0;
                foreach (var pathNode in _verticalPath)
                {
                    _sequence.Append(transform.DOLocalMove(pathNode.localPosition, 0.1f)
                        .SetEase(Ease.Linear));
                    _sequence.Join(transform.DOLocalRotateQuaternion(pathNode.localRotation, 0.1f)
                        .SetEase(Ease.Linear));
                }

                var lastTarget = state == SaberState.Left ? _horizontalPath.Last() : _horizontalPath.First();
                _sequence.Append(transform.DOLocalMove(lastTarget.localPosition, 0.1f).SetEase(Ease.Linear));
                _sequence.Join(transform.DOLocalRotateQuaternion(lastTarget.localRotation, 0.1f).SetEase(Ease.Linear));
                _sequence.SetEase(Ease.Linear);
                _sequence.Play();
                return HitType.Super;
            }

            if (direction.x > 0.5f && state == SaberState.Left)
                swingingLeft = false;
            else if (direction.x < -0.5f && state == SaberState.Right)
                swingingLeft = true;
            else
                return HitType.Miss;

            for (var i = swingingLeft ? _horizontalPath.Length - 2 : 1;
                i < _horizontalPath.Length && i >= 0;
                i += swingingLeft ? -1 : 1)
            {
                _sequence.Append(transform.DOLocalMove(_horizontalPath[i].localPosition, 0.1f).SetEase(Ease.Linear));
                _sequence.Join(
                    transform.DOLocalRotateQuaternion(_horizontalPath[i].rotation, 0.1f).SetEase(Ease.Linear));
            }

            _sequence.SetEase(Ease.Linear);
            _sequence.Play();
            _streak = (_streak + 1) % (StreakRequirement + 1);
            return HitType.Regular;
        }
    }

    public enum SaberState
    {
        Left,
        Right,
        Swinging
    }

    public enum HitType
    {
        Miss,
        Regular,
        Super
    }
}