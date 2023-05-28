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

        public List<GameEntity> GameEntities { get; set; }
        [JsonIgnore]
        public List<GameEntity> OnScreenGameEntities { get; set; }
        [JsonIgnore]
        public List<GameEntity> OffScreenGameEntities { get; set; }
        public const string LEVELS_PATH = "./Data/Levels/";

        public GameEntityManager()
        {
            GameEntities = new();
            OnScreenGameEntities = new();
            OffScreenGameEntities = new();
        }

        public void Load(string entityDataFilename)
        {
            string fileName = LEVELS_PATH + entityDataFilename;
            string jsonString = File.ReadAllText(fileName);
            var loadedData = JsonSerializer.Deserialize<Gem[]>(jsonString)!;

            foreach (var entity in loadedData) 
            { 
                switch (entity.ID)
                {
                    case 1:
                        GameEntities.Add(new Gem() {
                            TileCoordinates = entity.TileCoordinates,
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
                    onScreenGameEntity.Update(level, gameLoop);
            }
            foreach (var offScreenGameEntity in OffScreenGameEntities)
            {
                offScreenGameEntity.Reset();
            }
        }

        public void InitializeSprites(RenderTarget renderTarget)
        {
/*            foreach (var gameEntity in GameEntities)
                gameEntity.InitializeSprite(r);*/
        }

        public void ResetAllGameEntities()
        {
            foreach (var gameEntity in GameEntities)
            {
                gameEntity.Reset();
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

            int xArea = 25;
            int yArea = 25;

            foreach (var gameEntity in GameEntities)
            {
                if (gameEntity.TileCoordinates.X >= player.TileCoordinates.X - xArea &&
                    gameEntity.TileCoordinates.X <= player.TileCoordinates.X + xArea &&
                    gameEntity.TileCoordinates.Y >= player.TileCoordinates.Y - yArea &&
                    gameEntity.TileCoordinates.Y <= player.TileCoordinates.Y + yArea)
                {
                    OnScreenGameEntities.Add(gameEntity);
                }
                else
                {
                    OffScreenGameEntities.Add(gameEntity);
                }

                // Debug.WriteLine("Entity - X: " + gameEntity.TileCoordinates.X + ";   Y: " + gameEntity.TileCoordinates.Y);
            }

            // Debug.WriteLine(GameEntities.Count);
        }
    }
}
