using Microsoft.Xna.Framework;

namespace RogueliteSurvivor.Extensions
{
    public static class GraphicsDeviceManagerExtensions
    {
        public static float GetWidthOffset(this GraphicsDeviceManager graphics, float divisor)
        {
            return graphics.PreferredBackBufferWidth / (divisor * Game1.ScaleFactor);
        }
        public static float GetHeightOffset(this GraphicsDeviceManager graphics, float divisor)
        {
            return graphics.PreferredBackBufferHeight / (divisor * Game1.ScaleFactor);
        }
    }
}
