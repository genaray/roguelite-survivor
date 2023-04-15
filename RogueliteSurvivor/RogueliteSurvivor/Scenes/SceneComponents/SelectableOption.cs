using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public class SelectableOption : IFormComponent, IDrawableComponent, ISelectableComponent
    {
        Texture2D itemTexture;
        Texture2D outlineTexture;
        Vector2 position;
        Rectangle itemSource;
        Rectangle outlineSource;
        Vector2 itemCenter;
        Vector2 outlineCenter;

        bool mouseOver = false;
        Rectangle buttonArea;

        public SelectableOption(string name, Texture2D itemTexture, Texture2D outlineTexture, Vector2 position, Rectangle itemSource, Rectangle outlineSource, Vector2 itemCenter, Vector2 outlineCenter)
        {
            Name = name;
            this.itemTexture = itemTexture;
            this.outlineTexture = outlineTexture;
            this.position = position;
            this.itemSource = itemSource;
            this.outlineSource = outlineSource;
            this.itemCenter = itemCenter;
            this.outlineCenter = outlineCenter;

            buttonArea = new Rectangle(((position - outlineCenter) * Game1.ScaleFactor).ToPoint(), new Point((int)(outlineSource.Width * Game1.ScaleFactor), (int)(outlineSource.Height * Game1.ScaleFactor)));
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


        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(
                    itemTexture,
                    position,
                    itemSource,
                    Color.White,
                    0f,
                    itemCenter,
                    1f,
                    SpriteEffects.None,
                    0f
                );

                if (Selected)
                {
                    spriteBatch.Draw(
                        outlineTexture,
                        position,
                        outlineSource,
                        Color.White,
                        0f,
                        outlineCenter,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
                else if (MouseOver())
                {
                    spriteBatch.Draw(
                        outlineTexture,
                        position,
                        outlineSource,
                        Color.White * 0.75f,
                        0f,
                        outlineCenter,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
    }
}
