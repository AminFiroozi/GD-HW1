using UnityEngine;
using Game.Input;

namespace Game.Background
{
    public static class DecorativeSpawnTracker
    {
        public const float MinSeparationDistance = 4f;

        private static float s_DistanceSinceLastSpawn;
        private static int s_LastAdvancedFrame = -1;

        public static void Advance()
        {
            if (Time.frameCount == s_LastAdvancedFrame)
            {
                return;
            }

            s_LastAdvancedFrame = Time.frameCount;
            s_DistanceSinceLastSpawn += GameSpeed.ScrollSpeed * Time.deltaTime;
        }

        public static bool CanSpawn()
        {
            return s_DistanceSinceLastSpawn >= MinSeparationDistance;
        }

        public static void NotifySpawned()
        {
            s_DistanceSinceLastSpawn = 0f;
        }
    }
}
