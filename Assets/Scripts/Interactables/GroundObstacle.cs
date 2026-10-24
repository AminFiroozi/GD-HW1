using UnityEngine;

namespace Game.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class GroundObstacle : Interactable
    {
        public float minScale = 1.5f;
        public float maxScale = 2.5f;

        private bool m_Hit;

        private void Awake()
        {
            float scale = Random.Range(minScale, maxScale);
            transform.localScale = new Vector3(scale, scale, 1f);
        }

        public override void OnPlayerInteract()
        {
            if (m_Hit)
            {
                return;
            }

            m_Hit = true;
            GetComponent<Collider2D>().enabled = false;

            Debug.Log("Player hit ground obstacle");
            Game.GameManager.Instance.LoseLife();
        }
    }
}
