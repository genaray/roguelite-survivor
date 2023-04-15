using Microsoft.Xna.Framework.Graphics;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public interface IFormComponent
    {
        string Name { get; set; }
        void Draw(SpriteBatch spriteBatch);
    }
}
