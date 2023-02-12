﻿using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Systems
{
    public class EnemySpawnSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription playerQuery = new QueryDescription()
                                            .WithAll<Player, Position>();
        Dictionary<string, Texture2D> textures;
        Random random;
        Box2D.NetStandard.Dynamics.World.World physicsWorld;

        public EnemySpawnSystem(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld)
            : base(world, new QueryDescription()
                                .WithAll<Enemy>())
        { 
            this.textures = textures;
            this.physicsWorld = physicsWorld;

            random = new Random();
        }

        public void Update(GameTime gameTime) 
        {
            int numEnemies = 0;
            world.Query(in query, (in Entity entity, ref Enemy enemy) =>
            {
                if(enemy.State == EnemyState.Alive)
                {
                    numEnemies++;
                }
                else
                {
                    world.Destroy(entity);
                }
            });

            if(numEnemies < 20)
            {
                for(int i = numEnemies; i < 20; i++)
                {
                    var body = new Box2D.NetStandard.Dynamics.Bodies.BodyDef();
                    body.position = new System.Numerics.Vector2(random.Next(32, 768), random.Next(32, 768));
                    body.fixedRotation = true;

                    var entity = world.Create(
                        new Enemy() { State = EnemyState.Alive },
                        new Position() { XY = new Vector2(body.position.X, body.position.Y) },
                        new Velocity() { Vector = Vector2.Zero },
                        new Speed() { speed = 8000f },
                        new Animation(0, 3, .1f, 2),
                        new SpriteSheet(textures["vampire_bat"], "vampire_bat", 4, 2),
                        new Collider(16, 16, physicsWorld, body)
                    );

                    entity.Get<Collider>().SetEntityForPhysics(entity);
                }
            }
        }
    }
}
