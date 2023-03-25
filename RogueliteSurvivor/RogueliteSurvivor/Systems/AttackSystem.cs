using Arch.Core;
using Arch.Core.Extensions;
using Box2D.NetStandard.Collision;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.ComponentFactories;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Physics;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RogueliteSurvivor.Systems
{
    public class AttackSystem : ArchSystem, IUpdateSystem
    {
        Dictionary<string, Texture2D> textures;
        Box2D.NetStandard.Dynamics.World.World physicsWorld;
        Random random;
        Dictionary<Spells, SpellContainer> spellContainers;

        QueryDescription spell1Query = new QueryDescription()
                                .WithAll<Spell1>();

        QueryDescription spell2Query = new QueryDescription()
                                .WithAll<Spell2>();

        QueryDescription spell3Query = new QueryDescription()
                                .WithAll<Spell3>();



        public AttackSystem(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers)
            : base(world, new QueryDescription())
        {
            this.textures = textures;
            this.physicsWorld = physicsWorld;
            this.spellContainers = spellContainers;
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

                if (spell.Type != SpellType.Aura)
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
                        SpellFactory.CreateProjectile(world, textures, physicsWorld, spellContainers, entity, spell, target, pos, effect);
                    }
                    else if (spell.Type == SpellType.SingleTarget)
                    {
                        SpellFactory.CreateSingleTarget(world, textures, physicsWorld, spellContainers, entity, spell, target, pos, effect);
                    }
                    else if(spell.Type == SpellType.EnemyProjectile)
                    {
                        SpellFactory.CreateEnemyProjectile(world, textures, physicsWorld, spellContainers, entity, spell, target, pos, effect);
                    }
                }
                else
                {
                    var body = spell.Child.Get<Position>();
                    System.Numerics.Vector2 position = new System.Numerics.Vector2(body.XY.X, body.XY.Y);
                    var aura = spell.Child.Get<Aura>();
                    AABB aabb = new AABB(
                        (position - System.Numerics.Vector2.One * aura.BaseRadius * aura.RadiusMultiplier) / PhysicsConstants.PhysicsToPixelsRatio,
                        (position + System.Numerics.Vector2.One * aura.BaseRadius * aura.RadiusMultiplier) / PhysicsConstants.PhysicsToPixelsRatio
                    );

                    physicsWorld.QueryAABB(out Fixture[] touched, aabb);

                    foreach(Fixture fixture in touched)
                    {
                        if (fixture != null && fixture.Body.UserData != null)
                        {
                            Entity touchedEntity = (Entity)fixture.Body.UserData;
                            if (touchedEntity.Has<Enemy>()
                                && Vector2.Distance(touchedEntity.Get<Position>().XY, entity.Get<Position>().XY) <= aura.BaseRadius * aura.RadiusMultiplier)
                            {
                                setEnemyHealthAndState(touchedEntity, touchedEntity.Get<EntityStatus>(), spell.Child.Get<Damage>(), spell.Child.Get<Owner>(), spell.CurrentEffectChance);
                            }
                        }
                    }
                }
            }

            return spell.Cooldown;
        }

        private void setEnemyHealthAndState(Entity entity, EntityStatus entityStatus, Damage damage, Owner owner, float spellEffectChance)
        {
            if (entityStatus.State == State.Alive)
            {
                Health health = entity.Get<Health>();
                health.Current -= (int)damage.Amount;
                if (entity.Has<DoubleDamage>())
                {
                    health.Current -= (int)damage.Amount;
                }

                if (health.Current < 1)
                {
                    entityStatus.State = State.ReadyToDie;
                    entity.Set(entityStatus);
                    Experience enemyExperience = entity.Get<Experience>();
                    KillCount killCount = (KillCount)owner.Entity.Get(typeof(KillCount));
                    Player playerExperience = owner.Entity.Get<Player>();
                    killCount.AddKill(entity.Get<Enemy>().Name);
                    if (entity.Has<DoubleExperience>())
                    {
                        playerExperience.TotalExperience += enemyExperience.Amount * 2;
                        playerExperience.ExperienceToNextLevel -= enemyExperience.Amount * 2;
                    }
                    else
                    {
                        playerExperience.TotalExperience += enemyExperience.Amount;
                        playerExperience.ExperienceToNextLevel -= enemyExperience.Amount;
                    }
                    owner.Entity.Set(killCount, playerExperience);
                }
                else
                {
                    Animation anim = entity.Get<Animation>();
                    anim.Overlay = Color.Red;

                    entity.Set(health, anim);
                    if (damage.SpellEffect != SpellEffects.None && spellEffectChance > 0)
                    {
                        SpellEffects effect = SpellEffects.None;
                        
                        if (random.Next(1000) < (spellEffectChance * 1000))
                        {
                            effect = damage.SpellEffect;
                        }
                        
                        switch (effect)
                        {
                            case SpellEffects.Burn:
                                if (!entity.Has<Burn>())
                                {
                                    entity.Add(new Burn() { TimeLeft = 5f, TickRate = .5f, NextTick = .5f });
                                }
                                else
                                {
                                    entity.Set(new Burn() { TimeLeft = 5f, TickRate = .5f, NextTick = .5f });
                                }
                                break;
                            case SpellEffects.Slow:
                                if (!entity.Has<Slow>())
                                {
                                    entity.Add(new Slow() { TimeLeft = 5f });
                                }
                                else
                                {
                                    entity.Set(new Slow() { TimeLeft = 5f });
                                }
                                break;
                            case SpellEffects.Shock:
                                if (!entity.Has<Shock>())
                                {
                                    entity.Add(new Shock() { TimeLeft = 1f });
                                }
                                else
                                {
                                    entity.Set(new Shock() { TimeLeft = 1f });
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
