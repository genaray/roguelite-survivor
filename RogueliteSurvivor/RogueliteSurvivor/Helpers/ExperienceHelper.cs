using System;

namespace RogueliteSurvivor.Helpers
{
    public static class ExperienceHelper
    {
        public static int ExperienceRequiredForLevel(int level)
        {
            return (int)(20f + 2f * MathF.Pow(level - 1, 2f));
        }
    }
}
