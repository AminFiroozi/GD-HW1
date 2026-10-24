using UnityEngine;
using Game.Input;

namespace Game.Background
{
    public class DecorativeSand : MonoBehaviour
    {
        public float speedMultiplier = 1f;

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
            {
                return;
            }

            transform.Translate(Vector3.left * GameSpeed.ScrollSpeed * speedMultiplier * Time.deltaTime);

            if (Camera.main.WorldToViewportPoint(transform.position).x < -0.1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
