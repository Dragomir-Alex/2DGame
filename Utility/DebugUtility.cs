using System;
using _2DGame.Entities;
using SFML.Graphics;
using SFML.System;

namespace _2DGame.Utility
{
    public static class DebugUtility
    {
        public const string CONSOLE_FONT_PATH = "./Data/Fonts/8bitOperatorPlus-Regular.ttf";

        public static Font consoleFont;

        public static void LoadContent()
        {
            consoleFont = new Font(CONSOLE_FONT_PATH);
        }

        public static void DrawPerformanceData(GameLoop gameLoop, Color color)
        {
            gameLoop.Window.SetView(gameLoop.Window.DefaultView);

            if (consoleFont == null) return;

            string totalTimeElapsedStr = gameLoop.GameTime.TotalTimeElapsed.ToString("0.000");
            string deltaTimeStr = gameLoop.GameTime.DeltaTime.ToString("0.00000");
            float fps = 1f / gameLoop.GameTime.DeltaTime;
            string fpsStr = fps.ToString("0.00");

            Text textA = new Text(totalTimeElapsedStr, consoleFont, 14);
            textA.Position = new Vector2f(4f, 8f);
            textA.FillColor = color;

            Text textB = new Text(deltaTimeStr, consoleFont, 14);
            textB.Position = new Vector2f(4f, 28f);
            textB.FillColor = color;

            Text textC = new Text(fpsStr + " FPS", consoleFont, 14);
            textC.Position = new Vector2f(4f, 48f);
            textC.FillColor = color;

            gameLoop.Window.Draw(textA);
            gameLoop.Window.Draw(textB);
            gameLoop.Window.Draw(textC);
        }

        public static void DrawGameData(GameLoop gameLoop, Player player, Color color)
        {
            gameLoop.Window.SetView(gameLoop.Window.DefaultView);

            if (consoleFont == null) return;

            string playerPositionStr = "X: " + player.Position.X.ToString() + "  Y: " + player.Position.Y.ToString();

            Text text = new Text(playerPositionStr, consoleFont, 14);
            text.Position = new Vector2f(4f, 68f);
            text.FillColor = color;

            gameLoop.Window.Draw(text);
        }

        public static void DrawMessage(GameLoop gameLoop, string message, Color color)
        {
            gameLoop.Window.SetView(gameLoop.Window.DefaultView);

            if (consoleFont == null) return;

            Text text = new Text(message, consoleFont, 20);
            text.Position = new Vector2f(80f, 80f);
            text.FillColor = color;

            gameLoop.Window.Draw(text);
        }
    }
}
