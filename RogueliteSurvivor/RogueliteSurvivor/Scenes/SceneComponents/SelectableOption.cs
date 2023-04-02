using Box2D.NetStandard.Dynamics.Fixtures;
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
    public class SelectableOption : IFormComponent, IDrawableComponent, ISelectableComponent
    {
        Texture2D itemTexture;
        Texture2D outlineTexture;
        Vector2 position;
        Rectangle itemSource;
        Rectangle outlineSource;
        Vector2 itemCenter;
        Vector2 outlineCenter;

        bool selected = false;
        bool mouseOver = false;
        bool visible = true;
        Rectangle buttonArea;

        public SelectableOption(Texture2D itemTexture, Texture2D outlineTexture, Vector2 position, Rectangle itemSource, Rectangle outlineSource, Vector2 itemCenter, Vector2 outlineCenter)
        {
            this.itemTexture = itemTexture;
            this.outlineTexture = outlineTexture;
            this.position = position;
            this.itemSource = itemSource;
            this.outlineSource = outlineSource;
            this.itemCenter = itemCenter;
            this.outlineCenter = outlineCenter;

            buttonArea = new Rectangle(((position - outlineCenter) * Game1.ScaleFactor).ToPoint(), new Point((int)(outlineSource.Width * Game1.ScaleFactor), (int)(outlineSource.Height * Game1.ScaleFactor)));
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

                if (Selected())
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
