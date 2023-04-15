using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Scenes.SceneComponents;
using System.Collections.Generic;
using System.Linq;

namespace RogueliteSurvivor.Scenes.Windows
{
    public class MainMenuWindow : Window
    {
        int selectedButton = 0;
        List<ISelectableComponent> buttons;
        SoundEffect hover;
        SoundEffect confirm;

        public static MainMenuWindow MainMenuWindowFactory(
            GraphicsDeviceManager graphics,
            Dictionary<string, Texture2D> textures,
            Vector2 position,
            SoundEffect hover,
            SoundEffect confirm,
            Dictionary<string, SpriteFont> fonts
            )
        {
            var components = new Dictionary<string, IFormComponent>()
            {
                { "lblTitle", new Label("lblTitle", fonts["Font"], "Roguelite Survivor", new Vector2(graphics.GetWidthOffset(2) - 62, graphics.GetHeightOffset(2) - 144), Color.White) },
                { "btnNewGame", new Button("btnNewGame", textures["MainMenuButtons"],new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) - 96),new Rectangle(0, 32, 128, 32),new Rectangle(128, 32, 128, 32),new Vector2(64, 16))},
                { "btnPlayerUpgrades", new Button("btnPlayerUpgrades",textures["MainMenuButtons"],new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) - 48),new Rectangle(0, 480, 128, 32),new Rectangle(128, 480, 128, 32),new Vector2(64, 16))},
                { "btnPlayStats", new Button("btnPlayStats",textures["MainMenuButtons"],new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2)),new Rectangle(0, 64, 128, 32),new Rectangle(128, 64, 128, 32),new Vector2(64, 16)) },
                { "btnOptions", new Button("btnOptions", textures["MainMenuButtons"], new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) + 48), new Rectangle(0, 160, 128, 32), new Rectangle(128, 160, 128, 32), new Vector2(64, 16)) },
                { "btnCredits", new Button("btnCredits", textures["MainMenuButtons"], new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) + 96), new Rectangle(0, 128, 128, 32), new Rectangle(128, 128, 128, 32), new Vector2(64, 16)) },
                { "btnExit", new Button("btnExit", textures["MainMenuButtons"], new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) + 144), new Rectangle(0, 96, 128, 32), new Rectangle(128, 96, 128, 32), new Vector2(64, 16)) },
            };

            return new MainMenuWindow(graphics, null, position, components, hover, confirm);
        }

        private MainMenuWindow(
            GraphicsDeviceManager graphics,
            Texture2D background,
            Vector2 position,
            Dictionary<string, IFormComponent> components,
            SoundEffect hover,
            SoundEffect confirm)
            : base(graphics, background, position, components)
        {
            buttons = new List<ISelectableComponent>();
            foreach (var component in components)
            {
                if (component.Value is ISelectableComponent)
                {
                    buttons.Add((ISelectableComponent)component.Value);
                }
            }

            this.hover = hover;
            this.confirm = confirm;
        }

        public override void SetActive()
        {
            selectedButton = 0;
            base.SetActive();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
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

                if (clicked || kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                {
                    confirm.Play();

                    switch (selectedButton)
                    {
                        case 0:
                            return MainMenuState.CharacterSelection.ToString();
                        case 1:
                            return MainMenuState.PlayerUpgrades.ToString();
                        case 2:
                            return MainMenuState.PlayStats.ToString();
                        case 3:
                            return MainMenuState.Options.ToString();
                        case 4:
                            return MainMenuState.Credits.ToString();
                        case 5:
                            return "exit";
                    }
                }
                else if (kState.IsKeyDown(Keys.Up) || gState.DPad.Up == ButtonState.Pressed || gState.ThumbSticks.Left.Y > 0.5f)
                {
                    if (selectedButton > 0)
                    {
                        selectedButton--;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                {
                    if (selectedButton < (buttons.Count - 1))
                    {
                        selectedButton++;
                        hover.Play();
                        resetReadyForInput();
                    }
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
