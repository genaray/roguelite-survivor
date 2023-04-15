using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public interface IWindow
    {
        Vector2 Position { get; set; }
        Dictionary<string, IFormComponent> Components { get; set; }
        string Update(GameTime gameTime, params object[] values);
        void Draw(SpriteBatch spriteBatch);
    }
}
