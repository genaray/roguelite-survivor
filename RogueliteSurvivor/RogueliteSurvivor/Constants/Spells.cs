using System.Collections.Generic;

namespace RogueliteSurvivor.Constants
{
    public enum Spells
    {
        None,
        Fireball,
        FireExplosion,
        FireAura,
        FireRain,
        IceShard,
        IceSpikes,
        IceAura,
        LightningBlast,
        LightningStrike,
        LightningAura,
        MagicShot,
        MagicBeam,
        MagicAura,
        PoisonCloud,
        PoisonDart,
        EnemyMelee,
        EnemyEnergyBlast,
        Five,
        Ten,
        Blank
    }

    public static class SpellsExtensions
    {
        public static List<Spells> PlayerUsableSpells()
        {
            return new List<Spells>()
            {
                Spells.Fireball,
                Spells.FireExplosion,
                Spells.FireAura,
                Spells.FireRain,
                Spells.IceShard,
                Spells.IceSpikes,
                Spells.IceAura,
                Spells.LightningBlast,
                Spells.LightningStrike,
                Spells.LightningAura,
                Spells.MagicShot,
                Spells.MagicBeam,
                Spells.MagicAura,
                Spells.PoisonCloud,
                Spells.PoisonDart,
            };
        }

        public static Spells GetSpellFromString(this string spellString)
        {
            switch (spellString)
            {
                case "None":
                    return Spells.None;
                case "Fireball":
                    return Spells.Fireball;
                case "FireExplosion":
                    return Spells.FireExplosion;
                case "FireAura":
                    return Spells.FireAura;
                case "FireRain":
                    return Spells.FireRain;
                case "IceShard":
                    return Spells.IceShard;
                case "IceSpikes":
                    return Spells.IceSpikes;
                case "IceAura":
                    return Spells.IceAura;
                case "LightningBlast":
                    return Spells.LightningBlast;
                case "LightningStrike":
                    return Spells.LightningStrike;
                case "LightningAura":
                    return Spells.LightningAura;
                case "MagicShot":
                    return Spells.MagicShot;
                case "MagicBeam":
                    return Spells.MagicBeam;
                case "MagicAura":
                    return Spells.MagicAura;
                case "PoisonCloud":
                    return Spells.PoisonCloud;
                case "PoisonDart":
                    return Spells.PoisonDart;
                case "EnemyMelee":
                    return Spells.EnemyMelee;
                case "EnemyEnergyBlast":
                    return Spells.EnemyEnergyBlast;
                default:
                    return Spells.None;
            }
        }

        public static string GetReadableSpellName(this Spells spell)
        {
            switch (spell)
            {
                case Spells.Fireball:
                    return "Fireball";
                case Spells.FireExplosion:
                    return "Fire Explosion";
                case Spells.FireAura:
                    return "Fire Aura";
                case Spells.FireRain:
                    return "Fire Rain";
                case Spells.IceShard:
                    return "Ice Shard";
                case Spells.IceSpikes:
                    return "Ice Spikes";
                case Spells.IceAura:
                    return "Ice Aura";
                case Spells.LightningBlast:
                    return "Lightning Blast";
                case Spells.LightningStrike:
                    return "Lightning Strike";
                case Spells.LightningAura:
                    return "Lightning Aura";
                case Spells.MagicShot:
                    return "Magic Shot";
                case Spells.MagicBeam:
                    return "Magic Beam";
                case Spells.MagicAura:
                    return "Magic Aura";
                case Spells.PoisonCloud:
                    return "Poison Cloud";
                case Spells.PoisonDart:
                    return "Poison Dart";
                case Spells.EnemyEnergyBlast:
                    return "Enemy Energy Blast";
                default:
                    return "None";
            }
        }
    }
}
