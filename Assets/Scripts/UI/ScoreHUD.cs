using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class ScoreHUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreLivesText;
        [SerializeField] private TMP_Text highScoreText;

        private void Update()
        {
            var gameManager = Game.GameManager.Instance;
            if (gameManager == null)
            {
                return;
            }

            if (scoreLivesText != null)
            {
                scoreLivesText.text = $"Score: {gameManager.Score:0}  Lives: {gameManager.Lives}";
            }

            if (highScoreText != null)
            {
                highScoreText.text = $"High Score: {gameManager.HighScore:0}";
            }
        }
    }
}
