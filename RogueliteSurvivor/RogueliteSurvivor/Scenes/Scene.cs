using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.Containers;

namespace RogueliteSurvivor.Scenes
{
    public abstract class Scene : IScene
    {
        protected SpriteBatch _spriteBatch;
        protected ContentManager Content;
        protected GraphicsDeviceManager _graphics;

        protected ProgressionContainer progressionContainer;
        protected SettingsContainer settingsContainer;

        public bool Loaded { get; protected set; }

        public Scene(SpriteBatch spriteBatch, ContentManager contentManager, GraphicsDeviceManager graphics, ProgressionContainer progressionContainer, SettingsContainer settingsContainer)
        {
            _spriteBatch = spriteBatch;
            Content = contentManager;
            _graphics = graphics;
            this.progressionContainer = progressionContainer;

            Loaded = false;
            this.settingsContainer = settingsContainer;
        }

        public abstract void Draw(GameTime gameTime, Matrix transform, params object[] values);
        public abstract void LoadContent();
        public abstract void SetActive();
        public abstract string Update(GameTime gameTime, params object[] values);
    }
}
