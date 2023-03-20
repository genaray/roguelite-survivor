﻿using Arch.Core;
using Arch.Core.Extensions;
using Box2D.NetStandard.Dynamics.Bodies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.ComponentFactories;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Physics;
using RogueliteSurvivor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

        public static bool ProcessPickup(ref Entity player, PickupType pickupType)
        {
            bool destroy = true;
            float pickupAmount = GetPickupAmount(pickupType);
            switch (pickupType)
            {
                case PickupType.Health:
                    var health = player.Get<Health>();
                    if (health.Current < health.Max)
                    {
                        health.Current = int.Min(health.Max, (int)pickupAmount + health.Current);
                        player.Set(health);
                    }
                    else
                    {
                        destroy = false;
                    }
                    break;
                case PickupType.Invincibility:
                    if(player.TryGet(out Invincibility invincibility))
                    {
                        invincibility.TimeRemaining = pickupAmount;
                        player.Set(invincibility);
                    }
                    else
                    {
                        player.Add(new Invincibility() { TimeRemaining = pickupAmount });
                    }
                    break;
                case PickupType.DoubleExperience:
                    if (player.TryGet(out DoubleExperience doubleExperience))
                    {
                        doubleExperience.TimeRemaining = pickupAmount;
                        player.Set(doubleExperience);
                    }
                    else
                    {
                        player.Add(new DoubleExperience() { TimeRemaining = pickupAmount });
                    }
                    break;
                case PickupType.DoubleDamage:
                    if (player.TryGet(out DoubleDamage doubleDamage))
                    {
                        doubleDamage.TimeRemaining = pickupAmount;
                        player.Set(doubleDamage);
                    }
                    else
                    {
                        player.Add(new DoubleDamage() { TimeRemaining = pickupAmount });
                    }
                    break;
                case PickupType.DoubleAttackSpeed:
                    if (player.TryGet(out DoubleAttackSpeed doubleAttackSpeed))
                    {
                        doubleAttackSpeed.TimeRemaining = pickupAmount;
                        player.Set(doubleAttackSpeed);
                    }
                    else
                    {
                        player.Add(new DoubleAttackSpeed() { TimeRemaining = pickupAmount });
                    }
                    break;
            }

            return destroy;
        }

        
    }
}
