using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class GameOverController : MonoBehaviour
    {
        public GameObject gameOverPanel;
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private TMP_Text highScoreText;

        public void Show()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            var gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                return;
            }

            if (finalScoreText != null)
            {
                finalScoreText.text = $"Score: {gameManager.Score:0}";
            }

            if (highScoreText != null)
            {
                highScoreText.text = $"High Score: {gameManager.HighScore:0}";
            }
        }

        public void RestartGame()
        {
            GameManager.Instance?.RestartGame();
        }
    }
}
