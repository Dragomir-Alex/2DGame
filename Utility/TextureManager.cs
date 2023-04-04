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

        public static void LoadTextures(Player player)
        {
            player.Texture = new Texture(TEXTURES_PATH + "player.png");
        }

        public static void InitializeSprites(Game gameLoop, Player player, Layer[] layers)
        {
            player.InitializeSprite();
            foreach (var layer in layers)
            {
                if (layer is DetailLayer)
                    ((DetailLayer)layer).InitializeSprite();
            }
        }

        public static void DrawTextures(GameLoop gameLoop, View view, Player player, Layer[] layers)
        {
            foreach (var layer in layers.Reverse())
            {
                if (layer is DetailLayer)
                {
                    gameLoop.Window.SetView(gameLoop.Window.DefaultView); // For UI and background only
                    gameLoop.Window.Draw(layer);
                }
                else
                {
                    gameLoop.Window.SetView(view); // Player camera
                    gameLoop.Window.Draw(player);
                    gameLoop.Window.Draw(layer);
                }
            }

            gameLoop.Window.SetView(view); // Back to player camera
        }
    }
}
