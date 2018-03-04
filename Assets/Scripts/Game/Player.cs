using UnityEngine;
using Utility;

namespace Game
{
    [RequireComponent(typeof(GameThirdPerson))]
    public class Player : MonoBehaviour
    {
        public const float SlashDuration = 0.2f;
        public const float EnergyReplenishRate = 1f;

        [SerializeField] private SlashPlane _slashPlane;
        private GameThirdPerson _charController;
        private float _energy;

        private void Start()
        {
            _charController = GetComponent<GameThirdPerson>();
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
        }
    }
}