using _2DGame.Entities;
using _2DGame.Layers;
using SFML.Window;
using System;

namespace _2DGame.Utility
{
    public static class KeyboardManager
    {
        public static void HandleKeys(Player player)
        {
            bool moveLeft = Keyboard.IsKeyPressed(Keyboard.Key.A);
            bool moveRight = Keyboard.IsKeyPressed(Keyboard.Key.D);
            bool moveUp = Keyboard.IsKeyPressed(Keyboard.Key.W);
            bool moveDown = Keyboard.IsKeyPressed(Keyboard.Key.S);

            bool isMove = moveLeft || moveRight || moveUp || moveDown;

            if (isMove)
            {
                if (moveLeft) player.GainNegativeXVelocity();
                if (moveRight) player.GainPositiveXVelocity();
                if (moveUp) player.GainNegativeYVelocity();
                if (moveDown) player.GainPositiveYVelocity();
            }
        }

        public static void Update(Player player, SpriteLayer spriteLayer)
        {
            HandleKeys(player);
            player.UpdateVelocity(); // Maybe I shouldn't put these here but for now... /s
            player.UpdatePosition(spriteLayer);
        }
    }
}
