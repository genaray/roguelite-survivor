using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json.Linq;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Helpers;
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

        private bool readyForInput = false;
        private float counter = 0f;

        private MainMenuState state;
        private Dictionary<string, Window> subScenes;

        private string selectedPlayer;
        private int selectedButton = 1;
        private string selectedMap;

        private List<IFormComponent> characterSelectionComponents;
        private List<Button> mapSelectionButtons;

        private Dictionary<string, PlayerContainer> playerContainers;
        Dictionary<string, EnemyContainer> enemyContainers;
        private List<MapContainer> mapContainers;
        private List<MapContainer> unlockedMaps;
        private CreditsContainer creditsContainer = null;

        private int descriptionLength = 80;


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
            selectedPlayer = playerContainers.First().Key;
            selectedMap = mapContainers.First().Name;

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
                }
            };

            characterSelectionComponents = new List<IFormComponent>()
            {
                new SelectableOption
                    (
                    "seoFireWizard",
                    textures["FireWizard"],
                    textures["PlayerSelectOutline"],
                    new Vector2(_graphics.GetWidthOffset(10.66f), _graphics.GetHeightOffset(2) - 64),
                    new Rectangle(16, 0, 16, 16),
                    new Rectangle(0, 0, 20, 20),
                    new Vector2(8, 8),
                    new Vector2(10, 10)
                ),
                new SelectableOption
                    (
                    "seoIceWizard",
                    textures["IceWizard"],
                    textures["PlayerSelectOutline"],
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 24, _graphics.GetHeightOffset(2) - 64),
                    new Rectangle(16, 0, 16, 16),
                    new Rectangle(0, 0, 20, 20),
                    new Vector2(8, 8),
                    new Vector2(10, 10)
                ),
                new SelectableOption
                    (
                    "seoLightningWizard",
                    textures["LightningWizard"],
                    textures["PlayerSelectOutline"],
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 48, _graphics.GetHeightOffset(2) - 64),
                    new Rectangle(16, 0, 16, 16),
                    new Rectangle(0, 0, 20, 20),
                    new Vector2(8, 8),
                    new Vector2(10, 10)
                ),
                new SelectableOption
                    (
                    "seoFlyingWizard",
                    textures["FlyingWizard"],
                    textures["PlayerSelectOutline"],
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 72, _graphics.GetHeightOffset(2) - 64),
                    new Rectangle(16, 0, 16, 16),
                    new Rectangle(0, 0, 20, 20),
                    new Vector2(8, 8),
                    new Vector2(10, 10)
                ),
                new Button(
                    "btnBack",
                    textures["MainMenuButtons"],
                    new Vector2(_graphics.GetWidthOffset(2), _graphics.GetHeightOffset(2) + 144),
                    new Rectangle(0, 0, 128, 32),
                    new Rectangle(128, 0, 128, 32),
                    new Vector2(64, 16)
                ),
            };

            mapSelectionButtons = new List<Button>()
            {
                new Button(
                    "btnPreviousMap",
                    textures["MainMenuButtons"],
                    new Vector2(_graphics.GetWidthOffset(2) - 160, _graphics.GetHeightOffset(2) + 96),
                    new Rectangle(0, 288, 128, 32),
                    new Rectangle(128, 288, 128, 32),
                    new Vector2(64, 16)
                ),
                new Button(
                    "btnStart",
                    textures["MainMenuButtons"],
                    new Vector2(_graphics.GetWidthOffset(2), _graphics.GetHeightOffset(2) + 96),
                    new Rectangle(0, 224, 128, 32),
                    new Rectangle(128, 224, 128, 32),
                    new Vector2(64, 16)
                ),
                new Button(
                    "btnNextMap",
                    textures["MainMenuButtons"],
                    new Vector2(_graphics.GetWidthOffset(2) + 160, _graphics.GetHeightOffset(2) + 96),
                    new Rectangle(0, 256, 128, 32),
                    new Rectangle(128, 256, 128, 32),
                    new Vector2(64, 16)
                ),
                new Button(
                    "btnBack",
                    textures["MainMenuButtons"],
                    new Vector2(_graphics.GetWidthOffset(2), _graphics.GetHeightOffset(2) + 144),
                    new Rectangle(0, 0, 128, 32),
                    new Rectangle(128, 0, 128, 32),
                    new Vector2(64, 16)
                ),
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

            if (!readyForInput)
            {
                counter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (counter > InputConstants.ResponseTime)
                {
                    counter = 0f;
                    readyForInput = true;
                }
            }
            else if (readyForInput)
            {
                var kState = Keyboard.GetState();
                var gState = GamePad.GetState(PlayerIndex.One);
                var mState = Mouse.GetState();
                bool clicked = false;

                if (state == MainMenuState.MainMenu)
                {
                    string action = subScenes[state.ToString()].Update(gameTime);
                    switch (action)
                    {
                        case "CharacterSelection":
                            state = MainMenuState.CharacterSelection;
                            selectedButton = 1;
                            setSelectedPlayer();
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

                    if (!string.IsNullOrEmpty(action) && subScenes.ContainsKey(state.ToString()))
                    {
                        subScenes[state.ToString()].SetActive();
                    }
                }
                else if (state == MainMenuState.CharacterSelection)
                {
                    if (mState.LeftButton == ButtonState.Pressed && characterSelectionComponents.Any(a => a is ISelectableComponent && ((ISelectableComponent)a).MouseOver()))
                    {
                        clicked = true;
                        selectedButton = characterSelectionComponents.IndexOf(characterSelectionComponents.First(a => a is ISelectableComponent && ((ISelectableComponent)a).MouseOver())) + 1;
                        setSelectedPlayer();
                    }

                    if (clicked || kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                    {
                        if (selectedButton <= playerContainers.Count)
                        {
                            state = MainMenuState.MapSelection;
                            selectedButton = 2;
                            setUnlockedMaps();
                        }
                        else
                        {
                            state = MainMenuState.MainMenu;
                            selectedButton = 1;
                        }
                        soundEffects["Confirm"].Play();
                        readyForInput = false;

                    }
                    else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                    {
                        if (selectedButton > 1)
                        {
                            selectedButton--;
                            setSelectedPlayer();
                            soundEffects["Hover"].Play();
                            readyForInput = false;
                        }
                    }
                    else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                    {
                        if (selectedButton < playerContainers.Count)
                        {
                            selectedButton++;
                            setSelectedPlayer();
                            soundEffects["Hover"].Play();
                            readyForInput = false;
                        }
                    }
                    else if (kState.IsKeyDown(Keys.Up) || gState.DPad.Up == ButtonState.Pressed || gState.ThumbSticks.Left.Y > 0.5f)
                    {
                        if ((selectedButton - 1) == playerContainers.Count)
                        {
                            selectedButton = 1;
                            soundEffects["Hover"].Play();
                            readyForInput = false;
                        }
                    }
                    else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                    {
                        if ((selectedButton - 1) < playerContainers.Count)
                        {
                            selectedButton = playerContainers.Count + 1;
                            soundEffects["Hover"].Play();
                            readyForInput = false;
                        }
                    }

                    for (int i = 1; i <= characterSelectionComponents.Count; i++)
                    {
                        if (characterSelectionComponents[i - 1] is ISelectableComponent)
                        {
                            ((ISelectableComponent)characterSelectionComponents[i - 1]).Selected = i == selectedButton;
                            ((ISelectableComponent)characterSelectionComponents[i - 1]).MouseOver(mState);
                        }
                    }
                }
                else if (state == MainMenuState.MapSelection)
                {
                    if (mState.LeftButton == ButtonState.Pressed && mapSelectionButtons.Any(a => a.MouseOver()))
                    {
                        clicked = true;
                        selectedButton = mapSelectionButtons.IndexOf(mapSelectionButtons.First(a => a.MouseOver())) + 1;
                    }

                    if (kState.IsKeyDown(Keys.Up) || gState.DPad.Up == ButtonState.Pressed || gState.ThumbSticks.Left.Y > 0.5f)
                    {
                        if (selectedButton == 4)
                        {
                            selectedButton = 2;
                            soundEffects["Hover"].Play();
                            readyForInput = false;
                        }
                    }
                    else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                    {
                        if (selectedButton < 4)
                        {
                            selectedButton = 4;
                            soundEffects["Hover"].Play();
                            readyForInput = false;
                        }
                    }
                    else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                    {
                        if (selectedButton == 2 || selectedButton == 3)
                        {
                            selectedButton--;
                            soundEffects["Hover"].Play();
                            readyForInput = false;
                        }
                    }
                    else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                    {
                        if (selectedButton == 1 || selectedButton == 2)
                        {
                            selectedButton++;
                            soundEffects["Hover"].Play();
                            readyForInput = false;
                        }
                    }
                    else if (clicked || kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                    {
                        switch (selectedButton)
                        {
                            case 1:
                                if (selectedMap != mapContainers[0].Name)
                                {
                                    int index = mapContainers.IndexOf(mapContainers.Where(a => a.Name == selectedMap).First()) - 1;
                                    selectedMap = mapContainers[index].Name;
                                    soundEffects["Hover"].Play();
                                    readyForInput = false;
                                }
                                break;
                            case 2:
                                if (unlockedMaps.Exists(a => a.Name == selectedMap))
                                {
                                    state = MainMenuState.MainMenu;
                                    retVal = "loading";
                                    readyForInput = false;
                                    selectedButton = 1;
                                    soundEffects["Confirm"].Play();
                                }
                                else
                                {
                                    soundEffects["Denied"].Play();
                                }
                                break;
                            case 3:
                                if (selectedMap != mapContainers.Last().Name)
                                {
                                    int index = mapContainers.IndexOf(mapContainers.Where(a => a.Name == selectedMap).First()) + 1;
                                    selectedMap = mapContainers[index].Name;
                                    soundEffects["Hover"].Play();
                                    readyForInput = false;
                                }
                                break;
                            case 4:
                                state = MainMenuState.CharacterSelection;
                                selectedButton = playerContainers.Keys.ToList().IndexOf(selectedPlayer) + 1;
                                readyForInput = false;
                                break;
                        }
                        soundEffects["Confirm"].Play();
                    }

                    for (int i = 1; i <= mapSelectionButtons.Count; i++)
                    {
                        mapSelectionButtons[i - 1].Selected = i == selectedButton;
                        mapSelectionButtons[i - 1].MouseOver(mState);
                    }
                }
                else if (state == MainMenuState.Options ||
                            state == MainMenuState.Credits ||
                            state == MainMenuState.PlayerUpgrades ||
                            state == MainMenuState.PlayStats)
                {
                    switch (subScenes[state.ToString()].Update(gameTime))
                    {
                        case "menu":
                            state = MainMenuState.MainMenu;
                            subScenes[state.ToString()].SetActive();
                            break;
                    }
                }
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


            if (state == MainMenuState.MainMenu
                || state == MainMenuState.Options
                || state == MainMenuState.Credits
                || state == MainMenuState.PlayerUpgrades
                || state == MainMenuState.PlayStats)
            {
                subScenes[state.ToString()].Draw(_spriteBatch);
            }
            else if (state == MainMenuState.CharacterSelection)
            {
                _spriteBatch.DrawString(
                    fonts["Font"],
                    "Choose your wizard:",
                    new Vector2(_graphics.GetWidthOffset(2) - 70, _graphics.GetHeightOffset(2) - 96),
                    Color.White
                );

                for (int i = 1; i <= characterSelectionComponents.Count; i++)
                {
                    characterSelectionComponents[i - 1].Draw(_spriteBatch);
                }

                int counter = -64;
                var playerContainer = playerContainers[selectedPlayer];
                foreach (var paragraph in playerContainer.Description)
                {
                    List<string> descriptionLines = new List<string>();
                    if (paragraph.Length < descriptionLength)
                    {
                        descriptionLines.Add(paragraph);
                    }
                    else
                    {
                        int startCharacter = 0;
                        do
                        {
                            int nextSpace = paragraph.LastIndexOf(' ', startCharacter + descriptionLength, descriptionLength);
                            descriptionLines.Add(paragraph.Substring(startCharacter, nextSpace - startCharacter));
                            startCharacter = nextSpace + 1;
                        } while (paragraph.Substring(startCharacter).Length > descriptionLength);
                        descriptionLines.Add(paragraph.Substring(startCharacter));
                    }


                    foreach (var descriptionLine in descriptionLines)
                    {
                        _spriteBatch.DrawString(
                            fonts["FontSmall"],
                            descriptionLine,
                            new Vector2(_graphics.GetWidthOffset(10.66f) + 125, _graphics.GetHeightOffset(2) + counter),
                            Color.White
                        );
                        counter += 12;
                    }
                    counter += 12;
                }

                counter += 12;

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    "Base Stats: ",
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 125, _graphics.GetHeightOffset(2) + counter),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Spell: ", playerContainer.StartingSpell.GetReadableSpellName()),
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 125, _graphics.GetHeightOffset(2) + counter + 12),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Starting Health: ", (int)(playerContainer.Health * 100 + progressionContainer.PlayerUpgrades.Health)),
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 125, _graphics.GetHeightOffset(2) + counter + 24),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Move Speed: ", (int)(playerContainer.Speed * 100 + progressionContainer.PlayerUpgrades.MoveSpeed)),
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 125, _graphics.GetHeightOffset(2) + counter + 36),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Spell Damage: ", (playerContainer.SpellDamage + 1f + (progressionContainer.PlayerUpgrades.Damage / 100f)).ToString("F"), "x"),
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 125, _graphics.GetHeightOffset(2) + counter + 48),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Spell Effect Chance: ", (playerContainer.SpellEffectChance + 1f + (progressionContainer.PlayerUpgrades.SpellEffectChance / 100f)).ToString("F"), "x"),
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 240, _graphics.GetHeightOffset(2) + counter + 12),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Attack Speed: ", (playerContainer.AttackSpeed + 1f + (progressionContainer.PlayerUpgrades.AttackSpeed / 100f)).ToString("F"), "x"),
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 240, _graphics.GetHeightOffset(2) + counter + 24),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Pierce: ", playerContainer.Pierce + progressionContainer.PlayerUpgrades.Pierce),
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 240, _graphics.GetHeightOffset(2) + counter + 36),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Area of Effect: ", (playerContainer.AreaOfEffect + 1f + (progressionContainer.PlayerUpgrades.AreaOfEffect / 100f)).ToString("F"), "x"),
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 240, _graphics.GetHeightOffset(2) + counter + 48),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    "Special Traits: ",
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 385, _graphics.GetHeightOffset(2) + counter),
                    Color.White
                );

                if (playerContainer.Traits.Any())
                {
                    int offsetY = 12;
                    foreach (var trait in playerContainer.Traits)
                    {
                        _spriteBatch.DrawString(
                            fonts["FontSmall"],
                            trait.ReadableTraitName(),
                            new Vector2(_graphics.GetWidthOffset(10.66f) + 385, _graphics.GetHeightOffset(2) + counter + offsetY),
                            Color.White
                        );
                        offsetY += 12;
                    }
                }
                else
                {
                    _spriteBatch.DrawString(
                        fonts["FontSmall"],
                        "None",
                        new Vector2(_graphics.GetWidthOffset(10.66f) + 385, _graphics.GetHeightOffset(2) + counter + 12),
                        Color.White
                    );
                }
            }
            else if (state == MainMenuState.MapSelection)
            {
                _spriteBatch.DrawString(
                    fonts["Font"],
                    "Select a map:",
                    new Vector2(_graphics.GetWidthOffset(2) - 50, _graphics.GetHeightOffset(2) - 96),
                    Color.White
                );

                var map = mapContainers.Where(a => a.Name == selectedMap).First();

                _spriteBatch.DrawString(
                    fonts["Font"],
                    map.Name,
                    new Vector2(_graphics.GetWidthOffset(10.66f), _graphics.GetHeightOffset(2) - 64),
                    Color.White
                );

                _spriteBatch.Draw(
                    textures[map.Name],
                    new Vector2(_graphics.GetWidthOffset(10.66f) + 48, _graphics.GetHeightOffset(2)),
                    new Rectangle(0, 0, 64, 64),
                    Color.White,
                    0f,
                    new Vector2(32, 32),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                if (unlockedMaps.Exists(a => a.Name == selectedMap))
                {
                    List<string> descriptionLines = new List<string>();
                    if (map.Description.Length < descriptionLength)
                    {
                        descriptionLines.Add(map.Description);
                    }
                    else
                    {
                        int startCharacter = 0;
                        do
                        {
                            int nextSpace = map.Description.LastIndexOf(' ', startCharacter + descriptionLength, descriptionLength);
                            descriptionLines.Add(map.Description.Substring(startCharacter, nextSpace - startCharacter));
                            startCharacter = nextSpace + 1;
                        } while (map.Description.Substring(startCharacter).Length > descriptionLength);
                        descriptionLines.Add(map.Description.Substring(startCharacter));
                    }

                    int counter = -64;
                    foreach (var descriptionLine in descriptionLines)
                    {
                        _spriteBatch.DrawString(
                            fonts["FontSmall"],
                            descriptionLine,
                            new Vector2(_graphics.GetWidthOffset(10.66f) + 125, _graphics.GetHeightOffset(2) + counter),
                            Color.White
                        );
                        counter += 12;
                    }
                    counter += 12;

                    _spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat("Best Time: ", (progressionContainer.LevelProgressions.Where(a => a.Name == map.Name).FirstOrDefault()?.BestTime ?? 0).ToFormattedTime()),
                        new Vector2(_graphics.GetWidthOffset(10.66f) + 125, _graphics.GetHeightOffset(2) + counter),
                        Color.White
                    );
                }
                else
                {
                    _spriteBatch.DrawString(
                        fonts["FontSmall"],
                        "Locked",
                        new Vector2(_graphics.GetWidthOffset(10.66f) + 125, _graphics.GetHeightOffset(2) - 64),
                        Color.White
                    );
                }

                for (int i = 1; i <= mapSelectionButtons.Count; i++)
                {
                    mapSelectionButtons[i - 1].Draw(_spriteBatch);
                }
            }

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

        private void setSelectedPlayer()
        {
            selectedPlayer = playerContainers.ToList()[selectedButton - 1].Key;
        }

        private void setUnlockedMaps()
        {
            unlockedMaps = new List<MapContainer>();

            foreach (MapContainer map in mapContainers)
            {
                bool canAdd = false;
                switch (map.UnlockRequirement.MapUnlockType)
                {
                    case MapUnlockType.None:
                        canAdd = true;
                        break;
                    case MapUnlockType.MapBestTime:
                        var levelProgression = progressionContainer.LevelProgressions.Where(a => a.Name == map.UnlockRequirement.RequirementText).FirstOrDefault();
                        canAdd = levelProgression != null && levelProgression.BestTime >= map.UnlockRequirement.RequirementAmount;
                        break;
                }

                if (canAdd)
                {
                    unlockedMaps.Add(map);
                }
            }
        }
    }
}
