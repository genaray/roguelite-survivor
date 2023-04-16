using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.Scenes.SceneComponents;
using System.Collections.Generic;

namespace RogueliteSurvivor.Scenes.Windows
{
    public class MapWindow : Window
    {
        public MapWindow(GraphicsDeviceManager graphics,
            Texture2D background,
            Vector2 position,
            Dictionary<string, IFormComponent> components,
            bool visible)
            : base(graphics, background, position, components)
        {
            Visible = visible;
        }

        public override string Update(GameTime gameTime, params object[] values)
        {
            return string.Empty;
        }
    }
}
