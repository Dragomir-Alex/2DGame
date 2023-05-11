using _2DGame.Layers;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransformableHitbox2D;

namespace _2DGame.Entities
{
    public class GameEntityManager : Drawable
    {
        public List<GameEntity> GameEntities { get; set; }
        public List<GameEntity> OnScreenGameEntities { get; set; }
        public List<GameEntity> OffScreenGameEntities { get; set; }
        public Player Player { get; set; }

        public GameEntityManager()
        {
            Player = new Player();
            GameEntities = new();
            OnScreenGameEntities = new();
            OffScreenGameEntities = new();
        }

        public void Load()
        {
            // TBA
        }

        public void Update(Level level)
        {
            Player.Update(level);

            UpdateEntityDistanceLists();

            foreach (var onScreenGameEntity in OnScreenGameEntities)
            {
                onScreenGameEntity.Update(level);
            }
            foreach (var offScreenGameEntity in OffScreenGameEntities)
            {
                offScreenGameEntity.Reset();
            }
        }

        public void InitializeSprites()
        {
            foreach (var gameEntity in GameEntities)
                gameEntity.InitializeSprite();
        }

        public void ResetAllGameEntities()
        {
            Player.Reset();
            foreach (var gameEntity in GameEntities)
            {
                gameEntity.Reset();
            }
        }

        private void UpdateEntityDistanceLists()
        {
            OnScreenGameEntities.Clear();
            OffScreenGameEntities.Clear();

            int xArea = 25;
            int yArea = 25;

            foreach (var gameEntity in GameEntities)
            {
                if (gameEntity.TileCoordinates.X >= Player.TileCoordinates.X - xArea &&
                    gameEntity.TileCoordinates.X <= Player.TileCoordinates.X + xArea &&
                    gameEntity.TileCoordinates.Y >= Player.TileCoordinates.X - yArea &&
                    gameEntity.TileCoordinates.Y <= Player.TileCoordinates.X + yArea)
                {
                    OnScreenGameEntities.Add(gameEntity);
                }
                else
                {
                    OffScreenGameEntities.Add(gameEntity);
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var gameEntity in OnScreenGameEntities)
            {
                gameEntity.Draw(target, states);
            }
        }
    }
}
