using Arch.Core;
using Arch.Core.Extensions;
using Box2D.NetStandard.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Helpers;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace RogueliteSurvivor.Systems
{
    public class RenderHudSystem : ArchSystem, IRenderSystem, IUpdateSystem
    {
        QueryDescription mapQuery = new QueryDescription()
                                            .WithAll<MapInfo>();

        GraphicsDeviceManager graphics;
        Dictionary<string, SpriteFont> fonts;
        StatPage statPage = StatPage.Spells;
        int statPageInt = 0;
        float stateChangeTime = .11f;
        public RenderHudSystem(World world, GraphicsDeviceManager graphics, Dictionary<string, SpriteFont> fonts)
            : base(world, new QueryDescription()
                                .WithAll<Player>())
        {
            this.graphics = graphics;
            this.fonts = fonts;
        }

        public void Update(GameTime gameTime, float totalElapsedTime, float scaleFactor)
        {
            if (stateChangeTime > InputConstants.ResponseTime)
            {
                KeyboardState kState = Keyboard.GetState();
                GamePadState gState = GamePad.GetState(PlayerIndex.One);

                if (kState.IsKeyDown(Keys.Tab) || gState.Buttons.Y == ButtonState.Pressed)
                {
                    incrementStatPageInt();

                    Entity? player = null;
                    world.Query(in query, (in Entity entity) =>
                    {
                        player = entity;
                    });

                    if(player.HasValue)
                    {
                        if (statPage == StatPage.Spell2 && !player.Value.Has<Spell2>())
                        {
                            incrementStatPageInt();
                        }
                        if (statPage == StatPage.Spell3 && !player.Value.Has<Spell3>())
                        {
                            incrementStatPageInt();
                        }
                    }
                    
                    stateChangeTime = 0f;
                }
            }
            else
            {
                stateChangeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        private void incrementStatPageInt()
        {
            statPageInt = (statPageInt + 1) % System.Enum.GetValues(typeof(StatPage)).Length;
            statPage = (StatPage)statPageInt;
        }

        public void Render(GameTime gameTime, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, Entity player, float totalElapsedTime, GameState gameState, int layer, float scaleFactor)
        {
            if (layer == 4)
            {
                Map? map = null;
                world.Query(in mapQuery, (ref Map mapInfo) =>
                {
                    if (map == null)
                    {
                        map = mapInfo;
                    }
                });

                world.Query(in query, (in Entity entity, ref Health health, ref KillCount killCount, ref AttackSpeed attackSpeed, ref Speed speed
                    , ref SpellDamage spellDamage, ref SpellEffectChance spellEffectChance, ref Pierce pierce, ref AreaOfEffect areaOfEffect, ref Player playerInfo) =>
                {

                    spriteBatch.Draw(
                        textures["Background"],
                        new Vector2(0, 315),
                        new Rectangle(0, 0, textures["Background"].Width, textures["Background"].Height),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0
                    );

                    spriteBatch.Draw(
                        textures["HealthBar"],
                        new Vector2(4, 319),
                        new Rectangle(0, 0, (int)(textures["HealthBar"].Width * ((float)health.Current / health.Max)), textures["HealthBar"].Height),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0
                    );

                    spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat("Health: ", health.Current, " / ", health.Max),
                        new Vector2(9, 321),
                        player.Has<Invincibility>() ? Color.Aqua : Color.White
                    );

                    spriteBatch.Draw(
                        textures["ExperienceBar"],
                        new Vector2(4, 342),
                        new Rectangle(0, 0, (int)(textures["ExperienceBar"].Width * ((float)(playerInfo.ExperienceRequiredForNextLevel - playerInfo.ExperienceToNextLevel) / playerInfo.ExperienceRequiredForNextLevel)), textures["ExperienceBar"].Height),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0
                    );

                    spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat("Level: ", playerInfo.Level, "  (", (playerInfo.ExperienceRequiredForNextLevel - playerInfo.ExperienceToNextLevel), " / ", playerInfo.ExperienceRequiredForNextLevel, ")"),
                        new Vector2(9, 344),
                        player.Has<DoubleExperience>() ? Color.Aqua : Color.White
                    );

                    switch (statPage)
                    {
                        case StatPage.Spells:
                            renderMainSpellStats(spriteBatch, entity, attackSpeed, speed, spellDamage, spellEffectChance, pierce, areaOfEffect);
                            break;
                        case StatPage.Spell1:
                            if (entity.TryGet(out Spell1 spell1))
                            {
                                renderSpellStats(spriteBatch, entity, spell1, pierce, areaOfEffect);
                                spriteBatch.Draw(
                                    textures["SpellsHud"],
                                    new Vector2(129, 319),
                                    getSpellHudSourceRectangle(Spells.Blank),
                                    Color.White,
                                    0f,
                                    Vector2.Zero,
                                    1f,
                                    SpriteEffects.None,
                                    0
                                );
                            }
                            break;
                        case StatPage.Spell2:
                            if (entity.TryGet(out Spell2 spell2))
                            {
                                renderSpellStats(spriteBatch, entity, spell2, pierce, areaOfEffect);
                                spriteBatch.Draw(
                                    textures["SpellsHud"],
                                    new Vector2(175, 319),
                                    getSpellHudSourceRectangle(Spells.Blank),
                                    Color.White,
                                    0f,
                                    Vector2.Zero,
                                    1f,
                                    SpriteEffects.None,
                                    0
                                );
                            }
                            break;
                        case StatPage.Spell3:
                            if (entity.TryGet(out Spell3 spell3))
                            {
                                renderSpellStats(spriteBatch, entity, spell3, pierce, areaOfEffect);
                                spriteBatch.Draw(
                                    textures["SpellsHud"],
                                    new Vector2(221, 319),
                                    getSpellHudSourceRectangle(Spells.Blank),
                                    Color.White,
                                    0f,
                                    Vector2.Zero,
                                    1f,
                                    SpriteEffects.None,
                                    0
                                );
                            }
                            break;
                    }

                    spriteBatch.Draw(
                        textures["SpellsHud"],
                        new Vector2(129, 319),
                        getSpellHudSourceRectangle(entity.Get<Spell1>().Spell),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0
                    );

                    spriteBatch.Draw(
                        textures["SpellsHud"],
                        new Vector2(175, 319),
                        getSpellHudSourceRectangle(entity.Has<Spell2>() ? entity.Get<Spell2>().Spell : Spells.Five),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0
                    );

                    spriteBatch.Draw(
                        textures["SpellsHud"],
                        new Vector2(221, 319),
                        getSpellHudSourceRectangle(entity.Has<Spell3>() ? entity.Get<Spell3>().Spell : Spells.Ten),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0
                    );

                    spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat(map.Value.Name, " - ", totalElapsedTime.ToFormattedTime()),
                        new Vector2(501, 321),
                        Color.White
                    );

                    spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat("Enemies Killed: ", killCount.Count),
                        new Vector2(501, 333),
                        Color.White
                    );

                    renderPowerupTimers(spriteBatch, textures, entity);
                });
            }
        }

        private void renderPowerupTimers(SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, Entity player)
        {
            Vector2 position = new Vector2(9, 306);
            
            if (player.TryGet(out Invincibility invincibility))
            {
                renderPowerupTimer(spriteBatch, 
                    textures, 
                    PickupHelper.GetPickupSourceRectangle(PickupType.Invincibility),
                    position,
                    invincibility.TimeRemaining, 
                    invincibility.MaxTime
                );

                position += Vector2.UnitX * 20;
            }
            if (player.TryGet(out DoubleExperience doubleExperience))
            {
                renderPowerupTimer(spriteBatch,
                    textures,
                    PickupHelper.GetPickupSourceRectangle(PickupType.DoubleExperience),
                    position,
                    doubleExperience.TimeRemaining,
                    doubleExperience.MaxTime
                );

                position += Vector2.UnitX * 20;
            }
            if (player.TryGet(out DoubleDamage doubleDamage))
            {
                renderPowerupTimer(spriteBatch,
                    textures,
                    PickupHelper.GetPickupSourceRectangle(PickupType.DoubleDamage),
                    position,
                    doubleDamage.TimeRemaining,
                    doubleDamage.MaxTime
                );

                position += Vector2.UnitX * 20;
            }
            if (player.TryGet(out DoubleAttackSpeed doubleAttackSpeed))
            {
                renderPowerupTimer(spriteBatch,
                    textures,
                    PickupHelper.GetPickupSourceRectangle(PickupType.DoubleAttackSpeed),
                    position,
                    doubleAttackSpeed.TimeRemaining,
                    doubleAttackSpeed.MaxTime
                );
            }
        }

        private void renderPowerupTimer(SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, Rectangle pickupSource, Vector2 position, float timeLeft, float maxTime)
        {
            spriteBatch.Draw(
                textures["PickupOverlay"],
                position,
                new Rectangle(0, 0, 18, 18),
                Color.White,
                0f,
                new Vector2(9, 9),
                1f,
                SpriteEffects.None,
                .1f
            );

            spriteBatch.Draw(
                textures["pickups"],
                position,
                pickupSource,
                Color.White,
                0f,
                new Vector2(8, 8),
                1f,
                SpriteEffects.None,
                .1f
            );

            spriteBatch.Draw(
                textures["PickupOverlay"],
                position + Vector2.UnitY * (18 - (18 * timeLeft / maxTime)),
                new Rectangle(18, 0, 18, (int)(18 * timeLeft / maxTime)),
                Color.White,
                0f,
                new Vector2(9, 9),
                1f,
                SpriteEffects.None,
                .1f
            );

            spriteBatch.DrawString(
                fonts["FontSmall"],
                (timeLeft + 1).ToFormattedSeconds(),
                position - Vector2.One * 5,
                Color.White
            );
        }

        private void renderMainSpellStats(SpriteBatch spriteBatch, Entity player, AttackSpeed attackSpeed, Speed speed, SpellDamage spellDamage, SpellEffectChance spellEffectChance, Pierce pierce, AreaOfEffect areaOfEffect)
        {

            spriteBatch.DrawString(
                fonts["FontSmall"],
                string.Concat("Attack Speed: ", (player.Has<DoubleAttackSpeed>() ? 2 * attackSpeed.CurrentAttackSpeed : attackSpeed.CurrentAttackSpeed).ToString("F"), "x"),
                new Vector2(269, 321),
                player.Has<DoubleAttackSpeed>() ? Color.Aqua : Color.White
            );

            spriteBatch.DrawString(
                fonts["FontSmall"],
                string.Concat("Spell Damage: ", (player.Has<DoubleDamage>() ? 2 * spellDamage.CurrentSpellDamage : spellDamage.CurrentSpellDamage).ToString("F"), "x"),
                new Vector2(269, 333),
                player.Has<DoubleDamage>() ? Color.Aqua : Color.White
            );

            spriteBatch.DrawString(
                fonts["FontSmall"],
                string.Concat("Spell Effect Chance: ", spellEffectChance.CurrentSpellEffectChance.ToString("F"), "x"),
                new Vector2(269, 345),
                Color.White
            );

            spriteBatch.DrawString(
                fonts["FontSmall"],
                string.Concat("Move Speed: ", speed.speed),
                new Vector2(400, 321),
                Color.White
            );

            spriteBatch.DrawString(
                fonts["FontSmall"],
                string.Concat("Pierce: ", pierce.Num),
                new Vector2(400, 333),
                Color.White
            );

            spriteBatch.DrawString(
                fonts["FontSmall"],
                string.Concat("Area of Effect: ", areaOfEffect.Radius, "x"),
                new Vector2(400, 345),
                Color.White
            );
        }
    
        private void renderSpellStats(SpriteBatch spriteBatch, Entity player, ISpell spell, Pierce pierce, AreaOfEffect areaOfEffect)
        {
            spriteBatch.DrawString(
                fonts["FontSmall"],
                string.Concat("Attack Speed: ", (player.Has<DoubleAttackSpeed>() ? 2 * spell.CurrentAttacksPerSecond : spell.CurrentAttacksPerSecond).ToString("F"), "/s"),
                new Vector2(269, 321),
                player.Has<DoubleAttackSpeed>() ? Color.Aqua : Color.White
            );

            spriteBatch.DrawString(
                fonts["FontSmall"],
                string.Concat("Spell Damage: ", (player.Has<DoubleDamage>() ? 2 * spell.CurrentDamage : spell.CurrentDamage).ToString("F")),
                new Vector2(269, 333),
                player.Has<DoubleDamage>() ? Color.Aqua : Color.White
            );

            spriteBatch.DrawString(
                fonts["FontSmall"],
                string.Concat("Spell Effect Chance: ", string.Format("{0:P0}", spell.CurrentEffectChance)),
                new Vector2(269, 345),
                Color.White
            );

            if(spell.Type == SpellType.Projectile)
            {
                spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Pierce: ", pierce.Num),
                    new Vector2(400, 321),
                    Color.White
                );
            }
            else if(spell.Type == SpellType.SingleTarget || spell.Type == SpellType.Aura)
            {
                spriteBatch.DrawString(
                    fonts["FontSmall"],
                    string.Concat("Area of Effect: ", areaOfEffect.Radius, "x"),
                    new Vector2(400, 321),
                    Color.White
                );
            }
        }
    
        private Rectangle getSpellHudSourceRectangle(Spells spell)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 37;
            rectangle.Height = 37;

            switch (spell)
            {
                case Spells.FireAura:
                    rectangle.X = 0;
                    rectangle.Y = 0;
                    break;
                case Spells.FireExplosion:
                    rectangle.X = 37;
                    rectangle.Y = 0;
                    break;
                case Spells.Fireball:
                    rectangle.X = 0;
                    rectangle.Y = 37;
                    break;
                case Spells.IceAura:
                    rectangle.X = 37;
                    rectangle.Y = 37;
                    break;
                case Spells.IceShard:
                    rectangle.X = 0;
                    rectangle.Y = 74;
                    break;
                case Spells.IceSpikes:
                    rectangle.X = 37;
                    rectangle.Y = 74;
                    break;
                case Spells.LightningAura:
                    rectangle.X = 0;
                    rectangle.Y = 111;
                    break;
                case Spells.LightningBlast:
                    rectangle.X = 37;
                    rectangle.Y = 111;
                    break;
                case Spells.LightningStrike:
                    rectangle.X = 0;
                    rectangle.Y = 148;
                    break;
                case Spells.MagicShot:
                    rectangle.X = 37;
                    rectangle.Y = 148;
                    break;
                case Spells.Five:
                    rectangle.X = 0;
                    rectangle.Y = 185;
                    break;
                case Spells.Ten:
                    rectangle.X = 37;
                    rectangle.Y = 185;
                    break;
                case Spells.Blank:
                    rectangle.X = 0;
                    rectangle.Y = 222;
                    break;
            }

            return rectangle;
        }
    }
}