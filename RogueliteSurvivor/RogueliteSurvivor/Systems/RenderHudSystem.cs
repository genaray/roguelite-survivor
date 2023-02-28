﻿using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using System.Collections.Generic;

namespace RogueliteSurvivor.Systems
{
    public class RenderHudSystem : ArchSystem, IRenderSystem
    {
        GraphicsDeviceManager graphics;
        Dictionary<string, SpriteFont> fonts;
        public RenderHudSystem(World world, GraphicsDeviceManager graphics, Dictionary<string, SpriteFont> fonts)
            : base(world, new QueryDescription()
                                .WithAll<Player>())
        {
            this.graphics = graphics;
            this.fonts = fonts;
        }

        static Vector2 HealthLocation = new Vector2(10, 10);
        static Vector2 TimeLocation = new Vector2(-100, 10);
        const int Increment = 100;

        public void Render(GameTime gameTime, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, Entity player, float totalElapsedTime, GameState gameState, int layer)
        {
            if (layer == 2)
            {
                int counter = 0;
                world.Query(in query, (in Entity entity, ref Health health, ref KillCount killCount, ref AttackSpeed attackSpeed, ref Speed speed, ref SpellDamage spellDamage, ref SpellEffectChance spellEffectChance) =>
                {
                    spriteBatch.Draw(
                        textures["HealthBar"],
                        HealthLocation + (Vector2.UnitY * Increment * counter),
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
                        string.Concat(health.Current, " / ", health.Max),
                        HealthLocation + (Vector2.UnitY * Increment * counter) + Vector2.UnitX * 5 + Vector2.UnitY * 2,
                        Color.White
                    );

                    spriteBatch.Draw(
                        textures["StatBar"],
                        HealthLocation + (Vector2.UnitY * Increment * counter),
                        new Rectangle(0, 0, textures["StatBar"].Width, textures["StatBar"].Height),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0
                    );

                    spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat("Enemies Killed: ", killCount.Count),
                        HealthLocation + (Vector2.UnitY * Increment * counter) + Vector2.UnitY * 16 + Vector2.UnitX * 5,
                        Color.White
                    );

                    spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat("Attack Speed: ", attackSpeed.CurrentAttackSpeed.ToString("F"), "x"),
                        HealthLocation + (Vector2.UnitY * Increment * counter) + Vector2.UnitY * 28 + Vector2.UnitX * 5,
                        Color.White
                    );

                    spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat("Spell Damage: ", spellDamage.CurrentSpellDamage.ToString("F"), "x"),
                        HealthLocation + (Vector2.UnitY * Increment * counter) + Vector2.UnitY * 40 + Vector2.UnitX * 5,
                        Color.White
                    );

                    spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat("Spell Effect Chance: ", spellEffectChance.CurrentSpellEffectChance.ToString("F"), "x"),
                        HealthLocation + (Vector2.UnitY * Increment * counter) + Vector2.UnitY * 52 + Vector2.UnitX * 5,
                        Color.White
                    );

                    spriteBatch.DrawString(
                        fonts["FontSmall"],
                        string.Concat("Move Speed: ", speed.speed.ToString("F")),
                        HealthLocation + (Vector2.UnitY * Increment * counter) + Vector2.UnitY * 64 + Vector2.UnitX * 5,
                        Color.White
                    );

                    counter++;
                });

                spriteBatch.DrawString(
                        fonts["Font"],
                        string.Concat("Time: ", float.Round(totalElapsedTime, 2)),
                        TimeLocation + Vector2.UnitX * (graphics.PreferredBackBufferWidth / 3),
                        Color.White
                    );

                if (gameState == GameState.Paused)
                {
                    spriteBatch.DrawString(
                        fonts["Font"],
                        "Game Paused",
                        new Vector2(graphics.PreferredBackBufferWidth / 6 - 50, graphics.PreferredBackBufferHeight / 6),
                        Color.White
                    );
                }
            }
        }
    }
}