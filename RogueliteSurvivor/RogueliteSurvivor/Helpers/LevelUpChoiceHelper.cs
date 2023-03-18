using Microsoft.Xna.Framework;
using RogueliteSurvivor.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Helpers
{
    public static class LevelUpChoiceHelper
    {
        public static Rectangle GetLevelUpChoiceButton(PickupType pickupType, bool isSelected)
        {
            int xOffset = isSelected ? 64 : 0;

            switch(pickupType)
            {
                case PickupType.Damage:
                    return new Rectangle(128 + xOffset, 128, 64, 64);
                case PickupType.Pierce:
                    return new Rectangle(xOffset, 64, 64, 64);
                case PickupType.AttackSpeed:
                    return new Rectangle(xOffset, 128, 64, 64);
                case PickupType.MoveSpeed:
                    return new Rectangle(xOffset, 192, 64, 64);
                case PickupType.SpellEffectChance:
                    return new Rectangle(128 + xOffset, 0, 64, 64);
                case PickupType.AreaOfEffect:
                    return new Rectangle(128 + xOffset, 64, 64, 64);
                case PickupType.Fireball:
                    return new Rectangle(xOffset, 256, 64, 64);
                case PickupType.FireExplosion:
                    return new Rectangle(xOffset, 320, 64, 64);
                case PickupType.IceShard:
                    return new Rectangle(xOffset, 384, 64, 64);
                case PickupType.IceSpikes:
                    return new Rectangle(xOffset, 448, 64, 64);
                case PickupType.LightningBlast:
                    return new Rectangle(128 + xOffset, 256, 64, 64);
                case PickupType.LightningStrike:
                    return new Rectangle(128 + xOffset, 320, 64, 64);
                default:
                    return new Rectangle();
            }
        }
    }
}
