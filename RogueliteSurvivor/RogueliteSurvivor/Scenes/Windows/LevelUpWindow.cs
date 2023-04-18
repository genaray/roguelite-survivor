using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Helpers;
using RogueliteSurvivor.Scenes.SceneComponents;
using System.Collections.Generic;
using System.Linq;

namespace RogueliteSurvivor.Scenes.Windows
{
    public class LevelUpWindow : Window
    {
        SoundEffect hover;
        SoundEffect confirm;
        List<LevelUpType> levelUpChoices;

        public static LevelUpWindow LevelUpWindowFactory(
            GraphicsDeviceManager graphics,
            Texture2D background,
            Vector2 position,
            SoundEffect hover,
            SoundEffect confirm,
            Dictionary<string, SpriteFont> fonts,
            Dictionary<string, Texture2D> textures)
        {
            var components = new Dictionary<string, IFormComponent>()
            {
                { "lblTitle", new Label("lblTitle", fonts["Font"], "Level Up! Select an upgrade:", new Vector2(graphics.GetWidthOffset(2) - fonts["Font"].MeasureString("Level Up! Select an upgrade:").X / 2, graphics.GetHeightOffset(2) - 144), Color.White) }
            };

            int offsetX = -136;
            for (int i = 0; i < 4; i++)
            {
                components.Add(
                    string.Concat("btnOption", i),
                    new Button(
                        string.Concat("btnOption", i),
                        textures["LevelUpChoices"],
                        new Vector2(graphics.GetWidthOffset(2) + offsetX, graphics.GetHeightOffset(2) + 32),
                        LevelUpChoiceHelper.GetLevelUpChoiceButton(LevelUpType.AreaOfEffect, false),
                        LevelUpChoiceHelper.GetLevelUpChoiceButton(LevelUpType.AreaOfEffect, true),
                        new Vector2(32, 32)
                    ));
                offsetX += 80;

                components.Add(
                    string.Concat("lblOption", i),
                    new Label(
                        string.Concat("lblOption", i),
                        fonts["Font"],
                        string.Empty,
                        Vector2.Zero,
                        Color.White
                    )
                );
            }

            return new LevelUpWindow(graphics, background, position, components, hover, confirm);
        }
        private LevelUpWindow(
            GraphicsDeviceManager graphics,
            Texture2D background,
            Vector2 position,
            Dictionary<string, IFormComponent> components,
            SoundEffect hover,
            SoundEffect confirm)
            : base(graphics, background, position, components)
        {
            this.hover = hover;
            this.confirm = confirm;
        }

        public void SetLevelUpOptions(List<LevelUpType> levelUpChoices)
        {
            this.levelUpChoices = levelUpChoices;

            for (int i = 0; i < 4; i++)
            {
                ((Button)buttons[i]).ResetButtonTexture(
                    LevelUpChoiceHelper.GetLevelUpChoiceButton(levelUpChoices[i], false),
                    LevelUpChoiceHelper.GetLevelUpChoiceButton(levelUpChoices[i], true)
                );

                ((Label)Components[string.Concat("lblOption", i)]).Text = LevelUpChoiceHelper.GetLevelUpDisplayTextForLevelUpChoice(levelUpChoices[i]);
                ((Label)Components[string.Concat("lblOption", i)]).Position = new Vector2(graphics.GetWidthOffset(2) - ((Label)Components[string.Concat("lblOption", i)]).Font.MeasureString(((Label)Components[string.Concat("lblOption", i)]).Text).X / 2, graphics.GetHeightOffset(2) + 96);
                Components[string.Concat("lblOption", i)].Visible = i == 0;
            }
        }

        public override string Update(GameTime gameTime, params object[] values)
        {
            var kState = Keyboard.GetState();
            var gState = GamePad.GetState(PlayerIndex.One);
            var mState = Mouse.GetState();

            if (isReadyForInput(gameTime))
            {
                bool clicked = false;
                bool optionChanged = false;
                if (mState.LeftButton == ButtonState.Pressed && buttons.Any(a => a.MouseOver()))
                {
                    clicked = true;
                    selectedButton = buttons.IndexOf(buttons.First(a => a.MouseOver()));
                }

                if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                {
                    if (selectedButton > 0)
                    {
                        optionChanged = true;
                        selectedButton--;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                {
                    if (selectedButton < (buttons.Count - 1))
                    {
                        optionChanged = true;
                        selectedButton++;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (clicked || kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                {
                    confirm.Play();
                    return levelUpChoices[selectedButton].ToString();
                }

                if (optionChanged)
                {
                    foreach (var component in Components)
                    {
                        if (component.Value is Label && component.Key != "lblTitle")
                        {
                            if (component.Key == string.Concat("lblOption", selectedButton))
                            {
                                ((Label)component.Value).Visible = true;
                            }
                            else
                            {
                                ((Label)component.Value).Visible = false;
                            }
                        }
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
