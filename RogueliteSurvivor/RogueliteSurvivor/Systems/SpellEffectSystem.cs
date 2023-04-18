using Arch.Core;
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
                burn.NextTick -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

                if (burn.NextTick < 0)
                {
                    health.Current -= (int)burn.DamagePerTick;
                    burn.NextTick += burn.TickRate;
                    anim.Overlay = Color.Red;

                    if (health.Current < 1)
                    {
                        entityStatus.State = Constants.State.ReadyToDie;
                    }
                }

                if (isDurationFinished(burn, gameTime))
                {
                    entity.Remove<Burn>();
                }
            });

            world.Query(in slowQuery, (in Entity entity, ref Slow slow) =>
            {
                if (isDurationFinished(slow, gameTime))
                {
                    entity.Remove<Slow>();
                }
            });

            world.Query(in shockQuery, (in Entity entity, ref Shock shock) =>
            {
                if (isDurationFinished(shock, gameTime))
                {
                    entity.Remove<Shock>();
                }
            });

            world.Query(in poisonQuery, (in Entity entity, ref Poison poison) =>
            {
                if (isDurationFinished(poison, gameTime))
                {
                    entity.Remove<Poison>();
                }
            });

            world.Query(in stationaryQuery, (ref Stationary stationary, ref EntityStatus entityStatus) =>
            {
                if (isDurationFinished(stationary, gameTime))
                {
                    entityStatus.State = Constants.State.Dead;
                }
            });
        }

        private bool isDurationFinished(IDuration duration, GameTime gameTime)
        {
            duration.TimeLeft -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

            return duration.TimeLeft < 0;
        }
    }
}
