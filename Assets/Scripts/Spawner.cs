using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float spawnChance;
    }

    public class Spawner : MonoBehaviour
    {
        public SpawnableObject[] spawnables;
        public float maxGapDistance = 12f;
        public float minGapDistance = 5f;
        public float difficultyRampDistance = 300f;
        public float spawnX = 13.71837f;

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

            m_DistanceSinceLastSpawn += Game.Input.GameSpeed.ScrollSpeed * Time.deltaTime;
            if (m_DistanceSinceLastSpawn >= m_NextGapTarget)
            {
                Spawn();
                m_DistanceSinceLastSpawn = 0f;
                m_NextGapTarget = Random.Range(minGapDistance, CurrentMaxGapDistance());
            }
        }

        private float CurrentMaxGapDistance()
        {
            float distanceTraveled = GameManager.Instance.DistanceTraveled;
            float difficultyT = Mathf.Clamp01(distanceTraveled / difficultyRampDistance);
            return Mathf.Lerp(maxGapDistance, minGapDistance, difficultyT);
        }

        private void Spawn()
        {
            GameObject prefabToSpawn = PickRandomSpawnable();
            if (prefabToSpawn != null)
            {
                Vector3 spawnPosition = new Vector3(spawnX, prefabToSpawn.transform.position.y, 0f);
                Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            }
        }

        private GameObject PickRandomSpawnable()
        {
            if (spawnables == null || spawnables.Length == 0)
            {
                return null;
            }

            float totalChance = 0f;
            foreach (SpawnableObject spawnable in spawnables)
            {
                totalChance += spawnable.spawnChance;
            }

            if (totalChance <= 0f)
            {
                return null;
            }

            float roll = Random.Range(0f, totalChance);
            float cumulative = 0f;
            foreach (SpawnableObject spawnable in spawnables)
            {
                cumulative += spawnable.spawnChance;
                if (roll <= cumulative)
                {
                    return spawnable.prefab;
                }
            }

            return spawnables[spawnables.Length - 1].prefab;
        }
    }
}
