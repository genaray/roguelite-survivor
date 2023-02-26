﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Components
{
    public struct SpellDamage
    {
        public SpellDamage(float baseSpellDamage) 
        {
            BaseSpellDamage = baseSpellDamage;
            CurrentSpellDamage = baseSpellDamage;
        }
        public float BaseSpellDamage { get; set;}
        public float CurrentSpellDamage { get; set; }
    }
}
