using System;
using _2DGame.Entities;
using _2DGame.Layers;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace _2DGame.Utility
{
    public static class TextureManager
    {
        public const string TEXTURES_PATH = "./Data/Assets/Textures/Sprites/";
        public const string TILESETS_PATH = "./Data/Assets/Textures/Tilesets/";

        public static void LoadTextures(Player player, BackgroundLayer background)
        {
            player.Texture = new Texture(TEXTURES_PATH + "player.png");
            background.Texture = new Texture(TEXTURES_PATH + "background.png");
        }

        public static void InitializeSprites(Game gameLoop, Player player, BackgroundLayer background)
        {
            player.InitializeSprite();
            background.InitializeSprite();
        }

        public static void DrawTextures(GameLoop gameLoop, View view, Player player, BackgroundLayer backgroundLayer)
        {
            gameLoop.Window.SetView(gameLoop.Window.DefaultView); // For UI and background only

            gameLoop.Window.Draw(backgroundLayer);

            gameLoop.Window.SetView(view); // Player camera

            gameLoop.Window.Draw(player);
        }
    }
}
