using UnityEngine;
using Game.Input;

namespace Game.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        public GameObject pausePanel;

        private PlayerInputHandler m_InputHandler;
        private bool m_IsPaused;

        private void OnEnable()
        {
            m_InputHandler = FindFirstObjectByType<PlayerInputHandler>();
            if (m_InputHandler != null)
            {
                m_InputHandler.PausePerformed += OnPausePerformed;
            }
        }

        private void OnDisable()
        {
            if (m_InputHandler != null)
            {
                m_InputHandler.PausePerformed -= OnPausePerformed;
            }
        }

        private void OnPausePerformed()
        {
            if (m_IsPaused)
            {
                Resume();
                return;
            }

            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            {
                return;
            }

            m_IsPaused = true;
            Time.timeScale = 0f;

            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
        }

        public void Resume()
        {
            m_IsPaused = false;
            Time.timeScale = 1f;

            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }

        public void QuitToMenu()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
