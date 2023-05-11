using System;
using _2DGame.Entities;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace _2DGame.Utility
{
    public static class DebugUtility
    {
        public static void DrawPerformanceData(GameLoop gameLoop, Color color)
        {
            gameLoop.Window.SetView(gameLoop.Window.DefaultView);

            if (TextureManager.DebugFont == null) return;

            string totalTimeElapsedStr = gameLoop.GameTime.TotalTimeElapsed.ToString("0.000");
            string deltaTimeStr = gameLoop.GameTime.DeltaTime.ToString("0.00000");
            float fps = 1f / gameLoop.GameTime.DeltaTime;
            string fpsStr = fps.ToString("0.00");

            Text textA = new Text("Time elapsed: " + totalTimeElapsedStr, TextureManager.DebugFont, 14);
            textA.Position = new Vector2f(4f, 8f);
            textA.FillColor = color;

            Text textB = new Text("Delta time: " + deltaTimeStr, TextureManager.DebugFont, 14);
            textB.Position = new Vector2f(4f, 28f);
            textB.FillColor = color;

            Text textC = new Text(fpsStr + " FPS", TextureManager.DebugFont, 14);
            textC.Position = new Vector2f(4f, 48f);
            textC.FillColor = color;

            gameLoop.Window.Draw(textA);
            gameLoop.Window.Draw(textB);
            gameLoop.Window.Draw(textC);

            textA.Dispose();
            textB.Dispose();
            textC.Dispose();
        }

        public static void DrawGameData(GameLoop gameLoop, Player player, Color color)
        {
            gameLoop.Window.SetView(gameLoop.Window.DefaultView);

            if (TextureManager.DebugFont == null) return;

            string playerPositionStr = "X: " + player.Position.X.ToString() + "  Y: " + player.Position.Y.ToString();
            string playerVelocityStr = "X-Vel: " + player.Velocity.X.ToString() + "  Y-Vel: " + player.Velocity.Y.ToString();
            string playerTileCoordinatesStr = "X-Tile: " + player.TileCoordinates.X.ToString() + "  Y-Tile: " + player.TileCoordinates.Y.ToString();

            Text positionText = new Text(playerPositionStr, TextureManager.DebugFont, 14);
            positionText.Position = new Vector2f(4f, 68f);
            positionText.FillColor = color;

            Text velocityText = new Text(playerVelocityStr, TextureManager.DebugFont, 14);
            velocityText.Position = new Vector2f(4f, 88f);
            velocityText.FillColor = color;

            Text tileCoordinatesText = new Text(playerTileCoordinatesStr, TextureManager.DebugFont, 14);
            tileCoordinatesText.Position = new Vector2f(4f, 108f);
            tileCoordinatesText.FillColor = color;

            gameLoop.Window.Draw(positionText);
            gameLoop.Window.Draw(velocityText);
            gameLoop.Window.Draw(tileCoordinatesText);

            positionText.Dispose();
            velocityText.Dispose();
            tileCoordinatesText.Dispose();
        }

        public static void DrawMessage(GameLoop gameLoop, string message, Color color)
        {
            gameLoop.Window.SetView(gameLoop.Window.DefaultView);

            if (TextureManager.DebugFont == null) return;

            Text text = new Text(message, TextureManager.DebugFont, 20);
            text.Position = new Vector2f(80f, 80f);
            text.FillColor = color;

            gameLoop.Window.Draw(text);

            text.Dispose();
        }

        public static void DrawDebugInfo(GameLoop gameLoop, Player player)
        {
            // Hitbox debug
            CircleShape shape = new CircleShape(2);
            shape.FillColor = new Color(100, 250, 50);
            shape.Position = player.Position;
            gameLoop.Window.Draw(shape);

            shape.Dispose();

            foreach (var line in player.Hitbox.Lines)
            {
                CircleShape shape2 = new CircleShape(2);
                shape2.FillColor = new Color(200, 50, 50);
                shape2.Position = new Vector2f(line.A.X, line.A.Y);
                gameLoop.Window.Draw(shape2);

                shape2.Dispose();
            }

            DebugUtility.DrawPerformanceData(gameLoop, Color.White);
            DebugUtility.DrawGameData(gameLoop, player, Color.White);
        }
    }
}
