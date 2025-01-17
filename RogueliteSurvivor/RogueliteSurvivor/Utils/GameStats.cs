﻿using System.Collections.Generic;

namespace RogueliteSurvivor.Utils
{
    public class GameStats
    {
        public float PlayTime { get; set; }
        public string Killer { get; set; }
        public string KillerMethod { get; set; }
        public int EnemiesKilled { get; set; }
        public int NumBooks { get; set; }
        public Dictionary<string, int> Kills { get; set; }
    }
}
