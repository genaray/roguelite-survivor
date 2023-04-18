using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Scenes.SceneComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueliteSurvivor.Scenes.Windows
{
    public class OptionsMenuWindow : Window
    {
        SoundEffect hover;
        SoundEffect confirm;
        SettingsContainer settingsContainer;

        public static OptionsMenuWindow OptionsMenuWindowFactory(
            GraphicsDeviceManager graphics,
            Dictionary<string, Texture2D> textures,
            Vector2 position,
            SoundEffect hover,
            SoundEffect confirm,
            Dictionary<string, SpriteFont> fonts,
            SettingsContainer settingsContainer)
        {
            var components = new Dictionary<string, IFormComponent>()
            {
                { "lblTitle", new Label("lblTitle", fonts["Font"], "Options", new Vector2(graphics.GetWidthOffset(2) - fonts["Font"].MeasureString("Options").X / 2, graphics.GetHeightOffset(2) - 144), Color.White) }
            };

            for (int i = 0; i < 4; i++)
            {
                string componentBaseName = string.Empty;
                string labelText = string.Empty;
                float volume = 0f;

                switch (i)
                {
                    case 0:
                        componentBaseName = "MasterVolume";
                        labelText = "Master Volume";
                        volume = settingsContainer.MasterVolume;
                        break;
                    case 1:
                        componentBaseName = "MenuMusicVolume";
                        labelText = "Menu Music Volume";
                        volume = settingsContainer.MenuMusicVolume;
                        break;
                    case 2:
                        componentBaseName = "GameMusicVolume";
                        labelText = "Game Music Volume";
                        volume = settingsContainer.GameMusicVolume;
                        break;
                    case 3:
                        componentBaseName = "SoundEffectsVolume";
                        labelText = "Sound Effects Volume";
                        volume = settingsContainer.SoundEffectsVolume;
                        break;
                }

                components.Add(
                    string.Concat("lbl", componentBaseName),
                    new Label(
                        string.Concat("lbl", componentBaseName),
                        fonts["Font"],
                        string.Concat(labelText, ": ", volume.ToString("P0")),
                        new Vector2(position.X - 125, position.Y - 72 + (i * 32)),
                        Color.White
                    )
                );

                components.Add(
                    string.Concat("btn", componentBaseName, "Down"),
                    new Button(
                        string.Concat(componentBaseName, "Down"),
                        textures["VolumeButtons"],
                        new Vector2(graphics.GetWidthOffset(2) + 85, graphics.GetHeightOffset(2) - 64 + (i * 32)),
                        new Rectangle(0, 0, 16, 16),
                        new Rectangle(16, 0, 16, 16),
                        new Vector2(8, 8)
                    )
                );
                components.Add(
                    string.Concat("btn", componentBaseName, "Up"),
                    new Button(
                        string.Concat(componentBaseName, "Up"),
                        textures["VolumeButtons"],
                        new Vector2(graphics.GetWidthOffset(2) + 109, graphics.GetHeightOffset(2) - 64 + (i * 32)),
                        new Rectangle(0, 16, 16, 16),
                        new Rectangle(16, 16, 16, 16),
                        new Vector2(8, 8)
                    )
                );
            }

            components.Add(
                "btnBack",
                new Button(
                    "btnBack",
                    textures["MainMenuButtons"],
                    new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) + 144),
                    new Rectangle(0, 192, 128, 32),
                    new Rectangle(128, 192, 128, 32),
                    new Vector2(64, 16)
                )
            );

            return new OptionsMenuWindow(graphics, null, position, components, hover, confirm, fonts, settingsContainer);
        }

        private OptionsMenuWindow
        (
            GraphicsDeviceManager graphics,
            Texture2D background,
            Vector2 position,
            Dictionary<string, IFormComponent> components,
            SoundEffect hover,
            SoundEffect confirm,
            Dictionary<string, SpriteFont> fonts,
            SettingsContainer settingsContainer
        )
            : base(graphics, background, position, components)
        {
            this.hover = hover;
            this.confirm = confirm;
            this.settingsContainer = settingsContainer;
        }

        public override string Update(GameTime gameTime, params object[] values)
        {
            var kState = Keyboard.GetState();
            var gState = GamePad.GetState(PlayerIndex.One);
            var mState = Mouse.GetState();

            if (isReadyForInput(gameTime))
            {
                bool clicked = false;

                if (mState.LeftButton == ButtonState.Pressed && buttons.Any(a => a.MouseOver()))
                {
                    clicked = true;
                    selectedButton = buttons.IndexOf(buttons.First(a => a.MouseOver()));
                }

                if (kState.IsKeyDown(Keys.Up) || gState.DPad.Up == ButtonState.Pressed || gState.ThumbSticks.Left.Y > 0.5f)
                {
                    if (selectedButton - 2 >= 0)
                    {
                        selectedButton -= 2;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                {
                    if (selectedButton < 7)
                    {
                        selectedButton += 2;
                        hover.Play();
                        resetReadyForInput();
                    }
                    else if (selectedButton == 7)
                    {
                        selectedButton = 8;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                {
                    if (selectedButton < 8 && selectedButton % 2 == 1)
                    {
                        selectedButton--;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                {
                    if (selectedButton < 8 && selectedButton % 2 == 0)
                    {
                        selectedButton++;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (clicked || kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                {
                    switch (selectedButton)
                    {
                        case 0:
                            settingsContainer.MasterVolume = MathF.Max(0f, settingsContainer.MasterVolume - 0.05f);
                            MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.MenuMusicVolume;
                            SoundEffect.MasterVolume = settingsContainer.MasterVolume * settingsContainer.SoundEffectsVolume;
                            ((Label)Components["lblMasterVolume"]).Text = string.Concat("Master Volume: ", settingsContainer.MasterVolume.ToString("P0"));
                            break;
                        case 1:
                            settingsContainer.MasterVolume = MathF.Min(1f, settingsContainer.MasterVolume + 0.05f);
                            MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.MenuMusicVolume;
                            SoundEffect.MasterVolume = settingsContainer.MasterVolume * settingsContainer.SoundEffectsVolume;
                            ((Label)Components["lblMasterVolume"]).Text = string.Concat("Master Volume: ", settingsContainer.MasterVolume.ToString("P0"));
                            break;
                        case 2:
                            settingsContainer.MenuMusicVolume = MathF.Max(0f, settingsContainer.MenuMusicVolume - 0.05f);
                            MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.MenuMusicVolume;
                            ((Label)Components["lblMenuMusicVolume"]).Text = string.Concat("Menu Music Volume: ", settingsContainer.MenuMusicVolume.ToString("P0"));
                            break;
                        case 3:
                            settingsContainer.MenuMusicVolume = MathF.Min(1f, settingsContainer.MenuMusicVolume + 0.05f);
                            MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.MenuMusicVolume;
                            ((Label)Components["lblMenuMusicVolume"]).Text = string.Concat("Menu Music Volume: ", settingsContainer.MenuMusicVolume.ToString("P0"));
                            break;
                        case 4:
                            settingsContainer.GameMusicVolume = MathF.Max(0f, settingsContainer.GameMusicVolume - 0.05f);
                            ((Label)Components["lblGameMusicVolume"]).Text = string.Concat("Game Music Volume: ", settingsContainer.GameMusicVolume.ToString("P0"));
                            break;
                        case 5:
                            settingsContainer.GameMusicVolume = MathF.Min(1f, settingsContainer.GameMusicVolume + 0.05f);
                            ((Label)Components["lblGameMusicVolume"]).Text = string.Concat("Game Music Volume: ", settingsContainer.GameMusicVolume.ToString("P0"));
                            break;
                        case 6:
                            settingsContainer.SoundEffectsVolume = MathF.Max(0f, settingsContainer.SoundEffectsVolume - 0.05f);
                            SoundEffect.MasterVolume = settingsContainer.MasterVolume * settingsContainer.SoundEffectsVolume;
                            ((Label)Components["lblSoundEffectsVolume"]).Text = string.Concat("Sound Effects Volume: ", settingsContainer.SoundEffectsVolume.ToString("P0"));
                            break;
                        case 7:
                            settingsContainer.SoundEffectsVolume = MathF.Min(1f, settingsContainer.SoundEffectsVolume + 0.05f);
                            SoundEffect.MasterVolume = settingsContainer.MasterVolume * settingsContainer.SoundEffectsVolume;
                            ((Label)Components["lblSoundEffectsVolume"]).Text = string.Concat("Sound Effects Volume: ", settingsContainer.SoundEffectsVolume.ToString("P0"));
                            break;
                        case 8:
                            settingsContainer.Save();
                            confirm.Play();
                            return "menu";
                    }

                    if (selectedButton > 1 && selectedButton < 6)
                    {
                        SoundEffectInstance instance = confirm.CreateInstance();

                        switch (selectedButton)
                        {
                            case 2:
                            case 3:
                                instance.Volume = settingsContainer.MasterVolume * settingsContainer.MenuMusicVolume;
                                break;
                            case 4:
                            case 5:
                                instance.Volume = settingsContainer.MasterVolume * settingsContainer.GameMusicVolume;
                                break;
                        }

                        instance.Play();
                    }
                    else
                    {
                        confirm.Play();
                    }

                    resetReadyForInput();
                }
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Selected = i == selectedButton;
                buttons[i].MouseOver(mState);
            }

            return string.Empty;
        }
    }
}
