using Arch.Core;
using Arch.Core.Extensions;
using Box2D.NetStandard.Dynamics.Bodies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Physics;
using System;
using System.Collections.Generic;

namespace RogueliteSurvivor.ComponentFactories
{
    public static class SpellFactory
    {
        public static T CreateSpell<T>(SpellContainer spellContainer)
            where T : ISpell, new()
        {
            T spell = new T()
            {
                Spell = spellContainer.Spell,
                BaseDamage = spellContainer.BaseDamage,
                CurrentDamage = spellContainer.CurrentDamage,
                BaseProjectileSpeed = spellContainer.BaseProjectileSpeed,
                CurrentProjectileSpeed = spellContainer.CurrentProjectileSpeed,
                BaseAttacksPerSecond = spellContainer.BaseAttacksPerSecond,
                CurrentAttacksPerSecond = spellContainer.CurrentAttacksPerSecond,
                BaseEffectChance = spellContainer.BaseEffectChance,
                CurrentEffectChance = spellContainer.CurrentEffectChance,
                Cooldown = 0f,
                Effect = spellContainer.Effect,
                Type = spellContainer.Type,
            };

            return spell;
        }

        public static Animation GetSpellAliveAnimation(SpellContainer spellContainer)
        {
            return new Animation(
                spellContainer.AliveAnimation.FirstFrame,
                spellContainer.AliveAnimation.LastFrame,
                spellContainer.AliveAnimation.PlaybackSpeed,
                spellContainer.AliveAnimation.NumDirections,
                spellContainer.AliveAnimation.Repeatable);
        }

        public static SpriteSheet GetSpellAliveSpriteSheet(Dictionary<string, Texture2D> textures, SpellContainer spellContainer, Vector2 currentPosition, Vector2 targetPosition, float scaleMultiplier = 1f)
        {
            return new SpriteSheet(
                textures[spellContainer.Spell.ToString()],
                spellContainer.Spell.ToString(),
                spellContainer.AliveSpriteSheet.FramesPerRow,
                spellContainer.AliveSpriteSheet.FramesPerColumn,
                spellContainer.AliveSpriteSheet.Rotation == "none" ? 0 : MathF.Atan2(targetPosition.Y - currentPosition.Y, targetPosition.X - currentPosition.X),
                spellContainer.AliveSpriteSheet.Scale * scaleMultiplier
            );
        }

        public static Animation GetSpellHitAnimation(SpellContainer spellContainer)
        {
            return new Animation(
                spellContainer.HitAnimation.FirstFrame,
                spellContainer.HitAnimation.LastFrame,
                spellContainer.HitAnimation.PlaybackSpeed,
                spellContainer.HitAnimation.NumDirections,
                spellContainer.HitAnimation.Repeatable);
        }

        public static SpriteSheet GetSpellHitSpriteSheet(Dictionary<string, Texture2D> textures, SpellContainer spellContainer, float rotation, float scaleMultiplier = 1f)
        {
            return new SpriteSheet(
                textures[spellContainer.Spell.ToString() + "Hit"],
                spellContainer.Spell.ToString() + "Hit",
                spellContainer.HitSpriteSheet.FramesPerRow,
                spellContainer.HitSpriteSheet.FramesPerColumn,
                spellContainer.HitSpriteSheet.Rotation == "none" ? 0 : rotation,
                spellContainer.HitSpriteSheet.Scale * scaleMultiplier
            );
        }

        public static SingleTarget CreateSingleTarget(SpellContainer spellContainer)
        {
            return new SingleTarget()
            {
                DamageStartDelay = spellContainer.DamageStartDelay,
                DamageEndDelay = spellContainer.DamageEndDelay,
            };
        }

