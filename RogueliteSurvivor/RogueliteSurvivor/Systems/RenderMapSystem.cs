﻿using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueliteSurvivor.Components;
using RogueliteSurvivor.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using TiledCS;

namespace RogueliteSurvivor.Systems
{
    public class TiledAnimation
    {
        public float Timer { get; set; }
        public int CurrentAnimationId { get; set; }
    }
    public class RenderMapSystem : ArchSystem, IRenderSystem
    {
        GraphicsDeviceManager graphics;
        Dictionary<TiledTileset, Dictionary<int, Rectangle>> usedRects;
        Dictionary<TiledTileset, Dictionary<Point, TiledAnimation>> animationTimers;
        public RenderMapSystem(World world, GraphicsDeviceManager graphics)
            : base(world, new QueryDescription()
                                .WithAll<MapInfo>())
        {
            this.graphics = graphics;
            usedRects = new Dictionary<TiledTileset, Dictionary<int, Rectangle>>();
            animationTimers = new Dictionary<TiledTileset, Dictionary<Point, TiledAnimation>>();
        }

        public void Render(GameTime gameTime, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, Entity player, float totalElapsedTime, GameState gameState, int layer, float scaleFactor)
        {
            Vector2 playerPosition = player.Get<Position>().XY;
            Vector2 offset = new Vector2(GetWidthOffset(graphics, scaleFactor, 2), GetHeightOffset(graphics, scaleFactor, 2));

            world.Query(in query, (ref MapInfo map) =>
            {
                foreach (var tileLayer in map.Map.Layers.Where(x => x.type == TiledLayerType.TileLayer && x.@class == layer.ToString()))
                {
                    if (tileLayer != null)
                    {
                        int minY, maxY, minX, maxX;
                        minY = (int)MathF.Max((playerPosition.Y - graphics.PreferredBackBufferHeight / 2) / 16f, 0);
                        maxY = (int)MathF.Min((playerPosition.Y + graphics.PreferredBackBufferHeight / 2) / 16f, tileLayer.height);
                        minX = (int)MathF.Max((playerPosition.X - graphics.PreferredBackBufferWidth / 2) / 16f, 0);
                        maxX = (int)MathF.Min((playerPosition.X + graphics.PreferredBackBufferWidth / 2) / 16f, tileLayer.width);

                        for (var y = minY; y < maxY; y++)
                        {
                            for (var x = minX; x < maxX; x++)
                            {
                                var index = (y * tileLayer.width) + x;
                                var gid = tileLayer.data[index];
                                var tileX = x * map.Map.TileWidth;
                                var tileY = y * map.Map.TileHeight;

                                if (gid == 0)
                                {
                                    continue;
                                }

                                var mapTileset = map.Map.GetTiledMapTileset(gid);

                                var tileset = map.Tilesets[mapTileset.firstgid];

                                if (!usedRects.ContainsKey(tileset))
                                {
                                    usedRects.Add(tileset, new Dictionary<int, Rectangle>());
                                }
                                string path = tileset.Image.source.Replace(".png", "").ToLower();

                                var tile = map.Map.GetTiledTile(mapTileset, tileset, gid);
                                if (tile.animation != null && tile.animation.Length > 0)
                                {
                                    Point xy = new Point(x, y);
                                    if (!animationTimers.ContainsKey(tileset))
                                    {
                                        animationTimers.Add(tileset, new Dictionary<Point, TiledAnimation>());
                                        animationTimers[tileset].Add(xy, new TiledAnimation() { Timer = tile.animation[0].duration / 1000f, CurrentAnimationId = 0 });
                                    }
                                    else if (!animationTimers[tileset].ContainsKey(xy))
                                    {
                                        animationTimers[tileset].Add(xy, new TiledAnimation() { Timer = tile.animation[0].duration / 1000f, CurrentAnimationId = 0 });
                                    }
                                    else
                                    {
                                        animationTimers[tileset][xy].Timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                                        if (animationTimers[tileset][xy].Timer <= 0)
                                        {
                                            animationTimers[tileset][xy].CurrentAnimationId = (animationTimers[tileset][xy].CurrentAnimationId + 1) % tile.animation.Length;
                                            animationTimers[tileset][xy].Timer += tile.animation[animationTimers[tileset][xy].CurrentAnimationId].duration / 1000f;
                                        }
                                        gid = tile.animation[animationTimers[tileset][xy].CurrentAnimationId].tileid + mapTileset.firstgid;
                                    }
                                }

                                if (!usedRects[tileset].ContainsKey(gid))
                                {
                                    var rect = map.Map.GetSourceRect(mapTileset, tileset, gid);
                                    usedRects[tileset].Add(gid, new Rectangle(rect.x, rect.y, rect.width, rect.height));
                                }

                                var destination = new Rectangle(tileX, tileY, map.Map.TileWidth, map.Map.TileHeight);

                                spriteBatch.Draw(textures[path], new Vector2(tileX + offset.X, tileY + offset.Y), usedRects[tileset][gid], Color.White, 0f, playerPosition, 1f, SpriteEffects.None, .05f * layer);
                            }
                        }
                    }
                }
            });
        }
    }
}
