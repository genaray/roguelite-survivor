﻿namespace RogueliteSurvivor.Components
{
    public struct Burn : IDuration
    {
        public float TimeLeft { get; set; }
        public float TickRate { get; set; }
        public float NextTick { get; set; }
        public float DamagePerTick { get; set; }
    }
}
