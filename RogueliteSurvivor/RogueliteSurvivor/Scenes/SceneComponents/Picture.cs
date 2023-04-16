using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public class Picture : IFormComponent
    {
        public string Name { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public bool Visible { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public Vector2 Center { get; set; }

        public Picture(string name, Texture2D texture, Vector2 position, Rectangle sourceRectangle, Vector2 center, bool visible = true)
        {
            Name = name;
            Texture = texture;
            Position = position;
            Visible = visible;
            SourceRectangle = sourceRectangle;
            Center = center;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(
                    Texture,
                    Position,
                    SourceRectangle,
                    Color.White,
                    0f,
                    Center,
                    1f,
                    SpriteEffects.None,
                    0f);
            }
        }
    }
}
