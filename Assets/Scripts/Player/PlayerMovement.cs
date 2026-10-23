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
        public float jumpCutMultiplier = 0.5f;
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
        public float runFrameRate = 12f;
        public Sprite[] dashFrames;
        public float dashFrameRate = 18f;
        public Sprite[] jumpFrames;
        public float jumpFrameRate = 10f;
        public Sprite[] hurtFrames;
        public float hurtFrameRate = 15f;
        public int postAnimationDelayFrames = 2;
        public Sprite[] deathFrames;
        public float deathFrameRate = 10f;
        public Sprite[] deathIdleFrames;
        public float deathIdleFrameRate = 8f;
        public int postDeathDelayFrames = 20;

        public bool IsDeathSequenceReadyForUI { get; private set; }

        private int m_FrameIndex;
        private float m_FrameTimer;
        private int m_HurtFrameIndex;
        private float m_HurtFrameTimer;
        private bool m_IsHurt;
        private bool m_WasGrounded = true;
        private bool m_JumpAnimPlaying;
        private bool m_PostAnimDelayActive;
        private int m_PostAnimDelayFramesRemaining;

        private bool m_IsDead;
        private int m_DeathFrameIndex;
        private float m_DeathFrameTimer;
        private bool m_DeathAnimPlaying;
        private bool m_DeathIdleLooping;
        private int m_DeathIdleFrameIndex;
        private float m_DeathIdleFrameTimer;
        private int m_PostDeathDelayFramesRemaining;

        private Rigidbody2D m_Rigidbody;
        private PlayerInputHandler m_Input;
        private BoxCollider2D m_Collider;

        private Vector2 m_StandColliderSize;
        private Vector2 m_StandColliderOffset;
        private bool m_WasDucking;
        private bool m_JumpCutApplied;
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
            if (m_IsDead)
            {
                UpdateAnimation();
                return;
            }

            if (Game.GameManager.Instance != null && Game.GameManager.Instance.IsGameOver)
            {
                return;
            }

            if (!IsGrounded() && !m_JumpCutApplied && !m_Input.JumpHeld && m_Rigidbody.linearVelocity.y > 0f)
            {
                m_Rigidbody.linearVelocity = new Vector2(m_Rigidbody.linearVelocity.x, m_Rigidbody.linearVelocity.y * jumpCutMultiplier);
                m_JumpCutApplied = true;
            }

            if (m_Input.JumpPressed && IsGrounded())
            {
                m_Rigidbody.linearVelocity = new Vector2(m_Rigidbody.linearVelocity.x, jumpSpeed);
                m_JumpCutApplied = false;
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

            if (m_IsDead)
            {
                UpdateDeathFrame();
                return;
            }

            if (m_IsHurt)
            {
                if (hurtFrames == null || hurtFrames.Length == 0)
                {
                    return;
                }

                m_HurtFrameTimer += Time.deltaTime;
                float hurtFrameDuration = 1f / hurtFrameRate;
                if (m_HurtFrameTimer >= hurtFrameDuration)
                {
                    m_HurtFrameTimer -= hurtFrameDuration;
                    m_HurtFrameIndex = (m_HurtFrameIndex + 1) % hurtFrames.Length;
                    m_SpriteRenderer.sprite = hurtFrames[m_HurtFrameIndex];
                }

                return;
            }

            bool grounded = IsGrounded();

            if (!grounded && m_WasGrounded)
            {
                m_JumpAnimPlaying = true;
                m_PostAnimDelayActive = false;
                m_FrameIndex = 0;
                m_FrameTimer = 0f;
            }

            m_WasGrounded = grounded;

            if (m_JumpAnimPlaying)
            {
                UpdateJumpFrame();
                return;
            }

            if (m_PostAnimDelayActive)
            {
                m_PostAnimDelayFramesRemaining--;
                if (m_PostAnimDelayFramesRemaining > 0)
                {
                    return;
                }

                m_PostAnimDelayActive = false;
            }

            if (!grounded)
            {
                return;
            }

            if (m_Input.DashHeld)
            {
                if (dashFrames == null || dashFrames.Length == 0)
                {
                    return;
                }

                m_FrameTimer += Time.deltaTime;
                float dashFrameDuration = 1f / dashFrameRate;
                if (m_FrameTimer >= dashFrameDuration)
                {
                    m_FrameTimer -= dashFrameDuration;
                    m_FrameIndex = (m_FrameIndex + 1) % dashFrames.Length;
                    m_SpriteRenderer.sprite = dashFrames[m_FrameIndex];
                }

                return;
            }

            if (runFrames == null || runFrames.Length == 0)
            {
                return;
            }

            m_FrameTimer += Time.deltaTime;
            float runFrameDuration = 1f / runFrameRate;
            if (m_FrameTimer >= runFrameDuration)
            {
                m_FrameTimer -= runFrameDuration;
                m_FrameIndex = (m_FrameIndex + 1) % runFrames.Length;
                m_SpriteRenderer.sprite = runFrames[m_FrameIndex];
            }
        }

        private void UpdateJumpFrame()
        {
            if (jumpFrames == null || jumpFrames.Length == 0)
            {
                m_JumpAnimPlaying = false;
                StartPostAnimDelay();
                return;
            }

            if (jumpFrames.Length == 1)
            {
                m_SpriteRenderer.sprite = jumpFrames[0];
                m_JumpAnimPlaying = false;
                StartPostAnimDelay();
                return;
            }

            m_FrameTimer += Time.deltaTime;
            float jumpFrameDuration = 1f / jumpFrameRate;
            if (m_FrameTimer >= jumpFrameDuration && m_FrameIndex < jumpFrames.Length - 1)
            {
                m_FrameTimer -= jumpFrameDuration;
                m_FrameIndex++;
                m_SpriteRenderer.sprite = jumpFrames[m_FrameIndex];
                if (m_FrameIndex >= jumpFrames.Length - 1)
                {
                    m_JumpAnimPlaying = false;
                    StartPostAnimDelay();
                }
            }
            else if (m_FrameIndex == 0)
            {
                m_SpriteRenderer.sprite = jumpFrames[0];
            }
        }

        private void StartPostAnimDelay()
        {
            m_PostAnimDelayActive = true;
            m_PostAnimDelayFramesRemaining = postAnimationDelayFrames;
        }

        private void UpdateDeathFrame()
        {
            if (m_DeathAnimPlaying)
            {
                if (deathFrames == null || deathFrames.Length == 0)
                {
                    m_DeathAnimPlaying = false;
                    StartDeathIdleLoop();
                    return;
                }

                if (deathFrames.Length == 1)
                {
                    m_SpriteRenderer.sprite = deathFrames[0];
                    m_DeathAnimPlaying = false;
                    StartDeathIdleLoop();
                    return;
                }

                m_DeathFrameTimer += Time.deltaTime;
                float deathFrameDuration = 1f / deathFrameRate;
                if (m_DeathFrameTimer >= deathFrameDuration && m_DeathFrameIndex < deathFrames.Length - 1)
                {
                    m_DeathFrameTimer -= deathFrameDuration;
                    m_DeathFrameIndex++;
                    m_SpriteRenderer.sprite = deathFrames[m_DeathFrameIndex];
                    if (m_DeathFrameIndex >= deathFrames.Length - 1)
                    {
                        m_DeathAnimPlaying = false;
                        StartDeathIdleLoop();
                    }
                }
                else if (m_DeathFrameIndex == 0)
                {
                    m_SpriteRenderer.sprite = deathFrames[0];
                }

                return;
            }

            if (m_DeathIdleLooping)
            {
                if (deathIdleFrames != null && deathIdleFrames.Length > 0)
                {
                    if (deathIdleFrames.Length == 1)
                    {
                        m_SpriteRenderer.sprite = deathIdleFrames[0];
                    }
                    else
                    {
                        m_DeathIdleFrameTimer += Time.deltaTime;
                        float deathIdleFrameDuration = 1f / deathIdleFrameRate;
                        if (m_DeathIdleFrameTimer >= deathIdleFrameDuration && m_DeathIdleFrameIndex < deathIdleFrames.Length - 1)
                        {
                            m_DeathIdleFrameTimer -= deathIdleFrameDuration;
                            m_DeathIdleFrameIndex++;
                            m_SpriteRenderer.sprite = deathIdleFrames[m_DeathIdleFrameIndex];
                        }
                    }
                }

                if (!IsDeathSequenceReadyForUI)
                {
                    m_PostDeathDelayFramesRemaining--;
                    if (m_PostDeathDelayFramesRemaining <= 0)
                    {
                        IsDeathSequenceReadyForUI = true;
                    }
                }
            }
        }

        private void StartDeathIdleLoop()
        {
            m_DeathIdleLooping = true;
            m_DeathIdleFrameIndex = 0;
            m_DeathIdleFrameTimer = 0f;
            if (deathIdleFrames != null && deathIdleFrames.Length > 0)
            {
                m_SpriteRenderer.sprite = deathIdleFrames[0];
            }
            m_PostDeathDelayFramesRemaining = postDeathDelayFrames;
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
            m_IsDead = true;
            IsDeathSequenceReadyForUI = false;
            m_DeathAnimPlaying = true;
            m_DeathIdleLooping = false;
            m_DeathFrameIndex = 0;
            m_DeathFrameTimer = 0f;
            m_DeathIdleFrameIndex = 0;
            m_DeathIdleFrameTimer = 0f;
            m_PostDeathDelayFramesRemaining = postDeathDelayFrames;
            m_Rigidbody.linearVelocity = Vector2.zero;
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
            m_HurtFrameIndex = 0;
            m_HurtFrameTimer = 0f;

            m_SpriteRenderer.color = hitFlashColor;
            yield return new WaitForSeconds(hitFlashDuration);
            m_SpriteRenderer.color = m_OriginalSpriteColor;
            m_HitFlashCoroutine = null;
            m_IsHurt = false;
            StartPostAnimDelay();
        }

    }
}
