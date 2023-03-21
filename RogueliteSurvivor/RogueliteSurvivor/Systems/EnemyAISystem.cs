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
        public EnemyAISystem(World world)
            : base(world, new QueryDescription()
                                .WithAll<Enemy, Position, Velocity, Speed, Target>())
        { }

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

            world.Query(in query, (ref Position pos, ref Velocity vel, ref Speed sp, ref Target target) =>
            {
                vel.Vector = Vector2.Normalize(target.TargetPosition - pos.XY);

                if (!map.IsTileWalkable((int)(pos.XY.X + vel.Vector.X), (int)(pos.XY.Y + vel.Vector.Y)))
                {
                    if(map.IsTileWalkable((int)(pos.XY.X + vel.Vector.X), (int)(pos.XY.Y)))
                    {
                        vel.Vector = Vector2.Normalize(vel.Vector * Vector2.UnitX);
                    }
                    else if (map.IsTileWalkable((int)(pos.XY.X), (int)(pos.XY.Y + vel.Vector.Y)))
                    {
                        vel.Vector = Vector2.Normalize(vel.Vector * Vector2.UnitY);
                    }
                }

                vel.Vector *= sp.speed;
            });
        }
    }
}
