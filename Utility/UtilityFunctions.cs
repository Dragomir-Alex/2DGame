using _2DGame.Entities;
using _2DGame.Layers;
using SFML.Graphics;
using SFML.System;
using System;
using System.Diagnostics;
using TransformableHitbox2D;

namespace _2DGame.Utility
{
    public static class UtilityFunctions
    {
        public static void Move(Sprite sprite, float x, float y)
        {
            Vector2f pos = sprite.Position;
            sprite.Position = new Vector2f(pos.X + x, pos.Y + y);
        }

        public static List<Tuple<Hitbox, int, int>> GameEntityLevelCollision(GameEntity gameEntity, SpriteLayer spriteLayer)
        {
            List<Tuple<Hitbox, int, int>> collidedTiles = new();

            for (int i = gameEntity.TileCoordinates.X - 2; i <= gameEntity.TileCoordinates.X + 2; ++i)
            {
                for (int j = gameEntity.TileCoordinates.Y - 2; j <= gameEntity.TileCoordinates.Y + 2; ++j)
                {
                    if (spriteLayer.LayerTilemap.TileHitboxData.ContainsKey((j, i)))
                    {
                        if (spriteLayer.LayerTilemap.TileHitboxData[(j, i)].Overlaps(gameEntity.Hitbox))
                        {
                            var newTile = new Tuple<Hitbox, int, int>(spriteLayer.LayerTilemap.TileHitboxData[(j, i)], j, i);
                            collidedTiles.Add(newTile);
                        }
                    }
                }
            }

            return collidedTiles;
        }

        public static Vector2f UpdateVelocity(Vector2f velocity, float xVelocityReduction, float yVelocityReduction, float xMaxVelocity, float yMaxVelocity)
        {
            if (velocity.X < 0f)
            {
                if (velocity.X + xVelocityReduction > 0f)
                    velocity = new Vector2f(0f, velocity.Y);
                else
                    velocity = new Vector2f(velocity.X + xVelocityReduction, velocity.Y);
            }

            if (velocity.X > 0f)
            {
                if (velocity.X - xVelocityReduction < 0f)
                    velocity = new Vector2f(0f, velocity.Y);
                else
                    velocity = new Vector2f(velocity.X - xVelocityReduction, velocity.Y);
            }

            if (velocity.X >= xMaxVelocity)
                velocity = new Vector2f(xMaxVelocity, velocity.Y);

            if (velocity.X <= -xMaxVelocity)
                velocity = new Vector2f(-xMaxVelocity, velocity.Y);

            if (velocity.Y < 0f)
            {
                if (velocity.Y + yVelocityReduction > 0f)
                    velocity = new Vector2f(velocity.X, 0);
                else
                    velocity = new Vector2f(velocity.X, velocity.Y + yVelocityReduction);
            }

            if (velocity.Y > 0f)
            {
                if (velocity.Y - yVelocityReduction < 0f)
                    velocity = new Vector2f(velocity.X, 0);
                else
                    velocity = new Vector2f(velocity.X, velocity.Y - yVelocityReduction);
            }

            if (velocity.Y >= yMaxVelocity)
                velocity = new Vector2f(velocity.X, yMaxVelocity);

            if (velocity.Y <= -yMaxVelocity)
                velocity = new Vector2f(velocity.X, -yMaxVelocity);

            return velocity;
        }
    }
}
