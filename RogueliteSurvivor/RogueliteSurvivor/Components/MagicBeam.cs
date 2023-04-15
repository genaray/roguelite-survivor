using Microsoft.Xna.Framework;

namespace RogueliteSurvivor.Components
{
    public struct MagicBeam
    {
        public int BaseRadius { get; set; }
        public float RadiusMultiplier { get; set; }
        public Vector2 HitPosition { get; set; }
    }
}
