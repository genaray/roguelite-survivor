using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Components
{
    public struct MagicBeam
    {
        public int BaseRadius { get; set; }
        public float RadiusMultiplier { get; set; }
        public Vector2 HitPosition { get; set; }
    }
}
