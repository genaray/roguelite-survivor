using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Helpers;
using RogueliteSurvivor.Scenes.SceneComponents;
using System.Collections.Generic;
using System.Linq;

namespace RogueliteSurvivor.Scenes.Windows
{
    public class CharacterSelectionWindow : Window
    {
        SoundEffect hover;
        SoundEffect confirm;
        Dictionary<string, PlayerContainer> playerContainers;
        ProgressionContainer progressionContainer;

        const int descriptionLength = 80;

        public static CharacterSelectionWindow CharacterSelectionWindowFactory(GraphicsDeviceManager graphics,
            Dictionary<string, Texture2D> textures,
            Vector2 position,
            SoundEffect hover,
            SoundEffect confirm,
            Dictionary<string, SpriteFont> fonts,
            Dictionary<string, PlayerContainer> playerContainers,
            ProgressionContainer progressionContainer)
        {
            var components = new Dictionary<string, IFormComponent>()
            {
                { "lblTitle", new Label("lblTitle", fonts["Font"], "Character Selection", new Vector2(graphics.GetWidthOffset(2) - 62, graphics.GetHeightOffset(2) - 144), Color.White) }
            };

            int offsetX = 0, offsetY = 0;
            foreach (var character in playerContainers)
            {
                components.Add(
                    string.Concat("seo", character.Key),
                    new SelectableOption
                    (
                        string.Concat("seo", character.Key),
                        textures[character.Value.Texture],
                        textures["PlayerSelectOutline"],
                        new Vector2(graphics.GetWidthOffset(10.66f) + offsetX, graphics.GetHeightOffset(2) - 64 + offsetY),
                        new Rectangle(16, 0, 16, 16),
                        new Rectangle(0, 0, 20, 20),
                        new Vector2(8, 8),
                        new Vector2(10, 10)
                    )
                );

                offsetX += 24;
                if (offsetX > 72)
                {
                    offsetX = 0;
                    offsetY += 24;
                }

                int descriptionOffsetY = -64;

                foreach (var paragraph in character.Value.Description)
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
                        components.Add(
                            string.Concat("lbl", character.Key, descriptionOffsetY),
                            new Label(
                                string.Concat("lbl", character.Key, descriptionOffsetY),
                                fonts["FontSmall"],
                                descriptionLine,
                                new Vector2(graphics.GetWidthOffset(10.66f) + 125, graphics.GetHeightOffset(2) + descriptionOffsetY),
                                Color.White,
                                character.Key == playerContainers.First().Key
                            )
                        );

                        descriptionOffsetY += 12;
                    }

                    descriptionOffsetY += 12;
                }

                descriptionOffsetY += 12;

                components.Add(
                    string.Concat("lbl", character.Key, "BaseStats"),
                    new Label(
                        string.Concat("lbl", character.Key, "BaseStats"),
                        fonts["FontSmall"],
                        "Base Stats: ",
                        new Vector2(graphics.GetWidthOffset(10.66f) + 125, graphics.GetHeightOffset(2) + descriptionOffsetY),
                        Color.White,
                        character.Key == playerContainers.First().Key
                    )
                );

                components.Add(
                    string.Concat("lbl", character.Key, "Spell"),
                    new Label(
                        string.Concat("lbl", character.Key, "Spell"),
                        fonts["FontSmall"],
                    string.Concat("Spell: ", character.Value.StartingSpell.GetReadableSpellName()),
                        new Vector2(graphics.GetWidthOffset(10.66f) + 125, graphics.GetHeightOffset(2) + descriptionOffsetY + 12),
                        Color.White,
                        character.Key == playerContainers.First().Key
                    )
                );

                components.Add(
                    string.Concat("lbl", character.Key, "Health"),
                    new Label(
                        string.Concat("lbl", character.Key, "Health"),
                        fonts["FontSmall"],
                    string.Concat("Starting Health: ", (int)(character.Value.Health * 100 + progressionContainer.PlayerUpgrades.Health)),
                        new Vector2(graphics.GetWidthOffset(10.66f) + 125, graphics.GetHeightOffset(2) + descriptionOffsetY + 24),
                        Color.White,
                        character.Key == playerContainers.First().Key
                    )
                );

                components.Add(
                    string.Concat("lbl", character.Key, "Move"),
                    new Label(
                        string.Concat("lbl", character.Key, "Move"),
                        fonts["FontSmall"],
                    string.Concat("Move Speed: ", (int)(character.Value.Speed * 100 + progressionContainer.PlayerUpgrades.MoveSpeed)),
                        new Vector2(graphics.GetWidthOffset(10.66f) + 125, graphics.GetHeightOffset(2) + descriptionOffsetY + 36),
                        Color.White,
                        character.Key == playerContainers.First().Key
                    )
                );

