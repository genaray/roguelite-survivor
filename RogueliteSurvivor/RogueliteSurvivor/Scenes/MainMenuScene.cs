﻿using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json.Linq;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Helpers;
using RogueliteSurvivor.Utils;
using System;
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

        private bool readyForInput = false;
        private float counter = 0f;

        private MainMenuState state;
        private string selectedPlayer;
        private int selectedButton = 1;
        private string selectedMap;

        private Dictionary<string, PlayerContainer> playerContainers;
        Dictionary<string, EnemyContainer> enemyContainers;
        private List<MapContainer> mapContainers;
        private List<MapContainer> unlockedMaps;
        private CreditsContainer creditsContainer = null;

        private int statsPage = 0;
        private int descriptionLength = 80;


        public MainMenuScene(SpriteBatch spriteBatch, ContentManager contentManager, GraphicsDeviceManager graphics, Dictionary<string, PlayerContainer> playerContainers, Dictionary<string, MapContainer> mapContainers, ProgressionContainer progressionContainer, Dictionary<string, EnemyContainer> enemyContainers, float scaleFactor, SettingsContainer settingsContainer)
            : base(spriteBatch, contentManager, graphics, progressionContainer, scaleFactor, settingsContainer)
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
                    { "PlayerSelectOutline", Content.Load<Texture2D>(Path.Combine("UI", "player-select-outline")) },
                    { "FireWizard", Content.Load<Texture2D>(Path.Combine("Player", "FireWizard")) },
                    { "IceWizard", Content.Load<Texture2D>(Path.Combine("Player", "IceWizard")) },
                    { "LightningWizard", Content.Load<Texture2D>(Path.Combine("Player", "LightningWizard")) },
                    { "FlyingWizard", Content.Load<Texture2D>(Path.Combine("Player", "FlyingWizard")) },
                };

                foreach(var map in mapContainers)
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

            if(songs == null)
            {
                songs = new Dictionary<string, Song> {
                    { "MenuTheme", Content.Load<Song>(Path.Combine("Music", "Night Ambient 2 (Loop)")) }
                };
            }

            if(creditsContainer == null)
            {
                JObject credits = JObject.Parse(File.ReadAllText(Path.Combine(Content.RootDirectory, "Datasets", "credits.json")));
                creditsContainer = CreditsContainer.ToCreditsContainer(credits["data"]);
            }

            state = MainMenuState.MainMenu;
            selectedPlayer = playerContainers.First().Key;
            selectedMap = mapContainers.First().Name;
            
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

                if (state == MainMenuState.MainMenu)
                {
                    if (kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                    {
                        switch (selectedButton)
                        {
                            case 1:
                                state = MainMenuState.CharacterSelection;
                                break;
                            case 2:
                                state = MainMenuState.PlayStats;
                                break;
                            case 3:
                                state = MainMenuState.Options;
                                break;
                            case 4:
                                state = MainMenuState.Credits;
                                break;
                            case 5:
                                retVal = "exit";
                                break;
                        }
                        selectedButton = 1;
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Up) || gState.DPad.Up == ButtonState.Pressed || gState.ThumbSticks.Left.Y > 0.5f)
                    {
                        selectedButton = int.Max(selectedButton - 1, 1);
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                    {
                        selectedButton = int.Min(selectedButton + 1, 5);
                        readyForInput = false;
                    }
                }
                else if (state == MainMenuState.CharacterSelection)
                {
                    if (kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                    {
                        state = MainMenuState.MapSelection;
                        setUnlockedMaps();
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                    {
                        selectedButton = int.Max(selectedButton - 1, 1);
                        setSelectedPlayer();
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                    {
                        selectedButton = int.Min(selectedButton + 1, playerContainers.Count);
                        setSelectedPlayer();
                        readyForInput = false;
                    }
                    else if (gState.Buttons.B == ButtonState.Pressed || kState.IsKeyDown(Keys.Escape))
                    {
                        state = MainMenuState.MainMenu;
                        selectedButton = 1;
                        readyForInput = false;
                    }
                }
                else if(state == MainMenuState.MapSelection)
                {
                    if ((kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                        && unlockedMaps.Exists(a => a.Name == selectedMap))
                    {
                        state = MainMenuState.MainMenu;
                        retVal = "loading";
                        readyForInput = false;
                        selectedButton = 1;
                    }
                    else if (gState.Buttons.B == ButtonState.Pressed || kState.IsKeyDown(Keys.Escape))
                    {
                        state = MainMenuState.CharacterSelection;
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                    {
                        if (selectedMap != mapContainers[0].Name)
                        {
                            int index = mapContainers.IndexOf(mapContainers.Where(a => a.Name == selectedMap).First()) - 1;
                            selectedMap = mapContainers[index].Name;
                        }
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                    {
                        if (selectedMap != mapContainers.Last().Name)
                        {
                            int index = mapContainers.IndexOf(mapContainers.Where(a => a.Name == selectedMap).First()) + 1;
                            selectedMap = mapContainers[index].Name;
                        }
                        readyForInput = false;
                    }
                    
                }
                else if (state == MainMenuState.Credits)
                {
                    if (gState.Buttons.A == ButtonState.Pressed || gState.Buttons.B == ButtonState.Pressed 
                        || kState.IsKeyDown(Keys.Escape) || kState.IsKeyDown(Keys.Enter))
                    {
                        state = MainMenuState.MainMenu;
                        readyForInput = false;
                    }
                }
                else if(state == MainMenuState.PlayStats)
                {
                    if (kState.IsKeyDown(Keys.Escape) || gState.Buttons.B == ButtonState.Pressed)
                    {
                        state = MainMenuState.MainMenu;
                        readyForInput = false;
                        selectedButton = 1;
                        statsPage = 0;
                    }
                    else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                    {
                        statsPage = (statsPage - 1) % 2;
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                    {
                        statsPage = (statsPage + 1) % 2;
                        readyForInput = false;
                    }
                }
                else if (state == MainMenuState.Options)
                {
                    if (gState.Buttons.B == ButtonState.Pressed || kState.IsKeyDown(Keys.Escape))
                    {
                        state = MainMenuState.MainMenu;
                        settingsContainer.Save();
                        selectedButton = 1;
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Up) || gState.DPad.Up == ButtonState.Pressed || gState.ThumbSticks.Left.Y > 0.5f)
                    {
                        if(selectedButton - 2 > 0)
                        {
                            selectedButton -= 2;
                        }
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                    {
                        if (selectedButton + 2 < 9)
                        {
                            selectedButton += 2;
                        }
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                    {
                        if(selectedButton % 2 == 0)
                        {
                            selectedButton--;
                        }
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                    {
                        if (selectedButton % 2 == 1)
                        {
                            selectedButton++;
                        }
                        readyForInput = false;
                    }
                    else if (kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                    {
                        switch (selectedButton)
                        {
                            case 1:
                                settingsContainer.MasterVolume = MathF.Max(0f, settingsContainer.MasterVolume - 0.05f);
                                MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.MenuMusicVolume;
                                break;
                            case 2:
                                settingsContainer.MasterVolume = MathF.Min(1f, settingsContainer.MasterVolume + 0.05f);
                                MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.MenuMusicVolume;
                                break;
                            case 3:
                                settingsContainer.MenuMusicVolume = MathF.Max(0f, settingsContainer.MenuMusicVolume - 0.05f);
                                MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.MenuMusicVolume;
                                break;
                            case 4:
                                settingsContainer.MenuMusicVolume = MathF.Min(1f, settingsContainer.MenuMusicVolume + 0.05f);
                                MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.MenuMusicVolume;
                                break;
                            case 5:
                                settingsContainer.GameMusicVolume = MathF.Max(0f, settingsContainer.GameMusicVolume - 0.05f);
                                break;
                            case 6:
                                settingsContainer.GameMusicVolume = MathF.Min(1f, settingsContainer.GameMusicVolume + 0.05f);
                                break;
                            case 7:
                                settingsContainer.SoundEffectsVolume = MathF.Max(0f, settingsContainer.SoundEffectsVolume - 0.05f);
                                SoundEffect.MasterVolume = settingsContainer.MasterVolume * settingsContainer.SoundEffectsVolume;
                                break;
                            case 8:
                                settingsContainer.SoundEffectsVolume = MathF.Min(1f, settingsContainer.SoundEffectsVolume + 0.05f);
                                SoundEffect.MasterVolume = settingsContainer.MasterVolume * settingsContainer.SoundEffectsVolume;
                                break;
                        }
                        readyForInput = false;
                    }
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
                new Vector2(GetWidthOffset(2) - 62, GetHeightOffset(2) - 128),
                Color.White
            );

            if (state == MainMenuState.MainMenu)
            {
                _spriteBatch.Draw(
                    textures["MainMenuButtons"],
                    new Vector2(GetWidthOffset(2), GetHeightOffset(2) - 48),
                    new Rectangle(0 + selectedButton == 1 ? 128 : 0, 32, 128, 32),
                    Color.White,
                    0f,
                    new Vector2(64, 16),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["MainMenuButtons"],
                    new Vector2(GetWidthOffset(2), GetHeightOffset(2)),
                    new Rectangle(0 + selectedButton == 2 ? 128 : 0, 64, 128, 32),
                    Color.White,
                    0f,
                    new Vector2(64, 16),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["MainMenuButtons"],
                    new Vector2(GetWidthOffset(2), GetHeightOffset(2) + 48),
                    new Rectangle(0 + selectedButton == 3 ? 128 : 0, 160, 128, 32),
                    Color.White,
                    0f,
                    new Vector2(64, 16),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["MainMenuButtons"],
                    new Vector2(GetWidthOffset(2), GetHeightOffset(2) + 96),
                    new Rectangle(0 + selectedButton == 4 ? 128 : 0, 128, 128, 32),
                    Color.White,
                    0f,
                    new Vector2(64, 16),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["MainMenuButtons"],
                    new Vector2(GetWidthOffset(2), GetHeightOffset(2) + 144),
                    new Rectangle(0 + selectedButton == 5 ? 128 : 0, 96, 128, 32),
                    Color.White,
                    0f,
                    new Vector2(64, 16),
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }
            else if (state == MainMenuState.CharacterSelection)
            {
                _spriteBatch.DrawString(
                    fonts["Font"],
                    "Choose your wizard:",
                    new Vector2(GetWidthOffset(2) - 70, GetHeightOffset(2) - 32),
                    Color.White
                );

                _spriteBatch.Draw(
                    textures["PlayerSelectOutline"],
                    new Vector2(GetWidthOffset(10.66f) + ((selectedButton - 1) * 24), GetHeightOffset(2)),
                    new Rectangle(0, 0, 20, 20),
                    Color.White,
                    0f,
                    new Vector2(10, 10),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["FireWizard"],
                    new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2)),
                    new Rectangle(16, 0, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["IceWizard"],
                    new Vector2(GetWidthOffset(10.66f) + 24, GetHeightOffset(2)),
                    new Rectangle(16, 0, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["LightningWizard"],
                    new Vector2(GetWidthOffset(10.66f) + 48, GetHeightOffset(2)),
                    new Rectangle(16, 0, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["FlyingWizard"],
                    new Vector2(GetWidthOffset(10.66f) + 72, GetHeightOffset(2)),
                    new Rectangle(16, 0, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                int counter = 0;
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
                            new Vector2(GetWidthOffset(10.66f) + 125, GetHeightOffset(2) + counter),
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
                    new Vector2(GetWidthOffset(10.66f) + 125, GetHeightOffset(2) + counter),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Spell: ", playerContainer.StartingSpell.GetReadableSpellName()),
                    new Vector2(GetWidthOffset(10.66f) + 125, GetHeightOffset(2) + counter + 12),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Starting Health: ", (int)(playerContainer.Health *  100)),
                    new Vector2(GetWidthOffset(10.66f) + 125, GetHeightOffset(2) + counter + 24),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Move Speed: ", (int)(playerContainer.Speed * 100)),
                    new Vector2(GetWidthOffset(10.66f) + 125, GetHeightOffset(2) + counter + 36),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Spell Damage: ", (playerContainer.SpellDamage + 1f).ToString("F"), "x"),
                    new Vector2(GetWidthOffset(10.66f) + 125, GetHeightOffset(2) + counter + 48),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Spell Effect Chance: ", (playerContainer.SpellEffectChance + 1f).ToString("F"), "x"),
                    new Vector2(GetWidthOffset(10.66f) + 240, GetHeightOffset(2) + counter + 12),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Attack Speed: ", (playerContainer.AttackSpeed + 1f).ToString("F"), "x"),
                    new Vector2(GetWidthOffset(10.66f) + 240, GetHeightOffset(2) + counter + 24),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Pierce: ", playerContainer.Pierce),
                    new Vector2(GetWidthOffset(10.66f) + 240, GetHeightOffset(2) + counter + 36),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Area of Effect: ", (playerContainer.AreaOfEffect + 1f).ToString("F"), "x"),
                    new Vector2(GetWidthOffset(10.66f) + 240, GetHeightOffset(2) + counter + 48),
                    Color.White
                );

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    "Special Traits: ",
                    new Vector2(GetWidthOffset(10.66f) + 385, GetHeightOffset(2) + counter),
                    Color.White
                );

                if (playerContainer.Traits.Any())
                {
                    int offsetY = 12;
                    foreach(var trait in playerContainer.Traits)
                    {
                        _spriteBatch.DrawString(
                            fonts["FontSmall"],
                            trait.ReadableTraitName(),
                            new Vector2(GetWidthOffset(10.66f) + 385, GetHeightOffset(2) + counter + offsetY),
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
                        new Vector2(GetWidthOffset(10.66f) + 385, GetHeightOffset(2) + counter + 12),
                        Color.White
                    );
                }


                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    "Press Esc on the keyboard or B on the controller to return to the main menu",
                    new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) + 128),
                    Color.White
                );
            }
            else if (state == MainMenuState.MapSelection)
            {
                _spriteBatch.DrawString(
                    fonts["Font"],
                    "Select a map:",
                    new Vector2(GetWidthOffset(2) - 50, GetHeightOffset(2) - 32),
                    Color.White
                );

                var map = mapContainers.Where(a => a.Name == selectedMap).First();

                _spriteBatch.DrawString(
                    fonts["Font"],
                    map.Name,
                    new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2)),
                    Color.White
                );

                _spriteBatch.Draw(
                    textures[map.Name],
                    new Vector2(GetWidthOffset(10.66f) + 48, GetHeightOffset(2) + 64),
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

                    int counter = 0;
                    foreach (var descriptionLine in descriptionLines)
                    {
                        _spriteBatch.DrawString(
                            fonts["FontSmall"],
                            descriptionLine,
                            new Vector2(GetWidthOffset(10.66f) + 125, GetHeightOffset(2) + counter),
                            Color.White
                        );
                        counter += 12;
                    }
                    counter += 12;

                    _spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat("Best Time: ", float.Round(progressionContainer.LevelProgressions.Where(a => a.Name == map.Name).FirstOrDefault()?.BestTime ?? 0, 2)),
                        new Vector2(GetWidthOffset(10.66f) + 125, GetHeightOffset(2) + counter),
                        Color.White
                    );
                }
                else
                {
                    _spriteBatch.DrawString(
                        fonts["FontSmall"],
                        "Locked",
                        new Vector2(GetWidthOffset(10.66f) + 125, GetHeightOffset(2)),
                        Color.White
                    );
                }
                


                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    "Press left and right to cycle through the maps",
                    new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) + 116),
                    Color.White
                );
                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    "Press Esc on the keyboard or B on the controller to return to player selection",
                    new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) + 128),
                    Color.White
                );
            }
            else if (state == MainMenuState.Credits)
            {
                int counterX = 0, counterY = -96;
                foreach(var outsideResource in creditsContainer.OutsideResources)
                {
                    _spriteBatch.DrawString(
                        fonts["Font"],
                        outsideResource.Author,
                        new Vector2(GetWidthOffset(2) - 300 + counterX, GetHeightOffset(2) + counterY),
                        Color.White
                    );
                    counterY += 18;

                    foreach(var package in outsideResource.Packages)
                    {
                        if(package.Length > 40)
                        {
                            string part1, part2;
                            part1 = package.Substring(0, package.IndexOf(' ', 30));
                            part2 = package.Substring(package.IndexOf(' ', 30));
                            _spriteBatch.DrawString(
                                fonts["FontSmall"],
                                part1,
                                new Vector2(GetWidthOffset(2) - 288 + counterX, GetHeightOffset(2) + counterY),
                                Color.White
                            );
                            counterY += 12;
                            _spriteBatch.DrawString(
                                fonts["FontSmall"],
                                part2,
                                new Vector2(GetWidthOffset(2) - 276 + counterX, GetHeightOffset(2) + counterY),
                                Color.White
                            );
                        }
                        else
                        {
                            _spriteBatch.DrawString(
                                fonts["FontSmall"],
                                package,
                                new Vector2(GetWidthOffset(2) - 288 + counterX, GetHeightOffset(2) + counterY),
                                Color.White
                            );
                        }
                        
                        counterY += 12;
                    }

                    counterY += 18;

                    if(counterY > 100)
                    {
                        counterY = -96;
                        counterX += 200;
                    }
                }
            }
            else if (state == MainMenuState.PlayStats)
            {
                int counterX = 0, counterY = 0;
                switch (statsPage)
                {
                    case 0:
                        _spriteBatch.DrawString(
                            fonts["Font"],
                            "Play Stats - Maps",
                            new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) - 96),
                            Color.White
                        );

                        foreach(var map in progressionContainer.LevelProgressions)
                        {
                            _spriteBatch.DrawString(
                                fonts["FontSmall"],
                                map.Name,
                                new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) - 72 + counterY),
                                Color.White
                            );
                            _spriteBatch.DrawString(
                                fonts["FontSmall"],
                                string.Concat("  Best Time: ", float.Round(map.BestTime, 2)),
                                new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) - 60 + counterY),
                                Color.White
                            );
                            
                            counterY += 36;
                        }
                        break;
                    case 1:
                        _spriteBatch.DrawString(
                            fonts["Font"],
                            "Play Stats - Enemies",
                            new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) - 96),
                            Color.White
                        );

                        foreach(var enemy in enemyContainers)
                        {
                            var enemyProgression = progressionContainer.EnemyKillStats.Where(a => a.Name == enemy.Value.ReadableName).FirstOrDefault();
                            _spriteBatch.DrawString(
                                fonts["FontSmall"],
                                enemy.Value.ReadableName,
                                new Vector2(GetWidthOffset(10.66f) + counterX, GetHeightOffset(2) - 72 + counterY),
                                Color.White
                            );
                            if (enemyProgression != null) {
                                _spriteBatch.DrawString(
                                    fonts["FontSmall"],
                                    string.Concat("  Kills: ", enemyProgression.Kills),
                                    new Vector2(GetWidthOffset(10.66f) + counterX, GetHeightOffset(2) - 60 + counterY),
                                    Color.White
                                );
                                _spriteBatch.DrawString(
                                    fonts["FontSmall"],
                                    string.Concat("  Killed By: ", enemyProgression.KilledBy),
                                    new Vector2(GetWidthOffset(10.66f) + counterX, GetHeightOffset(2) - 48+ counterY),
                                    Color.White
                                );
                                counterY += 48;
                            }
                            else
                            {
                                _spriteBatch.DrawString(
                                    fonts["FontSmall"],
                                    "  Not Yet Found",
                                    new Vector2(GetWidthOffset(10.66f) + counterX, GetHeightOffset(2) - 60 + counterY),
                                    Color.White
                                );

                                counterY += 36;
                            }

                            if(counterY > 160)
                            {
                                counterX += 112;
                                counterY = 0;
                            }
                        }
                        break;
                }

                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    "Press left and right to cycle through the stat pages",
                    new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) + 116),
                    Color.White
                );
                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    "Press Esc on the keyboard or B on the controller to return to the main menu",
                    new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) + 128),
                    Color.White
                );
            }
            else if(state == MainMenuState.Options)
            {
                _spriteBatch.DrawString(
                    fonts["Font"],
                    string.Concat("Master Volume: ", settingsContainer.MasterVolume.ToString("P0")),
                    new Vector2(GetWidthOffset(2) - 125, GetHeightOffset(2) - 96),
                    Color.White
                );

                _spriteBatch.Draw(
                    textures["VolumeButtons"],
                    new Vector2(GetWidthOffset(2) + 85, GetHeightOffset(2) - 88),
                    new Rectangle(0 + selectedButton == 1 ? 16 : 0, 0, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["VolumeButtons"],
                    new Vector2(GetWidthOffset(2) + 109, GetHeightOffset(2) - 88),
                    new Rectangle(0 + selectedButton == 2 ? 16 : 0, 16, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.DrawString(
                    fonts["Font"],
                    string.Concat("Menu Music Volume: ", settingsContainer.MenuMusicVolume.ToString("P0")),
                    new Vector2(GetWidthOffset(2) - 125, GetHeightOffset(2) - 64),
                    Color.White
                );

                _spriteBatch.Draw(
                    textures["VolumeButtons"],
                    new Vector2(GetWidthOffset(2) + 85, GetHeightOffset(2) - 56),
                    new Rectangle(0 + selectedButton == 3 ? 16 : 0, 0, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["VolumeButtons"],
                    new Vector2(GetWidthOffset(2) + 109, GetHeightOffset(2) - 56),
                    new Rectangle(0 + selectedButton == 4 ? 16 : 0, 16, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.DrawString(
                    fonts["Font"],
                    string.Concat("Game Music Volume: ", settingsContainer.GameMusicVolume.ToString("P0")),
                    new Vector2(GetWidthOffset(2) - 125, GetHeightOffset(2) - 32),
                    Color.White
                );

                _spriteBatch.Draw(
                    textures["VolumeButtons"],
                    new Vector2(GetWidthOffset(2) + 85, GetHeightOffset(2) - 24),
                    new Rectangle(0 + selectedButton == 5 ? 16 : 0, 0, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["VolumeButtons"],
                    new Vector2(GetWidthOffset(2) + 109, GetHeightOffset(2) - 24),
                    new Rectangle(0 + selectedButton == 6 ? 16 : 0, 16, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.DrawString(
                    fonts["Font"],
                    string.Concat("Sound Effect Volume: ", settingsContainer.SoundEffectsVolume.ToString("P0")),
                    new Vector2(GetWidthOffset(2) - 125, GetHeightOffset(2)),
                    Color.White
                );

                _spriteBatch.Draw(
                    textures["VolumeButtons"],
                    new Vector2(GetWidthOffset(2) + 85, GetHeightOffset(2) + 8),
                    new Rectangle(0 + selectedButton == 7 ? 16 : 0, 0, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(
                    textures["VolumeButtons"],
                    new Vector2(GetWidthOffset(2) + 109, GetHeightOffset(2) + 8),
                    new Rectangle(0 + selectedButton == 8 ? 16 : 0, 16, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8, 8),
                    1f,
                    SpriteEffects.None,
                    0f
                );



                _spriteBatch.DrawString(
                    fonts["FontSmall"],
                    "Press Esc on the keyboard or B on the controller to return to the Main Menu",
                    new Vector2(GetWidthOffset(10.66f), GetHeightOffset(2) + 128),
                    Color.White
                );
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

            foreach(MapContainer map in mapContainers ) 
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
