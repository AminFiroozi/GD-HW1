using UnityEngine;

namespace Game.Input
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Jump")]
        public float jumpSpeed = 14f;
        public Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
        public float groundCheckRadius = 0.12f;
        public LayerMask groundLayer = ~0;

        // Duck collider keeps the same bottom edge as standing (half height, offset halved)
        // so it lines up with the ground instead of sinking into it.
        [Header("Duck")]
        public Vector2 duckColliderSize = new Vector2(0.5f, 0.45f);
        public Vector2 duckColliderOffset = new Vector2(0f, -0.225f);

        [Header("Hit Flash")]
        public Color hitFlashColor = Color.white;
        public float hitFlashDuration = 0.15f;

        [Header("Jump Dust")]
        public ParticleSystem jumpDustParticles;

        [Header("Dash Trail")]
        public TrailRenderer dashTrail;

        private Rigidbody2D m_Rigidbody;
        private PlayerInputHandler m_Input;
        private BoxCollider2D m_Collider;

        private Vector2 m_StandColliderSize;
        private Vector2 m_StandColliderOffset;
        private bool m_WasDucking;
        private Animator animator;
        private SpriteRenderer m_SpriteRenderer;
        private Color m_OriginalSpriteColor;
        private Coroutine m_HitFlashCoroutine;


        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_Input = GetComponent<PlayerInputHandler>();
            m_Collider = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            if (m_SpriteRenderer != null)
            {
                m_OriginalSpriteColor = m_SpriteRenderer.color;
            }


            m_StandColliderSize = m_Collider.size;
            m_StandColliderOffset = m_Collider.offset;
        }

        private void Update()
        {
            if (Game.GameManager.Instance != null && Game.GameManager.Instance.IsGameOver)
            {
                return;
            }

            if (m_Input.JumpPressed && IsGrounded())
            {
                m_Rigidbody.linearVelocity = new Vector2(m_Rigidbody.linearVelocity.x, jumpSpeed);
                if (jumpDustParticles != null)
                {
                    jumpDustParticles.Play();
                }
            }

            UpdateDuckCollider();
            if (animator != null)
            {
                animator.SetBool("IsJumping", !IsGrounded());
            }

            GameSpeed.SetDashing(m_Input.DashHeld);

            if (dashTrail != null)
            {
                dashTrail.emitting = m_Input.DashHeld;
            }
        }

        private void UpdateDuckCollider()
        {
            if (m_Input.DuckHeld == m_WasDucking)
            {
                return;
            }

            m_WasDucking = m_Input.DuckHeld;
            if (m_WasDucking)
            {
                m_Collider.size = duckColliderSize;
                m_Collider.offset = duckColliderOffset;
            }
            else
            {
                m_Collider.size = m_StandColliderSize;
                m_Collider.offset = m_StandColliderOffset;
            }
        }

        private static readonly Collider2D[] s_GroundCheckResults = new Collider2D[8];

        private bool IsGrounded()
        {
            // OverlapCircle only returns one of possibly several overlapping colliders
            // (often our own, since the check point sits just past our collider's bottom
            // edge), so we scan every overlap and exclude ourselves rather than trusting
            // a single arbitrary result.
            Vector2 checkPoint = (Vector2)transform.position + groundCheckOffset;
            int count = Physics2D.OverlapCircleNonAlloc(checkPoint, groundCheckRadius, s_GroundCheckResults, groundLayer);
            for (int i = 0; i < count; i++)
            {
                if (s_GroundCheckResults[i] != null && s_GroundCheckResults[i].gameObject != gameObject)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector2 checkPoint = (Vector2)transform.position + groundCheckOffset;
            Gizmos.DrawWireSphere(checkPoint, groundCheckRadius);
        }
    

private void OnTriggerEnter2D(Collider2D other)
        {
            var interactable = other.GetComponent<Game.Interactables.Interactable>();
            if (interactable != null)
            {
                interactable.OnPlayerInteract();

                if (interactable is Game.Interactables.GroundObstacle || interactable is Game.Interactables.AirObstacle)
                {
                    if (animator != null)
                    {
                        animator.SetTrigger("Hit");
                    }

                    StartHitFlash();
                }
            }
        }

public void PlayDeathAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }
        }

        private void StartHitFlash()
        {
            if (m_SpriteRenderer == null)
            {
                return;
            }

            if (m_HitFlashCoroutine != null)
            {
                StopCoroutine(m_HitFlashCoroutine);
                m_SpriteRenderer.color = m_OriginalSpriteColor;
            }

            m_HitFlashCoroutine = StartCoroutine(HitFlashRoutine());
        }

        private System.Collections.IEnumerator HitFlashRoutine()
        {
            m_SpriteRenderer.color = hitFlashColor;
            yield return new WaitForSeconds(hitFlashDuration);
            m_SpriteRenderer.color = m_OriginalSpriteColor;
            m_HitFlashCoroutine = null;
        }

}
}
