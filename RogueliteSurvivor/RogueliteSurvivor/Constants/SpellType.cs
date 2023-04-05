using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Constants
{
    public enum SpellType
    {
        None,
        Projectile,
        SingleTarget,
        Aura,
        MagicBeam,
        Stationary,
        EnemyProjectile,
    }

    public static class SpellTypeExtensions
    {
        public static SpellType GetSpellTypeFromString(this string spellTypeString)
        {
            switch (spellTypeString)
            {
                case "None":
                    return SpellType.None;
                case "Projectile":
                    return SpellType.Projectile;
                case "SingleTarget":
                    return SpellType.SingleTarget;
                case "Aura":
                    return SpellType.Aura;
                case "MagicBeam":
                    return SpellType.MagicBeam;
                case "Stationary":
                    return SpellType.Stationary;
                case "EnemyProjectile":
                    return SpellType.EnemyProjectile;
                default:
                    return SpellType.None;
            }
        }
    }
}
