using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Scenes.SceneComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Scenes.Windows
{
    public class InGameOptionsMenuWindow : Window
    {
        int selectedButton = 0;
        List<ISelectableComponent> buttons;
        bool readyForInput = false;
        float counter = 0f;
        SoundEffect hover;
        SoundEffect confirm;
        Dictionary<string, SpriteFont> fonts;
        SettingsContainer settingsContainer;

        public InGameOptionsMenuWindow
        (
            Texture2D background,
            Vector2 position,
            List<IFormComponent> components,
            SoundEffect hover,
            SoundEffect confirm,
            Dictionary<string, SpriteFont> fonts,
            SettingsContainer settingsContainer
        ) 
            : base(background, position, components)
        {
            buttons = new List<ISelectableComponent>();
            foreach (var component in components)
            {
                if (component is ISelectableComponent)
                {
                    buttons.Add((ISelectableComponent)component);
                }
            }

            this.hover = hover;
            this.confirm = confirm;
            this.fonts = fonts;
            this.settingsContainer = settingsContainer;
        }

        public override void SetActive()
        {
            selectedButton = 0;
            counter = 0f;
            readyForInput = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.DrawString(
            fonts["Font"],
                    string.Concat("Master Volume: ", settingsContainer.MasterVolume.ToString("P0")),
                    new Vector2(position.X - 125, position.Y - 72),
                    Color.White
                );

            spriteBatch.DrawString(
            fonts["Font"],
                string.Concat("Menu Music Volume: ", settingsContainer.MenuMusicVolume.ToString("P0")),
                new Vector2(position.X - 125, position.Y - 40),
                Color.White
            );

            spriteBatch.DrawString(
            fonts["Font"],
                string.Concat("Game Music Volume: ", settingsContainer.GameMusicVolume.ToString("P0")),
                new Vector2(position.X - 125, position.Y - 8),
                Color.White
            );

            spriteBatch.DrawString(
            fonts["Font"],
                string.Concat("Sound Effect Volume: ", settingsContainer.SoundEffectsVolume.ToString("P0")),
                new Vector2(position.X - 125, position.Y + 24),
                Color.White
            );
        }

        public override string Update(GameTime gameTime, params object[] values)
        {
            var kState = Keyboard.GetState();
            var gState = GamePad.GetState(PlayerIndex.One);
            var mState = Mouse.GetState();

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
                        readyForInput = false;
                    }
                }
                else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                {
                    if (selectedButton < 7)
                    {
                        selectedButton += 2;
                        hover.Play();
                        readyForInput = false;
                    }
                    else if (selectedButton == 7)
                    {
                        selectedButton = 8;
                        hover.Play();
                        readyForInput = false;
                    }
                }
                else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                {
                    if (selectedButton < 8 && selectedButton % 2 == 1)
                    {
                        selectedButton--;
                        hover.Play();
                        readyForInput = false;
                    }
                }
                else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                {
                    if (selectedButton < 8 && selectedButton % 2 == 0)
                    {
                        selectedButton++;
                        hover.Play();
                        readyForInput = false;
                    }
                }
                else if (clicked || kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                {
                    switch (selectedButton)
                    {
                        case 0:
                            settingsContainer.MasterVolume = MathF.Max(0f, settingsContainer.MasterVolume - 0.05f);
                            MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.GameMusicVolume;
                            SoundEffect.MasterVolume = settingsContainer.MasterVolume * settingsContainer.SoundEffectsVolume;
                            break;
                        case 1:
                            settingsContainer.MasterVolume = MathF.Min(1f, settingsContainer.MasterVolume + 0.05f);
                            MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.GameMusicVolume;
                            SoundEffect.MasterVolume = settingsContainer.MasterVolume * settingsContainer.SoundEffectsVolume;
                            break;
                        case 2:
                            settingsContainer.MenuMusicVolume = MathF.Max(0f, settingsContainer.MenuMusicVolume - 0.05f);
                            break;
                        case 3:
                            settingsContainer.MenuMusicVolume = MathF.Min(1f, settingsContainer.MenuMusicVolume + 0.05f);
                            break;
                        case 4:
                            settingsContainer.GameMusicVolume = MathF.Max(0f, settingsContainer.GameMusicVolume - 0.05f);
                            MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.GameMusicVolume;
                            break;
                        case 5:
                            settingsContainer.GameMusicVolume = MathF.Min(1f, settingsContainer.GameMusicVolume + 0.05f);
                            MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.GameMusicVolume;
                            break;
                        case 6:
                            settingsContainer.SoundEffectsVolume = MathF.Max(0f, settingsContainer.SoundEffectsVolume - 0.05f);
                            SoundEffect.MasterVolume = settingsContainer.MasterVolume * settingsContainer.SoundEffectsVolume;
                            break;
                        case 7:
                            settingsContainer.SoundEffectsVolume = MathF.Min(1f, settingsContainer.SoundEffectsVolume + 0.05f);
                            SoundEffect.MasterVolume = settingsContainer.MasterVolume * settingsContainer.SoundEffectsVolume;
                            break;
                        case 8:
                            settingsContainer.Save();
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

                    readyForInput = false;
                }
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Selected(i == selectedButton);
                buttons[i].MouseOver(mState);
            }

            return string.Empty;
        }
    }
}
