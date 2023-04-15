namespace RogueliteSurvivor.Constants
{
    public enum SpellEffects
    {
        None,
        Burn,
        Shock,
        Slow,
        Poison,
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
                case "Poison":
                    return SpellEffects.Poison;
                default:
                    return SpellEffects.None;
            }
        }
    }
}
