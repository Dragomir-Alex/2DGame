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
        static Texture? playerTexture;

        public static Texture? PlayerTexture { get { return playerTexture; } }

        public static void LoadTextures(BackgroundLayer background)
        {
            playerTexture = new Texture(TEXTURES_PATH + "player.png");
            background.Texture = new Texture(TEXTURES_PATH + "background.png");
        }

        public static void InitializeSprites(Game gameLoop, Player player, BackgroundLayer background)
        {
            Sprite playerSprite = new Sprite();
            playerSprite.Texture = PlayerTexture;
            player.CharacterSprite = playerSprite;

            background.InitializeSprite(gameLoop);
        }

        public static void DrawTextures(GameLoop gameLoop, View view, Player player, BackgroundLayer background)
        {
            gameLoop.Window.SetView(gameLoop.Window.DefaultView); // For UI and backgrounds only

            background.Draw(gameLoop);

            gameLoop.Window.SetView(view); // Player camera

            player.CharacterSprite.Position = player.Position;
            gameLoop.Window.Draw(player.CharacterSprite);

        }
    }
}
