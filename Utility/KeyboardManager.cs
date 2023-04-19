using _2DGame.Entities;
using _2DGame.Layers;
using _2DGame.MainMenu;
using SFML.System;
using SFML.Window;
using System;
using System.Diagnostics;

namespace _2DGame.Utility
{
    public static class KeyboardManager
    {
        private static bool previousPauseState;
        private static bool previousDebugState;
        private static bool previousLeftClickState;

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

        public static void ProcessMainMenuKeys(GameLoop gameLoop, Menu menu)
        {
            bool leftClick = Mouse.IsButtonPressed(Mouse.Button.Left);
            Vector2i mousePosition = Mouse.GetPosition(gameLoop.Window);

            foreach (var button in menu.Pages[menu.CurrentPage].Buttons)
            {
                if (button.ButtonText.Position.X <= mousePosition.X &&
                    button.ButtonText.Position.X + button.ButtonText.GetGlobalBounds().Width >= mousePosition.X &&
                    button.ButtonText.Position.Y <= mousePosition.Y &&
                    button.ButtonText.Position.Y + button.ButtonText.GetGlobalBounds().Height >= mousePosition.Y)
                {
                    button.IsSelected = true;

                    if (leftClick && leftClick != previousLeftClickState)
                    {
                        menu.ProcessButtonAction(button.OnMouseClick(), gameLoop);
                    }
                }
                else
                {
                    button.IsSelected = false;
                }
            }

            previousLeftClickState = leftClick;
        }

        public static void ProcessLevelKeys(Game game)
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

        public static void ProcessPauseScreenKeys(Game game)
        {
            bool esc = Keyboard.IsKeyPressed(Keyboard.Key.Escape);

            if (esc)
            {
                game.CurrentState = GameLoop.GameState.LoadingMenu;
            }

            ProcessLevelKeys(game);
        }
    }
}
