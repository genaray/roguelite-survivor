using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public interface IWindow
    {
        string Update(GameTime gameTime, params object[] values);
        void Draw(SpriteBatch spriteBatch);
    }
}
