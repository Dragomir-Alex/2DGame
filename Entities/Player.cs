using _2DGame.LayerData;
using _2DGame.Layers;
using _2DGame.Utility;
using InstilledBee.SFML.SimpleCollision;
using SFML.Graphics;
using SFML.System;
using System.Diagnostics;
using System.Numerics;
using TransformableHitbox2D;

namespace _2DGame.Entities
{
    public class Player : Drawable
    {
        public Texture? Texture { get; set; }
        public Sprite CharacterSprite { get; set; }
        public Hitbox CharacterHitbox { get; set; }
        public Vector2f Position; // Not sure what to do with these at the moment, make them vars?
        public Vector2f Velocity;
        public TileCoordinates TileCoordinates; // Ditto

        public Vector2f MaxVelocity { get; set; }
        public View Camera { get; set; }
        private const float X_VELOCITY_GAIN = 0.3f;
        private const float Y_VELOCITY_GAIN = 0.3f;
        private const float X_VELOCITY_REDUCTION = 0.15f;
        private const float Y_VELOCITY_REDUCTION = 0.15f;

        public Player()
        {
            Texture = null;
            CharacterSprite = new Sprite();
            Position = new Vector2f(0f, 0f);
            Velocity = new Vector2f(0f, 0f);
            MaxVelocity = new Vector2f(5f, 5f);
            Camera = new View();
            TileCoordinates = new TileCoordinates();
        }

        public void Initialize(Vector2i startPosition)
        {
            Position = new Vector2f(startPosition.X * Tilemap.TILE_SIZE, startPosition.Y * Tilemap.TILE_SIZE);
            InitializeHitbox();
        }

        public void InitializeSprite()
        {
            if (Texture != null)
            {
                this.Texture.Smooth = true;

                Sprite playerSprite = new Sprite();
                playerSprite.Texture = Texture;
                CharacterSprite = playerSprite;
                UpdateSpritePosition();
                CollisionTester.AddBitMask(Texture);
            }
        }
        private void InitializeHitbox()
        {
            if (CharacterSprite != null)
            {
                Vector2[] vector2Arr = new Vector2[] { new Vector2(0, 0), new Vector2(Texture.Size.X, 0), new Vector2(Texture.Size.X, Texture.Size.Y), new Vector2(0, Texture.Size.Y), new Vector2(0, 0) };
                CharacterHitbox = new Hitbox(vector2Arr);
                UpdateHitboxPosition();
            }
        }

        public void SetPlayerCamera(Vector2f center, Vector2f size)
        {
            Camera.Center = center;
            Camera.Size = size;
        }

        public void UpdatePlayerCamera(uint screenWidth, uint screenHeight, Level level)
        {
            if ((int)Position.X + CharacterSprite.Texture.Size.X > screenWidth / 2)
                Camera.Center = new Vector2f((int)Position.X + CharacterSprite.Texture.Size.X, (int)Position.Y + CharacterSprite.Texture.Size.Y);    
            else
                Camera.Center = new Vector2f(screenWidth / 2, (int)Position.Y + CharacterSprite.Texture.Size.Y);
        }

        public void UpdatePosition(SpriteLayer spriteLayer)
        {
            Position.X += (int)Velocity.X;
            Position.Y += (int)Velocity.Y;

            UpdateAllPositionProperties();

            List<Tuple<Hitbox, int, int>> collidedTiles = PlayerLevelCollision(spriteLayer);
            if (collidedTiles.Count != 0)
            {
                RemoveCharacterFromSolidTiles(collidedTiles);
            }
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
                        if (spriteLayer.LayerTilemap.TileHitboxData[(j, i)].Overlaps(CharacterHitbox))
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
                foreach (var crossPoint in CharacterHitbox.CrossPointsWith(collidedTile.Item1))
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
                        Position.X += xDisplacement;
                    else Position.Y += yDisplacement;
                }
            }

            UpdateAllPositionProperties();
        }

        public void UpdateSpritePosition()
        {
            CharacterSprite.Position = new Vector2f(Position.X - Texture.Size.X / 2, Position.Y - Texture.Size.Y / 2);
        }

        public void UpdateHitboxPosition()
        {
            TransformableHitbox2D.Transform transform = new();
            transform.Position = new Vector2(CharacterSprite.Position.X, CharacterSprite.Position.Y);
            CharacterHitbox.Transform(transform);
        }

        public void UpdateTileCoordinates()
        {
            TileCoordinates.X = (int)((int)Position.X / LayerData.Tilemap.TILE_SIZE);
            if (Position.X < 0)
                TileCoordinates.X--;

            TileCoordinates.Y = (int)((int)Position.Y / LayerData.Tilemap.TILE_SIZE);
            if (Position.Y < 0)
                TileCoordinates.Y--;
        }

        public void UpdateAllPositionProperties()
        {
            UpdateSpritePosition();
            UpdateHitboxPosition();
            UpdateTileCoordinates();
        }

        public void GainPositiveXVelocity() { Velocity.X += X_VELOCITY_GAIN; }
        public void GainNegativeXVelocity() { Velocity.X -= X_VELOCITY_GAIN; }
        public void GainPositiveYVelocity() { Velocity.Y += Y_VELOCITY_GAIN; }
        public void GainNegativeYVelocity() { Velocity.Y -= Y_VELOCITY_GAIN; }

        public void UpdateVelocity()
        {
            if (Velocity.X < 0f)
            {
                if (Velocity.X + X_VELOCITY_REDUCTION > 0f)
                    Velocity.X = 0f;
                else Velocity.X += X_VELOCITY_REDUCTION;
            }

            if (Velocity.X > 0f)
            {
                if (Velocity.X - X_VELOCITY_REDUCTION < 0f)
                    Velocity.X = 0f;
                else
                    Velocity.X -= X_VELOCITY_REDUCTION;
            }

            if (Velocity.X > MaxVelocity.X)
                Velocity.X = MaxVelocity.X;

            if (Velocity.X < -MaxVelocity.X)
                Velocity.X = -MaxVelocity.X;

            if (Velocity.Y < 0f)
            {
                if (Velocity.Y + Y_VELOCITY_REDUCTION > 0f)
                    Velocity.Y = 0f;
                else
                    Velocity.Y += Y_VELOCITY_REDUCTION;
            }

            if (Velocity.Y > 0f)
            {
                if (Velocity.Y - Y_VELOCITY_REDUCTION < 0f)
                    Velocity.Y = 0f;
                else
                    Velocity.Y -= Y_VELOCITY_REDUCTION;
            }

            if (Velocity.Y > MaxVelocity.Y)
                Velocity.Y = MaxVelocity.Y;

            if (Velocity.Y < -MaxVelocity.Y)
                Velocity.Y = -MaxVelocity.Y;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(CharacterSprite);
        }
    }
}
