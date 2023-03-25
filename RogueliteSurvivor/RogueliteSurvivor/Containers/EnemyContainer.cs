using Newtonsoft.Json.Linq;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Helpers;
using System.Collections.Generic;

namespace RogueliteSurvivor.Containers
{
    public class EnemyContainer
    {
        public EnemyContainer() { }
        public string Name { get; set; }
        public string ReadableName { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
        public float Speed { get; set; }
        public Spells Spell { get; set; }
        public Spells Spell2 { get; set; }
        public int Width { get; set; }
        public int Experience { get; set; }
        public List<string> Traits { get; set; }
        public SpriteSheetContainer SpriteSheet { get; set; }
        public AnimationContainer Animation { get; set; }


        public static string EnemyContainerName(JToken enemy)
        {
            return (string)enemy["name"];
        }

        public static EnemyContainer ToEnemyContainer(JToken enemy)
        {
            return new EnemyContainer()
            {
                Name = (string)enemy["name"],
                ReadableName = (string)enemy["readableName"],
                Health = (int)enemy["health"],
                Damage = (int)enemy["damage"],
                Speed = (float)enemy["speed"],
                Spell = ((string)enemy["spell"]).GetSpellFromString(),
                Spell2 = enemy["spell2"] != null ? ((string)enemy["spell2"]).GetSpellFromString() : Spells.None,
                Width = (int)enemy["width"],
                Experience = (int)enemy["experience"],
                Traits = TraitsHelper.GetTraits(enemy["traits"]),
                Animation = AnimationContainer.ToAnimationContainer(enemy["animation"]),
                SpriteSheet = SpriteSheetContainer.ToSpriteSheetContainer(enemy["spriteSheet"])
            };
        }
    }
}
