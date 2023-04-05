﻿using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using RogueliteSurvivor.Components;
using System;

namespace RogueliteSurvivor.Systems
{
    public class SpellEffectSystem : ArchSystem, IUpdateSystem
    {
        private QueryDescription burnQuery = new QueryDescription()
                                                    .WithAll<Enemy, Burn, Health>();
        private QueryDescription slowQuery = new QueryDescription()
                                                    .WithAll<Slow>();
        private QueryDescription shockQuery = new QueryDescription()
                                                    .WithAll<Shock>();
        private QueryDescription stationaryQuery = new QueryDescription()
                                                    .WithAll<Stationary>();
        private QueryDescription poisonQuery = new QueryDescription()
                                                    .WithAll<Poison>();
        public SpellEffectSystem(World world)
            : base(world, new QueryDescription())
        {
        }

        public void Update(GameTime gameTime, float totalElapsedTime, float scaleFactor)
        {
            world.Query(in burnQuery, (in Entity entity, ref EntityStatus entityStatus, ref Burn burn, ref Health health, ref Animation anim) =>
            {
                burn.TimeLeft -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                burn.NextTick -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

                if (burn.NextTick < 0)
                {
                    health.Current -= 1;
                    burn.NextTick += burn.TickRate;
                    anim.Overlay = Color.Red;

                    if (health.Current < 1)
                    {
                        entityStatus.State = Constants.State.ReadyToDie;
                    }

                    if (burn.TimeLeft < 0)
                    {
                        entity.Remove<Burn>();
                    }
                }
            });

            world.Query(in slowQuery, (in Entity entity, ref Slow slow) =>
            {
                slow.TimeLeft -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

                if (slow.TimeLeft < 0)
                {
                    entity.Remove<Slow>();
                }
            });

            world.Query(in shockQuery, (in Entity entity, ref Shock shock) =>
            {
                shock.TimeLeft -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

                if (shock.TimeLeft < 0)
                {
                    entity.Remove<Shock>();
                }
            });

            world.Query(in poisonQuery, (in Entity entity, ref Poison poison) =>
            {
                poison.TimeLeft -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

                if (poison.TimeLeft < 0)
                {
                    entity.Remove<Shock>();
                }
            });

            world.Query(in stationaryQuery, (ref Stationary stationary, ref EntityStatus entityStatus) =>
            {
                stationary.TimeLeft -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

                if(stationary.TimeLeft < 0)
                {
                    entityStatus.State = Constants.State.Dead;
                }
            });
        }
    }
}
