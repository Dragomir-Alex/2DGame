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
        private static bool previousToggleCollisionsState;

        public static void ProcessPlayerKeys(Player player)
        {
            bool moveLeft = Keyboard.IsKeyPressed(Keyboard.Key.A);
            bool moveRight = Keyboard.IsKeyPressed(Keyboard.Key.D);
            bool moveUp = Keyboard.IsKeyPressed(Keyboard.Key.W);
            bool moveDown = Keyboard.IsKeyPressed(Keyboard.Key.S);

            bool isMove = moveLeft || moveRight || moveUp || moveDown;

            if (isMove)
            {
                if (moveLeft) player.LeftButtonAction();
                if (moveRight) player.RightButtonAction();
                if (moveUp) player.UpButtonAction();
                if (moveDown) player.DownButtonAction();
            }
        }

        public static void ProcessMainMenuKeys(GameLoop gameLoop, Menu menu)
        {
            bool leftClick = Mouse.IsButtonPressed(Mouse.Button.Left);
            Vector2i mousePosition = Mouse.GetPosition(gameLoop.Window);

            // Debug.WriteLine("Mouse position: " + mousePosition.X.ToString() + "   " + mousePosition.Y.ToString());

            foreach (var button in menu.Pages[menu.CurrentPage].Buttons)
            {
                if (button.ButtonText.GetGlobalBounds().Contains(mousePosition.X, mousePosition.Y))
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

        public static void ProcessLevelKeys(Game game, Player player)
        {
            bool pause = Keyboard.IsKeyPressed(Keyboard.Key.P);
            bool debug = Keyboard.IsKeyPressed(Keyboard.Key.F1);
            bool toggleCollisions = Keyboard.IsKeyPressed(Keyboard.Key.F2);

            if (pause && pause != previousPauseState)
            {
                game.TogglePause();
            }

            if (debug && debug != previousDebugState)
            {
                game.ToggleDebugMode();
            }

            if (toggleCollisions && toggleCollisions != previousToggleCollisionsState)
            {
                player.ToggleDebugMode();
            }

            previousPauseState = pause;
            previousDebugState = debug;
            previousToggleCollisionsState = toggleCollisions;
        }

        public static void ProcessPauseScreenKeys(Game game, Player player)
        {
            bool esc = Keyboard.IsKeyPressed(Keyboard.Key.Escape);

            if (esc)
            {
                game.CurrentState = GameLoop.GameState.LoadingMenu;
            }

            ProcessLevelKeys(game, player);
        }
    }
}
