using UnityEngine;

namespace Game.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class AirObstacle : Interactable
    {
        private bool m_Hit;

        public override void OnPlayerInteract()
        {
            if (m_Hit)
            {
                return;
            }

            m_Hit = true;
            GetComponent<Collider2D>().enabled = false;

            Debug.Log("Player hit air obstacle");
            Game.GameManager.Instance.LoseLife();
        }
    }
}
