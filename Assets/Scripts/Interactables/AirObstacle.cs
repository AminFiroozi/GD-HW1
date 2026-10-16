using UnityEngine;

namespace Game.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class AirObstacle : Interactable
    {
public override void OnPlayerInteract()
        {
            Debug.Log("Player hit air obstacle");
            Game.GameManager.Instance.LoseLife();
        }
    }
}
