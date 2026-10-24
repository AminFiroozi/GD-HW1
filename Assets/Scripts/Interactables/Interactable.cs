using UnityEngine;
using Game.Input;

namespace Game.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        public float speedMultiplier = 1f;

        protected virtual void Update()
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

        public abstract void OnPlayerInteract();
    }
}
