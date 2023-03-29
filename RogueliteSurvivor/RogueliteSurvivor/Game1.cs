using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json.Linq;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Physics;
using RogueliteSurvivor.Scenes;
using RogueliteSurvivor.Utils;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RogueliteSurvivor
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static float ScaleFactor;
        private Matrix transformMatrix;

        Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        Dictionary<string, PlayerContainer> playerCharacters = new Dictionary<string, PlayerContainer>();
        Dictionary<string, MapContainer> mapContainers = new Dictionary<string, MapContainer>();
        Dictionary<string, EnemyContainer> enemyContainers = new Dictionary<string, EnemyContainer>();
        ProgressionContainer progressionContainer = null;
        SettingsContainer settingsContainer = null;
        string currentScene = "main-menu";
        string nextScene = string.Empty;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.ApplyChanges(); //Needed because the graphics device is null before this is called
            _graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            ScaleFactor = GraphicsDevice.Adapter.CurrentDisplayMode.Width / 640f;
            _graphics.ApplyChanges();

#if DEBUG
            //Do Nothing
#else
            _graphics.ToggleFullScreen();
#endif
        }

        protected override void Initialize()
        {
            transformMatrix = Matrix.CreateScale(ScaleFactor, ScaleFactor, 1f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            loadSettings();
            loadPlayerCharacters();
            loadPlayableMaps();
            loadProgression();
            loadEnemies();

            GameScene gameScene = new GameScene(_spriteBatch, Content, _graphics, playerCharacters, mapContainers, progressionContainer, enemyContainers, settingsContainer);

            MainMenuScene mainMenu = new MainMenuScene(_spriteBatch, Content, _graphics, playerCharacters, mapContainers, progressionContainer, enemyContainers, settingsContainer);
            mainMenu.LoadContent();
            mainMenu.SetActive();

            LoadingScene loadingScene = new LoadingScene(_spriteBatch, Content, _graphics, progressionContainer, settingsContainer);
            loadingScene.LoadContent();

            GameOverScene gameOverScene = new GameOverScene(_spriteBatch, Content, _graphics, progressionContainer, mapContainers, settingsContainer);
            gameOverScene.LoadContent();

            scenes.Add("game", gameScene);
            scenes.Add("main-menu", mainMenu);
            scenes.Add("loading", loadingScene);
            scenes.Add("game-over", gameOverScene);
        }

        private void loadPlayerCharacters()
        {
            JObject players = JObject.Parse(File.ReadAllText(Path.Combine(Content.RootDirectory, "Datasets", "player-characters.json")));
            playerCharacters = new Dictionary<string, PlayerContainer>();

            foreach (var player in players["data"])
            {
                playerCharacters.Add(
                    PlayerContainer.GetPlayerContainerName(player),
                    PlayerContainer.ToPlayerContainer(player)
                );
            }
        }

        private void loadPlayableMaps()
        {
            JObject maps = JObject.Parse(File.ReadAllText(Path.Combine(Content.RootDirectory, "Datasets", "maps.json")));
            mapContainers = new Dictionary<string, MapContainer>();

            foreach (var map in maps["data"])
            {
                mapContainers.Add(
                    MapContainer.MapContainerName(map),
                    MapContainer.ToMapContainer(map)
                );
            }
        }

        private void loadEnemies()
        {
            JObject enemies = JObject.Parse(File.ReadAllText(Path.Combine(Content.RootDirectory, "Datasets", "enemies.json")));

            foreach (var enemy in enemies["data"])
            {
                enemyContainers.Add(
                    EnemyContainer.EnemyContainerName(enemy),
                    EnemyContainer.ToEnemyContainer(enemy)
                );
            }
        }

        private void loadProgression()
        {
            if(!File.Exists(Path.Combine("Saves", "savegame.json")))
            {
                progressionContainer = new ProgressionContainer();
                progressionContainer.LevelProgressions = new List<LevelProgressionContainer>();
                
                foreach(var map in mapContainers)
                {
                    progressionContainer.LevelProgressions.Add(new LevelProgressionContainer()
                    {
                        Name = map.Key,
                        BestTime = 0
                    });
                }

                progressionContainer.Save();
            }
            else
            {
                JObject progression = JObject.Parse(File.ReadAllText(Path.Combine("Saves", "savegame.json")));
                progressionContainer = ProgressionContainer.ToProgressionContainer(progression);

                foreach(var map in mapContainers)
                {
                    if(!progressionContainer.LevelProgressions.Exists(a => a.Name == map.Key))
                    {
                        progressionContainer.LevelProgressions.Add(new LevelProgressionContainer()
                        {
                            Name = map.Key,
                            BestTime = 0
                        });
                    }
                }
            }
        }

        private void loadSettings()
        {
            settingsContainer = new SettingsContainer();
            if (!File.Exists("settings.json"))
            {
                settingsContainer.Save();
            }
            else
            {
                JObject settings = JObject.Parse(File.ReadAllText("settings.json"));
                settingsContainer = SettingsContainer.ToSettingsContainer(settings);
            }

            SoundEffect.MasterVolume = settingsContainer.MasterVolume * settingsContainer.SoundEffectsVolume;
        }

        protected override void Update(GameTime gameTime)
        {
            switch (currentScene)
            {
                case "loading":
                    nextScene = scenes[currentScene].Update(gameTime, scenes["game"].Loaded);
                    break;
                default:
                    nextScene = scenes[currentScene].Update(gameTime);
                    break;
            }

            if (!string.IsNullOrEmpty(nextScene))
            {
                switch (nextScene)
                {
                    case "game":
                        break;
                    case "main-menu":
                        break;
                    case "game-over":
                        GameStats gameStats = ((GameScene)scenes["game"]).GetGameStats();
                        ((GameOverScene)scenes["game-over"]).SetGameStats(gameStats);
                        break;
                    case "loading":
                        GameSettings gameSettings = ((MainMenuScene)scenes["main-menu"]).GetGameSettings();
                        ((GameOverScene)scenes["game-over"]).SetGameSettings(gameSettings);
                        Task.Run(() =>
                            {
                                ((GameScene)scenes["game"]).SetGameSettings(gameSettings);
                                scenes["game"].LoadContent();
                            });
                        break;
                    case "exit":
                        Exit();
                        break;
                }

                currentScene = nextScene;
                scenes[currentScene].SetActive();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (currentScene)
            {
                case "loading":
                    scenes[currentScene].Draw(gameTime, transformMatrix, scenes["game"].Loaded);
                    break;
                default:
                    scenes[currentScene].Draw(gameTime, transformMatrix);
                    break;
            }

            base.Draw(gameTime);
        }
    }
}