        public static void CreateProjectile(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers,Entity entity, ISpell spell, Target target, Position pos, SpellEffects effect)
        {
            var projectile = world.Create<Projectile, EntityStatus, Position, Velocity, Speed, Animation, SpriteSheet, Damage, Owner, Pierce, Body>();

            var body = new BodyDef();
            var velocityVector = Vector2.Normalize(target.TargetPosition - pos.XY);
            var position = pos.XY + velocityVector;
            body.position = new System.Numerics.Vector2(position.X, position.Y) / PhysicsConstants.PhysicsToPixelsRatio;
            body.fixedRotation = true;

            projectile.Set(
                new Projectile(),
                new EntityStatus(),
                new Position() { XY = new Vector2(position.X, position.Y) },
                new Velocity() { Vector = velocityVector * spell.CurrentProjectileSpeed },
                new Speed() { speed = spell.CurrentProjectileSpeed },
                GetSpellAliveAnimation(spellContainers[spell.Spell]),
                GetSpellAliveSpriteSheet(textures, spellContainers[spell.Spell], pos.XY, target.TargetPosition),
                new Damage() { Amount = spell.CurrentDamage, BaseAmount = spell.CurrentDamage, SpellEffect = effect },
                new Owner() { Entity = entity },
                new Pierce(entity.Has<Pierce>() ? entity.Get<Pierce>().Num : 0),
                BodyFactory.CreateCircularBody(projectile, 14, physicsWorld, body, .1f)
            );
        }

        public static void CreateSingleTarget(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers, Entity entity, ISpell spell, Target target, Position pos, SpellEffects effect)
        {
            var singleTarget = world.Create<SingleTarget, EntityStatus, Position, Speed, Animation, SpriteSheet, Damage, Owner, Body>();

            var body = new BodyDef();
            body.position = new System.Numerics.Vector2(target.TargetPosition.X, target.TargetPosition.Y) / PhysicsConstants.PhysicsToPixelsRatio;
            body.fixedRotation = true;

            float radiusMultiplier = entity.Has<AreaOfEffect>() ? entity.Get<AreaOfEffect>().Radius : 1f;

            singleTarget.Set(
                CreateSingleTarget(spellContainers[spell.Spell]),
                new EntityStatus(),
                new Position() { XY = target.TargetPosition },
                new Speed() { speed = spell.CurrentProjectileSpeed },
                GetSpellAliveAnimation(spellContainers[spell.Spell]),
                GetSpellAliveSpriteSheet(textures, spellContainers[spell.Spell], pos.XY, target.TargetPosition, radiusMultiplier),
                new Damage() { Amount = spell.CurrentDamage, BaseAmount = spell.CurrentDamage, SpellEffect = effect },
                new Owner() { Entity = entity },
                BodyFactory.CreateCircularBody(singleTarget, (int)(32 * radiusMultiplier), physicsWorld, body, .1f, false)
            );
        }

        public static Entity CreateAura(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers, Entity entity, ISpell spell, SpellEffects effect)
        {
            var aura = world.Create<Aura, EntityStatus, Position, Animation, SpriteSheet, Damage, Owner, Body>();
            var position = entity.Get<Position>();
            var body = new BodyDef();
            body.position = new System.Numerics.Vector2(position.XY.X, position.XY.Y) / PhysicsConstants.PhysicsToPixelsRatio;
            body.fixedRotation = true;

            float radiusMultiplier = entity.Has<AreaOfEffect>() ? entity.Get<AreaOfEffect>().Radius : 1f;

            aura.Set(
                new Aura() { BaseRadius = 32, RadiusMultiplier = radiusMultiplier },
                new EntityStatus(),
                new Position() { XY = position.XY },
                GetSpellAliveAnimation(spellContainers[spell.Spell]),
                GetSpellAliveSpriteSheet(textures, spellContainers[spell.Spell], position.XY, position.XY, radiusMultiplier),
                new Damage() { Amount = spell.CurrentDamage, BaseAmount = spell.CurrentDamage, SpellEffect = effect },
                new Owner() { Entity = entity },
                BodyFactory.CreateCircularBody(aura, (int)(32 * radiusMultiplier), physicsWorld, body, .1f, false)
            );

            return aura;
        }
    }
}
