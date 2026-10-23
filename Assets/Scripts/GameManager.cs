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
        public float DistanceTraveled { get; private set; } = 0f;

        public float ScorePerDistance = 1f;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            HighScore = PlayerPrefs.GetFloat("HighScore", 0f);
            Game.Input.GameSpeed.Reset();
        }

        private void Update()
        {
            if (IsGameOver)
            {
                return;
            }

            float deltaDistance = Game.Input.GameSpeed.ScrollSpeed * Time.deltaTime;
            DistanceTraveled += deltaDistance;
            Game.Input.GameSpeed.UpdateForDistance(DistanceTraveled);
            AddScore(deltaDistance * ScorePerDistance);
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
            Debug.Log("Game Over");

            var playerMovement = FindFirstObjectByType<Game.Input.PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.PlayDeathAnimation();
            }

            var gameOverController = FindFirstObjectByType<Game.UI.GameOverController>();

            StartCoroutine(ShowGameOverUIRoutine(playerMovement, gameOverController));
        }

        private System.Collections.IEnumerator ShowGameOverUIRoutine(Game.Input.PlayerMovement playerMovement, Game.UI.GameOverController gameOverController)
        {
            if (playerMovement != null)
            {
                yield return new WaitUntil(() => playerMovement.IsDeathSequenceReadyForUI);
            }

            Time.timeScale = 0f;

            if (gameOverController != null)
            {
                gameOverController.Show();
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
