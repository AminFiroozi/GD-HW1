using UnityEngine;

namespace Game.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public GameObject menuPanel;

        private void Start()
        {
            Time.timeScale = 0f;

            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
            }
        }

        public void PlayGame()
        {
            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }

            Time.timeScale = 1f;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
