using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json.Linq;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Scenes.SceneComponents;
using RogueliteSurvivor.Scenes.Windows;
using RogueliteSurvivor.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RogueliteSurvivor.Scenes
{
    public class MainMenuScene : Scene
    {
        private Dictionary<string, Texture2D> textures = null;
        private Dictionary<string, SpriteFont> fonts = null;
        private Dictionary<string, Song> songs = null;
        private Dictionary<string, SoundEffect> soundEffects = null;

        private MainMenuState state;
        private Dictionary<string, Window> subScenes;

        private string selectedPlayer;
        private string selectedMap;

        private Dictionary<string, PlayerContainer> playerContainers;
        Dictionary<string, EnemyContainer> enemyContainers;
        private List<MapContainer> mapContainers;
        private CreditsContainer creditsContainer = null;

        public MainMenuScene(SpriteBatch spriteBatch, ContentManager contentManager, GraphicsDeviceManager graphics, Dictionary<string, PlayerContainer> playerContainers, Dictionary<string, MapContainer> mapContainers, ProgressionContainer progressionContainer, Dictionary<string, EnemyContainer> enemyContainers, SettingsContainer settingsContainer)
            : base(spriteBatch, contentManager, graphics, progressionContainer, settingsContainer)
        {
            this.playerContainers = playerContainers;
            this.mapContainers = mapContainers.Values.ToList();
            this.enemyContainers = enemyContainers;
        }

        public override void LoadContent()
        {
            if (textures == null)
            {
                textures = new Dictionary<string, Texture2D>
                {
                    { "MainMenuButtons", Content.Load<Texture2D>(Path.Combine("UI", "main-menu-buttons")) },
                    { "VolumeButtons", Content.Load<Texture2D>(Path.Combine("UI", "volume-buttons")) },
                    { "MainBackground", Content.Load<Texture2D>(Path.Combine("UI", "main-background")) },
                    { "PlayerSelectOutline", Content.Load<Texture2D>(Path.Combine("UI", "player-select-outline")) },
                    { "FireWizard", Content.Load<Texture2D>(Path.Combine("Player", "FireWizard")) },
                    { "IceWizard", Content.Load<Texture2D>(Path.Combine("Player", "IceWizard")) },
                    { "LightningWizard", Content.Load<Texture2D>(Path.Combine("Player", "LightningWizard")) },
                    { "FlyingWizard", Content.Load<Texture2D>(Path.Combine("Player", "FlyingWizard")) },
                };

                foreach (var map in mapContainers)
                {
                    textures.Add(map.Name, Content.Load<Texture2D>(Path.Combine("Maps", map.Folder, map.Name)));
                }
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
                    { "MenuTheme", Content.Load<Song>(Path.Combine("Music", "Night Ambient 2 (Loop)")) }
                };
            }


            if (soundEffects == null)
            {
                soundEffects = new Dictionary<string, SoundEffect>()
                {
                    { "Hover", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "001_Hover_01")) },
                    { "Confirm", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "013_Confirm_03")) },
                    { "Pickup", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "029_Decline_09")) },
                    { "Denied", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "033_Denied_03")) },
                };
            }

            if (creditsContainer == null)
            {
                JObject credits = JObject.Parse(File.ReadAllText(Path.Combine(Content.RootDirectory, "Datasets", "credits.json")));
                creditsContainer = CreditsContainer.ToCreditsContainer(credits["data"]);
            }

            state = MainMenuState.MainMenu;

            subScenes = new Dictionary<string, Window>()
            {
                {
                    MainMenuState.MainMenu.ToString(),
                    MainMenuWindow.MainMenuWindowFactory(
                        _graphics,
                        textures,
                        new Vector2(_graphics.GetWidthOffset(2), _graphics.GetHeightOffset(2)),
                        soundEffects["Hover"],
                        soundEffects["Confirm"],
                        fonts
                    )
                },
                {
                    MainMenuState.Options.ToString(),
                    OptionsMenuWindow.OptionsMenuWindowFactory(
                        _graphics,
                        textures,
                        new Vector2(_graphics.GetWidthOffset(2), _graphics.GetHeightOffset(2)),
                        soundEffects["Hover"],
                        soundEffects["Confirm"],
                        fonts,
                        settingsContainer
                    )
                },
                {
                    MainMenuState.Credits.ToString(),
                    CreditsWindow.CreditsWindowFactory(
                        _graphics,
                        textures,
                        new Vector2(_graphics.GetWidthOffset(2), _graphics.GetHeightOffset(2)),
                        soundEffects["Hover"],
                        soundEffects["Confirm"],
                        fonts,
                        creditsContainer
                    )
                },
                {
                    MainMenuState.PlayerUpgrades.ToString(),
                    PlayerUpgradesWindow.PlayerUpgradesWindowFactory(
                        _graphics,
                        textures,
                        new Vector2(_graphics.GetWidthOffset(2), _graphics.GetHeightOffset(2)),
                        soundEffects["Hover"],
                        soundEffects["Confirm"],
                        soundEffects["Denied"],
                        fonts,
                        progressionContainer
                    )
                },
                {
                    MainMenuState.PlayStats.ToString(),
                    PlayStatsWindow.PlayStatsWindowFactory(
                        _graphics,
                        textures,
                        new Vector2(_graphics.GetWidthOffset(2), _graphics.GetHeightOffset(2)),
                        soundEffects["Hover"],
                        soundEffects["Confirm"],
                        fonts,
                        progressionContainer,
                        enemyContainers
                    )
                },
                {
                    MainMenuState.CharacterSelection.ToString(),
                    CharacterSelectionWindow.CharacterSelectionWindowFactory(
                        _graphics,
                        textures,
                        new Vector2(_graphics.GetWidthOffset(2), _graphics.GetHeightOffset(2)),
                        soundEffects["Hover"],
                        soundEffects["Confirm"],
                        fonts,
                        playerContainers,
                        progressionContainer)
                },
                {
                    MainMenuState.MapSelection.ToString(),
                    MapSelectionWindow.MapSelectionWindowFactory(
                        _graphics,
                        textures,
                        new Vector2(_graphics.GetWidthOffset(2), _graphics.GetHeightOffset(2)),
                        soundEffects["Hover"],
                        soundEffects["Confirm"],
                        soundEffects["Denied"],
                        fonts,
                        mapContainers,
                        progressionContainer)
                }
            };
            Loaded = true;
        }

        public override void SetActive()
        {
            MediaPlayer.Play(songs["MenuTheme"]);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.MenuMusicVolume;
        }

        public override string Update(GameTime gameTime, params object[] values)
        {
            string retVal = string.Empty;

            string action = subScenes[state.ToString()].Update(gameTime);

            if (state == MainMenuState.MainMenu)
            {
                switch (action)
                {
                    case "CharacterSelection":
                        state = MainMenuState.CharacterSelection;
                        break;
                    case "PlayerUpgrades":
                        state = MainMenuState.PlayerUpgrades;
                        break;
                    case "PlayStats":
                        state = MainMenuState.PlayStats;
                        break;
                    case "Options":
                        state = MainMenuState.Options;
                        break;
                    case "Credits":
                        state = MainMenuState.Credits;
                        break;
                    case "exit":
                        retVal = "exit";
                        break;
                }
            }
            else if (state == MainMenuState.CharacterSelection)
            {
                if (!string.IsNullOrEmpty(action))
                {
                    switch (action)
                    {
                        case "menu":
                            state = MainMenuState.MainMenu;
                            break;
                        default:
                            state = MainMenuState.MapSelection;
                            selectedPlayer = action;
                            break;
                    }
                }
            }
            else if (state == MainMenuState.MapSelection)
            {
                if (!string.IsNullOrEmpty(action))
                {
                    switch (action)
                    {
                        case "menu":
                            state = MainMenuState.CharacterSelection;
                            break;
                        default:
                            state = MainMenuState.MainMenu;
                            selectedMap = action;
                            retVal = "loading";
                            break;
                    }
                }
            }
            else if (state == MainMenuState.Options ||
                        state == MainMenuState.Credits ||
                        state == MainMenuState.PlayerUpgrades ||
                        state == MainMenuState.PlayStats)
            {
                switch (action)
                {
                    case "menu":
                        state = MainMenuState.MainMenu;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(action) && subScenes.ContainsKey(state.ToString()))
            {
                subScenes[state.ToString()].SetActive();
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

            subScenes[state.ToString()].Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public GameSettings GetGameSettings()
        {
            var gameSettings = new GameSettings()
            {
                PlayerName = selectedPlayer,
                MapName = selectedMap,
            };

            return gameSettings;
        }
    }
}
