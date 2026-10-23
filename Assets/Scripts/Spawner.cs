using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;
        public float minGapDistance = 5f;
        public float maxGapDistance = 12f;
        public float minScoreToSpawn = 0f;

        [System.NonSerialized] public float distanceSinceLastSpawn;
        [System.NonSerialized] public float nextGapTarget;
    }

    public class Spawner : MonoBehaviour
    {
        public SpawnableObject[] spawnables;
        public float difficultyRampDistance = 300f;
        public float spawnX = 13.71837f;

        private void Start()
        {
            foreach (SpawnableObject spawnable in spawnables)
            {
                spawnable.nextGapTarget = Random.Range(spawnable.minGapDistance, spawnable.maxGapDistance);
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
            {
                return;
            }

            float distance = Game.Input.GameSpeed.ScrollSpeed * Time.deltaTime;

            foreach (SpawnableObject spawnable in spawnables)
            {
                if (GameManager.Instance.Score < spawnable.minScoreToSpawn)
                {
                    continue;
                }

                spawnable.distanceSinceLastSpawn += distance;
                if (spawnable.distanceSinceLastSpawn >= spawnable.nextGapTarget)
                {
                    Spawn(spawnable.prefab);
                    spawnable.distanceSinceLastSpawn = 0f;
                    spawnable.nextGapTarget = Random.Range(spawnable.minGapDistance, CurrentMaxGapDistance(spawnable));
                }
            }
        }

        private float CurrentMaxGapDistance(SpawnableObject spawnable)
        {
            float distanceTraveled = GameManager.Instance.DistanceTraveled;
            float difficultyT = Mathf.Clamp01(distanceTraveled / difficultyRampDistance);
            return Mathf.Lerp(spawnable.maxGapDistance, spawnable.minGapDistance, difficultyT);
        }

        private void Spawn(GameObject prefabToSpawn)
        {
            if (prefabToSpawn == null)
            {
                return;
            }

            Vector3 spawnPosition = new Vector3(spawnX, prefabToSpawn.transform.position.y, 0f);
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
