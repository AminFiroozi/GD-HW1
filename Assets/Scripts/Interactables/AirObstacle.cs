using UnityEngine;

namespace Game.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class AirObstacle : Interactable
    {
        public Sprite[] flapFrames;
        public float frameRate = 8f;

        private SpriteRenderer m_SpriteRenderer;
        private int m_FrameIndex;
        private float m_FrameTimer;
        private bool m_Hit;

        private void Awake()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Update()
        {
            base.Update();

            if (flapFrames == null || flapFrames.Length == 0)
            {
                return;
            }

            m_FrameTimer += Time.deltaTime;
            float frameDuration = 1f / frameRate;
            if (m_FrameTimer >= frameDuration)
            {
                m_FrameTimer -= frameDuration;
                m_FrameIndex = (m_FrameIndex + 1) % flapFrames.Length;
                m_SpriteRenderer.sprite = flapFrames[m_FrameIndex];
            }
        }

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
