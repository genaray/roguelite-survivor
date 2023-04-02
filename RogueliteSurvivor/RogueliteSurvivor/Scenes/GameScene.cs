﻿using Arch.Core;
using Arch.Core.Extensions;
using Box2D.NetStandard.Dynamics.Bodies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json.Linq;
using RogueliteSurvivor.ComponentFactories;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Helpers;
using RogueliteSurvivor.Physics;
using RogueliteSurvivor.Scenes.SceneComponents;
using RogueliteSurvivor.Scenes.Windows;
using RogueliteSurvivor.Systems;
using RogueliteSurvivor.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;

namespace RogueliteSurvivor.Scenes
{
    public class GameScene : Scene
    {

        private List<IUpdateSystem> updateSystems;
        private List<IRenderSystem> renderSystems;
        private Entity player;

        private World world;
        private Box2D.NetStandard.Dynamics.World.World physicsWorld;
        private System.Numerics.Vector2 gravity = System.Numerics.Vector2.Zero;

        private Dictionary<string, Texture2D> textures;
        private Dictionary<string, SpriteFont> fonts;
        private Dictionary<string, Song> songs = null;
        private Dictionary<string, SoundEffect> soundEffects = null;

        private float totalGameTime = 0f;

        private GameState gameState;
        private float stateChangeTime = .11f;
        private GameSettings gameSettings;

        private Dictionary<string, EnemyContainer> enemyContainers;
        private Dictionary<Spells, SpellContainer> spellContainers;
        private Dictionary<string, PlayerContainer> playerContainers;
        private Dictionary<string, MapContainer> mapContainers;

        private MapContainer mapContainer;
        
        private Random random;
        private List<LevelUpType> levelUpChoices = new List<LevelUpType>();
        private List<Button> levelUpButtons;
        private LevelUpType selectedLevelUpChoice;

        private Dictionary<string, Window> subScenes;

        private GameContactListener gameContactListener;

        public GameScene(SpriteBatch spriteBatch, ContentManager contentManager, GraphicsDeviceManager graphics, Dictionary<string, PlayerContainer> playerContainers, Dictionary<string, MapContainer> mapContainers, ProgressionContainer progressionContainer, Dictionary<string, EnemyContainer> enemyContainers, SettingsContainer settingsContainer)
            : base(spriteBatch, contentManager, graphics, progressionContainer, settingsContainer)
        {
            this.playerContainers = playerContainers;
            this.mapContainers = mapContainers;
            this.enemyContainers = enemyContainers;

            world = World.Create();
            physicsWorld = new Box2D.NetStandard.Dynamics.World.World(gravity);
            gameContactListener = new GameContactListener();
            physicsWorld.SetContactListener(gameContactListener);
            physicsWorld.SetContactFilter(new GameContactFilter());

            random = new Random();
        }

        public void SetGameSettings(GameSettings gameSettings)
        {
            this.gameSettings = gameSettings;
        }

        public GameStats GetGameStats()
        {
            var stats = new GameStats();

            KillCount killCount = (KillCount)player.Get(typeof(KillCount));


            stats.EnemiesKilled = killCount.Count;
            string killerName = killCount.KillerName;
            if (!string.IsNullOrEmpty(killerName))
            {
                stats.Killer = enemyContainers[killerName].ReadableName;
                stats.KillerMethod = killCount.KillerMethod;
            }
            else
            {
                stats.Killer = "Nobody";
                stats.KillerMethod = "pummeled";
            }
            stats.PlayTime = totalGameTime;
            stats.Kills = new Dictionary<string, int>();
            foreach(var enemy in killCount.Kills)
            {
                stats.Kills.Add(enemyContainers[enemy.Key].ReadableName, enemy.Value);
            }
            return stats;
        }

        public override void LoadContent()
        {
            resetWorld();
            loadTexturesAndFonts();
            loadSoundEffects();
            loadSubscenes();

            loadMap();
            loadSpells();
            initializeSystems();
            placePlayer();

            totalGameTime = 0;
            gameState = GameState.Running;

            Loaded = true;
        }

