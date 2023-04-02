using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RogueliteSurvivor.Scenes.SceneComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using RogueliteSurvivor.Constants;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework.Audio;

namespace RogueliteSurvivor.Scenes.Windows
{
    public class InGameMenuWindow : Window
    {
        int selectedButton = 0;
        List<ISelectableComponent> buttons;
        bool readyForInput = false;
        float counter = 0f;
        SoundEffect hover;
        SoundEffect confirm;

        public InGameMenuWindow
        (
            Texture2D background, 
            Vector2 position, 
            List<IFormComponent> components,
            SoundEffect hover,
            SoundEffect confirm
        ) 
            : base(background, position, components)
        {
            buttons = new List<ISelectableComponent>();
            foreach( var component in components )
            {
                if(component is ISelectableComponent )
                {
                    buttons.Add((ISelectableComponent)component);
                }
            }

            this.hover = hover;
            this.confirm = confirm;
        }

        public override void SetActive()
        {
            selectedButton = 0;
            counter = 0f;
            readyForInput = false;
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
                        readyForInput = false;
                    }
                }
                else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                {
                    if (selectedButton < (buttons.Count - 1))
                    {
                        selectedButton++;
                        hover.Play();
                        readyForInput = false;
                    }
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
