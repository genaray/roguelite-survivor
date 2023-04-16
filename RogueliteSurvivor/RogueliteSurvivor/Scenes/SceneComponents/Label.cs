using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public class Label : IFormComponent
    {
        public string Name { get; set; }
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Vector2 Position { get; set; }
        public Color ForeColor { get; set; }
        public bool Visible { get; set; }

        public Label(string name, SpriteFont font, string text, Vector2 position, Color foreColor, bool visible = true)
        {
            Name = name;
            Font = font;
            Text = text;
            Position = position;
            ForeColor = foreColor;
            Visible = visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.DrawString(
                    Font,
                    Text,
                    Position,
                    ForeColor
                );
            }
        }
    }
}
