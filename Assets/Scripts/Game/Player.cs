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

        private void Start()
        {
            _saber.Active += () => _charController.FirstPersonActive;
            _charController = GetComponent<GameThirdPerson>();
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
                    Debug.DrawLine(slashInfo.EnterPosition, slashInfo.ExitPosition, Color.red, 1f, false);
                }
            });
        }

        private void Update()
        {
            _charController.FirstPersonActive = Input.GetKey(KeyCode.Mouse1);

            if (!_slashPlane.Active && Input.GetKeyDown(KeyCode.Mouse0))
                _slashPlane.Active = true;

            if (_slashPlane.Active)
                _slashPlane.Active = Input.GetKey(KeyCode.Mouse0);
        }
    }
}