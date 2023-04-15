using Arch.Core;
using Arch.Core.Extensions;
using Box2D.NetStandard.Collision;
using Box2D.NetStandard.Dynamics.Fixtures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.ComponentFactories;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Helpers;
using System;
using System.Collections.Generic;

namespace RogueliteSurvivor.Systems
{
    public class AttackSystem : ArchSystem, IUpdateSystem
    {
        Dictionary<string, Texture2D> textures;
        Box2D.NetStandard.Dynamics.World.World physicsWorld;
        Random random;
        Dictionary<Spells, SpellContainer> spellContainers;
        Dictionary<string, SoundEffect> soundEffects;

        QueryDescription spell1Query = new QueryDescription()
                                .WithAll<Spell1>();

        QueryDescription spell2Query = new QueryDescription()
                                .WithAll<Spell2>();

        QueryDescription spell3Query = new QueryDescription()
                                .WithAll<Spell3>();

        QueryDescription stationaryQuery = new QueryDescription()
                                .WithAll<Stationary>();



        public AttackSystem(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers, Dictionary<string, SoundEffect> soundEffects)
            : base(world, new QueryDescription())
        {
            this.textures = textures;
            this.physicsWorld = physicsWorld;
            this.spellContainers = spellContainers;
            this.soundEffects = soundEffects;
            random = new Random();
        }

        public void Update(GameTime gameTime, float totalElapsedTime, float scaleFactor)
        {
            world.Query(in spell1Query, (in Entity entity, ref Position pos, ref Spell1 spell1) =>
            {
                spell1.Cooldown = processSpell(gameTime, entity, pos, spell1);
            });

            world.Query(in spell2Query, (in Entity entity, ref Position pos, ref Spell2 spell2) =>
            {
                spell2.Cooldown = processSpell(gameTime, entity, pos, spell2);
            });

            world.Query(in spell3Query, (in Entity entity, ref Position pos, ref Spell3 spell3) =>
            {
                spell3.Cooldown = processSpell(gameTime, entity, pos, spell3);
            });

            world.Query(in stationaryQuery, (ref Stationary stationary, ref Position pos, ref EntityStatus entityStatus, ref AttackSpeed attackSpeed, ref Damage damage, ref Owner owner, ref SpellEffectChance spellEffectChance) =>
            {
                if (entityStatus.State == State.Alive)
                {
                    stationary.Cooldown += (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                    if (stationary.Cooldown > attackSpeed.CurrentAttackSpeed)
                    {
                        stationary.Cooldown -= attackSpeed.CurrentAttackSpeed;

                        checkHitInRadius(pos,
                                            damage,
                                            owner,
                                            stationary.BaseRadius,
                                            stationary.RadiusMultiplier,
                                            spellEffectChance.CurrentSpellEffectChance);
                    }
                }
            });
        }

        private float processSpell(GameTime gameTime, Entity entity, Position pos, ISpell spell)
        {
            spell.Cooldown += (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
            if (entity.Has<DoubleAttackSpeed>())
            {
                spell.Cooldown += (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
            }

            if (spell.Type != SpellType.None
                    && spell.Cooldown > spell.CurrentAttackSpeed)
            {
                spell.Cooldown -= spell.CurrentAttackSpeed;

                if (spell.Type != SpellType.Aura && spell.Type != SpellType.MagicBeam)
                {
                    var target = entity.Get<Target>();
                    SpellEffects effect = SpellEffects.None;
                    if (spell.Effect != SpellEffects.None)
                    {
                        if (random.Next(1000) < (spell.CurrentEffectChance * 1000))
                        {
                            effect = spell.Effect;
                        }
                    }

                    if (spell.Type == SpellType.Projectile)
                    {
                        SpellFactory.CreateProjectile(world, textures, physicsWorld, spellContainers, entity, spell, target, pos, effect, soundEffects);
                    }
                    else if (spell.Type == SpellType.SingleTarget)
                    {
                        SpellFactory.CreateSingleTarget(world, textures, physicsWorld, spellContainers, entity, spell, target, pos, effect, soundEffects);
                    }
                    else if (spell.Type == SpellType.EnemyProjectile)
                    {
                        SpellFactory.CreateEnemyProjectile(world, textures, physicsWorld, spellContainers, entity, spell, target, pos, effect, soundEffects);
                    }
                    else if (spell.Type == SpellType.Stationary)
                    {
                        SpellFactory.CreateStationary(world, textures, spellContainers, entity, spell);
                    }
                }
                else if (spell.Type == SpellType.Aura)
                {
                    var aura = spell.ChildReference.Entity.Get<Aura>();
                    checkHitInRadius(spell.ChildReference.Entity.Get<Position>(),
                                        spell.ChildReference.Entity.Get<Damage>(),
                                        spell.ChildReference.Entity.Get<Owner>(),
                                        aura.BaseRadius,
                                        aura.RadiusMultiplier,
                                        spell.CurrentEffectChance);
                }
                else if (spell.Type == SpellType.MagicBeam)
                {
                    var magicBeam = spell.ChildReference.Entity.Get<MagicBeam>();
                    checkHitInRadius(spell.ChildReference.Entity.Get<Position>(),
                                        spell.ChildReference.Entity.Get<Damage>(),
                                        spell.ChildReference.Entity.Get<Owner>(),
                                        magicBeam.BaseRadius,
                                        magicBeam.RadiusMultiplier,
                                        spell.CurrentEffectChance);
                }
            }

            return spell.Cooldown;
        }

        private void checkHitInRadius(Position body, Damage damage, Owner owner, float baseRadius, float radiusMultiplier, float currentEffectChance)
        {
            System.Numerics.Vector2 position = new System.Numerics.Vector2(body.XY.X, body.XY.Y);
            Vector2 comparePosition = body.XY;
            AABB aabb = new AABB(
                (position - System.Numerics.Vector2.One * baseRadius * radiusMultiplier) / PhysicsConstants.PhysicsToPixelsRatio,
                (position + System.Numerics.Vector2.One * baseRadius * radiusMultiplier) / PhysicsConstants.PhysicsToPixelsRatio
            );

            physicsWorld.QueryAABB(out Fixture[] touched, aabb);

            foreach (Fixture fixture in touched)
            {
                if (fixture != null && fixture.Body.UserData != null)
                {
                    Entity touchedEntity = (Entity)fixture.Body.UserData;
                    if (touchedEntity.Has<Enemy>()
                        && Vector2.Distance(touchedEntity.Get<Position>().XY, comparePosition) <= baseRadius * radiusMultiplier)
                    {
                        SpellEffects effect = SpellEffects.None;

                        if (random.Next(1000) < (currentEffectChance * 1000))
                        {
                            effect = damage.SpellEffect;
                        }
                        AttackHelpers.SetEnemyHealthAndState(touchedEntity, touchedEntity.Get<EntityStatus>(), damage, owner, effect);
                    }
                }
            }
        }
    }
}
