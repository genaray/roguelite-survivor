using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Helpers;
using RogueliteSurvivor.Scenes.SceneComponents;
using RogueliteSurvivor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RogueliteSurvivor.Scenes
{
    public class GameOverScene : Scene
    {
        private Dictionary<string, Texture2D> textures;
        private Dictionary<string, SpriteFont> fonts = null;

        private Dictionary<string, MapContainer> mapContainers;
        private Dictionary<string, Song> songs = null;
        private Dictionary<string, SoundEffect> soundEffects = null;

        private GameSettings gameSettings;
        private GameStats gameStats;
        private bool saved = false;
        private bool newBest = false;
        private MapContainer unlockedMap;
        private GameOverState state = GameOverState.Main;
        private float stateChangeTime = .11f;

        private List<Button> buttons;
        private int selectedButton = 1;

        public GameOverScene(SpriteBatch spriteBatch, ContentManager contentManager, GraphicsDeviceManager graphics, ProgressionContainer progressionContainer, Dictionary<string, MapContainer> mapContainers, SettingsContainer settingsContainer)
            : base(spriteBatch, contentManager, graphics, progressionContainer, settingsContainer)
        {
            this.mapContainers = mapContainers;
        }

        public override void LoadContent()
        {
            if (textures == null)
            {
                textures = new Dictionary<string, Texture2D>
                {
                    { "MainMenuButtons", Content.Load<Texture2D>(Path.Combine("UI", "main-menu-buttons")) },
                    { "MainBackground", Content.Load<Texture2D>(Path.Combine("UI", "main-background")) },
                };
            }

            if (fonts == null)
            {
                fonts = new Dictionary<string, SpriteFont>()
                {
                    { "Font", Content.Load<SpriteFont>(Path.Combine("Fonts", "Font")) },
                    { "FontSmall", Content.Load<SpriteFont>(Path.Combine("Fonts", "FontSmall")) },
                };
            }

            if (songs == null)
            {
                songs = new Dictionary<string, Song> {
                    { "GameOver", Content.Load<Song>(Path.Combine("Music", "Fx 3")) }
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
                    new Rectangle(0, 320, 128, 32),
                    new Rectangle(128, 320, 128, 32),
                    new Vector2(64, 16)
                ),
                new Button(
                    textures["MainMenuButtons"],
                    new Vector2(GetWidthOffset(2), GetHeightOffset(2) + 96),
                    new Rectangle(0, 352, 128, 32),
                    new Rectangle(128, 352, 128, 32),
                    new Vector2(64, 16)
                ),
                new Button(
                    textures["MainMenuButtons"],
                    new Vector2(GetWidthOffset(2), GetHeightOffset(2) + 144),
                    new Rectangle(0, 192, 128, 32),
                    new Rectangle(128, 192, 128, 32),
                    new Vector2(64, 16)
                ),
            };

            Loaded = true;
        }

        public override void SetActive()
        {
            MediaPlayer.Play(songs["GameOver"]);
            MediaPlayer.IsRepeating = false; 
            MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.MenuMusicVolume;
            state = GameOverState.Main;
            buttons[0].Visible(false);
            buttons[1].Visible(true);
            selectedButton = 2;
        }

        public void SetGameSettings(GameSettings gameSettings)
        {
            this.gameSettings = gameSettings;
        }

        public void SetGameStats(GameStats gameStats)
        {
            this.gameStats = gameStats;
        }

        public override string Update(GameTime gameTime, params object[] values)
        {
            string retVal = string.Empty;
            var kState = Keyboard.GetState();
            var gState = GamePad.GetState(PlayerIndex.One);
            var mState = Mouse.GetState();
            bool clicked = false;

            stateChangeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!saved)
            {
                unlockedMap = null;
                var level = progressionContainer.LevelProgressions.Where(a => a.Name == gameSettings.MapName).FirstOrDefault();
                newBest = gameStats.PlayTime > level.BestTime;

                if (newBest)
                {                    
                    unlockedMap = mapContainers.Values.Where(a => a.UnlockRequirement.MapUnlockType == Constants.MapUnlockType.MapBestTime 
                                                                    && a.UnlockRequirement.RequirementText == gameSettings.MapName
                                                                    && a.UnlockRequirement.RequirementAmount <= gameStats.PlayTime).FirstOrDefault();

                    if(unlockedMap != null && level.BestTime >= unlockedMap.UnlockRequirement.RequirementAmount)
                    {
                        unlockedMap = null;
                    }

                    level.BestTime = MathF.Max(gameStats.PlayTime, level.BestTime);
                }

                if(progressionContainer.EnemyKillStats == null)
                {
                    progressionContainer.EnemyKillStats = new List<EnemyKillStatsContainer>();
                }

                foreach(var enemy in gameStats.Kills)
                {
                    var enemyStats = progressionContainer.EnemyKillStats.Where(a => a.Name == enemy.Key).FirstOrDefault();
                    if(enemyStats == null)
                    {
                        enemyStats = new EnemyKillStatsContainer() { Name = enemy.Key, Kills = enemy.Value, KilledBy = gameStats.Killer == enemy.Key ? 1 : 0 };
                        progressionContainer.EnemyKillStats.Add(enemyStats);
                    }
                    else
                    {
                        enemyStats.Kills += enemy.Value;
                        enemyStats.KilledBy += gameStats.Killer == enemy.Key ? 1 : 0;
                    }
                }
                progressionContainer.NumBooks += gameStats.NumBooks;
                progressionContainer.Save();
                saved = true;
            }

            if (stateChangeTime > InputConstants.ResponseTime)
            {
                if (mState.LeftButton == ButtonState.Pressed && buttons.Any(a => a.MouseOver()))
                {
                    clicked = true;
                    selectedButton = buttons.IndexOf(buttons.First(a => a.MouseOver())) + 1;
                }

                if (clicked || kState.IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                {
                    switch (selectedButton)
                    {
                        case 1:
                            state = GameOverState.Main;
                            buttons[0].Visible(false);
                            buttons[1].Visible(true);
                            selectedButton = 2;
                            break;
                        case 2:
                            state = GameOverState.AdvancedStats;
                            buttons[0].Visible(true);
                            buttons[1].Visible(false);
                            selectedButton = 1;
                            break;
                        case 3:
                            retVal = "main-menu";
                            saved = false;
                            break;
                    }
                    soundEffects["Confirm"].Play();
                    stateChangeTime = 0;
                }
                else if (kState.IsKeyDown(Keys.Up) || gState.DPad.Up == ButtonState.Pressed || gState.ThumbSticks.Left.Y > 0.5f)
                {
                    if (selectedButton == 3)
                    {
                        selectedButton = state == GameOverState.Main ? 2 : 1;
                        soundEffects["Hover"].Play();
                        stateChangeTime = 0;
                    }
                }
                else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                {
                    if (selectedButton < 3)
                    {
                        selectedButton = 3;
                        soundEffects["Hover"].Play();
                        stateChangeTime = 0;
                    }
                }
            }

            for (int i = 1; i <= buttons.Count; i++)
            {
                buttons[i - 1].Selected(i == selectedButton);
                buttons[i - 1].MouseOver(mState);
            }

            return retVal;
        }

        public override void Draw(GameTime gameTime, Matrix transformMatrix, params object[] values)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: transformMatrix);

            _spriteBatch.Draw(
                textures["MainBackground"],
                Vector2.Zero,
                new Rectangle(0, 0, 640, 360),
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );

            if (state == GameOverState.Main)
            {
                _spriteBatch.DrawString(
                    fonts["Font"],
                   string.Concat("You survived for ", gameStats.PlayTime.ToFormattedTime(), " before ", getProperArticle(gameStats.Killer), gameStats.Killer, " ", gameStats.KillerMethod," you into oblivion..."),
                    new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) - 64),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["Font"],
                   string.Concat("You killed ", gameStats.EnemiesKilled, " enemies and collected ", gameStats.NumBooks, " books to learn from!"),
                    new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) - 32),
                    Color.White
                );

                if (newBest)
                {
                    _spriteBatch.DrawString(
                        fonts["Font"],
                       string.Concat("New best time for ", gameSettings.MapName, "!"),
                        new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2)),
                        Color.White
                    );
                }

                if (unlockedMap != null)
                {
                    _spriteBatch.DrawString(
                        fonts["Font"],
                       string.Concat("You've unlocked ", unlockedMap.Name, "!"),
                        new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) + 32),
                        Color.White
                    );
                }
            }
            else if(state == GameOverState.AdvancedStats)
            {
                _spriteBatch.DrawString(
                    fonts["Font"],
                    "Enemies Killed: ",
                    new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) - 64),
                    Color.White
                );

                int counter = 0;
                foreach(var enemy in gameStats.Kills)
                {
                    _spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat(" - ", enemy.Key, " : ", enemy.Value),
                        new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) - 48 + counter),
                        Color.White
                    );
                    counter += 12;
                }

            }

            for (int i = 1; i <= buttons.Count; i++)
            {
                buttons[i - 1].Draw(_spriteBatch);
            }

            _spriteBatch.End();
        }

        private string getProperArticle(string enemyName)
        {
            if (enemyName.StartsWith('A') || enemyName.StartsWith('E') || enemyName.StartsWith('I') || enemyName.StartsWith('O') || enemyName.StartsWith('U'))
            {
                return "an ";
            }

            return "a ";
        }
    }
}
