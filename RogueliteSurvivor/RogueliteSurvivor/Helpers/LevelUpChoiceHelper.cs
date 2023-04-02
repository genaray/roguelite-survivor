using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.ComponentFactories;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Helpers
{
    public static class LevelUpChoiceHelper
    {
        public static Rectangle GetLevelUpChoiceButton(LevelUpType levelUpType, bool isSelected)
        {
            int xOffset = isSelected ? 64 : 0;

            switch(levelUpType)
            {
                case LevelUpType.Damage:
                    return new Rectangle(128 + xOffset, 128, 64, 64);
                case LevelUpType.Pierce:
                    return new Rectangle(xOffset, 64, 64, 64);
                case LevelUpType.AttackSpeed:
                    return new Rectangle(xOffset, 128, 64, 64);
                case LevelUpType.MoveSpeed:
                    return new Rectangle(xOffset, 192, 64, 64);
                case LevelUpType.SpellEffectChance:
                    return new Rectangle(128 + xOffset, 0, 64, 64);
                case LevelUpType.AreaOfEffect:
                    return new Rectangle(128 + xOffset, 64, 64, 64);
                case LevelUpType.Fireball:
                    return new Rectangle(xOffset, 256, 64, 64);
                case LevelUpType.FireExplosion:
                    return new Rectangle(xOffset, 320, 64, 64);
                case LevelUpType.FireAura:
                    return new Rectangle(128 + xOffset, 384, 64, 64);
                case LevelUpType.IceShard:
                    return new Rectangle(xOffset, 384, 64, 64);
                case LevelUpType.IceSpikes:
                    return new Rectangle(xOffset, 448, 64, 64);
                case LevelUpType.IceAura:
                    return new Rectangle(128 + xOffset, 448, 64, 64);
                case LevelUpType.LightningBlast:
                    return new Rectangle(128 + xOffset, 256, 64, 64);
                case LevelUpType.LightningStrike:
                    return new Rectangle(128 + xOffset, 320, 64, 64);
                case LevelUpType.LightningAura:
                    return new Rectangle(128 + xOffset, 512, 64, 64);
                case LevelUpType.MagicShot:
                    return new Rectangle(xOffset, 512, 64, 64);
                case LevelUpType.MagicBeam:
                    return new Rectangle(xOffset, 576, 64, 64);
                case LevelUpType.MagicAura:
                    return new Rectangle(128 + xOffset, 576, 64, 64);
                default:
                    return new Rectangle();
            }
        }

        public static float GetLevelUpAmount(LevelUpType levelUpType)
        {
            float pickup = 0f;

            switch (levelUpType)
            {
                case LevelUpType.AttackSpeed:
                    pickup = .1f;
                    break;
                case LevelUpType.Damage:
                    pickup = .25f;
                    break;
                case LevelUpType.MoveSpeed:
                    pickup = 5f;
                    break;
                case LevelUpType.SpellEffectChance:
                    pickup = .25f;
                    break;
                case LevelUpType.Pierce:
                    pickup = 1f;
                    break;
                case LevelUpType.AreaOfEffect:
                    pickup = .25f;
                    break;
            }

            return pickup;
        }

        public static string GetLevelUpDisplayTextForLevelUpChoice(LevelUpType pickupType)
        {
            string retVal = string.Empty;
            switch (pickupType)
            {
                case LevelUpType.AttackSpeed:
                    retVal = string.Concat("Attack Speed: +", GetLevelUpAmount(pickupType).ToString("F"), "x");
                    break;
                case LevelUpType.Damage:
                    retVal = string.Concat("Spell Damage: +", GetLevelUpAmount(pickupType).ToString("F"), "x");
                    break;
                case LevelUpType.SpellEffectChance:
                    retVal = string.Concat("Spell Effect Chance: +", GetLevelUpAmount(pickupType).ToString("F"), "x");
                    break;
                case LevelUpType.MoveSpeed:
                    retVal = string.Concat("Move Speed: +", ((int)GetLevelUpAmount(pickupType)).ToString());
                    break;
                case LevelUpType.Pierce:
                    retVal = string.Concat("Pierce: +", ((int)GetLevelUpAmount(pickupType)).ToString());
                    break;
                case LevelUpType.AreaOfEffect:
                    retVal = string.Concat("Area of Effect: +", GetLevelUpAmount(pickupType).ToString("F"), "x");
                    break;
            }

            return retVal;
        }

        public static LevelUpType GetPickupTypeFromString(this string levelUpString)
        {
            switch (levelUpString)
            {
                case "Fireball":
                    return LevelUpType.Fireball;
                case "FireExplosion":
                    return LevelUpType.FireExplosion;
                case "FireAura":
                    return LevelUpType.FireAura;
                case "IceShard":
                    return LevelUpType.IceShard;
                case "IceSpikes":
                    return LevelUpType.IceSpikes;
                case "IceAura":
                    return LevelUpType.IceAura;
                case "LightningBlast":
                    return LevelUpType.LightningBlast;
                case "LightningStrike":
                    return LevelUpType.LightningStrike;
                case "LightningAura":
                    return LevelUpType.LightningAura;
                case "MagicShot":
                    return LevelUpType.MagicShot;
                case "MagicBeam":
                    return LevelUpType.MagicBeam;
                case "MagicAura":
                    return LevelUpType.MagicAura;
                default:
                    return LevelUpType.None;
            }
        }

        public static bool ProcessLevelUp(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, ref Entity player, LevelUpType levelUpType, Dictionary<Spells, SpellContainer> spellContainers, Dictionary<string, SoundEffect> soundEffects)
        {
            bool destroy = true;
            float pickupAmount = GetLevelUpAmount(levelUpType);
            switch (levelUpType)
            {
                case LevelUpType.None:
                    processAttackSpeed(player, 0f);
                    processDamage(player, 0f);
                    processSpellEffectChance(player, 0f);
                    processAreaOfEffect(world, textures, physicsWorld, spellContainers, player, 0f);
                    break;
                case LevelUpType.AttackSpeed:
                    processAttackSpeed(player, pickupAmount);
                    break;
                case LevelUpType.Damage:
                    processDamage(player, pickupAmount);
                    break;
                case LevelUpType.SpellEffectChance:
                    processSpellEffectChance(player, pickupAmount);
                    break;
                case LevelUpType.MoveSpeed:
                    var moveSpeed = player.Get<Speed>();
                    moveSpeed.speed += pickupAmount;
                    player.Set(moveSpeed);
                    break;
                case LevelUpType.Pierce:
                    var pierce = player.Get<Pierce>();
                    pierce.Num += (int)pickupAmount;
                    player.Set(pierce);
                    break;
                case LevelUpType.AreaOfEffect:
                    processAreaOfEffect(world, textures, physicsWorld, spellContainers, player, pickupAmount);
                    break;
                case LevelUpType.Fireball:
                case LevelUpType.FireExplosion:
                case LevelUpType.FireAura:
                case LevelUpType.IceShard:
                case LevelUpType.IceSpikes:
                case LevelUpType.IceAura:
                case LevelUpType.LightningBlast:
                case LevelUpType.LightningStrike:
                case LevelUpType.LightningAura:
                case LevelUpType.MagicShot:
                case LevelUpType.MagicBeam:
                case LevelUpType.MagicAura:
                    var spells = player.GetAllComponents().Where(a => a is ISpell).ToList();
                    ISpell spell = null;
                    if (spells.Count == 1)
                    {
                        spell = SpellFactory.CreateSpell<Spell2>(spellContainers[levelUpType.ToString().GetSpellFromString()]);
                        player.Add((Spell2)spell);
                    }
                    else if (spells.Count == 2)
                    {
                        spell = SpellFactory.CreateSpell<Spell3>(spellContainers[levelUpType.ToString().GetSpellFromString()]);
                        player.Add((Spell3)spell);
                    }

                    if (spell.Type == SpellType.Aura)
                    {
                        var aura = SpellFactory.CreateAura(world, textures, physicsWorld, spellContainers, player, spell, spell.Effect);
                        spell.Child = aura;
                        if (spells.Count == 1)
                        {
                            player.Set((Spell2)spell);
                        }
                        else if (spells.Count == 2)
                        {
                            player.Set((Spell3)spell);
                        }
                    }
                    else if (spell.Type == SpellType.MagicBeam)
                    {
                        var beam = SpellFactory.CreateMagicBeam(world, textures, physicsWorld, spellContainers, player, spell, spell.Effect, soundEffects);
                        spell.Child = beam;
                        if (spells.Count == 1)
                        {
                            player.Set((Spell2)spell);
                        }
                        else if (spells.Count == 2)
                        {
                            player.Set((Spell3)spell);
                        }
                    }

                    processAttackSpeed(player, 0f);
                    processDamage(player, 0f);
                    processSpellEffectChance(player, 0f);
                    processAreaOfEffect(world, textures, physicsWorld, spellContainers, player, 0f);
                    break;
            }

            return destroy;
        }

        private static void processAttackSpeed(Entity player, float pickupAmount)
        {
            AttackSpeed attackSpeed = player.Get<AttackSpeed>();
            attackSpeed.CurrentAttackSpeed += attackSpeed.BaseAttackSpeed * pickupAmount;

            var spells = player.GetAllComponents().Where(a => a is ISpell).ToList();

            if (player.TryGet(out Spell1 spell1))
            {
                spell1.CurrentAttacksPerSecond = attackSpeed.CurrentAttackSpeed * spell1.BaseAttacksPerSecond;
                player.Set(spell1);
            }
            if (player.TryGet(out Spell2 spell2))
            {
                spell2.CurrentAttacksPerSecond = attackSpeed.CurrentAttackSpeed * spell2.BaseAttacksPerSecond;
                player.Set(spell2);
            }
            if (player.TryGet(out Spell3 spell3))
            {
                spell3.CurrentAttacksPerSecond = attackSpeed.CurrentAttackSpeed * spell3.BaseAttacksPerSecond;
                player.Set(spell3);
            }

            player.Set(attackSpeed);
        }

        private static void processDamage(Entity player, float pickupAmount)
        {
            SpellDamage spellDamage = player.Get<SpellDamage>();
            spellDamage.CurrentSpellDamage += spellDamage.BaseSpellDamage * pickupAmount;

            if (player.TryGet(out Spell1 spell1))
            {
                spell1.CurrentDamage = spellDamage.CurrentSpellDamage * spell1.BaseDamage;
                player.Set(spell1);
            }
            if (player.TryGet(out Spell2 spell2))
            {
                spell2.CurrentDamage = spellDamage.CurrentSpellDamage * spell2.BaseDamage;
                player.Set(spell2);
            }
            if (player.TryGet(out Spell3 spell3))
            {
                spell3.CurrentDamage = spellDamage.CurrentSpellDamage * spell3.BaseDamage;
                player.Set(spell3);
            }

            player.Set(spellDamage);
        }

        private static void processSpellEffectChance(Entity player, float pickupAmount)
        {
            SpellEffectChance spellEffectChance = player.Get<SpellEffectChance>();
            spellEffectChance.CurrentSpellEffectChance += spellEffectChance.BaseSpellEffectChance * pickupAmount;

            if (player.TryGet(out Spell1 spell1))
            {
                spell1.CurrentEffectChance = MathF.Min(1f, spellEffectChance.CurrentSpellEffectChance * spell1.BaseEffectChance);
                player.Set(spell1);
            }
            if (player.TryGet(out Spell2 spell2))
            {
                spell2.CurrentEffectChance = MathF.Min(1f, spellEffectChance.CurrentSpellEffectChance * spell2.BaseEffectChance);
                player.Set(spell2);
            }
            if (player.TryGet(out Spell3 spell3))
            {
                spell3.CurrentEffectChance = MathF.Min(1f, spellEffectChance.CurrentSpellEffectChance * spell3.BaseEffectChance);
                player.Set(spell3);
            }

            player.Set(spellEffectChance);
        }

        private static void processAreaOfEffect(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, Dictionary<Spells, SpellContainer> spellContainers, Entity player, float pickupAmount)
        {
            var areaOfAffect = player.Get<AreaOfEffect>();
            areaOfAffect.Radius += pickupAmount;

            if (player.TryGet(out Spell1 spell1))
            {
                if (spell1.Type == SpellType.Aura)
                {
                    SpellFactory.UpdateAura(player, spell1, pickupAmount);
                }
                else if(spell1.Type == SpellType.MagicBeam)
                {
                    SpellFactory.UpdateMagicBeam(player, spell1, pickupAmount);
                }
            }
            if (player.TryGet(out Spell2 spell2))
            {
                if (spell2.Type == SpellType.Aura)
                {
                    SpellFactory.UpdateAura(player, spell2, pickupAmount);
                }
                else if (spell2.Type == SpellType.MagicBeam)
                {
                    SpellFactory.UpdateMagicBeam(player, spell2, pickupAmount);
                }
            }
            if (player.TryGet(out Spell3 spell3))
            {
                if (spell3.Type == SpellType.Aura)
                {
                    SpellFactory.UpdateAura(player, spell3, pickupAmount);
                }
                else if (spell3.Type == SpellType.MagicBeam)
                {
                    SpellFactory.UpdateMagicBeam(player, spell3, pickupAmount);
                }
            }

            player.Set(areaOfAffect);
        }
    }
}
