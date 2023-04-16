using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Scenes.SceneComponents;
using System.Collections.Generic;
using System.Linq;

namespace RogueliteSurvivor.Scenes.Windows
{
    public class InGameMenuWindow : Window
    {
        SoundEffect hover;
        SoundEffect confirm;

        public static InGameMenuWindow InGameMenuWindowFactory(GraphicsDeviceManager graphics,
            Dictionary<string, Texture2D> textures,
            Vector2 position,
            SoundEffect hover,
            SoundEffect confirm)
        {
            var components = new Dictionary<string, IFormComponent>()
            {
                {"btnContinue",
                new Button(
                    "btnContinue",
                    textures["MainMenuButtons"],
                    new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) - 72),
                    new Rectangle(0, 448, 128, 32),
                    new Rectangle(128, 448, 128, 32),
                    new Vector2(64, 16)
                ) },
                { "btnOptions",
                new Button(
                    "btnOptions",
                    textures["MainMenuButtons"],
                    new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) - 24),
                    new Rectangle(0, 160, 128, 32),
                    new Rectangle(128, 160, 128, 32),
                    new Vector2(64, 16)
                ) },
                {"btnRestart",
                    new Button(
                    "btnRestart",
                    textures["MainMenuButtons"],
                    new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) + 24),
                    new Rectangle(0, 384, 128, 32),
                    new Rectangle(128, 384, 128, 32),
                    new Vector2(64, 16)
                ) },
                { "btnEndRun", new Button("btnEndRun", textures["MainMenuButtons"], new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) + 72), new Rectangle(0, 512, 128, 32), new Rectangle(128, 512, 128, 32), new Vector2(64, 16)) },
            };
            return new InGameMenuWindow(graphics, textures["InGameMenuWindow"], position, components, hover, confirm);
        }

        private InGameMenuWindow
        (
            GraphicsDeviceManager graphics,
            Texture2D background,
            Vector2 position,
            Dictionary<string, IFormComponent> components,
            SoundEffect hover,
            SoundEffect confirm
        )
            : base(graphics, background, position, components)
        {
            this.hover = hover;
            this.confirm = confirm;
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
                            return "continue";
                        case 1:
                            return "options";
                        case 2:
                            return "restart";
                        case 3:
                            return "game-over";
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
