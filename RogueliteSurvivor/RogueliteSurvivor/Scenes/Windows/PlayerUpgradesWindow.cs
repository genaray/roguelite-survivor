using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Scenes.SceneComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueliteSurvivor.Scenes.Windows
{
    public class PlayerUpgradesWindow : Window
    {
        SoundEffect hover;
        SoundEffect confirm;
        SoundEffect denied;
        ProgressionContainer progressionContainer;

        public static PlayerUpgradesWindow PlayerUpgradesWindowFactory(
            GraphicsDeviceManager graphics,
            Dictionary<string, Texture2D> textures,
            Vector2 position,
            SoundEffect hover,
            SoundEffect confirm,
            SoundEffect denied,
            Dictionary<string, SpriteFont> fonts,
            ProgressionContainer progressionContainer)
        {
            var components = new Dictionary<string, IFormComponent>()
            {
                { "lblTitle", new Label("lblTitle", fonts["Font"], "Player Upgrades", new Vector2(graphics.GetWidthOffset(2) - fonts["Font"].MeasureString("Player Upgrades").X / 2, graphics.GetHeightOffset(2) - 144), Color.White) }
            };

            for (int i = 0; i < 7; i++)
            {
                string componentBaseName = string.Empty;
                string labelText = string.Empty;
                string labelCostText = string.Empty;

                switch (i)
                {
                    case 0:
                        componentBaseName = "Health";
                        labelText = string.Concat("Health: +", progressionContainer.PlayerUpgrades.Health);
                        labelCostText = progressionContainer.PlayerUpgrades.Health < 20
                            ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.Health + 4) / 4) * 10))
                            : "Stat Maxed";
                        break;
                    case 1:
                        componentBaseName = "Damage";
                        labelText = string.Concat("Damage: +", (progressionContainer.PlayerUpgrades.Damage / 100f).ToString("F"), "x");
                        labelCostText = progressionContainer.PlayerUpgrades.Damage < 20
                            ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.Damage + 4) / 4) * 10))
                            : "Stat Maxed";
                        break;
                    case 2:
                        componentBaseName = "SpellEffectChance";
                        labelText = string.Concat("Spell Effect Chance: +", (progressionContainer.PlayerUpgrades.SpellEffectChance / 100f).ToString("F"), "x");
                        labelCostText = progressionContainer.PlayerUpgrades.SpellEffectChance < 20
                            ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.SpellEffectChance + 4) / 4) * 10))
                            : "Stat Maxed";
                        break;
                    case 3:
                        componentBaseName = "Pierce";
                        labelText = string.Concat("Pierce: +", progressionContainer.PlayerUpgrades.Pierce);
                        labelCostText = progressionContainer.PlayerUpgrades.Pierce < 1
                            ? string.Concat("Book Cost: ", 100)
                            : "Stat Maxed";
                        break;
                    case 4:
                        componentBaseName = "AttackSpeed";
                        labelText = string.Concat("Attack Speed: +", (progressionContainer.PlayerUpgrades.AttackSpeed / 100f).ToString("F"), "x");
                        labelCostText = progressionContainer.PlayerUpgrades.AttackSpeed < 20
                            ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.AttackSpeed + 4) / 4) * 10))
                            : "Stat Maxed";
                        break;
                    case 5:
                        componentBaseName = "AreaOfEffect";
                        labelText = string.Concat("Area Of Effect: +", (progressionContainer.PlayerUpgrades.AreaOfEffect / 100f).ToString("F"), "x");
                        labelCostText = progressionContainer.PlayerUpgrades.AreaOfEffect < 20
                            ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.AreaOfEffect + 4) / 4) * 10))
                            : "Stat Maxed";
                        break;
                    case 6:
                        componentBaseName = "MoveSpeed";
                        labelText = string.Concat("Move Speed: +", progressionContainer.PlayerUpgrades.MoveSpeed);
                        labelCostText = progressionContainer.PlayerUpgrades.MoveSpeed < 20
                            ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.MoveSpeed + 4) / 4) * 10))
                            : "Stat Maxed";
                        break;
                }

                components.Add(
                    string.Concat("lbl", componentBaseName),
                    new Label(
                        string.Concat("lbl", componentBaseName),
                        fonts["Font"],
                        labelText,
                        new Vector2(position.X - 175, position.Y - 120 + (i * 32)),
                        Color.White
                    )
                );

                components.Add(
                    string.Concat("lbl", componentBaseName, "Cost"),
                    new Label(
                        string.Concat("lbl", componentBaseName, "Cost"),
                        fonts["Font"],
                        labelCostText,
                        new Vector2(position.X + 75, position.Y - 120 + (i * 32)),
                        Color.White
                    )
                );

                components.Add(
                    string.Concat("btn", componentBaseName, "Decrease"),
                    new Button(
                        string.Concat("btn", componentBaseName, "Decrease"),
                        textures["VolumeButtons"],
                        new Vector2(graphics.GetWidthOffset(2) + 26, graphics.GetHeightOffset(2) - 112 + (i * 32)),
                        new Rectangle(0, 0, 16, 16),
                        new Rectangle(16, 0, 16, 16),
                        new Vector2(8, 8)
                    )
                );
                components.Add(
                    string.Concat("btn", componentBaseName, "Increase"),
                    new Button(
                        string.Concat("btn", componentBaseName, "Increase"),
                        textures["VolumeButtons"],
                        new Vector2(graphics.GetWidthOffset(2) + 50, graphics.GetHeightOffset(2) - 112 + (i * 32)),
                        new Rectangle(0, 16, 16, 16),
                        new Rectangle(16, 16, 16, 16),
                        new Vector2(8, 8)
                    )
                );
            }

            components.Add(
                "lblBooksToRead",
                new Label(
                    "lblBooksToRead",
                    fonts["Font"],
                    string.Concat("Books to read: ", progressionContainer.NumBooks),
                    new Vector2(graphics.GetWidthOffset(2) - 55, graphics.GetHeightOffset(2) + 104),
                    Color.White
                )
            );

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

            return new PlayerUpgradesWindow(graphics, null, position, components, hover, confirm, denied, progressionContainer);
        }

        private PlayerUpgradesWindow
        (
            GraphicsDeviceManager graphics,
            Texture2D background,
            Vector2 position,
            Dictionary<string, IFormComponent> components,
            SoundEffect hover,
            SoundEffect confirm,
            SoundEffect denied,
            ProgressionContainer progressionContainer
        )
            : base(graphics, background, position, components)
        {
            this.hover = hover;
            this.confirm = confirm;
            this.denied = denied;
            this.progressionContainer = progressionContainer;
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
                    if (selectedButton + 2 < buttons.Count)
                    {
                        selectedButton += 2;
                        hover.Play();
                        resetReadyForInput();
                    }
                    else if (selectedButton == (buttons.Count - 2))
                    {
                        selectedButton = buttons.Count - 1;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                {
                    if (selectedButton < (buttons.Count - 1) && selectedButton % 2 == 1)
                    {
                        selectedButton--;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                {
                    if (selectedButton < (buttons.Count - 1) && selectedButton % 2 == 0)
                    {
                        selectedButton++;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (clicked || kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                {
                    bool failed = false;

                    if (((IFormComponent)buttons[selectedButton]).Name == "btnBack")
                    {
                        progressionContainer.Save();
                        confirm.Play();
                        return "menu";
                    }
                    else
                    {
                        failed = processPlayerUpgrade();
                    }

                    if (failed)
                    {
                        denied.Play();
                    }
                    else
                    {
                        confirm.Play();
                        updateLabels();
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

        private bool processPlayerUpgrade()
        {
            bool failed = false;
            switch (selectedButton)
            {
                case 0:
                    if (progressionContainer.PlayerUpgrades.Health > 0)
                    {
                        progressionContainer.NumBooks += (int)(MathF.Pow(2, progressionContainer.PlayerUpgrades.Health / 4) * 10);
                        progressionContainer.PlayerUpgrades.Health -= 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 1:
                    int healthBookCost = (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.Health + 4) / 4) * 10);
                    if (progressionContainer.NumBooks >= healthBookCost && progressionContainer.PlayerUpgrades.Health < 20)
                    {
                        progressionContainer.NumBooks -= healthBookCost;
                        progressionContainer.PlayerUpgrades.Health += 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 2:
                    if (progressionContainer.PlayerUpgrades.Damage > 0)
                    {
                        progressionContainer.NumBooks += (int)(MathF.Pow(2, progressionContainer.PlayerUpgrades.Damage / 4) * 10);
                        progressionContainer.PlayerUpgrades.Damage -= 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 3:
                    int damageBookCost = (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.Damage + 4) / 4) * 10);
                    if (progressionContainer.NumBooks >= damageBookCost && progressionContainer.PlayerUpgrades.Damage < 20)
                    {
                        progressionContainer.NumBooks -= damageBookCost;
                        progressionContainer.PlayerUpgrades.Damage += 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 4:
                    if (progressionContainer.PlayerUpgrades.SpellEffectChance > 0)
                    {
                        progressionContainer.NumBooks += (int)(MathF.Pow(2, progressionContainer.PlayerUpgrades.SpellEffectChance / 4) * 10);
                        progressionContainer.PlayerUpgrades.SpellEffectChance -= 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 5:
                    int spellEffectChanceBookCost = (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.SpellEffectChance + 4) / 4) * 10);
                    if (progressionContainer.NumBooks >= spellEffectChanceBookCost && progressionContainer.PlayerUpgrades.SpellEffectChance < 20)
                    {
                        progressionContainer.NumBooks -= spellEffectChanceBookCost;
                        progressionContainer.PlayerUpgrades.SpellEffectChance += 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 6:
                    if (progressionContainer.PlayerUpgrades.Pierce > 0)
                    {
                        progressionContainer.NumBooks += 100;
                        progressionContainer.PlayerUpgrades.Pierce -= 1;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 7:
                    int pierceBookCost = 100;
                    if (progressionContainer.NumBooks >= pierceBookCost && progressionContainer.PlayerUpgrades.Pierce < 1)
                    {
                        progressionContainer.NumBooks -= pierceBookCost;
                        progressionContainer.PlayerUpgrades.Pierce += 1;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 8:
                    if (progressionContainer.PlayerUpgrades.AttackSpeed > 0)
                    {
                        progressionContainer.NumBooks += (int)(MathF.Pow(2, progressionContainer.PlayerUpgrades.AttackSpeed / 4) * 10);

                        progressionContainer.PlayerUpgrades.AttackSpeed -= 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 9:
                    int attackSpeedBookCost = (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.AttackSpeed + 4) / 4) * 10);
                    if (progressionContainer.NumBooks >= attackSpeedBookCost && progressionContainer.PlayerUpgrades.AttackSpeed < 20)
                    {
                        progressionContainer.NumBooks -= attackSpeedBookCost;
                        progressionContainer.PlayerUpgrades.AttackSpeed += 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 10:
                    if (progressionContainer.PlayerUpgrades.AreaOfEffect > 0)
                    {
                        progressionContainer.NumBooks += (int)(MathF.Pow(2, progressionContainer.PlayerUpgrades.AreaOfEffect / 4) * 10);
                        progressionContainer.PlayerUpgrades.AreaOfEffect -= 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 11:
                    int areaOfEffectBookCost = (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.AreaOfEffect + 4) / 4) * 10);
                    if (progressionContainer.NumBooks >= areaOfEffectBookCost && progressionContainer.PlayerUpgrades.AreaOfEffect < 20)
                    {
                        progressionContainer.NumBooks -= areaOfEffectBookCost;
                        progressionContainer.PlayerUpgrades.AreaOfEffect += 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 12:
                    if (progressionContainer.PlayerUpgrades.MoveSpeed > 0)
                    {
                        progressionContainer.NumBooks += (int)(MathF.Pow(2, progressionContainer.PlayerUpgrades.MoveSpeed / 4) * 10);
                        progressionContainer.PlayerUpgrades.MoveSpeed -= 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
                case 13:
                    int moveSpeedBookCost = (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.MoveSpeed + 4) / 4) * 10);
                    if (progressionContainer.NumBooks >= moveSpeedBookCost && progressionContainer.PlayerUpgrades.MoveSpeed < 20)
                    {
                        progressionContainer.NumBooks -= moveSpeedBookCost;
                        progressionContainer.PlayerUpgrades.MoveSpeed += 4;
                    }
                    else
                    {
                        failed = true;
                    }
                    break;
            }
            return failed;
        }

        private void updateLabels()
        {
            switch (selectedButton)
            {
                case 0:
                case 1:
                    ((Label)Components["lblHealth"]).Text = string.Concat("Health: +", progressionContainer.PlayerUpgrades.Health);
                    ((Label)Components["lblHealthCost"]).Text = progressionContainer.PlayerUpgrades.Health < 20
                        ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.Health + 4) / 4) * 10))
                        : "Stat Maxed";
                    break;
                case 2:
                case 3:
                    ((Label)Components["lblDamage"]).Text = string.Concat("Damage: +", (progressionContainer.PlayerUpgrades.Damage / 100f).ToString("F"), "x");
                    ((Label)Components["lblDamageCost"]).Text = progressionContainer.PlayerUpgrades.Damage < 20
                        ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.Damage + 4) / 4) * 10))
                        : "Stat Maxed";
                    break;
                case 4:
                case 5:
                    ((Label)Components["lblSpellEffectChance"]).Text = string.Concat("Spell Effect Chance: +", (progressionContainer.PlayerUpgrades.SpellEffectChance / 100f).ToString("F"), "x");
                    ((Label)Components["lblSpellEffectChanceCost"]).Text = progressionContainer.PlayerUpgrades.SpellEffectChance < 20
                        ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.SpellEffectChance + 4) / 4) * 10))
                        : "Stat Maxed";
                    break;
                case 6:
                case 7:
                    ((Label)Components["lblPierce"]).Text = string.Concat("Pierce: +", progressionContainer.PlayerUpgrades.Pierce);
                    ((Label)Components["lblPierceCost"]).Text = progressionContainer.PlayerUpgrades.Pierce < 1
                        ? string.Concat("Book Cost: ", 100)
                        : "Stat Maxed";
                    break;
                case 8:
                case 9:
                    ((Label)Components["lblAttackSpeed"]).Text = string.Concat("Attack Speed: +", (progressionContainer.PlayerUpgrades.AttackSpeed / 100f).ToString("F"), "x");
                    ((Label)Components["lblAttackSpeedCost"]).Text = progressionContainer.PlayerUpgrades.AttackSpeed < 20
                        ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.AttackSpeed + 4) / 4) * 10))
                        : "Stat Maxed";
                    break;
                case 10:
                case 11:
                    ((Label)Components["lblAreaOfEffect"]).Text = string.Concat("Area Of Effect: +", (progressionContainer.PlayerUpgrades.AreaOfEffect / 100f).ToString("F"), "x");
                    ((Label)Components["lblAreaOfEffectCost"]).Text = progressionContainer.PlayerUpgrades.AreaOfEffect < 20
                        ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.AreaOfEffect + 4) / 4) * 10))
                        : "Stat Maxed";
                    break;
                case 12:
                case 13:
                    ((Label)Components["lblMoveSpeed"]).Text = string.Concat("Move Speed: +", progressionContainer.PlayerUpgrades.MoveSpeed);
                    ((Label)Components["lblMoveSpeedCost"]).Text = progressionContainer.PlayerUpgrades.MoveSpeed < 20
                        ? string.Concat("Book Cost: ", (int)(MathF.Pow(2, (progressionContainer.PlayerUpgrades.MoveSpeed + 4) / 4) * 10))
                        : "Stat Maxed";
                    break;
            }
        }
    }
}
