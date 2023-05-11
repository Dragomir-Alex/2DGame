using _2DGame.LayerData;
using InstilledBee.SFML.SimpleCollision;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TransformableHitbox2D;

namespace _2DGame.Entities
{
    public abstract class GameEntity : Drawable
    {
        public Texture? Texture { get; set; }
        public Sprite Sprite { get; set; }
        public Hitbox? Hitbox { get; set; }
        public TileCoordinates TileCoordinates { get; set; }
        public Vector2f Position { get; set; }

        public GameEntity()
        {
            Texture = null;
            Sprite = new Sprite();
            Position = new Vector2f(0f, 0f);
            TileCoordinates = new TileCoordinates();
        }

        public virtual void Initialize(Vector2i startPosition)
        {
            Position = new Vector2f(startPosition.X * Tilemap.TILE_SIZE, startPosition.Y * Tilemap.TILE_SIZE);
            InitializeHitbox();
        }

        public virtual void InitializeSprite()
        {
            if (Texture != null)
            {
                Sprite = new Sprite(Texture);
                UpdateSpritePosition();
                // CollisionTester.AddBitMask(Texture);
            }
        }

        public virtual void Reset()
        {
            Position = new Vector2f(0, 0);
        }

        protected void UpdateAllPositionProperties()
        {
            UpdateSpritePosition();
            UpdateHitboxPosition();
            UpdateTileCoordinates();
        }

        protected void InitializeHitbox()
        {
            if (Sprite != null)
            {
                Vector2[] vector2Arr = new Vector2[] { new Vector2(0, 0), new Vector2(Texture.Size.X, 0), new Vector2(Texture.Size.X, Texture.Size.Y), new Vector2(0, Texture.Size.Y), new Vector2(0, 0) };
                Hitbox = new Hitbox(vector2Arr);
                UpdateHitboxPosition();
            }
        }

        protected void UpdateHitboxPosition()
        {
            TransformableHitbox2D.Transform transform = new();
            transform.Position = new Vector2(Sprite.Position.X, Sprite.Position.Y);
            Hitbox.Transform(transform);
        }

        protected void UpdateSpritePosition()
        {
            Sprite.Position = new Vector2f(Position.X - Texture.Size.X / 2, Position.Y - Texture.Size.Y / 2);
        }

        protected void UpdateTileCoordinates()
        {
            TileCoordinates.X = (int)((int)Position.X / LayerData.Tilemap.TILE_SIZE);
            if (Position.X < 0)
                TileCoordinates.X--;

            TileCoordinates.Y = (int)((int)Position.Y / LayerData.Tilemap.TILE_SIZE);
            if (Position.Y < 0)
                TileCoordinates.Y--;
        }

        public abstract void Update(Level level);

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Sprite);
        }
    }
}
