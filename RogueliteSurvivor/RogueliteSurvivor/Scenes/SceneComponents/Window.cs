using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.Constants;
using System.Collections.Generic;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public abstract class Window : IWindow, IFormComponent, IDrawableComponent
    {
        protected GraphicsDeviceManager graphics;
        Texture2D background;
        Vector2 center;
        Rectangle source;
        bool readyForInput = false;
        float counter = 0f;

        public Vector2 Position { get; set; }
        public Dictionary<string, IFormComponent> Components { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }


        public Window(GraphicsDeviceManager graphics, Texture2D background, Vector2 position, Dictionary<string, IFormComponent> components)
        {
            this.graphics = graphics;
            this.background = background;
            Position = position;
            Components = components;
            Visible = true;
            if (background != null)
            {
                source = new Rectangle(0, 0, background.Width, background.Height);
                center = new Vector2(background.Width / 2, background.Height / 2);
            }
        }

        public virtual void SetActive()
        {
            resetReadyForInput();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                if (background != null)
                {
                    spriteBatch.Draw(
                        background,
                        Position,
                        source,
                        Color.White,
                        0f,
                        center,
                        1f,
                        SpriteEffects.None,
                        0
                    );
                }
                foreach (var component in Components)
                {
                    component.Value.Draw(spriteBatch);
                }
            }
        }

        protected bool isReadyForInput(GameTime gameTime)
        {
            if (!readyForInput)
            {
                counter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (counter > InputConstants.ResponseTime)
                {
                    counter = 0f;
                    readyForInput = true;
                }
            }

            return readyForInput;
        }

        protected void resetReadyForInput()
        {
            counter = 0f;
            readyForInput = false;
        }

        public abstract string Update(GameTime gameTime, params object[] values);
    }
}