        private void loadTexturesAndFonts()
        {
            textures = new Dictionary<string, Texture2D>
            {
                { "Fireball", Content.Load<Texture2D>(Path.Combine("Spells", "fireball")) },
                { "IceShard", Content.Load<Texture2D>(Path.Combine("Spells", "ice-shard")) },
                { "LightningBlast", Content.Load<Texture2D>(Path.Combine("Spells", "lightning-blast")) },
                { "FireExplosion", Content.Load<Texture2D>(Path.Combine("Spells", "fire-explosion")) },
                { "IceSpikes", Content.Load<Texture2D>(Path.Combine("Spells", "ice-spikes")) },
                { "LightningStrike", Content.Load<Texture2D>(Path.Combine("Spells", "lightning-strike")) },
                { "FireAura", Content.Load<Texture2D>(Path.Combine("Spells", "fire-aura")) },
                { "IceAura", Content.Load<Texture2D>(Path.Combine("Spells", "ice-aura")) },
                { "LightningAura", Content.Load<Texture2D>(Path.Combine("Spells", "lightning-aura")) },
                { "MagicShot", Content.Load<Texture2D>(Path.Combine("Spells", "magic-shot")) },
                { "EnemyEnergyBlast", Content.Load<Texture2D>(Path.Combine("Spells", "enemy-energy-blast")) },

                { "StatBar", Content.Load<Texture2D>(Path.Combine("Hud", "StatBar")) },
                { "HealthBar", Content.Load<Texture2D>(Path.Combine("Hud", "HealthBar")) },
                { "ExperienceBar", Content.Load<Texture2D>(Path.Combine("Hud", "ExperienceBar")) },
                { "StatsBackground", Content.Load<Texture2D>(Path.Combine("Hud", "StatsBackground")) },
                { "Background", Content.Load<Texture2D>(Path.Combine("Hud", "Background")) },
                { "SpellsHud", Content.Load<Texture2D>(Path.Combine("Hud", "SpellsHud")) },
                { "PickupOverlay", Content.Load<Texture2D>(Path.Combine("Hud", "PickupOverlay")) },

                { "LevelUpChoices", Content.Load<Texture2D>(Path.Combine("UI", "level-up-buttons")) },
                { "MainMenuButtons", Content.Load<Texture2D>(Path.Combine("UI", "main-menu-buttons")) },
                { "VolumeButtons", Content.Load<Texture2D>(Path.Combine("UI", "volume-buttons")) },
                { "InGameMenuWindow", Content.Load<Texture2D>(Path.Combine("UI", "in-game-menu-window")) },
                { "InGameOptionsWindow", Content.Load<Texture2D>(Path.Combine("UI", "in-game-options-window")) },

                { "pickups", Content.Load<Texture2D>(Path.Combine("Pickups", "player-pickups")) },

                { "MiniBlood1", Content.Load<Texture2D>(Path.Combine("Effects", "mini-blood-1")) },
                { "MiniBlood2", Content.Load<Texture2D>(Path.Combine("Effects", "mini-blood-2")) },
                { "MiniBlood3", Content.Load<Texture2D>(Path.Combine("Effects", "mini-blood-3")) },
                { "MiniBlood4", Content.Load<Texture2D>(Path.Combine("Effects", "mini-blood-4")) },
                { "MiniBlood5", Content.Load<Texture2D>(Path.Combine("Effects", "mini-blood-5")) },
                { "MiniBlood6", Content.Load<Texture2D>(Path.Combine("Effects", "mini-blood-6")) },
                { "MiniBlood7", Content.Load<Texture2D>(Path.Combine("Effects", "mini-blood-7")) },
                { "MiniBlood8", Content.Load<Texture2D>(Path.Combine("Effects", "mini-blood-8")) },
                { "MiniBlood9", Content.Load<Texture2D>(Path.Combine("Effects", "mini-blood-9")) },
                { "Blood1", Content.Load<Texture2D>(Path.Combine("Effects", "blood-1")) },
                { "Blood2", Content.Load<Texture2D>(Path.Combine("Effects", "blood-2")) },
                { "Blood3", Content.Load<Texture2D>(Path.Combine("Effects", "blood-3")) },
                { "Blood4", Content.Load<Texture2D>(Path.Combine("Effects", "blood-4")) },
                { "Blood5", Content.Load<Texture2D>(Path.Combine("Effects", "blood-5")) },

                { "IceShardHit", Content.Load<Texture2D>(Path.Combine("Effects", "ice-shard-hit")) },
                { "LightningBlastHit", Content.Load<Texture2D>(Path.Combine("Effects", "lightning-blast-hit")) },
                { "FireballHit", Content.Load<Texture2D>(Path.Combine("Effects", "fireball-hit")) },
                { "MagicShotHit", Content.Load<Texture2D>(Path.Combine("Effects", "magic-shot-hit")) },
                { "EnemyEnergyBlastHit", Content.Load<Texture2D>(Path.Combine("Effects", "enemy-energy-blast-hit")) },
            };

            fonts = new Dictionary<string, SpriteFont>()
            {
                { "Font", Content.Load<SpriteFont>(Path.Combine("Fonts", "Font")) },
                { "FontSmall", Content.Load<SpriteFont>(Path.Combine("Fonts", "FontSmall")) },
            };
        }

