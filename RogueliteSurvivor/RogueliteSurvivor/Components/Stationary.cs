using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Components
{
    public struct Stationary
    {
        public float TimeLeft { get; set; }
        public float Cooldown { get; set; }
        public float BaseRadius { get; set; }
        public float RadiusMultiplier { get; set; }
    }
}
