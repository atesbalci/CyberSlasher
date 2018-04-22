using System;
using DG.Tweening;
using UnityEngine;
using Utility;
using UniRx;

namespace Game
{
    [RequireComponent(typeof(GameThirdPerson))]
    public class Player : MonoBehaviour
    {
        private const float SlashDuration = 0.2f;
        private const float EnergyReplenishRate = 1f;
        private static readonly Color RegularHitColor = new Color(1f, 0.5f, 0f);
        private static readonly Color SuperHitColor = Color.red;

        [SerializeField] private SlashPlane _slashPlane;
        [SerializeField] private Saber _saber;
        [Space(10)] [SerializeField] private TrailRenderer _slashTrailPrefab;
        private GameThirdPerson _charController;
        private float _energy;
        private Sequence _leaningSequence;

        private void Start()
        {
            _charController = GetComponent<GameThirdPerson>();
            _charController.FirstPersonActive = false;
            MessageManager.ReceiveEvent<SlashCompletedEvent>().Subscribe(ev =>
            {
                var hit = _saber.Swing(ev.AverageDirection, ev.Slashees.Count > 0);
                if (hit == HitType.Miss)
                    return;
                foreach (var slashInfo in ev.Slashees)
                {
                    var trail = Instantiate(_slashTrailPrefab, slashInfo.EnterPosition, Quaternion.identity);
                    trail.material.color = hit == HitType.Super ? SuperHitColor : RegularHitColor;
                    trail.transform.DOMove(slashInfo.ExitPosition, 0.2f);
                    Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(l => Destroy(trail.gameObject)).AddTo(trail.gameObject);
                    break; //nice solution lol
                }
            });
        }

        private void Lean(bool left)
        {
            if (_leaningSequence != null && _leaningSequence.ElapsedPercentage() > 0.8f)
            {
                _leaningSequence.Complete();
                _leaningSequence = null;
            }
            if (_leaningSequence == null)
            {
                _leaningSequence = DOTween.Sequence();
                _leaningSequence.Append(transform.DOLocalMove((left ? Vector3.left : Vector3.right) * 0.5f, 0.1f)
                    .SetRelative());
                _leaningSequence.Append(transform.DOLocalMove(Vector3.zero, 0.1f));
                _leaningSequence.OnKill(() => _leaningSequence = null);
                _leaningSequence.OnComplete(() => _leaningSequence = null);
            }
        }

        private void Update()
        {
            _charController.FirstPersonActive = Input.GetKey(KeyCode.Mouse1);

            if (!_slashPlane.Active && Input.GetKeyDown(KeyCode.Mouse0)
                                    && Mathf.Approximately(_energy, SlashDuration))
                _slashPlane.Active = true;

            if (_slashPlane.Active)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (_energy > 0f)
                        _energy -= GameTime.DeltaTime;
                    else
                        _slashPlane.Active = false;
                }
                else
                    _slashPlane.Active = false;
            }
            else
            {
                _energy = Mathf.Min(_energy + GameTime.DeltaTime * EnergyReplenishRate,
                    SlashDuration);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Lean(false);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                Lean(true);
            }
        }
    }
}