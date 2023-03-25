using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using RogueliteSurvivor.Components;
using System;

namespace RogueliteSurvivor.Systems
{
    public class EnemyAISystem : ArchSystem, IUpdateSystem
    {
        QueryDescription mapQuery = new QueryDescription()
                                            .WithAll<MapInfo>();
        QueryDescription playerQuery = new QueryDescription()
                                            .WithAll<Player>();

        public EnemyAISystem(World world)
            : base(world, new QueryDescription()
                                .WithAll<Enemy, Position, Velocity, Speed, Target>())
        { }

        int modulus = 0;
        int maxModulus = 30;

        public void Update(GameTime gameTime, float totalElapsedTime, float scaleFactor)
        {
            MapInfo map = null;
            world.Query(in mapQuery, (ref MapInfo mapInfo) =>
            {
                if (map == null)
                {
                    map = mapInfo;
                }
            });

            world.Query(in query, (in Entity entity, ref EntityStatus status, ref Position pos, ref Velocity vel, ref Speed sp, ref Target target) =>
            {
                target.TargetPosition = findTarget(playerQuery, pos.XY);
                if ( ((entity.Id % maxModulus) - modulus) == 0 
                    && status.State == Constants.State.Alive)
                {
                    Vector2 destination = map.GetNextPathStep(pos.XY, target.TargetPosition, entity.Has<CanFly>() ? MovementType.Air : MovementType.Ground);
                    vel.Vector = Vector2.Normalize(destination - pos.XY);
                    vel.Vector *= sp.speed;
                }
            });
            modulus = (modulus + 1) % maxModulus;
        }

        private Vector2 findTarget(QueryDescription targetQuery, Vector2 sourcePosition)
        {
            Vector2 targetPos = new Vector2(9999, 9999);
            world.Query(in targetQuery, (ref EntityStatus status, ref Position otherPos) =>
            {
                if (status.State == Constants.State.Alive)
                {
                    if (Vector2.Distance(sourcePosition, otherPos.XY) < Vector2.Distance(sourcePosition, targetPos))
                    {
                        targetPos = otherPos.XY;
                    }
                }
            });

            return targetPos;
        }
    }
}
