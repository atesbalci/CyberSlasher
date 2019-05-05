using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Helpers
{
    [RequireComponent(typeof(FirstPersonController))]
    public class ThirdPersonActivenessSetter : MonoBehaviour
    {
        private FirstPersonController _controller;
        private Vector2 _defaultSensitivity;

        private void Awake()
        {
            _controller = GetComponent<FirstPersonController>();
            _defaultSensitivity = new Vector2(_controller.m_MouseLook.XSensitivity, _controller.m_MouseLook.YSensitivity);
        }

        public bool FirstPersonActive
        {
            get { return _controller.m_MouseLook.lockCursor; }
            set
            {
                _controller.m_MouseLook.SetCursorLock(value);
                _controller.m_MouseLook.XSensitivity = value ? _defaultSensitivity.x : 0f;
                _controller.m_MouseLook.YSensitivity = value ? _defaultSensitivity.y : 0f;
            }
        }
    }
}
