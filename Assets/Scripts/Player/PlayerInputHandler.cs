using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public bool JumpPressed { get; private set; }
        public bool DashHeld { get; private set; }
        public bool DuckHeld { get; private set; }

        public event Action JumpPerformed;
        public event Action PausePerformed;

        private PlayerControls m_Controls;

        private void Awake()
        {
            m_Controls = new PlayerControls();
        }

        private void OnEnable()
        {
            if (m_Controls == null)
            {
                m_Controls = new PlayerControls();
            }

            m_Controls.Enable();

            m_Controls.Player.Jump.performed += OnJumpPerformed;
            m_Controls.Player.Dash.performed += OnDashPerformed;
            m_Controls.Player.Dash.canceled += OnDashCanceled;
            m_Controls.Player.Duck.performed += OnDuckPerformed;
            m_Controls.Player.Duck.canceled += OnDuckCanceled;
            m_Controls.Player.Pause.performed += OnPausePerformed;
        }

        private void OnDisable()
        {
            m_Controls.Player.Jump.performed -= OnJumpPerformed;
            m_Controls.Player.Dash.performed -= OnDashPerformed;
            m_Controls.Player.Dash.canceled -= OnDashCanceled;
            m_Controls.Player.Duck.performed -= OnDuckPerformed;
            m_Controls.Player.Duck.canceled -= OnDuckCanceled;
            m_Controls.Player.Pause.performed -= OnPausePerformed;

            m_Controls.Disable();
        }

        private void OnDestroy()
        {
            m_Controls.Dispose();
        }

        private void LateUpdate()
        {
            JumpPressed = false;
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            JumpPressed = true;
            JumpPerformed?.Invoke();
        }

        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            DashHeld = true;
        }

        private void OnDashCanceled(InputAction.CallbackContext context)
        {
            DashHeld = false;
        }

        private void OnDuckPerformed(InputAction.CallbackContext context)
        {
            DuckHeld = true;
        }

        private void OnDuckCanceled(InputAction.CallbackContext context)
        {
            DuckHeld = false;
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            PausePerformed?.Invoke();
        }
    }
}