        private void loadSoundEffects()
        {
            if(soundEffects == null)
            {
                soundEffects = new Dictionary<string, SoundEffect>()
                {
                    { "LevelUp", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "02_Heal_02")) },
                    { "EnemyMelee1", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "03_Claw_03")) },
                    { "FireballHit", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "04_Fire_explosion_04_medium")) },
                    { "EnemyMelee2", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "08_Bite_04")) },
                    { "IceSpikeHit", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "13_Ice_explosion_01")) },
                    { "LightningStrikeHit", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "18_Thunder_02")) },
                    { "ProjectileCast1", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "22_Slash_04")) },
                    { "MagicShotHit", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "22_Water_02")) },
                    { "LightningBlastHit", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "25_Wind_01")) },
                    { "ProjectileCast2", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "35_Miss_Evade_02")) },
                    { "IceShardHit", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "45_Charge_05")) },
                    { "EnemyDeath1", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "69_Enemy_death_01")) },
                    { "EnemyDeath2", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "77_flesh_02")) },
                    { "Hover", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "001_Hover_01")) },
                    { "Confirm", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "013_Confirm_03")) },
                    { "Pickup", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "029_Decline_09")) },
                    { "Denied", Content.Load<SoundEffect>(Path.Combine("Sound Effects", "033_Denied_03")) },
                };

                gameContactListener.SetSoundEffects(soundEffects);
            }
        }

        private void loadSpells()
        {
            JObject spells = JObject.Parse(File.ReadAllText(Path.Combine(Content.RootDirectory, "Datasets", "spells.json")));
            spellContainers = new Dictionary<Spells, SpellContainer>();

            foreach (var spell in spells["data"])
            {
                spellContainers.Add(
                    SpellContainer.SpellContainerName(spell),
                    SpellContainer.ToSpellContainer(spell)
                );
            }
        }

        private void loadSubscenes()
        {
            subScenes = new Dictionary<string, Window>()
            {
                { 
                    "InGameMenuWindow", 
                    new InGameMenuWindow
                    (
                        textures["InGameMenuWindow"], 
                        new Vector2(GetWidthOffset(2), GetHeightOffset(2)), 
                        new List<IFormComponent>()
                        {
                            new Button(
                                textures["MainMenuButtons"],
                                new Vector2(GetWidthOffset(2), GetHeightOffset(2) - 72),
                                new Rectangle(0, 448, 128, 32),
                                new Rectangle(128, 448, 128, 32),
                                new Vector2(64, 16)
                            ),
                            new Button(
                                textures["MainMenuButtons"],
                                new Vector2(GetWidthOffset(2), GetHeightOffset(2) - 24),
                                new Rectangle(0, 160, 128, 32),
                                new Rectangle(128, 160, 128, 32),
                                new Vector2(64, 16)
                            ),
                            new Button(
                                textures["MainMenuButtons"],
                                new Vector2(GetWidthOffset(2), GetHeightOffset(2) + 24),
                                new Rectangle(0, 384, 128, 32),
                                new Rectangle(128, 384, 128, 32),
                                new Vector2(64, 16)
                            ),
                            new Button(
                                textures["MainMenuButtons"],
                                new Vector2(GetWidthOffset(2), GetHeightOffset(2) + 72),
                                new Rectangle(0, 192, 128, 32),
                                new Rectangle(128, 192, 128, 32),
                                new Vector2(64, 16)
                            ),
                        },
                        soundEffects["Hover"],
                        soundEffects["Confirm"]
                    ) 
                },
                {
                    "InGameOptionsWindow",
                    new InGameOptionsMenuWindow
                    (
                        textures["InGameOptionsWindow"],
                        new Vector2(GetWidthOffset(2), GetHeightOffset(2)),
                        new List<IFormComponent>()
                        {
                            new Button(textures["VolumeButtons"],
                                new Vector2(GetWidthOffset(2) + 85, GetHeightOffset(2) - 64),
                                new Rectangle(0, 0, 16, 16),
                                new Rectangle(16, 0, 16, 16),
                                new Vector2(8, 8)
                            ),
                            new Button(textures["VolumeButtons"],
                                new Vector2(GetWidthOffset(2) + 109, GetHeightOffset(2) - 64),
                                new Rectangle(0, 16, 16, 16),
                                new Rectangle(16, 16, 16, 16),
                                new Vector2(8, 8)
                            ),
                            new Button(textures["VolumeButtons"],
                                new Vector2(GetWidthOffset(2) + 85, GetHeightOffset(2) - 32),
                                new Rectangle(0, 0, 16, 16),
                                new Rectangle(16, 0, 16, 16),
                                new Vector2(8, 8)
                            ),
                            new Button(textures["VolumeButtons"],
                                new Vector2(GetWidthOffset(2) + 109, GetHeightOffset(2) - 32),
                                new Rectangle(0, 16, 16, 16),
                                new Rectangle(16, 16, 16, 16),
                                new Vector2(8, 8)
                            ),
                            new Button(textures["VolumeButtons"],
                                new Vector2(GetWidthOffset(2) + 85, GetHeightOffset(2)),
                                new Rectangle(0, 0, 16, 16),
                                new Rectangle(16, 0, 16, 16),
                                new Vector2(8, 8)
                            ),
                            new Button(textures["VolumeButtons"],
                                new Vector2(GetWidthOffset(2) + 109, GetHeightOffset(2)),
                                new Rectangle(0, 16, 16, 16),
                                new Rectangle(16, 16, 16, 16),
                                new Vector2(8, 8)
                            ),
                            new Button(textures["VolumeButtons"],
                                new Vector2(GetWidthOffset(2) + 85, GetHeightOffset(2) + 32),
                                new Rectangle(0, 0, 16, 16),
                                new Rectangle(16, 0, 16, 16),
                                new Vector2(8, 8)
                            ),
                            new Button(textures["VolumeButtons"],
                                new Vector2(GetWidthOffset(2) + 109, GetHeightOffset(2) + 32),
                                new Rectangle(0, 16, 16, 16),
                                new Rectangle(16, 16, 16, 16),
                                new Vector2(8, 8)
                            ),
                            new Button(
                                textures["MainMenuButtons"],
                                new Vector2(GetWidthOffset(2), GetHeightOffset(2) + 64),
                                new Rectangle(0, 0, 128, 32),
                                new Rectangle(128, 0, 128, 32),
                                new Vector2(64, 16)
                            ),
                        },
                        soundEffects["Hover"],
                        soundEffects["Confirm"],
                        fonts,
                        settingsContainer
                    )
                }
            };
        }

        private void resetWorld()
        {
            if (world.CountEntities(new QueryDescription()) > 0)
            {
                List<Entity> entities = new List<Entity>();
                world.GetEntities(new QueryDescription(), entities);
                foreach (var entity in entities)
                {
                    world.Destroy(entity);
                }
            }

            if (physicsWorld.GetBodyCount() > 0)
            {
                var physicsBody = physicsWorld.GetBodyList();
                while (physicsBody != null)
                {
                    var nextPhysicsBody = physicsBody.GetNext();
                    physicsWorld.DestroyBody(physicsBody);
                    physicsBody = nextPhysicsBody;
                };
            }
        }

        private void initializeSystems()
        {
            var renderHud = new RenderHudSystem(world, _graphics, fonts);

            updateSystems = new List<IUpdateSystem>
            {
                new PlayerInputSystem(world),
                new TargetingSystem(world),
                new EnemyAISystem(world),
                new AnimationSetSystem(world),
                new AnimationUpdateSystem(world),
                new CollisionSystem(world, physicsWorld, soundEffects),
                new AttackSystem(world, textures, physicsWorld, spellContainers, soundEffects),
                new SpellEffectSystem(world),
                new PickupSystem(world, soundEffects),
                new EnemySpawnSystem(world, textures, physicsWorld, _graphics, enemyContainers, spellContainers, mapContainer),
                new AttackSpellCleanupSystem(world),
                new DeathSystem(world, textures, physicsWorld, spellContainers, soundEffects),
                renderHud,
            };

            renderSystems = new List<IRenderSystem>
            {
                new RenderSpriteSystem(world, _graphics),
                new RenderPickupSystem(world, _graphics),
                new RenderMapSystem(world, _graphics),
                renderHud,
            };
        }

        private void loadMap()
        {
            mapContainer = mapContainers[gameSettings.MapName];

            var mapEntity = world.Create<Map, MapInfo>();
            mapEntity.Set(new Map() { Name = gameSettings.MapName }, new MapInfo(Path.Combine(Content.RootDirectory, "Maps", mapContainer.Folder, mapContainer.MapFilename), Path.Combine(Content.RootDirectory, "Maps", mapContainer.Folder), physicsWorld, mapEntity, mapContainer.Spawnables));

            foreach (var tilesetImage in mapContainer.TilesetImages)
            {
                textures.Add(tilesetImage.ToLower(), Content.Load<Texture2D>(Path.Combine("Maps", mapContainer.Folder, tilesetImage)));
            }

            foreach(var wave in mapContainer.EnemyWaves)
            {
                foreach(var enemy in wave.Enemies)
                {
                    if(!textures.ContainsKey(enemy.Type))
                    {
                        textures.Add(enemy.Type, Content.Load<Texture2D>(Path.Combine("Enemies", enemy.Type)));
                    }
                }
            }

            songs = new Dictionary<string, Song> { {"GameMusic", Content.Load<Song>(Path.Combine("Music", mapContainer.Music)) } };
        }

        private void placePlayer()
        {
            var playerContainer = playerContainers[gameSettings.PlayerName];
            textures.Add(playerContainer.Texture, Content.Load<Texture2D>(Path.Combine("Player", playerContainers[gameSettings.PlayerName].Texture)));

            var body = new BodyDef();
            body.position = new System.Numerics.Vector2(mapContainer.Start.X, mapContainer.Start.Y) / PhysicsConstants.PhysicsToPixelsRatio;
            body.fixedRotation = true;

            player = world.Create<Player, EntityStatus, Position, Velocity, Speed, AttackSpeed, SpellDamage, SpellEffectChance, Pierce, AreaOfEffect, Animation, SpriteSheet, Target, Spell1, Health, KillCount, Body>();
            Spell1 spell = SpellFactory.CreateSpell<Spell1>(spellContainers[playerContainer.StartingSpell]);

            player.Set(
                new Player() { Level = 1, ExperienceToNextLevel = ExperienceHelper.ExperienceRequiredForLevel(2), ExperienceRequiredForNextLevel = ExperienceHelper.ExperienceRequiredForLevel(2), TotalExperience = 0 },
                new EntityStatus(),
                new Position() { XY = new Vector2(mapContainer.Start.X, mapContainer.Start.Y) },
                new Velocity() { Vector = Vector2.Zero },
                new Speed() { speed = 100 * playerContainer.Speed },
                new AttackSpeed(1f + playerContainer.AttackSpeed),
                new SpellDamage(1f + playerContainer.SpellDamage),
                new SpellEffectChance(1f + playerContainer.SpellEffectChance),
                new Pierce(0 + playerContainer.Pierce),
                new AreaOfEffect(1f + playerContainer.AreaOfEffect),
                PlayerFactory.GetPlayerAnimation(playerContainer),
                PlayerFactory.GetPlayerSpriteSheet(playerContainer, textures),
                new Target(),
                spell,
                new Health() { Current = (int)(100 * playerContainer.Health)
                                , Max = (int)(100 * playerContainer.Health)
                                , BaseMax = (int)(100 * playerContainer.Health)
                },
                new KillCount(),
                BodyFactory.CreateCircularBody(player, 14, physicsWorld, body, 99)
            );

            if(spell.Type == SpellType.Aura)
            {
                var aura = SpellFactory.CreateAura(world, textures, physicsWorld, spellContainers, player, spell, spell.Effect);
                spell.Child = aura;
                player.Set(spell);
            }
            
            TraitsHelper.AddTraitsToEntity(player, playerContainer.Traits);

            LevelUpChoiceHelper.ProcessLevelUp(world, textures, physicsWorld, ref player, LevelUpType.None, spellContainers);
        }

        public override void SetActive()
        {
            MediaPlayer.Play(songs["GameMusic"]);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = settingsContainer.MasterVolume * settingsContainer.GameMusicVolume;
        }

        public override string Update(GameTime gameTime, params object[] values)
        {
            string retVal = string.Empty;
            var kState = Keyboard.GetState();
            var gState = GamePad.GetState(PlayerIndex.One);
            stateChangeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gameState == GameState.LevelUp)
            {
                if (stateChangeTime > InputConstants.ResponseTime)
                {
                    var mState = Mouse.GetState();
                    bool clicked = false;

                    if (mState.LeftButton == ButtonState.Pressed && levelUpButtons.Any(a => a.MouseOver()))
                    {
                        clicked = true;
                        selectedLevelUpChoice = levelUpChoices[levelUpButtons.IndexOf(levelUpButtons.First(a => a.MouseOver()))];
                    }

                    if (clicked || kState.IsKeyDown(Keys.Enter) || gState.Buttons.A == ButtonState.Pressed)
                    {
                        gameState = GameState.Running;
                        LevelUpChoiceHelper.ProcessLevelUp(world, textures, physicsWorld, ref player, selectedLevelUpChoice, spellContainers);
                        soundEffects["Confirm"].Play();
                        stateChangeTime = 0f;
                    }
                    else if (kState.IsKeyDown(Keys.Left) || gState.DPad.Left == ButtonState.Pressed || gState.ThumbSticks.Left.X < -0.5f)
                    {
                        if (selectedLevelUpChoice != levelUpChoices[0])
                        {
                            int index = levelUpChoices.IndexOf(levelUpChoices.Where(a => a == selectedLevelUpChoice).First()) - 1;
                            selectedLevelUpChoice = levelUpChoices[index];
                            soundEffects["Hover"].Play();
                            stateChangeTime = 0f;
                        }
                        
                    }
                    else if (kState.IsKeyDown(Keys.Right) || gState.DPad.Right == ButtonState.Pressed || gState.ThumbSticks.Left.X > 0.5f)
                    {
                        if (selectedLevelUpChoice != levelUpChoices.Last())
                        {
                            int index = levelUpChoices.IndexOf(levelUpChoices.Where(a => a == selectedLevelUpChoice).First()) + 1;
                            selectedLevelUpChoice = levelUpChoices[index];
                            soundEffects["Hover"].Play();
                            stateChangeTime = 0f;
                        }
                    }

                    for (int i = 1; i <= levelUpButtons.Count; i++)
                    {
                        levelUpButtons[i - 1].Selected(selectedLevelUpChoice == levelUpChoices[i - 1]);
                        levelUpButtons[i - 1].MouseOver(mState);
                    }
                }
            }
            else if(gameState == GameState.InGameMenu)
            {
                if (stateChangeTime > InputConstants.ResponseTime)
                {
                    switch (subScenes["InGameMenuWindow"].Update(gameTime))
                    {
                        case "continue":
                            gameState = GameState.Running;
                            stateChangeTime = 0f;
                            break;
                        case "options":
                            gameState = GameState.Options;
                            stateChangeTime = 0f;
                            break;
                        case "restart":
                            retVal = "loading";
                            stateChangeTime = 0f;
                            Loaded = false;
                            break;
                        case "game-over":
                            stateChangeTime = 0f;
                            Loaded = false;
                            retVal = "game-over";
                            break;
                    }
                }
            }
            else if(gameState == GameState.Options)
            {
                if (stateChangeTime > InputConstants.ResponseTime)
                {
                    switch (subScenes["InGameOptionsWindow"].Update(gameTime))
                    {
                        case "menu":
                            gameState = GameState.InGameMenu;
                            stateChangeTime = 0f;
                            break;
                    }
                }
            }
            else
            {
                if (stateChangeTime > InputConstants.ResponseTime && (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)))
                {
                    gameState = GameState.InGameMenu;
                    stateChangeTime = 0f;
                }
                else if (player.Get<EntityStatus>().State == State.Dead)
                {
                    Loaded = false;
                    retVal = "game-over";
                }
                else
                {
                    if (gameState == GameState.Running)
                    {
                        totalGameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        foreach (var system in updateSystems)
                        {
                            system.Update(gameTime, totalGameTime, Game1.ScaleFactor);
                        }
                    }
                    stateChangeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    Player playerInfo = player.Get<Player>();
                    if (playerInfo.ExperienceToNextLevel <= 0)
                    {
                        soundEffects["LevelUp"].Play();
                        stateChangeTime = 0f;
                        playerInfo.Level++;
                        playerInfo.ExperienceRequiredForNextLevel = ExperienceHelper.ExperienceRequiredForLevel(playerInfo.Level + 1);
                        playerInfo.ExperienceToNextLevel += playerInfo.ExperienceRequiredForNextLevel;
                        var playerHealth = player.Get<Health>();
                        playerHealth.Max += (int)(0.05f * playerHealth.BaseMax);
                        playerHealth.Current = playerHealth.Max;
                        
                        player.Set(playerInfo, playerHealth);

                        gameState = GameState.LevelUp;

                        RandomTable<LevelUpType> pickupTable = new RandomTable<LevelUpType>();
                        var spells = player.GetAllComponents().Where(a => a is ISpell).ToList();

                        if (playerInfo.Level % 5 == 0 && spells.Count < 3)
                        {
                            var playerUsableSpells = SpellsExtensions.PlayerUsableSpells();
                            playerUsableSpells.RemoveAll(a => spells.Exists(b => a == ((ISpell)b).Spell));

                            foreach(var usableSpell in playerUsableSpells)
                            {
                                pickupTable.Add(usableSpell.ToString().GetPickupTypeFromString(), 1);
                            }
                        }
                        else
                        {
                            pickupTable
                                .Add(LevelUpType.AttackSpeed, 10)
                                .Add(LevelUpType.Damage, 10)
                                .Add(LevelUpType.MoveSpeed, 3)
                                .Add(LevelUpType.SpellEffectChance, 1)
                                .Add(LevelUpType.Pierce, 1)
                                .Add(LevelUpType.AreaOfEffect, 1);
                        }
                        
                            

                        levelUpChoices.Clear();
                        int max = Math.Min(4, pickupTable.NumberOfEntries);
                        for(int i = 0; i < max; i++)
                        {
                            LevelUpType choice;
                            do
                            {
                                choice = pickupTable.Roll(random);
                            }while(levelUpChoices.Contains(choice));

                            levelUpChoices.Add(choice);
                        }
                        selectedLevelUpChoice = levelUpChoices[0];
                        levelUpButtons = new List<Button>();

                        int counter = -136;
                        foreach (var levelUpChoice in levelUpChoices)
                        {
                            levelUpButtons.Add(
                                new Button(
                                    textures["LevelUpChoices"],
                                    new Vector2(GetWidthOffset(2) + counter, GetHeightOffset(2) + 32),
                                    LevelUpChoiceHelper.GetLevelUpChoiceButton(levelUpChoice, false),
                                    LevelUpChoiceHelper.GetLevelUpChoiceButton(levelUpChoice, true),
                                    new Vector2(32, 32)
                                )
                            );

                            counter += 80;
                        }
                    }
                }
            }

            return retVal;
        }

        public override void Draw(GameTime gameTime, Matrix transformMatrix, params object[] values)
        {
            if (gameState != GameState.LevelUp)
            {
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: transformMatrix);

                for (int layer = 1; layer < 5; layer++)
                {
                    foreach (var system in renderSystems)
                    {
                        system.Render(gameTime, _spriteBatch, textures, player, totalGameTime, gameState, layer, Game1.ScaleFactor);
                    }
                }

                switch (gameState)
                {
                    case GameState.InGameMenu:
                        subScenes["InGameMenuWindow"].Draw(_spriteBatch);
                        break;
                    case GameState.Options:
                        subScenes["InGameOptionsWindow"].Draw(_spriteBatch);
                        break;
                }

                _spriteBatch.End();
            }
            else
            {
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: transformMatrix);

                _spriteBatch.DrawString(
                    fonts["Font"],
                    "Level Up! Select an upgrade:",
                    new Vector2(GetWidthOffset(2) - 116, GetHeightOffset(2) - 64),
                    Color.White
                );

                foreach(var button in levelUpButtons)
                {
                    button.Draw(_spriteBatch);
                }

                _spriteBatch.DrawString(
                    fonts["Font"],
                    LevelUpChoiceHelper.GetLevelUpDisplayTextForLevelUpChoice(selectedLevelUpChoice),
                    new Vector2(GetWidthOffset(2) - LevelUpChoiceHelper.GetLevelUpDisplayTextForLevelUpChoice(selectedLevelUpChoice).Length * 4.5f, GetHeightOffset(2) + 96),
                    Color.White
                );

                _spriteBatch.End();
            }
        }
    }
}
