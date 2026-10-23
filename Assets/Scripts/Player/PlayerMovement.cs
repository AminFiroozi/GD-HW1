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

        [Header("Duck")]
        public Vector2 duckColliderSize = new Vector2(0.5f, 0.3f);
        public Vector2 duckColliderOffset = new Vector2(0f, -0.3f);

        [Header("Hit Flash")]
        public Color hitFlashColor = Color.white;
        public float hitFlashDuration = 0.15f;

        [Header("Jump Dust")]
        public ParticleSystem jumpDustParticles;

        [Header("Dash Trail")]
        public TrailRenderer dashTrail;

        [Header("Animation")]
        public Sprite[] runFrames;
        public Sprite[] jumpFrames;
        public Sprite[] hurtFrames;
        public float frameRate = 12f;

        private int m_FrameIndex;
        private float m_FrameTimer;
        private bool m_IsHurt;
        private bool m_WasGrounded = true;

        private Rigidbody2D m_Rigidbody;
        private PlayerInputHandler m_Input;
        private BoxCollider2D m_Collider;

        private Vector2 m_StandColliderSize;
        private Vector2 m_StandColliderOffset;
        private bool m_WasDucking;
        private SpriteRenderer m_SpriteRenderer;
        private Color m_OriginalSpriteColor;
        private Coroutine m_HitFlashCoroutine;


        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_Input = GetComponent<PlayerInputHandler>();
            m_Collider = GetComponent<BoxCollider2D>();
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
            UpdateAnimation();

            GameSpeed.SetDashing(m_Input.DashHeld);

            if (dashTrail != null)
            {
                dashTrail.emitting = m_Input.DashHeld;
            }
        }

        private void UpdateDuckCollider()
        {
            if (m_Input.DashHeld == m_WasDucking)
            {
                return;
            }

            m_WasDucking = m_Input.DashHeld;
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

        private void UpdateAnimation()
        {
            if (m_SpriteRenderer == null)
            {
                return;
            }

            if (m_IsHurt)
            {
                if (hurtFrames == null || hurtFrames.Length == 0)
                {
                    return;
                }

                m_FrameTimer += Time.deltaTime;
                float hurtFrameDuration = 1f / frameRate;
                if (m_FrameTimer >= hurtFrameDuration)
                {
                    m_FrameTimer -= hurtFrameDuration;
                    m_FrameIndex = (m_FrameIndex + 1) % hurtFrames.Length;
                    m_SpriteRenderer.sprite = hurtFrames[m_FrameIndex];
                }

                return;
            }

            bool grounded = IsGrounded();
            if (!grounded)
            {
                if (m_WasGrounded)
                {
                    m_FrameIndex = 0;
                    m_FrameTimer = 0f;
                }

                m_WasGrounded = false;

                if (jumpFrames == null || jumpFrames.Length == 0)
                {
                    return;
                }

                m_FrameTimer += Time.deltaTime;
                float jumpFrameDuration = 1f / frameRate;
                if (m_FrameTimer >= jumpFrameDuration && m_FrameIndex < jumpFrames.Length - 1)
                {
                    m_FrameTimer -= jumpFrameDuration;
                    m_FrameIndex++;
                    m_SpriteRenderer.sprite = jumpFrames[m_FrameIndex];
                }
                else if (m_FrameIndex == 0)
                {
                    m_SpriteRenderer.sprite = jumpFrames[0];
                }

                return;
            }

            m_WasGrounded = true;

            if (runFrames == null || runFrames.Length == 0)
            {
                return;
            }

            m_FrameTimer += Time.deltaTime;
            float frameDuration = 1f / frameRate;
            if (m_FrameTimer >= frameDuration)
            {
                m_FrameTimer -= frameDuration;
                m_FrameIndex = (m_FrameIndex + 1) % runFrames.Length;
                m_SpriteRenderer.sprite = runFrames[m_FrameIndex];
            }
        }

        private static readonly Collider2D[] s_GroundCheckResults = new Collider2D[8];

        private bool IsGrounded()
        {
            Vector2 checkPoint = (Vector2)transform.position + groundCheckOffset;
            int count = Physics2D.OverlapCircleNonAlloc(checkPoint, groundCheckRadius, s_GroundCheckResults, groundLayer);
            for (int i = 0; i < count; i++)
            {
                var hit = s_GroundCheckResults[i];
                if (hit != null && hit.gameObject != gameObject && !hit.isTrigger)
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
                    StartHitFlash();
                }
            }
        }

        public void PlayDeathAnimation()
        {
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
            m_IsHurt = true;
            m_FrameIndex = 0;
            m_FrameTimer = 0f;

            m_SpriteRenderer.color = hitFlashColor;
            yield return new WaitForSeconds(hitFlashDuration);
            m_SpriteRenderer.color = m_OriginalSpriteColor;
            m_HitFlashCoroutine = null;
            m_IsHurt = false;
        }

    }
}
