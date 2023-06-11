using _2DGame.Animation;
using _2DGame.Entities.Players;
using _2DGame.LayerData;
using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TransformableHitbox2D;

namespace _2DGame.Entities.Collectibles
{
    public class Gem : GameEntity, IAnimated
    {
        private Vector2f position = new();
        public override Vector2f Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateAllPositionProperties();
            }
        }
        public Vector2f Origin { get; private set; }
        public AnimatedSprite Sprite { get; set; }
        public IAnimated.Direction CurrentDirection { get; set; }

        public const int SCORE_VALUE = 500;
        public const int WIDTH = 28;
        public const int HEIGHT = 28;

        public Gem() : base(1) { }

        public override void Initialize(Vector2i startPosition)
        {
            Position = new Vector2f(
                startPosition.X * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2,
                startPosition.Y * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2);
            Origin = Position;
            InitializeHitbox();
        }   

        public override void Update(Level level, GameLoop gameLoop)
        {
            Position = new Vector2f(Origin.X, Origin.Y + (int)(5 * (float)Math.Sin(2 * (gameLoop.GameTime.TotalTimeElapsed + Origin.X % 3))));
        }

        public override void OnEntityCollision(GameEntity entity)
        {
            if (IsActive)
            {
                if (entity is Player)
                {
                    IsActive = false;
                    Score.Add(SCORE_VALUE);
                    SoundManager.PlaySound("Collect Gem");
                }
            }
        }

        protected override void UpdateHitbox()
        {
            if (Hitbox != null && Sprite != null)
            {
                TransformableHitbox2D.Transform transform = new();
                transform.Position = new Vector2(Position.X - Sprite.GetGlobalBounds().Width / 2, Position.Y - Sprite.GetGlobalBounds().Height / 2);
                Hitbox.Transform(transform);
            }
        }

        public override void Reset()
        {
            Position = Origin;
        }

        private void UpdateAllPositionProperties()
        {
            UpdateSpritePosition();
            UpdateHitbox();
            UpdateTileCoordinates();
        }

        private void UpdateSpritePosition()
        {
            if (Sprite != null)
            {
                Sprite.Position = new Vector2f(Position.X, Position.Y);
            }
        }

        private void InitializeHitbox()
        {
            Vector2[] vector2Arr = new Vector2[] {
                new Vector2(WIDTH / 2, 0),
                new Vector2(WIDTH, HEIGHT / 2),
                new Vector2(WIDTH / 2, HEIGHT),
                new Vector2(0, HEIGHT / 2),
                new Vector2(WIDTH / 2, 0)
            };
            Hitbox = new Hitbox(vector2Arr);
            UpdateHitbox();
        }
    }
}
