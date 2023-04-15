using Arch.Core;
using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.Bodies;
using RogueliteSurvivor.Constants;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Extensions;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TiledCS;

namespace RogueliteSurvivor.Components
{
    public enum MovementType
    {
        Ground,
        Air,
    }
    public class MapInfo
    {
        public TiledMap Map { get; set; }
        public Dictionary<int, TiledTileset> Tilesets { get; set; }
        private List<SpawnableAreaContainer> spawnableAreas;

        private Dictionary<MovementType, Grid> grids;
        private PathFinder pathFinder;

        public MapInfo(string mapPath, string tilesetPath, Box2D.NetStandard.Dynamics.World.World physicsWorld, Entity mapEntity, List<SpawnableAreaContainer> spawnableAreas)
        {
            Map = new TiledMap(mapPath);
            Tilesets = Map.GetTiledTilesetsCrossPlatform(tilesetPath);
            this.spawnableAreas = spawnableAreas;

            var tileLayers = Map.Layers.Where(x => x.type == TiledLayerType.TileLayer);

            var gridSize = new GridSize(Map.Width, Map.Height);
            var cellSize = new Size(Distance.FromMeters(1), Distance.FromMeters(1));
            var traversalVelocity = Roy_T.AStar.Primitives.Velocity.FromKilometersPerHour(4.5f);

            grids = new Dictionary<MovementType, Grid>
            {
                { MovementType.Ground, Grid.CreateGridWithLateralAndDiagonalConnections(gridSize, cellSize, traversalVelocity) },
                { MovementType.Air, Grid.CreateGridWithLateralAndDiagonalConnections(gridSize, cellSize, traversalVelocity) }
            };

            pathFinder = new PathFinder();

            for (int y = 0; y < Map.Width; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    bool passable = true;
                    string collisionShape = "SQ";
                    bool fullHeight = false;
                    foreach (var layer in tileLayers)
                    {
                        if (layer.properties[0].value == "true")
                        {
                            var tile = getTile(layer, x, y);

                            if (tile != null)
                            {
                                passable = !(tile.properties.Where(a => a.name == "Passable").First().value == "false");
                                collisionShape = tile.properties.Where(a => a.name == "Collision Shape").First().value;
                                fullHeight = (tile.properties.Where(a => a.name == "Passable").First().value == "true");
                            }
                        }
                    }

                    if (!passable)
                    {
                        grids[MovementType.Ground].DisconnectNode(new GridPosition(x, y));
                        grids[MovementType.Ground].RemoveDiagonalConnectionsIntersectingWithNode(new GridPosition(x, y));
                        if (fullHeight)
                        {
                            grids[MovementType.Air].DisconnectNode(new GridPosition(x, y));
                            grids[MovementType.Air].RemoveDiagonalConnectionsIntersectingWithNode(new GridPosition(x, y));
                        }

                        int tileX = x * Map.TileWidth + Map.TileWidth / 2;
                        int tileY = y * Map.TileHeight + Map.TileHeight / 2;

                        var body = new BodyDef();
                        body.position = new Vector2(tileX, tileY) / PhysicsConstants.PhysicsToPixelsRatio;
                        body.fixedRotation = true;
                        body.type = BodyType.Static;


                        var bodyShape = new Box2D.NetStandard.Dynamics.Fixtures.FixtureDef();
                        bodyShape.shape = getTileShape(Map.TileWidth, Map.TileHeight, collisionShape);
                        bodyShape.density = 1;
                        bodyShape.friction = 0.0f;

                        var PhysicsBody = physicsWorld.CreateBody(body);
                        PhysicsBody.CreateFixture(bodyShape);
                        PhysicsBody.SetUserData(mapEntity);
                    }
                }
            }
        }

        public bool IsTilePassable(int x, int y)
        {
            var tileLayers = Map.Layers.Where(x => x.type == TiledLayerType.TileLayer);
            bool passable = spawnableAreas.Exists(a => a.SpawnMinX <= x && a.SpawnMaxX >= x && a.SpawnMinY <= y && a.SpawnMaxY >= y);

            if (passable)
            {
                foreach (var layer in tileLayers)
                {
                    if (layer.properties[0].value == "true")
                    {
                        var tile = getTile(layer, x / Map.TileWidth, y / Map.TileHeight);

                        if (tile != null && tile.properties.Where(a => a.name == "Passable").First().value == "false")
                        {
                            passable = false;
                        }
                    }
                }
            }

            return passable;
        }

        public bool IsTileFullHeight(int x, int y)
        {
            bool fullHeight = true;
            var tileLayers = Map.Layers.Where(x => x.type == TiledLayerType.TileLayer);

            foreach (var layer in tileLayers)
            {
                if (layer.properties[0].value == "true")
                {
                    var tile = getTile(layer, x / Map.TileWidth, y / Map.TileHeight);

                    if (tile != null && tile.properties.Where(a => a.name == "Full Height").First().value == "false")
                    {
                        fullHeight = false;
                    }
                }
            }
            return fullHeight;
        }

        public Microsoft.Xna.Framework.Vector2 GetNextPathStep(Microsoft.Xna.Framework.Vector2 start, Microsoft.Xna.Framework.Vector2 destination, MovementType movementType = MovementType.Ground)
        {
            var path = pathFinder.FindPath(
                new GridPosition((int)(start.X / Map.TileWidth), (int)(start.Y / Map.TileHeight))
                , new GridPosition((int)(destination.X / Map.TileWidth), (int)(destination.Y / Map.TileHeight))
                , grids[movementType]
                );

            if (path != null && path.Edges.Count > 0)
            {
                var point = path.Edges.First().End;
                return new Microsoft.Xna.Framework.Vector2(point.Position.X * Map.TileWidth + (Map.TileWidth / 2), point.Position.Y * Map.TileHeight + (Map.TileHeight / 2));
            }
            else
            {
                return destination;
            }
        }

        private TiledTile getTile(TiledLayer layer, int x, int y)
        {
            int index = (y * layer.width) + x;
            int gid = layer.data[index];

            var mapTileset = Map.GetTiledMapTileset(gid);
            var tileset = Tilesets[mapTileset.firstgid];

            return Map.GetTiledTile(mapTileset, tileset, gid);
        }

        private PolygonShape getTileShape(int tileWidth, int tileHeight, string collisionShape)
        {
            PolygonShape shape = null;
            float hx = tileWidth / 2f / PhysicsConstants.PhysicsToPixelsRatio;
            float hy = tileHeight / 2f / PhysicsConstants.PhysicsToPixelsRatio;

            switch (collisionShape)
            {
                case "SQ":
                    shape = new PolygonShape(hx, hy);
                    break;
                case "SE":
                    shape = new PolygonShape(new Vector2(0f - hx, 0f - hy), new Vector2(hx, 0f - hy), new Vector2(0f - hx, hy));
                    break;
                case "NE":
                    shape = new PolygonShape(new Vector2(0f - hx, 0f - hy), new Vector2(hx, hy), new Vector2(0f - hx, hy));
                    break;
                case "NW":
                    shape = new PolygonShape(new Vector2(hx, 0f - hy), new Vector2(hx, hy), new Vector2(0f - hx, hy));
                    break;
                case "SW":
                    shape = new PolygonShape(new Vector2(hx, 0f - hy), new Vector2(hx, hy), new Vector2(0f - hx, 0f - hy));
                    break;
            }


            return shape;
        }
    }
}
