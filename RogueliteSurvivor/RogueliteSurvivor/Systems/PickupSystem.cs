using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Extensions;
using RogueliteSurvivor.Helpers;
using System;

namespace RogueliteSurvivor.Systems
{
    public class PickupSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription playerQuery = new QueryDescription()
                                            .WithAll<Player>();

        QueryDescription invincibleQuery = new QueryDescription().WithAll<Invincibility>();
        QueryDescription doubleExperienceQuery = new QueryDescription().WithAll<DoubleExperience>();
        QueryDescription doubleDamageQuery = new QueryDescription().WithAll<DoubleDamage>();
        QueryDescription doubleAttackSpeedQuery = new QueryDescription().WithAll<DoubleAttackSpeed>();

        public PickupSystem(World world)
            : base(world, new QueryDescription()
                                .WithAll<PickupSprite, Position>())
        { }

        public void Update(GameTime gameTime, float totalElapsedTime, float scaleFactor)
        {
            world.Query(in invincibleQuery, (in Entity entity, ref Invincibility invincibility) =>
            {
                invincibility.TimeRemaining -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                if(invincibility.TimeRemaining <= 0)
                {
                    entity.Remove<Invincibility>();
                }
            });

            world.Query(in doubleExperienceQuery, (in Entity entity, ref DoubleExperience doubleExperience) =>
            {
                doubleExperience.TimeRemaining -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                if (doubleExperience.TimeRemaining <= 0)
                {
                    entity.Remove<DoubleExperience>();
                }
            });

            world.Query(in doubleDamageQuery, (in Entity entity, ref DoubleDamage doubleDamage) =>
            {
                doubleDamage.TimeRemaining -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                if (doubleDamage.TimeRemaining <= 0)
                {
                    entity.Remove<DoubleDamage>();
                }
            });

            world.Query(in doubleAttackSpeedQuery, (in Entity entity, ref DoubleAttackSpeed doubleAttackSpeed) =>
            {
                doubleAttackSpeed.TimeRemaining -= (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                if (doubleAttackSpeed.TimeRemaining <= 0)
                {
                    entity.Remove<DoubleAttackSpeed>();
                }
            });

            Entity player = new Entity();
            Position? playerPos = null;
            float radiusMultiplier = 0;
            world.Query(in playerQuery, (in Entity entity, ref Position pos, ref AreaOfEffect areaOfEffect) =>
            {
                if (!playerPos.HasValue)
                {
                    player = entity;
                    playerPos = pos;
                    radiusMultiplier = areaOfEffect.Radius;
                }
            });

            if (playerPos.HasValue)
            {
                world.Query(in query, (in Entity entity, ref PickupSprite sprite, ref Position pos) =>
                {
                    if (Vector2.Distance(playerPos.Value.XY, pos.XY) < (16 * radiusMultiplier))
                    {
                        if (PickupHelper.ProcessPickup(ref player, sprite.Type))
                        {
                            world.Destroy(entity);
                        }
                    }
                    else
                    {
                        sprite.Count += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (sprite.Count > sprite.MaxCount)
                        {
                            sprite.Count = 0;
                            sprite.Current += sprite.Increment;

                            if (sprite.Current == sprite.Max || sprite.Current == sprite.Min)
                            {
                                sprite.Increment *= -1;
                            }
                        }
                    }
                });
            }
        }
    }
}
