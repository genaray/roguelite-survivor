using Arch.Core;
using Arch.Core.Extensions;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Helpers
{
    public static class AttackHelpers
    {
        public static void SetEnemyHealthAndState(Entity entity, EntityStatus entityStatus, Damage damage, Owner owner, SpellEffects? spellEffects = null)
        {
            if (entityStatus.State == State.Alive)
            {
                SpellEffects spellEffectToUse = spellEffects ?? damage.SpellEffect;
                Health health = entity.Get<Health>();
                bool hasPoison = entity.Has<Poison>();
                health.Current -= (int)(damage.Amount * (hasPoison ? 1.2f : 1f));
                if (owner.EntityReference.Entity.Has<DoubleDamage>())
                {
                    health.Current -= (int)(damage.Amount * (hasPoison ? 1.2f : 1f));
                }
                if (health.Current < 1)
                {
                    entityStatus.State = State.ReadyToDie;
                    entity.Set(entityStatus);
                    Experience enemyExperience = entity.Get<Experience>();
                    KillCount killCount = owner.EntityReference.Entity.Get<KillCount>();
                    Player playerExperience = owner.EntityReference.Entity.Get<Player>();
                    killCount.AddKill(entity.Get<Enemy>().Name);
                    if (owner.EntityReference.Entity.Has<DoubleExperience>())
                    {
                        playerExperience.TotalExperience += enemyExperience.Amount * 2;
                        playerExperience.ExperienceToNextLevel -= enemyExperience.Amount * 2;
                    }
                    else
                    {
                        playerExperience.TotalExperience += enemyExperience.Amount;
                        playerExperience.ExperienceToNextLevel -= enemyExperience.Amount;
                    }
                    owner.EntityReference.Entity.Set(killCount, playerExperience);
                }
                else
                {
                    Animation anim = entity.Get<Animation>();
                    anim.Overlay = Microsoft.Xna.Framework.Color.Red;

                    entity.Set(health, anim);
                    if (damage.SpellEffect != SpellEffects.None)
                    {
                        switch (damage.SpellEffect)
                        {
                            case SpellEffects.Burn:
                                if (!entity.Has<Burn>())
                                {
                                    entity.Add(new Burn() { TimeLeft = 5f, TickRate = .5f, NextTick = .5f });
                                }
                                else
                                {
                                    entity.Set(new Burn() { TimeLeft = 5f, TickRate = .5f, NextTick = .5f });
                                }
                                break;
                            case SpellEffects.Slow:
                                if (!entity.Has<Slow>())
                                {
                                    entity.Add(new Slow() { TimeLeft = 5f });
                                }
                                else
                                {
                                    entity.Set(new Slow() { TimeLeft = 5f });
                                }
                                break;
                            case SpellEffects.Shock:
                                if (!entity.Has<Shock>())
                                {
                                    entity.Add(new Shock() { TimeLeft = 1f });
                                }
                                else
                                {
                                    entity.Set(new Shock() { TimeLeft = 1f });
                                }
                                break;
                            case SpellEffects.Poison:
                                if (!entity.Has<Poison>())
                                {
                                    entity.Add(new Poison() { TimeLeft = 5f });
                                }
                                else
                                {
                                    entity.Set(new Poison() { TimeLeft = 5f });
                                }
                                break;
                        }
                    }
                }
            }
        }

    }
}
