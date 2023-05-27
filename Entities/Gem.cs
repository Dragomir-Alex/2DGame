using _2DGame.Animation;
using _2DGame.LayerData;
using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TransformableHitbox2D;
using static _2DGame.Entities.Player;

namespace _2DGame.Entities
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
        public AnimatedSprite Sprite { get; set; }
        public IAnimated.Direction CurrentDirection { get; set; }

        public const int SCORE_VALUE = 500;
        public const int WIDTH = 28;
        public const int HEIGHT = 28;

        public Gem() : base(1) { }

        public override void Initialize(Vector2i startPosition)
        {
            Position = new Vector2f(startPosition.X * Tilemap.TILE_SIZE, startPosition.Y * Tilemap.TILE_SIZE);
            InitializeHitbox();
        }

        public override void Update(Level level, GameLoop gameLoop)
        {
            // Nothing yet
        }

        public void OnPlayerCollision()
        {
            if (IsActive) 
            {
                IsActive = false;
                Score.Add(SCORE_VALUE);
                SoundManager.PlaySound("Collect Gem");
            }
        }

        private void UpdateAllPositionProperties()
        {
            UpdateSpritePosition();
            UpdateHitboxPosition();
            UpdateTileCoordinates();
        }

        private void UpdateSpritePosition()
        {
            if (Sprite != null)
            {
                Sprite.Position = new Vector2f(Position.X - Sprite.GetGlobalBounds().Width / 2, Position.Y - Sprite.GetGlobalBounds().Height / 2);
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
            UpdateHitboxPosition();
        }
    }
}
