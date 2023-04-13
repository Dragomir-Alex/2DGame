using System;
using _2DGame.Entities;
using _2DGame.Layers;
using _2DGame.MainMenu;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace _2DGame.Utility
{
    public static class TextureManager
    {
        public const string TEXTURES_PATH = "./Data/Assets/Textures/Sprites/";
        public const string TILESETS_PATH = "./Data/Assets/Textures/Tilesets/";
        public const string FONTS_PATH = "./Data/Assets/Fonts/";

        public static Font GameFont { get; private set; }
        public static Font GameFontBold { get; private set; }

        public static void LoadTextures(Level level, Menu menu)
        {
            level.Player.Texture = new Texture(TEXTURES_PATH + "player.png");
            menu.Pages[Menu.PageName.MainPage].LogoTexture = new Texture(TEXTURES_PATH + "logo.png");
            GameFont = new Font(FONTS_PATH + "8bitOperatorPlus-Regular.ttf");
            GameFontBold = new Font(FONTS_PATH + "8bitOperatorPlus-Bold.ttf");
        }

        public static void InitializeSprites(Game gameLoop, Level level, Menu menu)
        {
            // Level
            level.Player.InitializeSprite();
            foreach (var layer in level.Layers)
            {
                if (layer is DetailLayer detailLayer)
                    detailLayer.InitializeSprite();
            }

            // Menu
            foreach (var page in menu.Pages)
            {
                page.Value.InitializeSprites();
                if (page.Key == Menu.PageName.MainPage)
                {
                    page.Value.LogoSprite.Position = new Vector2f(Game.DEFAULT_WINDOW_WIDTH / 2 - page.Value.LogoTexture.Size.X / 2, Game.DEFAULT_WINDOW_HEIGHT / 6f);
                }
            }
        }

        public static void DrawTextures(GameLoop gameLoop, Level level)
        {
            foreach (var layer in level.Layers.Reverse())
            {
                if (layer is DetailLayer)
                {
                    gameLoop.Window.SetView(gameLoop.Window.DefaultView); // For UI and background only
                    gameLoop.Window.Draw(layer);
                }
                else
                {
                    gameLoop.Window.SetView(level.Camera); // Player camera
                    gameLoop.Window.Draw(level.Player);
                    gameLoop.Window.Draw(layer);
                }
            }

            gameLoop.Window.SetView(level.Camera); // Back to player camera
        }
    }
}
