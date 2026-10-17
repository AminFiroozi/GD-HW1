using System;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public class PlayerControls : IDisposable
    {
        private readonly InputActionMap m_Player;
        private readonly InputAction m_Player_Jump;
        private readonly InputAction m_Player_Dash;
        private readonly InputAction m_Player_Pause;

        public PlayerControls()
        {
            m_Player = new InputActionMap("Player");
            m_Player_Jump = m_Player.AddAction("Jump", InputActionType.Button, "<Keyboard>/space");
            m_Player_Dash = m_Player.AddAction("Dash", InputActionType.Button, "<Keyboard>/shift");
            m_Player_Pause = m_Player.AddAction("Pause", InputActionType.Button, "<Keyboard>/escape");
        }

        public struct PlayerActions
        {
            private readonly PlayerControls m_Wrapper;

            public PlayerActions(PlayerControls wrapper)
            {
                m_Wrapper = wrapper;
            }

            public InputAction Jump => m_Wrapper.m_Player_Jump;
            public InputAction Dash => m_Wrapper.m_Player_Dash;
            public InputAction Pause => m_Wrapper.m_Player_Pause;

            public void Enable() => m_Wrapper.m_Player.Enable();
            public void Disable() => m_Wrapper.m_Player.Disable();
            public bool enabled => m_Wrapper.m_Player.enabled;
        }

        public PlayerActions Player => new PlayerActions(this);

        public void Enable() => m_Player.Enable();
        public void Disable() => m_Player.Disable();

        public void Dispose()
        {
            m_Player.Disable();
            m_Player.Dispose();
        }
    }
}