                components.Add(
                    string.Concat("lbl", character.Key, "Damage"),
                    new Label(
                        string.Concat("lbl", character.Key, "Damage"),
                        fonts["FontSmall"],
                    string.Concat("Spell Damage: ", (character.Value.SpellDamage + 1f + (progressionContainer.PlayerUpgrades.Damage / 100f)).ToString("F"), "x"),
                        new Vector2(graphics.GetWidthOffset(10.66f) + 125, graphics.GetHeightOffset(2) + descriptionOffsetY + 48),
                        Color.White,
                        character.Key == playerContainers.First().Key
                    )
                );

                components.Add(
                    string.Concat("lbl", character.Key, "Effect"),
                    new Label(
                        string.Concat("lbl", character.Key, "Effect"),
                        fonts["FontSmall"],
                    string.Concat("Spell Effect Chance: ", (character.Value.SpellEffectChance + 1f + (progressionContainer.PlayerUpgrades.SpellEffectChance / 100f)).ToString("F"), "x"),
                        new Vector2(graphics.GetWidthOffset(10.66f) + 240, graphics.GetHeightOffset(2) + descriptionOffsetY + 12),
                        Color.White,
                        character.Key == playerContainers.First().Key
                    )
                );

                components.Add(
                    string.Concat("lbl", character.Key, "Attack"),
                    new Label(
                        string.Concat("lbl", character.Key, "Attack"),
                        fonts["FontSmall"],
                    string.Concat("Attack Speed: ", (character.Value.AttackSpeed + 1f + (progressionContainer.PlayerUpgrades.AttackSpeed / 100f)).ToString("F"), "x"),
                        new Vector2(graphics.GetWidthOffset(10.66f) + 240, graphics.GetHeightOffset(2) + descriptionOffsetY + 24),
                        Color.White,
                        character.Key == playerContainers.First().Key
                    )
                );

                components.Add(
                    string.Concat("lbl", character.Key, "Pierce"),
                    new Label(
                        string.Concat("lbl", character.Key, "Pierce"),
                        fonts["FontSmall"],
                    string.Concat("Pierce: ", character.Value.Pierce + progressionContainer.PlayerUpgrades.Pierce),
                        new Vector2(graphics.GetWidthOffset(10.66f) + 240, graphics.GetHeightOffset(2) + descriptionOffsetY + 36),
                        Color.White,
                        character.Key == playerContainers.First().Key
                    )
                );

                components.Add(
                    string.Concat("lbl", character.Key, "Area"),
                    new Label(
                        string.Concat("lbl", character.Key, "Area"),
                        fonts["FontSmall"],
                    string.Concat("Area of Effect: ", (character.Value.AreaOfEffect + 1f + (progressionContainer.PlayerUpgrades.AreaOfEffect / 100f)).ToString("F"), "x"),
                        new Vector2(graphics.GetWidthOffset(10.66f) + 240, graphics.GetHeightOffset(2) + descriptionOffsetY + 48),
                        Color.White,
                        character.Key == playerContainers.First().Key
                    )
                );

                components.Add(
                    string.Concat("lbl", character.Key, "Special"),
                    new Label(
                        string.Concat("lbl", character.Key, "Special"),
                        fonts["FontSmall"],
                        "Special Traits: ",
                        new Vector2(graphics.GetWidthOffset(10.66f) + 385, graphics.GetHeightOffset(2) + descriptionOffsetY),
                        Color.White,
                        character.Key == playerContainers.First().Key
                    )
                );

                if (character.Value.Traits.Any())
                {
                    int specialTraitOffset = 12;
                    foreach (var trait in character.Value.Traits)
                    {
                        components.Add(
                            string.Concat("lbl", character.Key, trait),
                            new Label(
                                string.Concat("lbl", character.Key, trait),
                                fonts["FontSmall"],
                                trait.ReadableTraitName(),
                                new Vector2(graphics.GetWidthOffset(10.66f) + 385, graphics.GetHeightOffset(2) + descriptionOffsetY + specialTraitOffset),
                                Color.White,
                                character.Key == playerContainers.First().Key
                            )
                        );
                        specialTraitOffset += 12;
                    }
                }
                else
                {
                    components.Add(
                    string.Concat("lbl", character.Key, "None"),
                    new Label(
                        string.Concat("lbl", character.Key, "None"),
                        fonts["FontSmall"],
                        "None",
                        new Vector2(graphics.GetWidthOffset(10.66f) + 385, graphics.GetHeightOffset(2) + descriptionOffsetY + 12),
                        Color.White,
                        character.Key == playerContainers.First().Key
                    )
                );
                }
            }

