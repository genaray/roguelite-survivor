using Microsoft.Xna.Framework.Graphics;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public interface IFormComponent
    {
        string Name { get; set; }
        bool Visible { get; set; }
        void Draw(SpriteBatch spriteBatch);
    }
}
