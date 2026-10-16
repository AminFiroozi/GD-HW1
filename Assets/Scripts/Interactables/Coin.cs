using UnityEngine;

namespace Game.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class Coin : Interactable
    {
        public override void OnPlayerInteract()
        {
            Debug.Log("Player collected coin");
            Destroy(gameObject);
        }
    }
}
