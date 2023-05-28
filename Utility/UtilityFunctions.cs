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

        public static List<Tuple<Hitbox, int, int>> GameEntityLevelCollision(GameEntity currentEntity, SpriteLayer spriteLayer)
        {
            List<Tuple<Hitbox, int, int>> collidedTiles = new();

            for (int i = currentEntity.TileCoordinates.X - 2; i <= currentEntity.TileCoordinates.X + 2; ++i)
            {
                for (int j = currentEntity.TileCoordinates.Y - 2; j <= currentEntity.TileCoordinates.Y + 2; ++j)
                {
                    if (spriteLayer.LayerTilemap.TileHitboxData.ContainsKey((j, i)))
                    {
                        if (spriteLayer.LayerTilemap.TileHitboxData[(j, i)].Overlaps(currentEntity.Hitbox))
                        {
                            var newTile = new Tuple<Hitbox, int, int>(spriteLayer.LayerTilemap.TileHitboxData[(j, i)], j, i);
                            collidedTiles.Add(newTile);
                        }
                    }
                }
            }

            return collidedTiles;
        }
    }
}
