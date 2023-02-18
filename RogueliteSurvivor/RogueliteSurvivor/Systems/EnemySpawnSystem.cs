﻿using Arch.Core;
using Arch.Core.Extensions;
using Box2D.NetStandard.Dynamics.Bodies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Physics;
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
        GraphicsDeviceManager graphics;

        const int ENEMY_COUNT = 20;

        public EnemySpawnSystem(World world, Dictionary<string, Texture2D> textures, Box2D.NetStandard.Dynamics.World.World physicsWorld, GraphicsDeviceManager graphics)
            : base(world, new QueryDescription()
                                .WithAll<Enemy>())
        { 
            this.textures = textures;
            this.physicsWorld = physicsWorld;
            this.graphics = graphics;

            random = new Random();
        }

        public void Update(GameTime gameTime, float totalElapsedTime) 
        {
            int numEnemies = 0;

            Vector2 offset = new Vector2(graphics.PreferredBackBufferWidth / 6, graphics.PreferredBackBufferHeight / 6);
            Position? player = null;
            world.Query(in playerQuery, (ref Position playerPos) =>
            {
                if (!player.HasValue)
                {
                    player = playerPos;
                }
            });

            world.Query(in query, (in Entity entity, ref Enemy enemy) =>
            {
                numEnemies++;

                if (enemy.State == EntityState.Dead)
                {
                    enemy.State = EntityState.Alive;

                    var body = (Body)entity.Get(typeof(Body));
                    var position = entity.Get<Position>();
                    var health = entity.Get<Health>();

                    body.SetTransform(getSpawnPosition(player.Value.XY, offset), 0);
                    position.XY = new Vector2(body.Position.X, body.Position.Y);
                    health.Current = health.Max;

                    entity.SetRange(body, position, health);
                }
            });

            if(numEnemies < ENEMY_COUNT)
            {
                for(int i = numEnemies; i < ENEMY_COUNT; i++)
                {
                    var body = new BodyDef();
                    body.position = getSpawnPosition(player.Value.XY, offset);
                    body.fixedRotation = true;

                    var entity = world.Create<Enemy, Position, Velocity, Speed, Animation, SpriteSheet, Target, Health, Damage, AttackSpeed, Body>();

                    entity.SetRange(
                        new Enemy() { State = EntityState.Alive },
                        new Position() { XY = new Vector2(body.position.X, body.position.Y) },
                        new Velocity() { Vector = Vector2.Zero },
                        new Speed() { speed = 2000f },
                        new Animation(0, 3, .1f, 2),
                        new SpriteSheet(textures["vampire_bat"], "vampire_bat", 4, 2),
                        new Target(),
                        new Health() { Current = 10, Max = 10 },
                        new Damage() { Amount = 2 },
                        new AttackSpeed() { BaseAttackSpeed = 0.5f, CurrentAttackSpeed = 0.5f, Cooldown = 0 },
                        BodyFactory.CreateCircularBody(entity, 16, physicsWorld, body)
                    );
                }
            }
        }

        private System.Numerics.Vector2 getSpawnPosition(Vector2 playerPosition, Vector2 offset)
        {
            int x, y;
            do
            {
                x = random.Next(64, 736);
                y = random.Next(64, 736);
            } while ((x > (playerPosition.X - offset.X) && x < (playerPosition.X + offset.X)) && (y > (playerPosition.Y - offset.Y) && y < (playerPosition.Y + offset.Y)));

            return new System.Numerics.Vector2(x, y);
        }
    }
}
