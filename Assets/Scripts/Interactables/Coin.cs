using UnityEngine;

namespace Game.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class Coin : Interactable
    {
public override void OnPlayerInteract()
        {
            Debug.Log("Player collected coin");
            Game.GameManager.Instance.AddScore(10f);
            Destroy(gameObject);
        }
    }
}
