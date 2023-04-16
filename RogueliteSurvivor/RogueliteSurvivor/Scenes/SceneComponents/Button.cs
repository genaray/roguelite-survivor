using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public class Button : IFormComponent, ISelectableComponent
    {
        Texture2D texture;
        Vector2 position;
        Rectangle nonSelectedSource;
        Rectangle selectedSource;
        Vector2 center;

        bool mouseOver = false;
        Rectangle buttonArea;

        public Button(string name, Texture2D texture, Vector2 position, Rectangle nonSelectedSource, Rectangle selectedSource, Vector2 center)
        {
            Name = name;
            this.texture = texture;
            this.position = position;
            this.nonSelectedSource = nonSelectedSource;
            this.selectedSource = selectedSource;
            this.center = center;

            buttonArea = new Rectangle(((position - center) * Game1.ScaleFactor).ToPoint(), new Point((int)(selectedSource.Width * Game1.ScaleFactor), (int)(selectedSource.Height * Game1.ScaleFactor)));
        }

        public string Name { get; set; }
        public bool Selected { get; set; } = false;
        public bool Visible { get; set; } = true;

        public bool MouseOver()
        {
            return mouseOver;
        }
        public void MouseOver(MouseState mState)
        {
            mouseOver = buttonArea.Contains(mState.Position);
        }

        public void ResetButtonTexture(Rectangle nonSelectedSource, Rectangle selectedSource)
        {
            this.nonSelectedSource = nonSelectedSource;
            this.selectedSource = selectedSource;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                if (Selected)
                {
                    spriteBatch.Draw(
                        texture,
                        position,
                        selectedSource,
                        Color.White,
                        0f,
                        center,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
                else if (MouseOver())
                {
                    spriteBatch.Draw(
                        texture,
                        position,
                        selectedSource,
                        Color.White * 0.75f,
                        0f,
                        center,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                    spriteBatch.Draw(
                        texture,
                        position,
                        nonSelectedSource,
                        Color.White * 0.25f,
                        0f,
                        center,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
                else
                {
                    spriteBatch.Draw(
                        texture,
                        position,
                        nonSelectedSource,
                        Color.White,
                        0f,
                        center,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
    }
}
