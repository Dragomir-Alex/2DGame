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

        private void UpdatePlayerCamera(uint screenWidth, uint screenHeight, Level level)
        {
            float xCenter = (int)Position.X + CharacterSprite.Texture.Size.X;
            float yCenter = (int)Position.Y + CharacterSprite.Texture.Size.Y;

            if ((int)Position.X + CharacterSprite.Texture.Size.X <= screenWidth / 2) // Left
            {
                xCenter = (int)screenWidth / 2;

            }
            else if ((int)Position.X + CharacterSprite.Texture.Size.X >= level.Width - screenWidth / 2) // Right
            {
                xCenter = (int)level.Width - screenWidth / 2;
            }

            if ((int)Position.Y + CharacterSprite.Texture.Size.Y <= screenHeight / 2) // Top
            {
                yCenter = (int)screenHeight / 2;

            }
            else if ((int)Position.Y + CharacterSprite.Texture.Size.Y >= level.Height - screenHeight / 2) // Bottom
            {
                yCenter = (int)level.Height - screenHeight / 2;
            }

            Camera.Center = new Vector2f(xCenter, yCenter);
        }

        private void UpdatePosition(SpriteLayer spriteLayer)
        {
            Position.X += (int)Velocity.X;
            Position.Y += (int)Velocity.Y;

            UpdateAllPositionProperties();

            PlayerBorderCollision(spriteLayer);

            List<Tuple<Hitbox, int, int>> collidedTiles = PlayerLevelCollision(spriteLayer);
            if (collidedTiles.Count != 0)
            {
                RemoveCharacterFromSolidTiles(collidedTiles);
            }
        }

        public void PlayerBorderCollision(SpriteLayer spriteLayer)
        {
            bool wasMoved = false;
            if (Position.X - CharacterSprite.Texture.Size.X / 2 < 0) // Left
            {
                Position.X = (float)CharacterSprite.Texture.Size.X / 2;
                wasMoved = true;
            }
            else if (Position.X + CharacterSprite.Texture.Size.X / 2 > spriteLayer.Width) // Right
            {
                Position.X = spriteLayer.Width - CharacterSprite.Texture.Size.X / 2;
                wasMoved = true;
            }

            if (Position.Y - CharacterSprite.Texture.Size.Y / 2 < 0) // Top
            {
                Position.Y = (float)CharacterSprite.Texture.Size.Y / 2;
                wasMoved = true;
            }
            else if (Position.Y + CharacterSprite.Texture.Size.Y / 2 > spriteLayer.Height) // Bottom
            {
                Position.Y = spriteLayer.Height - CharacterSprite.Texture.Size.Y / 2;
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

        private void UpdateVelocity()
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

        public void Update(Level level, uint windowWidth, uint windowHeight)
        {
            UpdateVelocity();
            UpdatePosition((SpriteLayer)level.Layers[LayerList.PRIMARY_LAYER]);
            UpdatePlayerCamera(windowWidth, windowHeight, level);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(CharacterSprite);
        }
    }
}