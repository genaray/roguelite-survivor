using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Scenes.SceneComponents;
using System.Collections.Generic;
using System.Linq;

namespace RogueliteSurvivor.Scenes.Windows
{
    public class CreditsWindow : Window
    {
        SoundEffect hover;
        SoundEffect confirm;

        public static CreditsWindow CreditsWindowFactory(
            GraphicsDeviceManager graphics,
            Dictionary<string, Texture2D> textures,
            Vector2 position,
            SoundEffect hover,
            SoundEffect confirm,
            Dictionary<string, SpriteFont> fonts,
            CreditsContainer creditsContainer)
        {
            var components = new Dictionary<string, IFormComponent>()
            {
                { "lblTitle", new Label("lblTitle", fonts["Font"], "Credits", new Vector2(graphics.GetWidthOffset(2) - 62, graphics.GetHeightOffset(2) - 144), Color.White) }
            };


            int counterX = 0, counterY = -104;
            foreach (var outsideResource in creditsContainer.OutsideResources)
            {
                components.Add(
                    string.Concat("lnk", outsideResource.Author),
                    new LinkLabel(
                        string.Concat("lnk", outsideResource.Author),
                        fonts["Font"],
                        outsideResource.Author,
                        new Vector2(graphics.GetWidthOffset(2) - 300 + counterX, graphics.GetHeightOffset(2) + counterY),
                        Color.Blue,
                        outsideResource.Link
                    )
                );

                counterY += 18;

                foreach (var package in outsideResource.Packages)
                {
                    if (package.Length > 40)
                    {
                        string part1, part2;
                        part1 = package.Substring(0, package.IndexOf(' ', 30));
                        part2 = package.Substring(package.IndexOf(' ', 30));
                        components.Add(
                            string.Concat("lbl", outsideResource.Author, part1),
                            new Label(
                                string.Concat("lbl", outsideResource.Author, part1),
                                fonts["FontSmall"],
                                part1,
                                new Vector2(graphics.GetWidthOffset(2) - 288 + counterX, graphics.GetHeightOffset(2) + counterY),
                                Color.White
                            )
                        );

                        counterY += 12;
                        components.Add(
                            string.Concat("lbl", outsideResource.Author, part2),
                            new Label(
                                string.Concat("lbl", outsideResource.Author, part2),
                                fonts["FontSmall"],
                                part2,
                                new Vector2(graphics.GetWidthOffset(2) - 276 + counterX, graphics.GetHeightOffset(2) + counterY),
                                Color.White
                            )
                        );
                    }
                    else
                    {
                        components.Add(
                            string.Concat("lbl", outsideResource.Author, package),
                            new Label(
                                string.Concat("lbl", outsideResource.Author, package),
                                fonts["FontSmall"],
                                package,
                                new Vector2(graphics.GetWidthOffset(2) - 288 + counterX, graphics.GetHeightOffset(2) + counterY),
                                Color.White
                            )
                        );
                    }

                    counterY += 12;
                }

                counterY += 18;

                if (counterY > 86)
                {
                    counterY = -104;
                    counterX += 200;
                }
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

            return new CreditsWindow(graphics, null, position, components, hover, confirm);
        }

        private CreditsWindow
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

                if (mState.LeftButton == ButtonState.Pressed)
                {
                    if (buttons.Any(a => a.MouseOver()))
                    {
                        clicked = true;
                        selectedButton = buttons.IndexOf(buttons.First(a => a.MouseOver()));
                    }
                    else
                    {
                        for(int i = 0; i < Components.Count; i++)
                        {
                            if (Components.Values.ToList()[i] is LinkLabel &&
                                ((LinkLabel)Components.Values.ToList()[i]).MouseOver())
                            {
                                ((LinkLabel)Components.Values.ToList()[i]).GoToLink();
                                resetReadyForInput();
                            }
                        }
                    }
            }

                if (clicked || gState.Buttons.A == ButtonState.Pressed || kState.IsKeyDown(Keys.Enter))
                {
                    confirm.Play();
                    resetReadyForInput();
                    return "menu";
                }
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Selected = i == selectedButton;
                buttons[i].MouseOver(mState);
            }

            for (int i = 0; i < Components.Count; i++)
            {
                if (Components.Values.ToList()[i] is LinkLabel)
                {
                    ((LinkLabel)Components.Values.ToList()[i]).MouseOver(mState);
                }
            }

            return string.Empty;
        }
    }
}
