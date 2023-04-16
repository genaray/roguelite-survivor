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
    public class MapSelectionWindow : Window
    {
        SoundEffect hover;
        SoundEffect confirm;
        SoundEffect denied;
        List<MapContainer> mapContainers;
        List<MapContainer> unlockedMaps;
        ProgressionContainer progressionContainer;
        Dictionary<string, Texture2D> textures;
        Dictionary<string, SpriteFont> fonts;
        string selectedMap;

        const int descriptionLength = 80;

        public static MapSelectionWindow MapSelectionWindowFactory(
            GraphicsDeviceManager graphics,
            Dictionary<string, Texture2D> textures,
            Vector2 position,
            SoundEffect hover,
            SoundEffect confirm,
            SoundEffect denied,
            Dictionary<string, SpriteFont> fonts,
            List<MapContainer> mapContainers,
            ProgressionContainer progressionContainer
            )
        {
            var components = new Dictionary<string, IFormComponent>()
            {
                { "lblTitle", new Label("lblTitle", fonts["Font"], "Map Selection", new Vector2(graphics.GetWidthOffset(2) - 62, graphics.GetHeightOffset(2) - 144), Color.White) },
                { "btnPreviousMap", new Button(
                    "btnPreviousMap",
                    textures["MainMenuButtons"],
                    new Vector2(graphics.GetWidthOffset(2) - 160, graphics.GetHeightOffset(2) + 96),
                    new Rectangle(0, 288, 128, 32),
                    new Rectangle(128, 288, 128, 32),
                    new Vector2(64, 16)
                ) },
                { "btnStart", new Button(
                    "btnStart",
                    textures["MainMenuButtons"],
                    new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) + 96),
                    new Rectangle(0, 224, 128, 32),
                    new Rectangle(128, 224, 128, 32),
                    new Vector2(64, 16)
                ) },
                { "btnNextMap", new Button(
                    "btnNextMap",
                    textures["MainMenuButtons"],
                    new Vector2(graphics.GetWidthOffset(2) + 160, graphics.GetHeightOffset(2) + 96),
                    new Rectangle(0, 256, 128, 32),
                    new Rectangle(128, 256, 128, 32),
                    new Vector2(64, 16)
                ) },
                { "btnBack", new Button(
                    "btnBack",
                    textures["MainMenuButtons"],
                    new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2) + 144),
                    new Rectangle(0, 0, 128, 32),
                    new Rectangle(128, 0, 128, 32),
                    new Vector2(64, 16)
                ) }
            };

            foreach (var map in mapContainers)
            {
                components.Add(
                    string.Concat("wnd", map.Name),
                    new MapWindow(
                        graphics,
                        null,
                        new Vector2(graphics.GetWidthOffset(2), graphics.GetHeightOffset(2)),
                        new Dictionary<string, IFormComponent>(),
                        map == mapContainers[0]
                    )
                );
            }


            return new MapSelectionWindow(graphics, null, position, components, hover, confirm, denied, mapContainers, progressionContainer, textures, fonts);
        }

        private MapSelectionWindow(
            GraphicsDeviceManager graphics,
            Texture2D background,
            Vector2 position,
            Dictionary<string, IFormComponent> components,
            SoundEffect hover,
            SoundEffect confirm,
            SoundEffect denied,
            List<MapContainer> mapContainers,
            ProgressionContainer progressionContainer,
            Dictionary<string, Texture2D> textures,
            Dictionary<string, SpriteFont> fonts)
            : base(graphics, background, position, components)
        {
            this.hover = hover;
            this.confirm = confirm;
            this.denied = denied;
            this.mapContainers = mapContainers;
            this.progressionContainer = progressionContainer;
            this.textures = textures;
            this.fonts = fonts;

            selectedMap = mapContainers[0].Name;

            setUnlockedMaps();
        }

        public override void SetActive()
        {
            setUnlockedMaps();
            base.SetActive();
            selectedButton = 1;
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
                    if (selectedButton == 3)
                    {
                        selectedButton = 1;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Down) || gState.DPad.Down == ButtonState.Pressed || gState.ThumbSticks.Left.Y < -0.5f)
                {
                    if (selectedButton < 3)
                    {
                        selectedButton = 3;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                {
                    if (selectedButton == 1 || selectedButton == 2)
                    {
                        selectedButton--;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                {
                    if (selectedButton == 0 || selectedButton == 1)
                    {
                        selectedButton++;
                        hover.Play();
                        resetReadyForInput();
                    }
                }
                else if (clicked || kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                {
                    bool mapChanged = false;
                    switch (selectedButton)
                    {
                        case 0:
                            if (selectedMap != mapContainers[0].Name)
                            {
                                int index = mapContainers.IndexOf(mapContainers.Where(a => a.Name == selectedMap).First()) - 1;
                                selectedMap = mapContainers[index].Name;
                                mapChanged = true;
                                confirm.Play();
                                resetReadyForInput();
                            }
                            break;
                        case 1:
                            if (unlockedMaps.Exists(a => a.Name == selectedMap))
                            {
                                confirm.Play();
                                return selectedMap;
                            }
                            else
                            {
                                denied.Play();
                            }
                            resetReadyForInput();
                            break;
                        case 2:
                            if (selectedMap != mapContainers.Last().Name)
                            {
                                int index = mapContainers.IndexOf(mapContainers.Where(a => a.Name == selectedMap).First()) + 1;
                                selectedMap = mapContainers[index].Name;
                                mapChanged = true;
                                confirm.Play();
                                resetReadyForInput();
                            }
                            break;
                        case 3:
                            confirm.Play();
                            return "menu";
                    }

                    if (mapChanged)
                    {
                        foreach (var component in Components)
                        {
                            if (component.Value is MapWindow)
                            {
                                if (component.Key == string.Concat("wnd", selectedMap))
                                {
                                    ((MapWindow)component.Value).Visible = true;
                                }
                                else
                                {
                                    ((MapWindow)component.Value).Visible = false;
                                }
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

        private void setUnlockedMaps()
        {
            unlockedMaps = new List<MapContainer>();

            foreach (MapContainer map in mapContainers)
            {
                bool canAdd = false;
                switch (map.UnlockRequirement.MapUnlockType)
                {
                    case MapUnlockType.None:
                        canAdd = true;
                        break;
                    case MapUnlockType.MapBestTime:
                        var levelProgression = progressionContainer.LevelProgressions.Where(a => a.Name == map.UnlockRequirement.RequirementText).FirstOrDefault();
                        canAdd = levelProgression != null && levelProgression.BestTime >= map.UnlockRequirement.RequirementAmount;
                        break;
                }

                MapWindow mapWindow = (MapWindow)Components[string.Concat("wnd", map.Name)];
                mapWindow.Components.Clear();

                mapWindow.Components.Add(
                    "lblTitle",
                    new Label(
                        "lblTitle",
                        fonts["Font"],
                        map.Name,
                        new Vector2(graphics.GetWidthOffset(10.66f), graphics.GetHeightOffset(2) - 64),
                        Color.White
                    )
                );

                mapWindow.Components.Add(
                    "pctMap",
                    new Picture(
                        "pctMap",
                        textures[map.Name],
                        new Vector2(graphics.GetWidthOffset(10.66f) + 48, graphics.GetHeightOffset(2)),
                        new Rectangle(0, 0, 64, 64),
                        new Vector2(32, 32)
                    )
                );

                if (canAdd)
                {
                    unlockedMaps.Add(map);

                    List<string> descriptionLines = new List<string>();
                    if (map.Description.Length < descriptionLength)
                    {
                        descriptionLines.Add(map.Description);
                    }
                    else
                    {
                        int startCharacter = 0;
                        do
                        {
                            int nextSpace = map.Description.LastIndexOf(' ', startCharacter + descriptionLength, descriptionLength);
                            descriptionLines.Add(map.Description.Substring(startCharacter, nextSpace - startCharacter));
                            startCharacter = nextSpace + 1;
                        } while (map.Description.Substring(startCharacter).Length > descriptionLength);
                        descriptionLines.Add(map.Description.Substring(startCharacter));
                    }

                    int descriptionOffsetY = -64;

                    foreach (var descriptionLine in descriptionLines)
                    {
                        mapWindow.Components.Add(
                            string.Concat("lblDescription", descriptionOffsetY),
                            new Label(
                                string.Concat("lblDescription", descriptionOffsetY),
                                fonts["FontSmall"],
                                descriptionLine,
                                new Vector2(graphics.GetWidthOffset(10.66f) + 125, graphics.GetHeightOffset(2) + descriptionOffsetY),
                                Color.White
                            )
                        );

                        descriptionOffsetY += 12;
                    }

                    descriptionOffsetY += 12;

                    mapWindow.Components.Add(
                        "lblBestTime",
                        new Label(
                            "lblBestTime",
                            fonts["FontSmall"],
                            string.Concat("Best Time: ", (progressionContainer.LevelProgressions.Where(a => a.Name == map.Name).FirstOrDefault()?.BestTime ?? 0).ToFormattedTime()),
                            new Vector2(graphics.GetWidthOffset(10.66f) + 125, graphics.GetHeightOffset(2) + descriptionOffsetY),
                            Color.White
                        )
                    );
                }
                else
                {
                    mapWindow.Components.Add(
                        "lblLocked",
                        new Label(
                            "lblLocked",
                            fonts["FontSmall"],
                            "Locked",
                            new Vector2(graphics.GetWidthOffset(10.66f) + 125, graphics.GetHeightOffset(2) - 64),
                            Color.White
                        )
                    );
                }
            }
        }
    }
}
