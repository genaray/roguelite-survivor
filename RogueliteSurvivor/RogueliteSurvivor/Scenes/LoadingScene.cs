using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Scenes.SceneComponents;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RogueliteSurvivor.Scenes
{
    public class LoadingScene : Scene
    {
        private Dictionary<string, Texture2D> textures;
        private Dictionary<string, SpriteFont> fonts;
        private Dictionary<string, SoundEffect> soundEffects = null;

        private float counter = 0f;
        private readonly string[] dots = new string[4] { "", ".", "..", "..." };
        private int doot = 0;

        private List<Button> buttons;
        private int selectedButton = 1;

        public LoadingScene(SpriteBatch spriteBatch, ContentManager contentManager, GraphicsDeviceManager graphics, ProgressionContainer progressionContainer, SettingsContainer settingsContainer)
            : base(spriteBatch, contentManager, graphics, progressionContainer, settingsContainer)
        {
        }

        public override void LoadContent()
        {
            if (textures == null)
            {
                textures = new Dictionary<string, Texture2D>
                {
                    { "MainMenuButtons", Content.Load<Texture2D>(Path.Combine("UI", "main-menu-buttons")) },
                };
            }

            if (fonts == null)
            {
                fonts = new Dictionary<string, SpriteFont>()
                {
                    { "Font", Content.Load<SpriteFont>(Path.Combine("Fonts", "Font")) },
                };
            }

            if (soundEffects == null)
            {
                soundEffects = new Dictionary<string, SoundEffect>()
                {
                    { "Hover", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "001_Hover_01")) },
                    { "Confirm", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "013_Confirm_03")) },
                };
            }

            buttons = new List<Button>()
            {
                new Button(
                    textures["MainMenuButtons"],
                    new Vector2(GetWidthOffset(2), GetHeightOffset(2) + 96),
                    new Rectangle(0, 224, 128, 32),
                    new Rectangle(128, 224, 128, 32),
                    new Vector2(64, 16)
                )
            };

            Loaded = true;
        }

        public override void SetActive()
        {
            
        }

        public override string Update(GameTime gameTime, params object[] values)
        {
            string retVal = string.Empty;
            var kState = Keyboard.GetState();
            var gState = GamePad.GetState(PlayerIndex.One);
            var mState = Mouse.GetState();
            bool clicked = false;

            if ((bool)values[0])
            {
                if (mState.LeftButton == ButtonState.Pressed && buttons.Any(a => a.MouseOver()))
                {
                    clicked = true;
                    selectedButton = buttons.IndexOf(buttons.First(a => a.MouseOver())) + 1;
                }

                if (clicked || kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                {
                    retVal = "game";
                    doot = 0;
                    counter = 0f;
                    soundEffects["Confirm"].Play();
                }

                for (int i = 1; i <= buttons.Count; i++)
                {
                    buttons[i - 1].Selected(i == selectedButton);
                    buttons[i - 1].MouseOver(mState);
                }
            }
            else
            {
                counter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (counter > 0.33f)
                {
                    counter = 0f;
                    doot = (doot + 1) % 4;
                }
            }

            return retVal;
        }

        public override void Draw(GameTime gameTime, Matrix transformMatrix, params object[] values)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: transformMatrix);

            _spriteBatch.DrawString(
                fonts["Font"],
                "Roguelite Survivor",
                new Vector2(GetWidthOffset(2) - 62, GetHeightOffset(2) - 64),
                Color.White
            );

            if ((bool)values[0])
            {
                _spriteBatch.DrawString(
                    fonts["Font"],
                    "Get ready to send the undead back to their graves!",
                    new Vector2(GetWidthOffset(2) - 180, GetHeightOffset(2)),
                    Color.White
                );
            }
            else
            {
                _spriteBatch.DrawString(
                    fonts["Font"],
                    "Loading" + dots[doot],
                    new Vector2(GetWidthOffset(2) - 30, GetHeightOffset(2)),
                    Color.White
                );
            }


            if ((bool)values[0])
            {
                for (int i = 1; i <= buttons.Count; i++)
                {
                    buttons[i - 1].Draw(_spriteBatch);
                }
            }

            _spriteBatch.End();
        }
    }
}
