﻿using Arch.Core;
using Arch.Core.Extensions;
using Box2D.NetStandard.Collision;
using Box2D.NetStandard.Dynamics.Contacts;
using Box2D.NetStandard.Dynamics.World;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using Microsoft.Xna.Framework.Audio;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Helpers;
using System.Collections.Generic;

namespace RogueliteSurvivor.Physics
{
    public class GameContactListener : ContactListener
    {
        Dictionary<string, SoundEffect> soundEffects;

        public void SetSoundEffects(Dictionary<string, SoundEffect> soundEffects)
        {
            this.soundEffects = soundEffects;
        }

        public override void BeginContact(in Contact contact)
        {
            checkContact(contact);
        }

        public override void EndContact(in Contact contact)
        {

        }

        private void checkContact(Contact contact)
        {
            if (contact.GetFixtureA().Body.UserData != null && contact.GetFixtureB().Body.UserData != null)
            {
                Entity a = (Entity)contact.GetFixtureA().Body.UserData;
                Entity b = (Entity)contact.GetFixtureB().Body.UserData;


                if ((a.Has<Player>() && (b.Has<Enemy>() || b.Has<EnemyProjectile>())) || (b.Has<Player>() && (a.Has<Enemy>() || a.Has<EnemyProjectile>())))
                {
                    damagePlayer(a, b);
                }
                else if ((a.Has<Projectile>() && b.Has<Enemy>()) || (b.Has<Projectile>() && a.Has<Enemy>()))
                {
                    EntityStatus state;
                    Damage damage;
                    Owner owner;
                    if (a.Has<Projectile>())
                    {
                        state = b.Get<EntityStatus>();
                        damage = a.Get<Damage>();
                        owner = a.Get<Owner>();
                        updateProjectile(a, state);
                    }
                    else
                    {
                        state = b.Get<EntityStatus>();
                        damage = b.Get<Damage>();
                        owner = b.Get<Owner>();
                        updateProjectile(b, state);
                    }

                    if (state.State == State.Alive)
                    {
                        damageEnemy(a, b, damage, owner);
                    }
                }
                else if ((a.Has<SingleTarget>() && b.Has<Enemy>()) || (b.Has<SingleTarget>() && a.Has<Enemy>()))
                {
                    Damage damage;
                    Owner owner;
                    if (a.Has<SingleTarget>())
                    {
                        damage = a.Get<Damage>();
                        owner = a.Get<Owner>();
                    }
                    else
                    {
                        damage = b.Get<Damage>();
                        owner = b.Get<Owner>();
                    }

                    damageEnemy(a, b, damage, owner);
                }
                else if (((a.Has<Projectile>() || a.Has<EnemyProjectile>()) && b.Has<Map>()) || ((b.Has<Projectile>() || b.Has<EnemyProjectile>()) && a.Has<Map>()))
                {
                    if (a.Has<Projectile>() || a.Has<EnemyProjectile>())
                    {
                        setEntityDead(a, a.Get<EntityStatus>());
                    }
                    else
                    {
                        setEntityDead(b, b.Get<EntityStatus>());
                    }
                }

            }
        }

        private void damageEnemy(Entity a, Entity b, Damage damage, Owner owner)
        {
            if (a.Has<Enemy>())
            {
                AttackHelpers.SetEnemyHealthAndState(a, a.Get<EntityStatus>(), damage, owner);
            }
            else
            {
                AttackHelpers.SetEnemyHealthAndState(b, b.Get<EntityStatus>(), damage, owner);
            }
        }


        private void updateProjectile(Entity entity, EntityStatus entityStatus)
        {
            var pierce = entity.Get<Pierce>();
            if (pierce.Num > 0)
            {
                pierce.Num--;
                entity.Set(pierce);
                soundEffects[entity.Get<HitSound>().SoundEffect].Play();
            }
            else
            {
                setEntityDead(entity, entityStatus);
            }
        }

        private void setEntityDead(Entity entity, EntityStatus entityStatus)
        {
            if (entityStatus.State == State.Alive)
            {
                entityStatus.State = State.ReadyToDie;
                entity.Set(entityStatus);
            }
        }

        private void damagePlayer(Entity a, Entity b)
        {
            if (a.Has<Player>())
            {
                if (b.Has<Enemy>())
                {
                    setPlayerHealthAndStateFromMelee(a, b, a.Get<EntityStatus>());
                }
                else
                {
                    setPlayerHealthAndState(a, b, a.Get<EntityStatus>());
                    setEntityDead(b, b.Get<EntityStatus>());
                }
            }
            else
            {
                if (a.Has<Enemy>())
                {
                    setPlayerHealthAndStateFromMelee(b, a, b.Get<EntityStatus>());
                }
                else
                {
                    setPlayerHealthAndState(b, a, b.Get<EntityStatus>());
                    setEntityDead(a, a.Get<EntityStatus>());
                }
            }
        }

        private void setPlayerHealthAndStateFromMelee(Entity entity, Entity other, EntityStatus entityStatus)
        {
            var attackSpeed = other.Get<Spell1>();
            if (attackSpeed.Cooldown > attackSpeed.CurrentAttackSpeed)
            {
                soundEffects[other.Get<HitSound>().SoundEffect].Play();
                attackSpeed.Cooldown -= attackSpeed.CurrentAttackSpeed;
                setPlayerHealthAndState(entity, other, entityStatus);
                other.Set(attackSpeed);
            }
        }

        private void setPlayerHealthAndState(Entity entity, Entity other, EntityStatus entityStatus)
        {
            if (!entity.Has<Invincibility>())
            {
                Health health = entity.Get<Health>();
                Damage damage = other.Get<Damage>();
                health.Current -= (int)damage.Amount;
                Animation anim = entity.Get<Animation>();
                anim.Overlay = Microsoft.Xna.Framework.Color.Red;

                if (health.Current < 1)
                {
                    KillCount killCount = entity.Get<KillCount>();

                    if (other.Has<Enemy>())
                    {
                        killCount.KillerName = other.Get<Enemy>().Name;
                        killCount.KillerMethod = "pummeled";
                    }
                    else
                    {
                        killCount.KillerName = other.Get<Owner>().EntityReference.Entity.Get<Enemy>().Name;
                        killCount.KillerMethod = "blasted";
                    }

                    entity.Set(killCount);

                    entityStatus.State = State.Dead;
                }

                entity.Set(health, anim, entityStatus);
            }
        }

        public override void PostSolve(in Contact contact, in ContactImpulse impulse)
        {

        }

        public override void PreSolve(in Contact contact, in Manifold oldManifold)
        {

        }
    }
}
