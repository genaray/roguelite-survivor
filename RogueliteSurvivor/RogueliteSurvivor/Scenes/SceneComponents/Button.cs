using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public class Button : IFormComponent, IDrawableComponent, ISelectableComponent
    {
        Texture2D texture;
        Vector2 position;
        Rectangle nonSelectedSource;
        Rectangle selectedSource;
        Vector2 center;

        bool selected = false;
        bool mouseOver = false;
        bool visible = true;
        Rectangle buttonArea;

        public Button(Texture2D texture, Vector2 position, Rectangle nonSelectedSource, Rectangle selectedSource, Vector2 center)
        {
            this.texture = texture;
            this.position = position;
            this.nonSelectedSource = nonSelectedSource;
            this.selectedSource = selectedSource;
            this.center = center;

            buttonArea = new Rectangle(((position - center) * Game1.ScaleFactor).ToPoint(), new Point((int)(selectedSource.Width * Game1.ScaleFactor), (int)(selectedSource.Height * Game1.ScaleFactor)));
        }

        public bool Selected()
        {
            return selected;
        }
        public void Selected(bool selected)
        {
            this.selected = selected;
        }

        public bool MouseOver()
        {
            return mouseOver;
        }
        public void MouseOver(MouseState mState)
        {
            mouseOver = buttonArea.Contains(mState.Position);
        }

        public bool Visible()
        {
            return visible;
        }
        public void Visible(bool visible)
        {
            this.visible = visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible())
            {
                if (Selected())
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
