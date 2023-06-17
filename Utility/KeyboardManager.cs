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
        private static bool isProcessingGameOver = false;

        public static void ProcessPlayerKeys(GameLoop gameLoop, Player player)
        {
            if (!gameLoop.IsFocused) return;

            Debug.WriteLine(XInputController.GamePad.Buttons);


            bool moveLeft = Keyboard.IsKeyPressed(Keyboard.Key.Left)
                || Keyboard.IsKeyPressed(Keyboard.Key.A)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.DPadLeft);

            bool moveRight = Keyboard.IsKeyPressed(Keyboard.Key.Right)
                || Keyboard.IsKeyPressed(Keyboard.Key.D)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.DPadRight);

            bool moveUp = Keyboard.IsKeyPressed(Keyboard.Key.Up)
                || Keyboard.IsKeyPressed(Keyboard.Key.X)
                || Keyboard.IsKeyPressed(Keyboard.Key.W)
                || Keyboard.IsKeyPressed(Keyboard.Key.L)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.DPadUp)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.A);

            bool moveDown = Keyboard.IsKeyPressed(Keyboard.Key.Down)
                || Keyboard.IsKeyPressed(Keyboard.Key.S)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.DPadDown);
            bool attack = Keyboard.IsKeyPressed(Keyboard.Key.Z)
                || Keyboard.IsKeyPressed(Keyboard.Key.K)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.X);

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

            bool buttonPress = Mouse.IsButtonPressed(Mouse.Button.Left)
                || Keyboard.IsKeyPressed(Keyboard.Key.Enter)
                || Keyboard.IsKeyPressed(Keyboard.Key.Space)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.A);

            Vector2f mousePosition = gameLoop.Window.MapPixelToCoords(Mouse.GetPosition(gameLoop.Window), gameLoop.RenderTexture.GetView());

            foreach (var button in menu.Pages[menu.CurrentPage].Buttons)
            {
                if (button.ButtonText.GetGlobalBounds().Contains(mousePosition.X, mousePosition.Y))
                {
                    button.IsHovered = true;

                    if (buttonPress && buttonPress != previousLeftClickState)
                    {
                        menu.ProcessButtonAction(button.OnMouseClick(), gameLoop);
                    }
                }
                else
                {
                    button.IsHovered = false;
                }
            }

            previousLeftClickState = buttonPress;
        }

        public static void ProcessLevelKeys(Game game, Player player)
        {
            if (!game.IsFocused) return;

            bool pause = Keyboard.IsKeyPressed(Keyboard.Key.P)
                || Keyboard.IsKeyPressed(Keyboard.Key.Escape)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.Back)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.Start);

            bool debug = Keyboard.IsKeyPressed(Keyboard.Key.F1)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.LeftShoulder);
            bool toggleCollisions = Keyboard.IsKeyPressed(Keyboard.Key.F2)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.RightShoulder);

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

            bool esc = Keyboard.IsKeyPressed(Keyboard.Key.Escape)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.Start);

            if (esc && !previousPauseState)
            {
                game.CurrentState = GameLoop.GameState.LoadingMenu;
            }

            ProcessLevelKeys(game, player);
        }

        public static async Task ProcessGameOverScreenKeys(Game game, Leaderboard leaderboard, GameOverScreen gameOverScreen)
        {
            if (!game.IsFocused || isProcessingGameOver) return;

            bool enter = Keyboard.IsKeyPressed(Keyboard.Key.Enter)
                || XInputController.GamePad.Buttons.HasFlag(SharpDX.XInput.GamepadButtonFlags.Start);

            if (enter && gameOverScreen.GetPlayerName().Length != 0)
            {
                isProcessingGameOver = true;

                await leaderboard.Add(gameOverScreen.GetPlayerName(), Score.Value);
                leaderboard.Save();
                game.CurrentState = GameLoop.GameState.LoadingMenu;
                gameOverScreen.Reset(game);

                isProcessingGameOver = false;
            }
        }
    }
}
