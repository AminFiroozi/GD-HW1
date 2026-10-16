using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int Lives { get; private set; } = 3;
        public float Score { get; private set; } = 0f;
        public bool IsGameOver { get; private set; } = false;
        public float HighScore { get; private set; } = 0f;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            HighScore = PlayerPrefs.GetFloat("HighScore", 0f);
        }

        public void LoseLife()
        {
            if (IsGameOver)
            {
                return;
            }

            Lives--;
            Debug.Log($"Lost a life, lives remaining: {Lives}");

            if (Lives <= 0)
            {
                TriggerGameOver();
            }
        }

        public void AddScore(float amount)
        {
            if (IsGameOver)
            {
                return;
            }

            Score += amount;
            if (Score > HighScore)
            {
                HighScore = Score;
                PlayerPrefs.SetFloat("HighScore", HighScore);
            }
            Debug.Log($"Score: {Score}");
        }

private void TriggerGameOver()
        {
            IsGameOver = true;
            Time.timeScale = 0f;
            Debug.Log("Game Over");

            var playerMovement = FindFirstObjectByType<Game.Input.PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.PlayDeathAnimation();
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
