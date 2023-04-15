using Arch.Core;
using Arch.Core.Extensions;
using Newtonsoft.Json.Linq;
using RogueliteSurvivor.Components;
using System.Collections.Generic;
using System.Linq;

namespace RogueliteSurvivor.Helpers
{
    public static class TraitsHelper
    {
        public static List<string> GetTraits(JToken traits)
        {
            List<string> entityTraits = new List<string>();
            if (traits != null)
            {
                foreach (var trait in traits)
                {
                    entityTraits.Add((string)trait);
                }
            }
            return entityTraits;
        }

        public static string ReadableTraitName(this string trait)
        {
            switch (trait)
            {
                case "CanFly":
                    return "Can Fly";
                default:
                    return "None";
            }
        }

        public static void AddTraitsToEntity(Entity entity, List<string> traits)
        {
            if (traits.Any())
            {
                foreach (var trait in traits)
                {
                    switch (trait)
                    {
                        case "CanFly":
                            entity.Add(new CanFly());
                            break;
                    }
                }
            }
        }
    }
}
