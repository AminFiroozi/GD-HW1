using UnityEngine;
using Game.Input;

namespace Game.Background
{
    public class DecorativeSandSpawner : MonoBehaviour
    {
        public GameObject sandPrefab;
        public float minGapDistance = 8f;
        public float maxGapDistance = 22f;
        public float spawnX = 13.71837f;
        public float minYJitter = -0.1f;
        public float maxYJitter = 0.1f;
        public float minScaleJitter = 0.85f;
        public float maxScaleJitter = 1.15f;

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
            DecorativeSpawnTracker.Advance();

            if (m_DistanceSinceLastSpawn >= m_NextGapTarget && DecorativeSpawnTracker.CanSpawn())
            {
                Spawn();
                m_DistanceSinceLastSpawn = 0f;
                m_NextGapTarget = Random.Range(minGapDistance, maxGapDistance);
                DecorativeSpawnTracker.NotifySpawned();
            }
        }

        private void Spawn()
        {
            if (sandPrefab == null)
            {
                return;
            }

            float baseY = sandPrefab.transform.position.y + Random.Range(minYJitter, maxYJitter);
            Vector3 spawnPosition = new Vector3(spawnX, baseY, 0f);
            GameObject instance = Instantiate(sandPrefab, spawnPosition, Quaternion.identity);

            float scale = Random.Range(minScaleJitter, maxScaleJitter);
            instance.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
