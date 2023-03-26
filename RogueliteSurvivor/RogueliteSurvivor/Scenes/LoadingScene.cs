﻿using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueliteSurvivor.Containers;
using System.Collections.Generic;
using System.IO;

namespace RogueliteSurvivor.Scenes
{
    public class LoadingScene : Scene
    {
        //private Dictionary<string, Texture2D> textures;
        private Dictionary<string, SpriteFont> fonts;

        private float counter = 0f;
        private readonly string[] dots = new string[4] { "", ".", "..", "..." };
        private int doot = 0;

        public LoadingScene(SpriteBatch spriteBatch, ContentManager contentManager, GraphicsDeviceManager graphics, ProgressionContainer progressionContainer, float scaleFactor, SettingsContainer settingsContainer)
            : base(spriteBatch, contentManager, graphics, progressionContainer, scaleFactor, settingsContainer)
        {
        }

        public override void LoadContent()
        {
            fonts = new Dictionary<string, SpriteFont>()
            {
                { "Font", Content.Load<SpriteFont>(Path.Combine("Fonts", "Font")) },
            };

            Loaded = true;
        }

        public override void SetActive()
        {
            
        }

        public override string Update(GameTime gameTime, params object[] values)
        {
            string retVal = string.Empty;

            if ((bool)values[0])
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                {
                    retVal = "game";
                    doot = 0;
                    counter = 0f;
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
                _spriteBatch.DrawString(
                    fonts["Font"],
                    "Press Enter on the keyboard or A on the controller to start",
                    new Vector2(GetWidthOffset(2) - 200, GetHeightOffset(2) + 32),
                    Color.White
                );
            }

            _spriteBatch.End();
        }
    }
}
