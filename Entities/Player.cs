using _2DGame.Animation;
using _2DGame.LayerData;
using _2DGame.Layers;
using _2DGame.Utility;
using InstilledBee.SFML.SimpleCollision;
using NetTopologySuite.Mathematics;
using NetTopologySuite.Triangulate;
using SFML.Graphics;
using SFML.System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using TransformableHitbox2D;

namespace _2DGame.Entities
{
    public class Player : GameEntity
    {
        private bool canCollide;
        public View Camera { get; set; }
        public Vector2f Velocity { get; set; }
        public Health Health { get; set; }
        public Texture? Texture { get; set; }
        public AnimatedSprite Sprite { get; set; }

        public const int MAX_HEALTH = 5;

        public const float X_MAX_VELOCITY = 5f;
        public const float Y_MAX_VELOCITY = 5f;
        private const float X_VELOCITY_GAIN = 0.3f;
        private const float Y_VELOCITY_GAIN = 0.3f;
        private const float X_VELOCITY_REDUCTION = 0.15f;
        private const float Y_VELOCITY_REDUCTION = 0.15f;

        public const int HITBOX_WIDTH = 20;
        public const int HITBOX_HEIGHT = 40;

        public Player() : base()
        {
            Texture = null;
            Velocity = new Vector2f(0f, 0f);
            canCollide = true;
            Camera = new View();
            Health = new Health(MAX_HEALTH);
        }

        public override void Initialize(Vector2i startPosition)
        {
            Position = new Vector2f(startPosition.X * Tilemap.TILE_SIZE, startPosition.Y * Tilemap.TILE_SIZE);
            InitializeHitbox();
        }

        public void SetPlayerCamera(Vector2f center, Vector2f size)
        {
            Camera.Center = center;
            Camera.Size = size;
        }

        private void UpdatePlayerCamera(uint screenWidth, uint screenHeight, Level level)
        {
            float xCenter = (int)Position.X + HITBOX_WIDTH;
            float yCenter = (int)Position.Y + HITBOX_HEIGHT;

            if ((int)Position.X + HITBOX_WIDTH <= screenWidth / 2) // Left
            {
                xCenter = (int)screenWidth / 2;

            }
            else if ((int)Position.X + HITBOX_WIDTH >= level.Width - screenWidth / 2) // Right
            {
                xCenter = (int)level.Width - screenWidth / 2;
            }

            if ((int)Position.Y + HITBOX_HEIGHT <= screenHeight / 2) // Top
            {
                yCenter = (int)screenHeight / 2;

            }
            else if ((int)Position.Y + HITBOX_HEIGHT >= level.Height - screenHeight / 2) // Bottom
            {
                yCenter = (int)level.Height - screenHeight / 2;
            }

            Camera.Center = new Vector2f(xCenter, yCenter);
        }

        private void UpdatePosition(SpriteLayer spriteLayer)
        {
            Position = new Vector2f(Position.X + (int)Velocity.X, Position.Y + (int)Velocity.Y);

            UpdateAllPositionProperties();

            PlayerBorderCollision(spriteLayer);

            if (!canCollide) return;

            List<Tuple<Hitbox, int, int>> collidedTiles = PlayerLevelCollision(spriteLayer);
            if (collidedTiles.Count != 0)
            {
                RemoveCharacterFromSolidTiles(collidedTiles);
            }
        }

        public void PlayerBorderCollision(SpriteLayer spriteLayer)
        {
            bool wasMoved = false;
            if (Position.X - HITBOX_WIDTH / 2 < 0) // Left
            {
                Position = new Vector2f((float)HITBOX_WIDTH / 2, Position.Y);
                wasMoved = true;
            }
            else if (Position.X + HITBOX_WIDTH / 2 > spriteLayer.Width) // Right
            {
                Position = new Vector2f(spriteLayer.Width - HITBOX_WIDTH / 2, Position.Y);
                wasMoved = true;
            }

            if (Position.Y - HITBOX_HEIGHT / 2 < 0) // Top
            {
                Position = new Vector2f(Position.X, (float)HITBOX_HEIGHT / 2);
                wasMoved = true;
            }
            else if (Position.Y + HITBOX_HEIGHT / 2 > spriteLayer.Height) // Bottom
            {
                Position = new Vector2f(Position.X, spriteLayer.Height - HITBOX_HEIGHT / 2);
                wasMoved = true;
            }

            if (wasMoved) UpdateAllPositionProperties();
        }

        public List<Tuple<Hitbox, int, int>> PlayerLevelCollision(SpriteLayer spriteLayer)
        {
            List<Tuple<Hitbox, int, int>> collidedTiles = new();

            for (int i = TileCoordinates.X - 2; i <= TileCoordinates.X + 2; ++i)
            {
                for (int j = TileCoordinates.Y - 2; j <= TileCoordinates.Y + 2; ++j)
                {
                    if (spriteLayer.LayerTilemap.TileHitboxData.ContainsKey((j, i)))
                    {
                        if (spriteLayer.LayerTilemap.TileHitboxData[(j, i)].Overlaps(Hitbox))
                        {
                            var newTile = new Tuple<Hitbox, int, int>(spriteLayer.LayerTilemap.TileHitboxData[(j, i)], j, i);
                            collidedTiles.Add(newTile);
                        }
                    }
                }
            }

            return collidedTiles;
        }

