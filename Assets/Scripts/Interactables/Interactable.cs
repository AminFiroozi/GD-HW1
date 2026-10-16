using UnityEngine;
using Game.Input;

namespace Game.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        protected virtual void Update()
        {
            transform.Translate(Vector3.left * GameSpeed.ScrollSpeed * Time.deltaTime);

            if (Camera.main.WorldToViewportPoint(transform.position).x < -0.1f)
            {
                Destroy(gameObject);
            }
        }

        public abstract void OnPlayerInteract();
    }
}
