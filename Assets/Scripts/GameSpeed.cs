namespace Game.Input
{
    // Placeholder values until the parallax/obstacle-spawner phases exist to feel them out.
    public static class GameSpeed
    {
        public static float BaseScrollSpeed = 5f;
        public static float DashMultiplier = 1.8f;

        public static float ScrollSpeed { get; private set; } = BaseScrollSpeed;

        public static void SetDashing(bool isDashing)
        {
            ScrollSpeed = isDashing ? BaseScrollSpeed * DashMultiplier : BaseScrollSpeed;
        }
    }
}