        public void RemoveCharacterFromSolidTiles(List<Tuple<Hitbox, int, int>> collidedTiles)
        {
            foreach (var collidedTile in collidedTiles)
            {
                foreach (var crossPoint in Hitbox.CrossPointsWith(collidedTile.Item1))
                {
                    float xDisplacement, yDisplacement;

                    if (crossPoint.X > Position.X)  // Left collision
                    {
                        xDisplacement = -(Position.X + HITBOX_WIDTH / 2 - collidedTile.Item3 * Tilemap.TILE_SIZE);
                    }
                    else                            // Right collision
                    {
                        xDisplacement = (collidedTile.Item3 + 1) * Tilemap.TILE_SIZE - (Position.X - HITBOX_WIDTH / 2);
                    }

                    if (crossPoint.Y > Position.Y)  // Top collision
                    {
                        yDisplacement = -(Position.Y + HITBOX_HEIGHT / 2 - collidedTile.Item2 * Tilemap.TILE_SIZE);
                    }
                    else                            // Bottom collision
                    {
                        yDisplacement = (collidedTile.Item2 + 1) * Tilemap.TILE_SIZE - (Position.Y - HITBOX_HEIGHT / 2);
                    }

                    if (Math.Abs(xDisplacement) <= Math.Abs(yDisplacement))
                        Position = new Vector2f(Position.X + xDisplacement, Position.Y);
                    else
                        Position = new Vector2f(Position.X, Position.Y + yDisplacement);
                }
            }

            UpdateAllPositionProperties();
        }

        public void GainPositiveXVelocity() { Velocity = new Vector2f(Velocity.X + X_VELOCITY_GAIN, Velocity.Y); }
        public void GainNegativeXVelocity() { Velocity = new Vector2f(Velocity.X - X_VELOCITY_GAIN, Velocity.Y); }
        public void GainPositiveYVelocity() { Velocity = new Vector2f(Velocity.X, Velocity.Y + Y_VELOCITY_GAIN); }
        public void GainNegativeYVelocity() { Velocity = new Vector2f(Velocity.X, Velocity.Y - Y_VELOCITY_GAIN); }

        private void UpdateVelocity()
        {
            if (Velocity.X < 0f)
            {
                if (Velocity.X + X_VELOCITY_REDUCTION > 0f)
                    Velocity = new Vector2f(0f, Velocity.Y);
                else
                    Velocity = new Vector2f(Velocity.X + X_VELOCITY_REDUCTION, Velocity.Y);
            }

            if (Velocity.X > 0f)
            {
                if (Velocity.X - X_VELOCITY_REDUCTION < 0f)
                    Velocity = new Vector2f(0f, Velocity.Y);
                else
                    Velocity = new Vector2f(Velocity.X - X_VELOCITY_REDUCTION, Velocity.Y);
            }

            if (Velocity.X > X_MAX_VELOCITY)
                Velocity = new Vector2f(X_MAX_VELOCITY, Velocity.Y);

            if (Velocity.X < -X_MAX_VELOCITY)
                Velocity = new Vector2f(-X_MAX_VELOCITY, Velocity.Y);

            if (Velocity.Y < 0f)
            {
                if (Velocity.Y + Y_VELOCITY_REDUCTION > 0f)
                    Velocity = new Vector2f(Velocity.X, 0);
                else
                    Velocity = new Vector2f(Velocity.X, Velocity.Y + Y_VELOCITY_REDUCTION);
            }

            if (Velocity.Y > 0f)
            {
                if (Velocity.Y - Y_VELOCITY_REDUCTION < 0f)
                    Velocity = new Vector2f(Velocity.X, 0);
                else
                    Velocity = new Vector2f(Velocity.X, Velocity.Y - Y_VELOCITY_REDUCTION);
            }

            if (Velocity.Y > Y_MAX_VELOCITY)
                Velocity = new Vector2f(Velocity.X, Y_MAX_VELOCITY);

            if (Velocity.Y < -Y_MAX_VELOCITY)
                Velocity = new Vector2f(Velocity.X, -Y_MAX_VELOCITY);
        }

        public override void Update(Level level, GameLoop gameLoop)
        {
            UpdateVelocity();
            UpdatePosition((SpriteLayer)level.Layers[LayerList.PRIMARY_LAYER]);
            UpdatePlayerCamera(Game.DEFAULT_WINDOW_WIDTH, Game.DEFAULT_WINDOW_HEIGHT, level);
        }

        private void UpdateAllPositionProperties()
        {
            UpdateSpritePosition();
            UpdateHitboxPosition();
            UpdateTileCoordinates();
        }

        public override void Reset()
        {
            base.Reset();
            Velocity = new Vector2f(0, 0);
            Health.Reset();
        }

        public void ToggleCollisions()
        {
            canCollide = !canCollide;
        }

        private void InitializeHitbox()
        {
            Vector2[] vector2Arr = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(HITBOX_WIDTH, 0),
                new Vector2(HITBOX_WIDTH, HITBOX_HEIGHT),
                new Vector2(0, HITBOX_HEIGHT),
                new Vector2(0, 0)
            };
            Hitbox = new Hitbox(vector2Arr);
            UpdateHitboxPosition();
        }

        private void UpdateSpritePosition()
        {
            Sprite.Position = new Vector2f(Position.X - HITBOX_WIDTH - 12, Position.Y - HITBOX_HEIGHT + 8); // Magic numbers
        }

        protected override void UpdateHitboxPosition()
        {
            TransformableHitbox2D.Transform transform = new();
            transform.Position = new Vector2(Position.X - HITBOX_WIDTH / 2, Position.Y - HITBOX_HEIGHT / 2);
            Hitbox.Transform(transform);
        }

        public void InitializeSprite()
        {
            Sprite = TextureManager.PlayerAnimations["PlayerJumpUp"];
            UpdateSpritePosition();
        }
    }
}
