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

        public const int MAX_HEALTH = 5;
        public const float X_MAX_VELOCITY = 5f;
        public const float Y_MAX_VELOCITY = 5f;
        private const float X_VELOCITY_GAIN = 0.3f;
        private const float Y_VELOCITY_GAIN = 0.3f;
        private const float X_VELOCITY_REDUCTION = 0.15f;
        private const float Y_VELOCITY_REDUCTION = 0.15f;

        public Player() : base()
        {
            Velocity = new Vector2f(0f, 0f);
            canCollide = true;
            Camera = new View();
            Health = new Health(MAX_HEALTH);
        }

        public void SetPlayerCamera(Vector2f center, Vector2f size)
        {
            Camera.Center = center;
            Camera.Size = size;
        }

        private void UpdatePlayerCamera(uint screenWidth, uint screenHeight, Level level)
        {
            float xCenter = (int)Position.X + Sprite.Texture.Size.X;
            float yCenter = (int)Position.Y + Sprite.Texture.Size.Y;

            if ((int)Position.X + Sprite.Texture.Size.X <= screenWidth / 2) // Left
            {
                xCenter = (int)screenWidth / 2;

            }
            else if ((int)Position.X + Sprite.Texture.Size.X >= level.Width - screenWidth / 2) // Right
            {
                xCenter = (int)level.Width - screenWidth / 2;
            }

            if ((int)Position.Y + Sprite.Texture.Size.Y <= screenHeight / 2) // Top
            {
                yCenter = (int)screenHeight / 2;

            }
            else if ((int)Position.Y + Sprite.Texture.Size.Y >= level.Height - screenHeight / 2) // Bottom
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
            if (Position.X - Sprite.Texture.Size.X / 2 < 0) // Left
            {
                Position = new Vector2f((float)Sprite.Texture.Size.X / 2, Position.Y);
                wasMoved = true;
            }
            else if (Position.X + Sprite.Texture.Size.X / 2 > spriteLayer.Width) // Right
            {
                Position = new Vector2f(spriteLayer.Width - Sprite.Texture.Size.X / 2, Position.Y);
                wasMoved = true;
            }

            if (Position.Y - Sprite.Texture.Size.Y / 2 < 0) // Top
            {
                Position = new Vector2f(Position.X, (float)Sprite.Texture.Size.Y / 2);
                wasMoved = true;
            }
            else if (Position.Y + Sprite.Texture.Size.Y / 2 > spriteLayer.Height) // Bottom
            {
                Position = new Vector2f(Position.X, spriteLayer.Height - Sprite.Texture.Size.Y / 2);
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
                        xDisplacement = -(Position.X + Texture.Size.X / 2 - collidedTile.Item3 * Tilemap.TILE_SIZE);
                    }
                    else                            // Right collision
                    {
                        xDisplacement = (collidedTile.Item3 + 1) * Tilemap.TILE_SIZE - (Position.X - Texture.Size.X / 2);
                    }

                    if (crossPoint.Y > Position.Y)  // Top collision
                    {
                        yDisplacement = -(Position.Y + Texture.Size.Y / 2 - collidedTile.Item2 * Tilemap.TILE_SIZE);
                    }
                    else                            // Bottom collision
                    {
                        yDisplacement = (collidedTile.Item2 + 1) * Tilemap.TILE_SIZE - (Position.Y - Texture.Size.Y / 2);
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

        public override void Update(Level level)
        {
            UpdateVelocity();
            UpdatePosition((SpriteLayer)level.Layers[LayerList.PRIMARY_LAYER]);
            UpdatePlayerCamera(Game.DEFAULT_WINDOW_WIDTH, Game.DEFAULT_WINDOW_HEIGHT, level);
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
    }
}
