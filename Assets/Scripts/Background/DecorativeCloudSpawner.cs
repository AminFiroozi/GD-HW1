using UnityEngine;
using Game.Input;

namespace Game.Background
{
    public class DecorativeCloudSpawner : MonoBehaviour
    {
        public GameObject cloudPrefab;
        public Sprite[] cloudSprites;
        public float minGapDistance = 10f;
        public float maxGapDistance = 24f;
        public float spawnX = 13.71837f;
        public float minY = 4.3f;
        public float maxY = 5f;
        public float behindHillsSortingOrder = -25f;
        public float behindHillsSpeedMultiplier = 0.2f;
        public float aboveHillsSortingOrder = -10f;
        public float aboveHillsSpeedMultiplier = 0.35f;

        private float m_DistanceSinceLastSpawn;
        private float m_NextGapTarget;

        private void Start()
        {
            m_NextGapTarget = Random.Range(minGapDistance, maxGapDistance);
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
            {
                return;
            }

            float distance = GameSpeed.ScrollSpeed * Time.deltaTime;
            m_DistanceSinceLastSpawn += distance;

            if (m_DistanceSinceLastSpawn >= m_NextGapTarget)
            {
                Spawn();
                m_DistanceSinceLastSpawn = 0f;
                m_NextGapTarget = Random.Range(minGapDistance, maxGapDistance);
            }
        }

        private void Spawn()
        {
            if (cloudPrefab == null || cloudSprites == null || cloudSprites.Length == 0)
            {
                return;
            }

            float spawnY = Random.Range(minY, maxY);
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);
            GameObject instance = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity);

            SpriteRenderer spriteRenderer = instance.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];

            DecorativeCloud cloud = instance.GetComponent<DecorativeCloud>();

            bool behindHills = Random.value < 0.5f;
            if (behindHills)
            {
                spriteRenderer.sortingOrder = (int)behindHillsSortingOrder;
                cloud.speedMultiplier = behindHillsSpeedMultiplier;
            }
            else
            {
                spriteRenderer.sortingOrder = (int)aboveHillsSortingOrder;
                cloud.speedMultiplier = aboveHillsSpeedMultiplier;
            }
        }
    }
}
