using SFML.Graphics;
using SFML.System;
using System;
using System.Diagnostics;

namespace _2DGame.Utility
{
    public static class UtilityFunctions
    {
        public static void Move(Sprite sprite, float x, float y)
        {
            Vector2f pos = sprite.Position;
            sprite.Position = new Vector2f(pos.X + x, pos.Y + y);
        }
    }
}
