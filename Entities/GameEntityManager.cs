using _2DGame.Entities.Collectibles;
using _2DGame.Entities.Enemies;
using _2DGame.Entities.Players;
using _2DGame.LayerData;
using _2DGame.Layers;
using NetTopologySuite.Algorithm;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TransformableHitbox2D;

namespace _2DGame.Entities
{
    public class GameEntityManager
    {
        private JsonEntity?[] loadedData;
        public bool IsLoaded { get; private set; }
        public Vector2i PlayerStartTileCoordinates { get; private set; }
        public List<GameEntity> GameEntities { get; set; }
        [JsonIgnore]
        public List<GameEntity> OnScreenGameEntities { get; set; }
        [JsonIgnore]
        public List<GameEntity> OffScreenGameEntities { get; set; }

        public const string LEVELS_PATH = "./Data/Levels/";
        public const int X_AREA = 26;
        public const int Y_AREA = 18;

        public GameEntityManager()
        {
            GameEntities = new();
            OnScreenGameEntities = new();
            OffScreenGameEntities = new();
        }

        public void Load(string entityDataFilename)
        {
            if (IsLoaded) return;

            string fileName = LEVELS_PATH + entityDataFilename;
            string jsonString = File.ReadAllText(fileName);

            loadedData = JsonSerializer.Deserialize<JsonEntity[]>(jsonString)!;

            CreateEntitiesFromLoadedData();

            IsLoaded = true;
        }

        public void Reload()
        {
            GameEntities.Clear();
            OnScreenGameEntities.Clear();
            OffScreenGameEntities.Clear();

            CreateEntitiesFromLoadedData();
        }

        public void Initialize()
        {
            foreach (var gameEntity in GameEntities)
            {
                gameEntity.Initialize(new Vector2i(gameEntity.TileCoordinates.X, gameEntity.TileCoordinates.Y));
            }
        }

        public void Update(Level level, Player player, GameLoop gameLoop)
        {
            DeleteInactiveGameEntities();
            UpdateEntityDistanceLists(player);

            foreach (var onScreenGameEntity in OnScreenGameEntities)
            {
                if (onScreenGameEntity.IsActive)
                {
                    onScreenGameEntity.Update(level, gameLoop);
                    if (onScreenGameEntity is IEnemy)
                    {
                        (onScreenGameEntity as IEnemy).OnPlayerDetection(player);
                    }
                }
            }
            foreach (var offScreenGameEntity in OffScreenGameEntities)
            {
                offScreenGameEntity.Reset();
            }
        }

        private void DeleteInactiveGameEntities()
        {
            foreach (var gameEntity in GameEntities.ToList())
            {
                if (!gameEntity.IsActive)
                {
                    GameEntities.Remove(gameEntity);
                }
            }
        }

        private void UpdateEntityDistanceLists(Player player)
        {
            OnScreenGameEntities.Clear();
            OffScreenGameEntities.Clear();

            TileCoordinates cameraTileCoordinates = new TileCoordinates(
                (int)(player.Camera.Center.X / Tilemap.TILE_SIZE),
                (int)(player.Camera.Center.Y / Tilemap.TILE_SIZE));

            foreach (var gameEntity in GameEntities)
            {
                if (gameEntity.TileCoordinates.X >= cameraTileCoordinates.X - X_AREA &&
                    gameEntity.TileCoordinates.X <= cameraTileCoordinates.X + X_AREA &&
                    gameEntity.TileCoordinates.Y >= cameraTileCoordinates.Y - Y_AREA &&
                    gameEntity.TileCoordinates.Y <= cameraTileCoordinates.Y + Y_AREA)
                {
                    OnScreenGameEntities.Add(gameEntity);
                }
                else
                {
                    OffScreenGameEntities.Add(gameEntity);
                }
            }
        }

        private void CreateEntitiesFromLoadedData()
        {
            foreach (var entity in loadedData)
            {
                switch (entity.ID)
                {
                    case 0:
                        PlayerStartTileCoordinates = new Vector2i(entity.TileCoordinates.X, entity.TileCoordinates.Y);
                        break;

                    case 1:
                        GameEntities.Add(new Gem()
                        {
                            TileCoordinates = new TileCoordinates(entity.TileCoordinates),
                            Position = new Vector2f(
                                entity.TileCoordinates.X * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2,
                                entity.TileCoordinates.Y * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2)
                        });
                        break;

                    case 3:
                        GameEntities.Add(new FlyingEye()
                        {
                            TileCoordinates = new TileCoordinates(entity.TileCoordinates),
                            Position = new Vector2f(
                                entity.TileCoordinates.X * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2,
                                entity.TileCoordinates.Y * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2)
                        });
                        break;

                    case 4:
                        GameEntities.Add(new Heart()
                        {
                            TileCoordinates = new TileCoordinates(entity.TileCoordinates),
                            Position = new Vector2f(
                                entity.TileCoordinates.X * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2,
                                entity.TileCoordinates.Y * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2)
                        });
                        break;

                    case 5:
                        GameEntities.Add(new Mushroom()
                        {
                            TileCoordinates = new TileCoordinates(entity.TileCoordinates),
                            Position = new Vector2f(
                                entity.TileCoordinates.X * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2,
                                entity.TileCoordinates.Y * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2)
                        });
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
