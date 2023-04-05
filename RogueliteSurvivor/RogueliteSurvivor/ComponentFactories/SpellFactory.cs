using Arch.Core;
using Arch.Core.Extensions;
using Box2D.NetStandard.Dynamics.Bodies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

        public static void CreateProjectile(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers,Entity entity, ISpell spell, Target target, Position pos, SpellEffects effect, Dictionary<string, SoundEffect> soundEffects)
        {
            var projectile = world.Create<Projectile, EntityStatus, Position, Velocity, Speed, Animation, SpriteSheet, Damage, Owner, Pierce, HitSound, Body>();

            float damageMultiplier = entity.Has<DoubleDamage>() ? 2 : 1;

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
                new Damage() { Amount = spell.CurrentDamage * damageMultiplier, BaseAmount = spell.CurrentDamage * damageMultiplier, SpellEffect = effect },
                new Owner() { Entity = entity },
                new Pierce(entity.Has<Pierce>() ? entity.Get<Pierce>().Num : 0),
                new HitSound() { SoundEffect = spellContainers[spell.Spell].HitSound },
                BodyFactory.CreateCircularBody(projectile, 14, physicsWorld, body, .1f)
            );

            soundEffects[spellContainers[spell.Spell].CreateSound].Play();
        }

        public static void CreateEnemyProjectile(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers, Entity entity, ISpell spell, Target target, Position pos, SpellEffects effect, Dictionary<string, SoundEffect> soundEffects)
        {
            var projectile = world.Create<EnemyProjectile, EntityStatus, Position, Velocity, Speed, Animation, SpriteSheet, Damage, Owner, Pierce, HitSound, Body>();

            float damageMultiplier = entity.Has<DoubleDamage>() ? 2 : 1;

            var body = new BodyDef();
            var velocityVector = Vector2.Normalize(target.TargetPosition - pos.XY);
            var position = pos.XY + velocityVector;
            body.position = new System.Numerics.Vector2(position.X, position.Y) / PhysicsConstants.PhysicsToPixelsRatio;
            body.fixedRotation = true;

            projectile.Set(
                new EnemyProjectile(),
                new EntityStatus(),
                new Position() { XY = new Vector2(position.X, position.Y) },
                new Velocity() { Vector = velocityVector * spell.CurrentProjectileSpeed },
                new Speed() { speed = spell.CurrentProjectileSpeed },
                GetSpellAliveAnimation(spellContainers[spell.Spell]),
                GetSpellAliveSpriteSheet(textures, spellContainers[spell.Spell], pos.XY, target.TargetPosition),
                new Damage() { Amount = spell.CurrentDamage * damageMultiplier, BaseAmount = spell.CurrentDamage * damageMultiplier, SpellEffect = effect },
                new Owner() { Entity = entity },
                new Pierce(entity.Has<Pierce>() ? entity.Get<Pierce>().Num : 0),
                new HitSound() { SoundEffect = spellContainers[spell.Spell].HitSound },
                BodyFactory.CreateCircularBody(projectile, 14, physicsWorld, body, .1f)
            );

            soundEffects[spellContainers[spell.Spell].CreateSound].Play();
        }

        public static void CreateSingleTarget(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers, Entity entity, ISpell spell, Target target, Position pos, SpellEffects effect, Dictionary<string, SoundEffect> soundEffects)
        {
            var singleTarget = world.Create<SingleTarget, EntityStatus, Position, Speed, Animation, SpriteSheet, Damage, Owner, CreateSound, Body>();

            float damageMultiplier = entity.Has<DoubleDamage>() ? 2 : 1;

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
                new Damage() { Amount = spell.CurrentDamage * damageMultiplier, BaseAmount = spell.CurrentDamage * damageMultiplier, SpellEffect = effect },
                new Owner() { Entity = entity },
                new CreateSound() { SoundEffect = spellContainers[spell.Spell].CreateSound },
                BodyFactory.CreateCircularBody(singleTarget, (int)(32 * radiusMultiplier), physicsWorld, body, .1f, false)
            );
        }

        public static Entity CreateAura(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers, Entity entity, ISpell spell, SpellEffects effect)
        {
            var aura = world.Create<Aura, EntityStatus, Position, Animation, SpriteSheet, Damage, Owner>();
            var position = entity.Get<Position>();

            float radiusMultiplier = entity.Has<AreaOfEffect>() ? entity.Get<AreaOfEffect>().Radius : 1f;

            aura.Set(
                new Aura() { BaseRadius = 32, RadiusMultiplier = radiusMultiplier },
                new EntityStatus(),
                new Position() { XY = position.XY },
                GetSpellAliveAnimation(spellContainers[spell.Spell]),
                GetSpellAliveSpriteSheet(textures, spellContainers[spell.Spell], position.XY, position.XY, radiusMultiplier),
                new Damage() { Amount = spell.CurrentDamage, BaseAmount = spell.CurrentDamage, SpellEffect = effect },
                new Owner() { Entity = entity }
            );

            return aura;
        }

        public static void UpdateAura<T>(Entity entity, T spell, float radiusIncrease)
            where T : ISpell
        {
            float radiusMultiplier = (entity.Has<AreaOfEffect>() ? entity.Get<AreaOfEffect>().Radius : 1f) + radiusIncrease;
            
            var aura = spell.Child.Get<Aura>();
            aura.RadiusMultiplier = radiusMultiplier;
            var spriteSheet = spell.Child.Get<SpriteSheet>();
            spriteSheet.Scale = radiusMultiplier;

            spell.Child.Set(aura, spriteSheet);

        }

        public static Entity CreateMagicBeam(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers, Entity entity, ISpell spell, SpellEffects effect, Dictionary<string, SoundEffect> soundEffects)
        {
            var aura = world.Create<MagicBeam, EntityStatus, Position, Velocity, Speed, Target, Animation, SpriteSheet, Damage, Owner, LoopSound, Body>();

            float radiusMultiplier = entity.Has<AreaOfEffect>() ? entity.Get<AreaOfEffect>().Radius : 1f;

            var body = new BodyDef();
            var position = entity.Get<Position>();
            body.position = new System.Numerics.Vector2(position.XY.X, position.XY.Y) / PhysicsConstants.PhysicsToPixelsRatio;
            body.fixedRotation = true;

            var soundInstance = soundEffects[spellContainers[spell.Spell].CreateSound].CreateInstance();
            soundInstance.IsLooped = true;

            aura.Set(
                new MagicBeam() { BaseRadius = 14, RadiusMultiplier = radiusMultiplier, HitPosition = new Vector2(16, 36) },
                new EntityStatus(),
                new Position() { XY = position.XY },
                new Velocity(),
                new Speed() { speed = spell.CurrentProjectileSpeed },
                new Target(),
                GetSpellAliveAnimation(spellContainers[spell.Spell]),
                GetSpellAliveSpriteSheet(textures, spellContainers[spell.Spell], position.XY, position.XY, radiusMultiplier),
                new Damage() { Amount = spell.CurrentDamage, BaseAmount = spell.CurrentDamage, SpellEffect = effect },
                new Owner() { Entity = entity },
                new LoopSound() { SoundEffect = soundInstance },
                BodyFactory.CreateCircularBody(aura, 14, physicsWorld, body, .1f)
            );

            

            return aura;
        }

        public static void UpdateMagicBeam<T>(Entity entity, T spell, float radiusIncrease)
            where T : ISpell
        {
            float radiusMultiplier = (entity.Has<AreaOfEffect>() ? entity.Get<AreaOfEffect>().Radius : 1f) + radiusIncrease;

            var aura = spell.Child.Get<MagicBeam>();
            aura.RadiusMultiplier = radiusMultiplier;
            var spriteSheet = spell.Child.Get<SpriteSheet>();
            spriteSheet.Scale = radiusMultiplier;

            spell.Child.Set(aura, spriteSheet);

        }

        public static Entity CreateStationary(World world, Dictionary<string, Texture2D> textures, Dictionary<Spells, SpellContainer> spellContainers, Entity entity, ISpell spell)
        {
            var stationary = world.Create<Stationary, EntityStatus, Position, Animation, SpriteSheet, Damage, AttackSpeed, SpellEffectChance, Owner>();
            var position = entity.Get<Position>();

            float radiusMultiplier = entity.Has<AreaOfEffect>() ? entity.Get<AreaOfEffect>().Radius : 1f;

            stationary.Set(
                new Stationary() { TimeLeft = spell.CurrentAttacksPerSecond * 30, Cooldown = 0, BaseRadius = 16, RadiusMultiplier = radiusMultiplier },
                new EntityStatus(),
                new Position() { XY = position.XY },
                GetSpellAliveAnimation(spellContainers[spell.Spell]),
                GetSpellAliveSpriteSheet(textures, spellContainers[spell.Spell], position.XY, position.XY, radiusMultiplier),
                new Damage() { Amount = spell.CurrentDamage, BaseAmount = spell.CurrentDamage, SpellEffect = spell.Effect },
                new AttackSpeed() { BaseAttackSpeed = 0.5f, CurrentAttackSpeed = 0.5f },
                new SpellEffectChance() { BaseSpellEffectChance = spell.BaseEffectChance, CurrentSpellEffectChance = spell.CurrentEffectChance },
                new Owner() { Entity = entity }
            );

            return stationary;
        }
    }
}
