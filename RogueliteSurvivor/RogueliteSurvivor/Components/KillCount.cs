using System.Collections.Generic;

namespace RogueliteSurvivor.Components
{
    public class KillCount
    {
        public KillCount()
        {
            Kills = new Dictionary<string, int>();
            NumKills = 0;
        }

        public int NumKills { get; private set; }
        public int NumBooks { get; set; }
        public string KillerName { get; set; }
        public string KillerMethod { get; set; }
        public Dictionary<string, int> Kills { get; set; }

        public void AddKill(string killerName)
        {
            if (Kills.ContainsKey(killerName))
            {
                Kills[killerName]++;
            }
            else
            {
                Kills.Add(killerName, 1);
            }
            NumKills++;
        }
    }
}
