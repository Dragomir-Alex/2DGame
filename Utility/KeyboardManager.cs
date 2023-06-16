using _2DGame.Entities.Players;
using _2DGame.Layers;
using _2DGame.LevelUI;
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

        public static void ProcessPlayerKeys(GameLoop gameLoop, Player player)
        {
            if (!gameLoop.IsFocused) return;

            bool moveLeft = Keyboard.IsKeyPressed(Keyboard.Key.Left) || Keyboard.IsKeyPressed(Keyboard.Key.A);
            bool moveRight = Keyboard.IsKeyPressed(Keyboard.Key.Right) || Keyboard.IsKeyPressed(Keyboard.Key.D);
            bool moveUp = (Keyboard.IsKeyPressed(Keyboard.Key.Up) || Keyboard.IsKeyPressed(Keyboard.Key.X) || Keyboard.IsKeyPressed(Keyboard.Key.L));
            bool moveDown = Keyboard.IsKeyPressed(Keyboard.Key.Down) || Keyboard.IsKeyPressed(Keyboard.Key.S);
            bool attack = Keyboard.IsKeyPressed(Keyboard.Key.Z) || Keyboard.IsKeyPressed(Keyboard.Key.K);

            bool isMove = moveLeft || moveRight || moveUp || moveDown;

            if (isMove)
            {
                if (moveLeft) player.LeftButtonAction();
                if (moveRight) player.RightButtonAction();
                if (moveUp) player.UpButtonAction();
                if (moveDown) player.DownButtonAction();
            }

            if (attack)
            {
                player.AttackButtonAction();
            }
        }

        public static void ProcessMainMenuKeys(GameLoop gameLoop, Menu menu)
        {
            if (!gameLoop.IsFocused) return;

            bool leftClick = Mouse.IsButtonPressed(Mouse.Button.Left);

            Vector2f mousePosition = gameLoop.Window.MapPixelToCoords(Mouse.GetPosition(gameLoop.Window), gameLoop.RenderTexture.GetView());

            foreach (var button in menu.Pages[menu.CurrentPage].Buttons)
            {
                if (button.ButtonText.GetGlobalBounds().Contains(mousePosition.X, mousePosition.Y))
                {
                    button.IsHovered = true;

                    if (leftClick && leftClick != previousLeftClickState)
                    {
                        menu.ProcessButtonAction(button.OnMouseClick(), gameLoop);
                    }
                }
                else
                {
                    button.IsHovered = false;
                }
            }

            previousLeftClickState = leftClick;
        }

        public static void ProcessLevelKeys(Game game, Player player)
        {
            if (!game.IsFocused) return;

            bool pause = Keyboard.IsKeyPressed(Keyboard.Key.P) || Keyboard.IsKeyPressed(Keyboard.Key.Escape);
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
            if (!game.IsFocused) return;

            bool esc = Keyboard.IsKeyPressed(Keyboard.Key.Escape);

            if (esc && !previousPauseState)
            {
                game.CurrentState = GameLoop.GameState.LoadingMenu;
            }

            ProcessLevelKeys(game, player);
        }

        public static void ProcessGameOverScreenKeys(Game game, Leaderboard leaderboard, GameOverScreen gameOverScreen)
        {
            if (!game.IsFocused) return;

            bool enter = Keyboard.IsKeyPressed(Keyboard.Key.Enter);

            if (enter && gameOverScreen.GetPlayerName().Length != 0)
            {
                leaderboard.Add(gameOverScreen.GetPlayerName(), Score.Value);
                leaderboard.Save();
                gameOverScreen.Reset(game);
                game.CurrentState = GameLoop.GameState.LoadingMenu;
            }
        }
    }
}
