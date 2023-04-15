using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;

namespace RogueliteSurvivor.Helpers
{
    public static class PickupHelper
    {
        public static Rectangle GetPickupSourceRectangle(PickupType pickupType)
        {
            int x = 0, y = 0;

            switch (pickupType)
            {
                case PickupType.Health:
                    x = 208;
                    y = 224;
                    break;
                case PickupType.Invincibility:
                    x = 64;
                    y = 0;
                    break;
                case PickupType.DoubleExperience:
                    x = 192;
                    y = 256;
                    break;
                case PickupType.DoubleDamage:
                    x = 112;
                    y = 336;
                    break;
                case PickupType.DoubleAttackSpeed:
                    x = 128;
                    y = 64;
                    break;
                case PickupType.Book:
                    x = 128;
                    y = 304;
                    break;
            }

            return new Rectangle(x, y, 16, 16);
        }

        public static float GetPickupAmount(PickupType pickupType)
        {
            float pickup = 0f;

            switch (pickupType)
            {
                case PickupType.Health:
                    pickup = 5f;
                    break;
                case PickupType.Invincibility:
                    pickup = 15f;
                    break;
                case PickupType.DoubleExperience:
                    pickup = 30f;
                    break;
                case PickupType.DoubleDamage:
                    pickup = 30f;
                    break;
                case PickupType.DoubleAttackSpeed:
                    pickup = 30f;
                    break;
            }

            return pickup;
        }

        public static bool ProcessPickup(EntityReference player, PickupType pickupType)
        {
            bool destroy = true;
            float pickupAmount = GetPickupAmount(pickupType);
            switch (pickupType)
            {
                case PickupType.Health:
                    var health = player.Entity.Get<Health>();
                    if (health.Current < health.Max)
                    {
                        health.Current = int.Min(health.Max, (int)pickupAmount + health.Current);
                        player.Entity.Set(health);
                    }
                    else
                    {
                        destroy = false;
                    }
                    break;
                case PickupType.Invincibility:
                    if (player.Entity.TryGet(out Invincibility invincibility))
                    {
                        invincibility.TimeRemaining = pickupAmount;
                        invincibility.MaxTime = pickupAmount;
                        player.Entity.Set(invincibility);
                    }
                    else
                    {
                        player.Entity.Add(new Invincibility() { TimeRemaining = pickupAmount, MaxTime = pickupAmount });
                    }
                    break;
                case PickupType.DoubleExperience:
                    if (player.Entity.TryGet(out DoubleExperience doubleExperience))
                    {
                        doubleExperience.TimeRemaining = pickupAmount;
                        doubleExperience.MaxTime = pickupAmount;
                        player.Entity.Set(doubleExperience);
                    }
                    else
                    {
                        player.Entity.Add(new DoubleExperience() { TimeRemaining = pickupAmount, MaxTime = pickupAmount });
                    }
                    break;
                case PickupType.DoubleDamage:
                    if (player.Entity.TryGet(out DoubleDamage doubleDamage))
                    {
                        doubleDamage.TimeRemaining = pickupAmount;
                        doubleDamage.MaxTime = pickupAmount;
                        player.Entity.Set(doubleDamage);
                    }
                    else
                    {
                        player.Entity.Add(new DoubleDamage() { TimeRemaining = pickupAmount, MaxTime = pickupAmount });
                    }
                    break;
                case PickupType.DoubleAttackSpeed:
                    if (player.Entity.TryGet(out DoubleAttackSpeed doubleAttackSpeed))
                    {
                        doubleAttackSpeed.TimeRemaining = pickupAmount;
                        doubleAttackSpeed.MaxTime = pickupAmount;
                        player.Entity.Set(doubleAttackSpeed);
                    }
                    else
                    {
                        player.Entity.Add(new DoubleAttackSpeed() { TimeRemaining = pickupAmount, MaxTime = pickupAmount });
                    }
                    break;
                case PickupType.Book:
                    KillCount killCount = player.Entity.Get<KillCount>();
                    killCount.NumBooks++;
                    player.Entity.Set(killCount);
                    break;
            }

            return destroy;
        }


    }
}
