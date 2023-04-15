using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public class GenericWindow : Window
    {
        public GenericWindow(GraphicsDeviceManager graphics,
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
