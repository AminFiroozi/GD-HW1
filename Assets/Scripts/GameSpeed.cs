using UnityEngine;

namespace Game.Input
{
    public static class GameSpeed
    {
        public static float BaseScrollSpeed = 5f;
        public static float MaxScrollSpeed = 14f;
        public static float SpeedRampPerDistance = 0.01f;
        public static float DashMultiplier = 1.8f;

        private static float s_CurrentSpeed = BaseScrollSpeed;
        private static bool s_IsDashing;

        public static float ScrollSpeed => s_IsDashing ? s_CurrentSpeed * DashMultiplier : s_CurrentSpeed;

        public static void SetDashing(bool isDashing)
        {
            s_IsDashing = isDashing;
        }

        public static void UpdateForDistance(float distanceTraveled)
        {
            s_CurrentSpeed = Mathf.Min(BaseScrollSpeed + distanceTraveled * SpeedRampPerDistance, MaxScrollSpeed);
        }

        public static void Reset()
        {
            s_CurrentSpeed = BaseScrollSpeed;
            s_IsDashing = false;
        }
    }
}
