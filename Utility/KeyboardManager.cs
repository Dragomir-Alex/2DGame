using _2DGame.Entities;
using _2DGame.Layers;
using SFML.Window;
using System;

namespace _2DGame.Utility
{
    public static class KeyboardManager
    {
        private static bool previousPauseState;
        private static bool previousDebugState;

        public static void ProcessPlayerKeys(Player player)
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

        public static void ProcessMenuKeys(Game game)
        {
            bool pause = Keyboard.IsKeyPressed(Keyboard.Key.P);
            bool debug = Keyboard.IsKeyPressed(Keyboard.Key.F1);

            if (pause && pause != previousPauseState)
            {
                game.TogglePause();
            }

            if (debug && debug != previousDebugState)
            {
                game.ToggleDebugMode();
            }

            previousPauseState = pause;
            previousDebugState = debug;
        }
    }
}
