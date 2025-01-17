﻿using Arch.Core;
using Arch.Core.Extensions;
using Box2D.NetStandard.Dynamics.Bodies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.ComponentFactories;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using System;
using System.Collections.Generic;

namespace RogueliteSurvivor.Systems
{
    public class DeathSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription singleTargetQuery = new QueryDescription()
                                            .WithAll<SingleTarget>();

        QueryDescription projectileQuery = new QueryDescription()
                                            .WithAny<Projectile, EnemyProjectile>();

        QueryDescription enemyQuery = new QueryDescription()
                                            .WithAll<Enemy>();

        Dictionary<string, Texture2D> textures;
        Box2D.NetStandard.Dynamics.World.World physicsWorld;
        Dictionary<Spells, SpellContainer> spellContainers;
        Dictionary<string, SoundEffect> soundEffects;
        Random random;

        public DeathSystem(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers, Dictionary<string, SoundEffect> soundEffects)
            : base(world, new QueryDescription()
                                .WithAll<EntityStatus, Body>())
        {
            this.textures = textures;
            this.physicsWorld = physicsWorld;
            this.spellContainers = spellContainers;
            this.soundEffects = soundEffects;
            random = new Random();
        }

        public void Update(GameTime gameTime, float totalElapsedTime, float scaleFactor)
        {
            world.Query(in projectileQuery, (ref EntityStatus entityStatus, ref SpriteSheet spriteSheet, ref Animation animation, ref HitSound hitSound) =>
            {
                if (entityStatus.State == State.ReadyToDie)
                {
                    entityStatus.State = State.Dying;

                    Spells spell = spriteSheet.TextureName.GetSpellFromString();
                    animation = SpellFactory.GetSpellHitAnimation(spellContainers[spell]);
                    spriteSheet = SpellFactory.GetSpellHitSpriteSheet(textures, spellContainers[spell], spriteSheet.Rotation);
                    soundEffects[hitSound.SoundEffect].Play();
                }
                else if (entityStatus.State == State.Dying && animation.CurrentFrame == animation.LastFrame)
                {
                    entityStatus.State = State.Dead;
                }
            });

            world.Query(in enemyQuery, (ref EntityStatus entityStatus, ref SpriteSheet spriteSheet, ref Animation animation) =>
            {
                if (entityStatus.State == State.ReadyToDie)
                {
                    entityStatus.State = State.Dying;

                    if (spriteSheet.Width == 16)
                    {
                        int bloodToUse = random.Next(1, 9);
                        spriteSheet = new SpriteSheet(textures["MiniBlood" + bloodToUse], "MiniBlood" + bloodToUse, getMiniBloodNumFrames(bloodToUse), 1, 0, .5f);
                        animation = new Animation(0, getMiniBloodNumFrames(bloodToUse) - 1, 1 / 60f, 1, false);
                        soundEffects["EnemyDeath2"].Play();
                    }
                    else
                    {
                        int bloodToUse = random.Next(1, 5);
                        spriteSheet = new SpriteSheet(textures["Blood" + bloodToUse], "Blood" + bloodToUse, 30, 1, 0, .5f);
                        animation = new Animation(0, 29, 1 / 60f, 1, false);
                        soundEffects["EnemyDeath1"].Play();
                    }
                }
                else if (entityStatus.State == State.Dying && animation.CurrentFrame == animation.LastFrame)
                {
                    entityStatus.State = State.Dead;
                }
            });

            world.Query(in singleTargetQuery, (ref EntityStatus entityStatus, ref SingleTarget single, ref Animation animation) =>
            {
                if (entityStatus.State == State.Alive && single.DamageEndDelay < 0)
                {
                    entityStatus.State = State.Dying;
                }
                else if (entityStatus.State == State.Dying
                            && animation.CurrentFrame == animation.LastFrame)
                {
                    entityStatus.State = State.Dead;

                }
            });

            world.Query(in query, (in Entity entity, ref EntityStatus entityStatus, ref Body body) =>
            {
                if (entityStatus.State != State.Alive)
                {
                    physicsWorld.DestroyBody(body);
                    entity.Remove<Body, Burn, Slow, Shock, Poison>();
                }
            });
        }

        private int getMiniBloodNumFrames(int bloodToUse)
        {
            int retVal = 0;
            switch (bloodToUse)
            {
                case 1:
                case 2:
                case 6:
                case 7:
                    retVal = 30;
                    break;
                case 3:
                case 4:
                case 5:
                    retVal = 28;
                    break;
                case 8:
                    retVal = 29;
                    break;
                case 9:
                    retVal = 22;
                    break;
            }
            return retVal;
        }
    }
}
