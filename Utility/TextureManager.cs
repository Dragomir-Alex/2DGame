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
        public static Font DebugFont { get; private set; }

        public static void LoadFonts()
        {
            GameFont = new Font(FONTS_PATH + "8bitOperatorPlus-Regular.ttf");
            GameFontBold = new Font(FONTS_PATH + "8bitOperatorPlus-Bold.ttf");
            DebugFont = new Font(FONTS_PATH + "8bitOperatorPlus-Regular.ttf");
        }

        public static void LoadMenuTextures(Menu menu)
        {
            menu.Pages[Menu.PageName.MainPage].LogoTexture = new Texture(TEXTURES_PATH + "logo.png");
        }

        public static void LoadPlayerTextures(Player player)
        {
            player.Texture = new Texture(TEXTURES_PATH + "player.png");
        }

        public static void InitializeMenuSprites(Menu menu, LoadingScreen loadingScreen)
        {
            // Menu
            foreach (var page in menu.Pages)
            {
                page.Value.InitializeSprites();
                if (page.Key == Menu.PageName.MainPage)
                {
                    page.Value.LogoSprite.Position = new Vector2f(Game.DEFAULT_WINDOW_WIDTH / 2 - page.Value.LogoTexture.Size.X / 2, Game.DEFAULT_WINDOW_HEIGHT / 6f);
                }
            }

            // Loading Screen
            loadingScreen.InitializeSprites();
        }

        public static void InitializePlayerSprite(Player player)
        {
            player.InitializeSprite();
        }

        public static void InitializeLevelSprites(Level level)
        {
            foreach (var layer in level.Layers)
            {
                if (layer is DetailLayer detailLayer)
                    detailLayer.InitializeSprite();
            }
        }

        public static void DrawLevelTextures(GameLoop gameLoop, Level level, Player player)
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
                    gameLoop.Window.SetView(player.Camera); // Player camera
                    gameLoop.Window.Draw(layer);
                    gameLoop.Window.Draw(player);
                }
            }

            gameLoop.Window.SetView(player.Camera); // Back to player camera
        }
    }
}
