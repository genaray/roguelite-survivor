using Arch.Core;
using Arch.Core.Extensions;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using System.Numerics;

namespace RogueliteSurvivor.Physics
{
    public class GameContactFilter : ContactFilter
    {
        public override bool ShouldCollide(Fixture fixtureA, Fixture fixtureB)
        {
            bool retVal = false;
            Entity a = (Entity)fixtureA.Body.UserData;
            Entity b = (Entity)fixtureB.Body.UserData;

            if (a.Has<Map>() || b.Has<Map>())
            {
                if (a.Has<Projectile>() || b.Has<Projectile>()
                    || a.Has<CanFly>() || b.Has<CanFly>()
                    || a.Has<EnemyProjectile>() || b.Has<EnemyProjectile>())
                {
                    Vector2 position;
                    MapInfo map;
                    if (a.Has<Map>())
                    {
                        position = fixtureA.Body.Position * PhysicsConstants.PhysicsToPixelsRatio;
                        map = a.Get<MapInfo>();
                    }
                    else
                    {
                        position = fixtureB.Body.Position * PhysicsConstants.PhysicsToPixelsRatio;
                        map = b.Get<MapInfo>();
                    }

                    retVal = map.IsTileFullHeight((int)position.X, (int)position.Y);
                }
                else
                {
                    retVal = true;
                }
            }
            else if (a.Has<Enemy>()
                    && b.Has<Enemy>()
                    && a.Get<EntityStatus>().State == State.Alive
                    && b.Get<EntityStatus>().State == State.Alive
                    && ((a.Has<CanFly>() && b.Has<CanFly>()) || (!a.Has<CanFly>() && !b.Has<CanFly>())))
            {
                retVal = true;
            }
            else if (((a.Has<Projectile>() && b.Has<Enemy>()) || (b.Has<Projectile>() && a.Has<Enemy>()))
                    && a.Get<EntityStatus>().State == State.Alive
                    && b.Get<EntityStatus>().State == State.Alive)
            {
                retVal = true;
            }
            else if ((a.Has<SingleTarget>() && fixtureA.Body.IsAwake() && b.Has<Enemy>() && b.Get<EntityStatus>().State == State.Alive) || (b.Has<SingleTarget>() && fixtureB.Body.IsAwake() && a.Has<Enemy>() && a.Get<EntityStatus>().State == State.Alive))
            {
                retVal = true;
            }
            else if ((a.Has<Player>() && (b.Has<Enemy>() || b.Has<EnemyProjectile>()) && b.Get<EntityStatus>().State == State.Alive) || (b.Has<Player>() && (a.Has<Enemy>() || a.Has<EnemyProjectile>()) && a.Get<EntityStatus>().State == State.Alive))
            {
                retVal = true;
            }

            return retVal;
        }
    }
}
