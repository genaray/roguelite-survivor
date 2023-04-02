using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public abstract class Window : IWindow
    {
        Texture2D background;
        protected Vector2 position;
        Vector2 center;
        List<IFormComponent> components;
        Rectangle source;
        public Window(Texture2D background, Vector2 position, List<IFormComponent> components) 
        {
            this.background = background;
            this.position = position;
            this.components = components;
            if (background != null)
            {
                source = new Rectangle(0, 0, background.Width, background.Height);
                center = new Vector2(background.Width / 2, background.Height / 2);
            }
        }

        public abstract void SetActive();

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(background != null)
            {
                spriteBatch.Draw(
                    background,
                    position,
                    source,
                    Color.White,
                    0f,
                    center,
                    1f,
                    SpriteEffects.None,
                    0
                );
            }
            foreach (var component in components)
            {
                component.Draw(spriteBatch);
            }
        }

        public abstract string Update(GameTime gameTime, params object[] values);
    }
}
