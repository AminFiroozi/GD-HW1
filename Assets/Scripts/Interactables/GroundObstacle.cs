using UnityEngine;

namespace Game.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class GroundObstacle : Interactable
    {
        public override void OnPlayerInteract()
        {
            Debug.Log("Player hit ground obstacle");
        }
    }
}
