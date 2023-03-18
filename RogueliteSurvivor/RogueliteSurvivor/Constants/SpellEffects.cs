using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Constants
{
    public enum SpellEffects
    {
        None,
        Burn,
        Shock,
        Slow,
    }

    public static class SpellsEffectsExtensions
    {
        public static SpellEffects GetSpellEffectFromString(this string spellString)
        {
            switch (spellString)
            {
                case "None":
                    return SpellEffects.None;
                case "Burn":
                    return SpellEffects.Burn;
                case "Shock":
                    return SpellEffects.Shock;
                case "Slow":
                    return SpellEffects.Slow;
                default:
                    return SpellEffects.None;
            }
        }
    }
}
