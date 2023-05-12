﻿using _2DGame.ExternalLibraries;
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
    public abstract class GameEntity
    {
        public Hitbox? Hitbox { get; set; }
        public TileCoordinates TileCoordinates { get; set; }
        public Vector2f Position { get; set; }

        public GameEntity()
        {
            Position = new Vector2f(0f, 0f);
            TileCoordinates = new TileCoordinates();
        }

        public abstract void Initialize(Vector2i startPosition);

        public virtual void Reset()
        {
            Position = new Vector2f(0, 0);
        }

        protected virtual void UpdateHitboxPosition()
        {
            TransformableHitbox2D.Transform transform = new();
            transform.Position = new Vector2(Position.X, Position.Y);
            Hitbox.Transform(transform);
        }

        protected virtual void UpdateTileCoordinates()
        {
            TileCoordinates.X = (int)((int)Position.X / Tilemap.TILE_SIZE);
            if (Position.X < 0)
                TileCoordinates.X--;

            TileCoordinates.Y = (int)((int)Position.Y / Tilemap.TILE_SIZE);
            if (Position.Y < 0)
                TileCoordinates.Y--;
        }

        public abstract void Update(Level level, GameLoop gameLoop);
    }
}
