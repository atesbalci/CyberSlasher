using System;
using System.Linq;
using DG.Tweening;
using Game.Models;
using Helpers;
using UniRx;
using UnityEngine;

namespace Game.Engine
{
    [RequireComponent(typeof(ThirdPersonActivenessSetter))]
    public class Player : MonoBehaviour
    {
        public const float SwordRange = 3f;
        private const float SlashDuration = 0.2f;
        private const float EnergyReplenishRate = 1f;
        private const float MaxLeanAmount = 1f;
        private static readonly Color RegularHitColor = new Color(1f, 0.5f, 0f);
        private static readonly Color SuperHitColor = Color.red;

        public float Health { get; set; }

        [SerializeField] private SlashPlane _slashPlane;
        [SerializeField] private Saber _saber;
        [Space(10)] [SerializeField] private TrailRenderer _slashTrailPrefab;
        private ThirdPersonActivenessSetter _charController;
        private float _energy;
        private Sequence _leaningSequence;

        private void Start()
        {
            Health = 1f;
            _charController = GetComponent<ThirdPersonActivenessSetter>();
            _charController.FirstPersonActive = false;
            MessageManager.ReceiveEvent<SlashCompletedEvent>().Subscribe(ev =>
            {
                var slashees = ev.Slashees.Where(info => info.Slashee.Collider.transform.position.z < SwordRange).ToList();
                var hit = _saber.Swing(ev.AverageDirection, slashees.Count > 0);
                if (hit == HitType.Miss)
                    return;
                foreach (var slashInfo in slashees)
                {
                    var trail = Instantiate(_slashTrailPrefab, slashInfo.EnterPosition, Quaternion.identity);
                    trail.material.color = hit == HitType.Super ? SuperHitColor : RegularHitColor;
                    trail.transform.DOMove(slashInfo.ExitPosition, 0.2f);
                    Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(l => Destroy(trail.gameObject)).AddTo(trail.gameObject);
                    slashInfo.Slashee.Hit(hit);
                    break; //nice solution lol
                }
            }).AddTo(gameObject);
            MessageManager.ReceiveEvent<PlayerDamagedEvent>().Subscribe(ev => Health -= ev.Damage).AddTo(gameObject);
        }

        public float LeanState
        {
            get { return transform.localPosition.x / MaxLeanAmount; }
        }

        public void Lean(float lean)
        {
            if (_leaningSequence != null && _leaningSequence.ElapsedPercentage() > 0.8f)
            {
                _leaningSequence.Complete();
                _leaningSequence = null;
            }
            if (_leaningSequence == null)
            {
                _leaningSequence = DOTween.Sequence();
                _leaningSequence.Append(transform.DOLocalMove((lean < -0.5f ? Vector3.left : Vector3.right) * MaxLeanAmount, 0.5f)
                    .SetRelative().SetEase(Ease.OutSine));
                _leaningSequence.Append(transform.DOLocalMove(Vector3.zero, 0.1f));
                _leaningSequence.OnKill(() =>
                {
                    _leaningSequence = null;
                });
                _leaningSequence.OnComplete(() =>
                {
                    _leaningSequence = null;
                });
            }
        }

        private void LateUpdate()
        {
            _charController.FirstPersonActive = Input.GetKey(KeyCode.Mouse1);

            if (!_slashPlane.Active && Input.GetMouseButtonDown(0)
                                    && Mathf.Approximately(_energy, SlashDuration)
                                    && !EventSystem.current.currentSelectedGameObject)
                _slashPlane.Active = true;

            if (_slashPlane.Active)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (_energy > 0f)
                        _energy -= Time.deltaTime;
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

            if (Input.GetKeyDown(KeyCode.A))
            {
                Lean(-1f);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Lean(1f);
            }
        }
    }
}
