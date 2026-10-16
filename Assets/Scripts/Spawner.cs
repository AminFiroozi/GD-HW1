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
        public float minSpawnRate = 1.5f;
        public float maxSpawnRate = 3f;
        public float spawnX = 13.71837f;

        private void Start()
        {
            Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
        }

        private void Spawn()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            {
                return;
            }

            GameObject prefabToSpawn = PickRandomSpawnable();
            if (prefabToSpawn != null)
            {
                Vector3 spawnPosition = new Vector3(spawnX, prefabToSpawn.transform.position.y, 0f);
                Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            }

            Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
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
