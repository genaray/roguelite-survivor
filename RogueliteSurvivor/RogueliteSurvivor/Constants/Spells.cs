using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Constants
{
    public enum Spells
    {
        None,
        Fireball,
        FireExplosion,
        FireAura,
        IceShard,
        IceSpikes,
        IceAura,
        LightningBlast,
        LightningStrike,
        LightningAura,
        MagicShot,
        EnemyMelee,
        EnemyEnergyBlast
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
                Spells.IceShard,
                Spells.IceSpikes,
                Spells.IceAura,
                Spells.LightningBlast,
                Spells.LightningStrike,
                Spells.LightningAura, 
                Spells.MagicShot,
            };
        }

        public static Spells GetSpellFromString(this string spellString)
        {
            switch(spellString)
            {
                case "None":
                    return Spells.None;
                case "Fireball":
                    return Spells.Fireball;
                case "FireExplosion":
                    return Spells.FireExplosion;
                case "FireAura":
                    return Spells.FireAura;
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
                case Spells.EnemyEnergyBlast:
                    return "Enemy Energy Blast";
                default:
                    return "None";
            }
        }
    }
}
