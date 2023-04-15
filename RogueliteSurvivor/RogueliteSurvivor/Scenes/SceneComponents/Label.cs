using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public class Label : IFormComponent, IDrawableComponent
    {
        public string Name { get; set; }
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Vector2 Position { get; set; }
        public Color ForeColor { get; set; }
        public bool Visible { get; set; }

        public Label(string name, SpriteFont font, string text, Vector2 position, Color foreColor)
        {
            Name = name;
            Font = font;
            Text = text;
            Position = position;
            ForeColor = foreColor;
            Visible = true;
        }

        public void Draw(SpriteBatch spriteBatch)
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