            components.Add(
                "btnBack",
                new Button(
                    "btnBack",
                    textures["MainMenuButtons"],
                    new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) + 144),
                    new Rectangle(0, 0, 128, 32),
                    new Rectangle(128, 0, 128, 32),
                    new Vector2(64, 16)
                )
            );

            return new CharacterSelectionWindow(graphics, null, position, components, hover, confirm, playerContainers, progressionContainer);
        }

        private CharacterSelectionWindow(GraphicsDeviceManager graphics,
            Texture2D background,
            Vector2 position,
            Dictionary<string, IFormComponent> components,
            SoundEffect hover,
            SoundEffect confirm,
            Dictionary<string, PlayerContainer> playerContainers,
            ProgressionContainer progressionContainer)
            : base(graphics, background, position, components)
        {
            this.hover = hover;
            this.confirm = confirm;
            this.playerContainers = playerContainers;
            this.progressionContainer = progressionContainer;
        }

        public override void SetActive()
        {
            foreach (var character in playerContainers)
            {
                ((Label)Components[string.Concat("lbl", character.Key, "Health")]).Text = string.Concat("Starting Health: ", (int)(character.Value.Health * 100 + progressionContainer.PlayerUpgrades.Health));
                ((Label)Components[string.Concat("lbl", character.Key, "Move")]).Text = string.Concat("Move Speed: ", (int)(character.Value.Speed * 100 + progressionContainer.PlayerUpgrades.MoveSpeed));
                ((Label)Components[string.Concat("lbl", character.Key, "Damage")]).Text = string.Concat("Spell Damage: ", (character.Value.SpellDamage + 1f + (progressionContainer.PlayerUpgrades.Damage / 100f)).ToString("F"), "x");
                ((Label)Components[string.Concat("lbl", character.Key, "Effect")]).Text = string.Concat("Spell Effect Chance: ", (character.Value.SpellEffectChance + 1f + (progressionContainer.PlayerUpgrades.SpellEffectChance / 100f)).ToString("F"), "x");
                ((Label)Components[string.Concat("lbl", character.Key, "Attack")]).Text = string.Concat("Attack Speed: ", (character.Value.AttackSpeed + 1f + (progressionContainer.PlayerUpgrades.AttackSpeed / 100f)).ToString("F"), "x");
                ((Label)Components[string.Concat("lbl", character.Key, "Pierce")]).Text = string.Concat("Pierce: ", character.Value.Pierce + progressionContainer.PlayerUpgrades.Pierce);
                ((Label)Components[string.Concat("lbl", character.Key, "Area")]).Text = string.Concat("Area of Effect: ", (character.Value.AreaOfEffect + 1f + (progressionContainer.PlayerUpgrades.AreaOfEffect / 100f)).ToString("F"), "x");
            }
            setLabelVisibility();
            base.SetActive();
        }

        public override string Update(GameTime gameTime, params object[] values)
        {
            var kState = Keyboard.GetState();
            var gState = GamePad.GetState(PlayerIndex.One);
            var mState = Mouse.GetState();

            if (isReadyForInput(gameTime))
            {
                bool clicked = false;
                bool buttonChanged = false;

                if (mState.LeftButton == ButtonState.Pressed && buttons.Any(a => a.MouseOver()))
                {
                    clicked = true;
                    selectedButton = buttons.IndexOf(buttons.First(a => a.MouseOver()));
                }

                if (kState.IsKeyDown(Keys.Up) || gState.DPad.Up == ButtonState.Pressed || gState.ThumbSticks.Left.Y > 0.5f)
                {
                    if (selectedButton - 4 >= 0)
                    {
                        selectedButton -= 4;
                        buttonChanged = true;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                {
                    if (selectedButton + 4 < buttons.Count)
                    {
                        selectedButton += 4;
                        buttonChanged = true;
                        hover.Play();
                        resetReadyForInput();
                    }
                    else
                    {
                        selectedButton = buttons.Count - 1;
                        buttonChanged = true;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                {
                    if (selectedButton < (buttons.Count - 1) && selectedButton % 4 > 0)
                    {
                        selectedButton--;
                        buttonChanged = true;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                {
                    if (selectedButton < (buttons.Count - 1) && selectedButton % 4 < 3)
                    {
                        selectedButton++;
                        buttonChanged = true;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (clicked || kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                {
                    confirm.Play();
                    var button = (IFormComponent)buttons[selectedButton];

                    if (button.Name == "btnBack")
                    {
                        return "menu";
                    }
                    else
                    {
                        return button.Name.Replace("seo", string.Empty);
                    }
                }

                if (buttonChanged)
                {
                    setLabelVisibility();
                }
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Selected = i == selectedButton;
                buttons[i].MouseOver(mState);
            }

            return string.Empty;
        }

        private void setLabelVisibility()
        {
            var button = (IFormComponent)buttons[selectedButton];
            if (button.Name != "btnBack")
            {
                foreach (var component in Components)
                {
                    if (component.Value is Label && component.Key != "lblTitle")
                    {
                        if (component.Key.Replace("lbl", string.Empty).StartsWith(button.Name.Replace("seo", string.Empty)))
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
    }
}
