using Newtonsoft.Json.Linq;
using RogueliteSurvivor.Constants;
using System.Collections.Generic;

namespace RogueliteSurvivor.Containers
{
    public class PlayerContainer
    {
        public PlayerContainer() { }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<string> Description { get; set; }
        public string Texture { get; set; }
        public Spells StartingSpell { get; set; }
        public float Health { get; set; }
        public float Speed { get; set; }
        public float AttackSpeed { get; set; }
        public float SpellDamage { get; set; }
        public float SpellEffectChance { get; set; }
        public float AreaOfEffect { get; set; }
        public int Pierce { get; set; }
        public AnimationContainer Animation { get; set; }
        public SpriteSheetContainer SpriteSheet { get; set; }
        public static string GetPlayerContainerName(JToken player)
        {
            return (string)player["name"];
        }
        public static PlayerContainer ToPlayerContainer(JToken player)
        {
            return new PlayerContainer()
            {
                Name = (string)player["name"],
                DisplayName = (string)player["displayName"],
                Description = getDescriptionParagraphs(player["description"]),
                Texture = (string)player["texture"],
                StartingSpell = ((string)player["startingSpell"]).GetSpellFromString(),
                Health = (float)player["health"],
                Speed = (float)player["speed"],
                AttackSpeed = (float)player["attackSpeed"],
                SpellDamage = (float)player["spellDamage"],
                SpellEffectChance = (float)player["spellEffectChance"],
                AreaOfEffect = (float)player["areaOfEffect"],
                Pierce = (int)player["pierce"],
                Animation = AnimationContainer.ToAnimationContainer(player["animation"]),
                SpriteSheet = SpriteSheetContainer.ToSpriteSheetContainer(player["spriteSheet"])
            };
        }

        private static List<string> getDescriptionParagraphs(JToken description)
        {
            List<string> paragraphs = new List<string>();
            foreach (var paragraph in description)
            {
                paragraphs.Add((string)paragraph);
            }
            return paragraphs;
        }
    }
}
