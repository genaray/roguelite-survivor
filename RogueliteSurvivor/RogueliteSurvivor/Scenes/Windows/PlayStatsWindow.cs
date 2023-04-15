using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Helpers;
using RogueliteSurvivor.Scenes.SceneComponents;
using System.Collections.Generic;
using System.Linq;

namespace RogueliteSurvivor.Scenes.Windows
{
    public class PlayStatsWindow : Window
    {
        int selectedButton = 0;
        List<ISelectableComponent> buttons;
        List<Window> statPages;
        SoundEffect hover;
        SoundEffect confirm;
        int statsPage = 0;

        public static PlayStatsWindow PlayStatsWindowFactory(
            GraphicsDeviceManager graphics,
            Dictionary<string, Texture2D> textures,
            Vector2 position,
            SoundEffect hover,
            SoundEffect confirm,
            Dictionary<string, SpriteFont> fonts,
            ProgressionContainer progressionContainer,
            Dictionary<string, EnemyContainer> enemyContainers)
        {
            var mapComponents = new Dictionary<string, IFormComponent>()
            {
                { "lblTitle", new Label("lblTitle", fonts["Font"], "Maps", new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) - 96), Color.White) }
            };

            var enemyComponents = new Dictionary<string, IFormComponent>()
            {
                { "lblTitle", new Label("lblTitle", fonts["Font"], "Enemies", new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) - 96), Color.White) }
            };

            int counterX = 0, counterY = 0;
            foreach (var map in progressionContainer.LevelProgressions)
            {
                mapComponents.Add(
                    string.Concat("lbl", map.Name),
                    new Label(
                        string.Concat("lbl", map.Name),
                        fonts["FontSmall"],
                        map.Name,
                        new Vector2(graphics.GetWidthOffset(10.66f), graphics.GetHeightOffset(2) - 72 + counterY),
                        Color.White
                    )
                );
                mapComponents.Add(
                    string.Concat("lbl", map.Name, "BestTime"),
                    new Label(
                        string.Concat("lbl", map.Name, "BestTime"),
                        fonts["FontSmall"],
                        string.Concat("  Best Time: ", map.BestTime.ToFormattedTime()),
                        new Vector2(graphics.GetWidthOffset(10.66f), graphics.GetHeightOffset(2) - 60 + counterY),
                        Color.White
                    )
                );

                counterY += 36;
                if (counterY > 160)
                {
                    counterX += 112;
                    counterY = 0;
                }
            }

            counterX = 0;
            counterY = 0;

            foreach (var enemy in enemyContainers)
            {
                enemyComponents.Add(
                    string.Concat("lbl", enemy.Value.Name),
                    new Label(
                        string.Concat("lbl", enemy.Value.Name),
                        fonts["FontSmall"],
                        enemy.Value.ReadableName,
                        new Vector2(graphics.GetWidthOffset(10.66f) + counterX, graphics.GetHeightOffset(2) - 72 + counterY),
                        Color.White
                    )
                );

                var enemyProgression = progressionContainer.EnemyKillStats.Where(a => a.Name == enemy.Value.ReadableName).FirstOrDefault();
                if (enemyProgression != null)
                {
                    enemyComponents.Add(
                        string.Concat("lbl", enemy.Value.Name, "Kills"),
                        new Label(
                            string.Concat("lbl", enemy.Value.Name, "Kills"),
                            fonts["FontSmall"],
                            string.Concat("  Kills: ", enemyProgression.Kills),
                            new Vector2(graphics.GetWidthOffset(10.66f) + counterX, graphics.GetHeightOffset(2) - 60 + counterY),
                            Color.White
                        )
                    );
                    enemyComponents.Add(
                        string.Concat("lbl", enemy.Value.Name, "KilledBy"),
                        new Label(
                            string.Concat("lbl", enemy.Value.Name, "KilledBy"),
                            fonts["FontSmall"],
                            string.Concat("  Killed By: ", enemyProgression.KilledBy),
                            new Vector2(graphics.GetWidthOffset(10.66f) + counterX, graphics.GetHeightOffset(2) - 48 + counterY),
                            Color.White
                        )
                    );
                    counterY += 48;
                }
                else
                {
                    enemyComponents.Add(
                        string.Concat("lbl", enemy.Value.Name, "NotFound"),
                        new Label(
                            string.Concat("lbl", enemy.Value.Name),
                            fonts["FontSmall"],
                            "  Not Yet Found",
                            new Vector2(graphics.GetWidthOffset(10.66f) + counterX, graphics.GetHeightOffset(2) - 60 + counterY),
                            Color.White
                        )
                    );

                    counterY += 36;
                }

                if (counterY > 160)
                {
                    counterX += 112;
                    counterY = 0;
                }
            }


            var components = new Dictionary<string, IFormComponent>()
            {
                { "lblTitle", new Label("lblTitle", fonts["Font"], "Play Stats", new Vector2(graphics.GetWidthOffset(2) - 62, graphics.GetHeightOffset(2) - 144), Color.White) },
                { "btnPrevious",  new Button(
                    "btnPrevious",
                    textures["VolumeButtons"],
                    new Vector2(graphics.GetWidthOffset(2) - 24, graphics.GetHeightOffset(2) + 104),
                    new Rectangle(0, 32, 16, 16),
                    new Rectangle(16, 32, 16, 16),
                    new Vector2(8, 8)
                )},
                { "btnNext", new Button(
                    "btnNext",
                    textures["VolumeButtons"],
                    new Vector2(graphics.GetWidthOffset(2) + 24, graphics.GetHeightOffset(2) + 104),
                    new Rectangle(0, 48, 16, 16),
                    new Rectangle(16, 48, 16, 16),
                    new Vector2(8, 8)
                )},
                { "btnBack", new Button(
                    "btnBack",
                    textures["MainMenuButtons"],
                    new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) + 144),
                    new Rectangle(0, 192, 128, 32),
                    new Rectangle(128, 192, 128, 32),
                    new Vector2(64, 16)
                )},
                {
                    "wndMap", new GenericWindow(
                        graphics,
                        null,
                        new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2)),
                        mapComponents,
                        true)
                },
                {
                    "wndEnemies", new GenericWindow(
                        graphics,
                        null,
                        new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2)),
                        enemyComponents,
                        false)
                }
            };



            return new PlayStatsWindow(graphics, null, position, components, hover, confirm);
        }

        private PlayStatsWindow
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
            buttons = new List<ISelectableComponent>();
            statPages = new List<Window>();
            foreach (var component in components)
            {
                if (component.Value is ISelectableComponent)
                {
                    buttons.Add((ISelectableComponent)component.Value);
                }
                if (component.Value is Window)
                {
                    statPages.Add((Window)component.Value);
                }
            }

            this.hover = hover;
            this.confirm = confirm;
        }

        public override void SetActive()
        {
            selectedButton = 0;
            statsPage = 0;
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

                if (kState.IsKeyDown(Keys.Up) || gState.DPad.Up == ButtonState.Pressed || gState.ThumbSticks.Left.Y > 0.5f)
                {
                    if (selectedButton == 2)
                    {
                        selectedButton = 0;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                {
                    if (selectedButton < 2)
                    {
                        selectedButton = 2;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                {
                    if (selectedButton == 1)
                    {
                        selectedButton--;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                {
                    if (selectedButton == 0)
                    {
                        selectedButton++;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (clicked || gState.Buttons.A == ButtonState.Pressed || kState.IsKeyDown(Keys.Enter))
                {
                    confirm.Play();
                    switch (selectedButton)
                    {
                        case 0:
                            statsPage = (statsPage - 1) % 2;
                            resetReadyForInput();
                            break;
                        case 1:
                            statsPage = (statsPage + 1) % 2;
                            resetReadyForInput();
                            break;
                        case 2:
                            return "menu";
                    }

                    for (int i = 0; i < statPages.Count; i++)
                    {
                        statPages[i].Visible = i == statsPage;
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
