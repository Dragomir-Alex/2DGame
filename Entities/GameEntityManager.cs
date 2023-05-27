using _2DGame.LayerData;
using _2DGame.Layers;
using SFML.Graphics;
using System;
using System.Collections.Generic;
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
                    case 2:
                        GameEntities.Add(new Gem() {
                            TileCoordinates = entity.TileCoordinates,
                            Position = new SFML.System.Vector2f(entity.TileCoordinates.X * Tilemap.TILE_SIZE, entity.TileCoordinates.Y * Tilemap.TILE_SIZE)
                        });
                        break;

                    default:
                        break;
                }
            }
        }

        public void Update(Level level, Player player, GameLoop gameLoop)
        {
            UpdateEntityDistanceLists(player);

            foreach (var onScreenGameEntity in OnScreenGameEntities)
            {
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
                    gameEntity.TileCoordinates.Y >= player.TileCoordinates.X - yArea &&
                    gameEntity.TileCoordinates.Y <= player.TileCoordinates.X + yArea)
                {
                    OnScreenGameEntities.Add(gameEntity);
                }
                else
                {
                    OffScreenGameEntities.Add(gameEntity);
                }
            }
        }
    }
}
