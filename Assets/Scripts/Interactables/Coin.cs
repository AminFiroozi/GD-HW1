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

            var collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            StartCoroutine(PickupPopRoutine());
        }

        private System.Collections.IEnumerator PickupPopRoutine()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            Vector3 startScale = transform.localScale;
            Vector3 punchScale = startScale * 1.3f;
            Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

            float punchDuration = 0.08f;
            float elapsed = 0f;
            while (elapsed < punchDuration)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, punchScale, elapsed / punchDuration);
                yield return null;
            }

            float shrinkDuration = 0.12f;
            elapsed = 0f;
            while (elapsed < shrinkDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / shrinkDuration;
                transform.localScale = Vector3.Lerp(punchScale, Vector3.zero, t);
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0f), t);
                }
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
