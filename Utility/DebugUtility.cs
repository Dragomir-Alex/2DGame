using System;
using _2DGame.Entities.Players;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace _2DGame.Utility
{
    public static class DebugUtility
    {
        private static void DrawPerformanceData(GameLoop gameLoop, Color color)
        {
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

            gameLoop.RenderTexture.Draw(textA);
            gameLoop.RenderTexture.Draw(textB);
            gameLoop.RenderTexture.Draw(textC);

            textA.Dispose();
            textB.Dispose();
            textC.Dispose();
        }

        private static void DrawGameData(GameLoop gameLoop, Player player, Color color)
        {
            if (TextureManager.DebugFont == null) return;

            string playerPositionStr = "X: " + player.Position.X.ToString() + "  Y: " + player.Position.Y.ToString();
            string playerVelocityStr = "X-Vel: " + Math.Round(player.Velocity.X, 2).ToString() + "  Y-Vel: " + Math.Round(player.Velocity.Y, 2).ToString();
            string playerTileCoordinatesStr = "X-Tile: " + player.TileCoordinates.X.ToString() + "  Y-Tile: " + player.TileCoordinates.Y.ToString();
            string playerState = "Player state: " + player.CurrentState.ToString();
            string playerDirection = "Player direction: " + player.CurrentDirection.ToString();

            Text positionText = new Text(playerPositionStr, TextureManager.DebugFont, 14);
            positionText.Position = new Vector2f(4f, 68f);
            positionText.FillColor = color;

            Text velocityText = new Text(playerVelocityStr, TextureManager.DebugFont, 14);
            velocityText.Position = new Vector2f(4f, 88f);
            velocityText.FillColor = color;

            Text tileCoordinatesText = new Text(playerTileCoordinatesStr, TextureManager.DebugFont, 14);
            tileCoordinatesText.Position = new Vector2f(4f, 108f);
            tileCoordinatesText.FillColor = color;

            Text stateText = new Text(playerState, TextureManager.DebugFont, 14);
            stateText.Position = new Vector2f(4f, 128f);
            stateText.FillColor = color;

            Text directionText = new Text(playerDirection, TextureManager.DebugFont, 14);
            directionText.Position = new Vector2f(4f, 148f);
            directionText.FillColor = color;

            gameLoop.RenderTexture.Draw(positionText);
            gameLoop.RenderTexture.Draw(velocityText);
            gameLoop.RenderTexture.Draw(tileCoordinatesText);
            gameLoop.RenderTexture.Draw(stateText);
            gameLoop.RenderTexture.Draw(directionText);

            positionText.Dispose();
            velocityText.Dispose();
            tileCoordinatesText.Dispose();
            stateText.Dispose();
            directionText.Dispose();
        }

        public static void DrawMessage(GameLoop gameLoop, string message, Color color)
        {
            gameLoop.RenderTexture.SetView(gameLoop.RenderTexture.DefaultView);

            if (TextureManager.DebugFont == null) return;

            Text text = new Text(message, TextureManager.DebugFont, 20);
            text.Position = new Vector2f(80f, 80f);
            text.FillColor = color;

            gameLoop.RenderTexture.Draw(text);

            text.Dispose();
        }

        public static void DrawDebugInfo(GameLoop gameLoop, Player player, Level level)
        {
            // Hitbox debug
            CircleShape shape = new CircleShape(2);
            shape.Origin = new Vector2f(2, 2);
            shape.FillColor = new Color(100, 250, 50);
            shape.Position = player.Position;
            gameLoop.RenderTexture.Draw(shape);
            shape.Dispose();

            foreach (var line in player.Hitbox.Lines)
            {
                CircleShape shape2 = new CircleShape(2);
                shape2.Origin = new Vector2f(2, 2);
                shape2.FillColor = new Color(200, 50, 50);
                shape2.Position = new Vector2f(line.A.X, line.A.Y);
                gameLoop.RenderTexture.Draw(shape2);

                shape2.Dispose();
            }

            foreach (var entity in level.GameEntityManager.OnScreenGameEntities) 
            {
                CircleShape shape3 = new CircleShape(2);
                shape3.Origin = new Vector2f(2, 2);
                shape3.FillColor = new Color(100, 250, 50);
                shape3.Position = entity.Position;
                gameLoop.RenderTexture.Draw(shape3);
                shape3.Dispose();

                foreach (var line in entity.Hitbox.Lines)
                {
                    CircleShape shape4 = new CircleShape(2);
                    shape4.Origin = new Vector2f(2, 2);
                    shape4.FillColor = new Color(200, 50, 50);
                    shape4.Position = new Vector2f(line.A.X, line.A.Y);
                    gameLoop.RenderTexture.Draw(shape4);

                    shape4.Dispose();
                }
            }

            gameLoop.RenderTexture.SetView(gameLoop.RenderTexture.DefaultView);

            RectangleShape rectangleBackground = new RectangleShape(new Vector2f(175f, 175f));
            rectangleBackground.FillColor = new Color(0, 0, 0, 100);
            gameLoop.RenderTexture.Draw(rectangleBackground);
            rectangleBackground.Dispose();

            DrawPerformanceData(gameLoop, Color.White);
            DrawGameData(gameLoop, player, Color.White);

        }
    }
}
