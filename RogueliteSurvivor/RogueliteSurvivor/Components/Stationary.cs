namespace RogueliteSurvivor.Components
{
    public struct Stationary : IDuration
    {
        public float TimeLeft { get; set; }
        public float Cooldown { get; set; }
        public float BaseRadius { get; set; }
        public float RadiusMultiplier { get; set; }
    }
}
