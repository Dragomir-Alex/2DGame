using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using SFML.Audio;
using _2DGame.Utility;
using _2DGame.Entities;
using _2DGame.Layers;
using System.Diagnostics;

namespace _2DGame
{
    public class Game : GameLoop
    {
        public const uint DEFAULT_WINDOW_WIDTH = 1280;
        public const uint DEFAULT_WINDOW_HEIGHT = 720;
        public const string WINDOW_TITLE = "Game";
        public const uint LAYER_COUNT = 8;

        public Game() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.Black) { }
        private Player player;
        private View view;
        private BackgroundLayer backgroundLayer;
        private SpriteLayer spriteLayer;
        private Level level;
        private Settings settings;

        public override void Draw(GameTime gameTime)
        {
            TextureManager.DrawTextures(this, view, player, backgroundLayer);
            Window.Draw(spriteLayer);

            DebugUtility.DrawPerformanceData(this, Color.White);
            DebugUtility.DrawGameData(this, player, Color.White);
        }

        public override void Instantiate()
        {
            player = new Player();
            backgroundLayer = new BackgroundLayer();

            //TEST

            uint[,] tiles = {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
                { 0, 0, 0, 0, 0, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 0, 0, 0, 0, 0, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 0, 0, 0, 0, 0, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 0, 0, 0, 0, 0, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19},
                { 0, 0, 0, 0, 0, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 0, 0, 0, 0, 0, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 0, 0, 0, 0, 0, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 0, 0, 0, 0, 0, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29}
            };

            spriteLayer = new SpriteLayer("aztec.png", tiles);

            level = new Level();
            settings = new Settings();
        }

        public override void Initialize()
        {
            settings.MusicVolume = 0;
            backgroundLayer.AutoXSpeed = -1;
            backgroundLayer.AutoYSpeed = -1;
            player.SetPlayerCamera(new Vector2f(DEFAULT_WINDOW_WIDTH / 2, DEFAULT_WINDOW_HEIGHT / 2), new Vector2f(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
            view = player.Camera;
            level.TrackFilename = "lush.ogg";
            level.InitializeLevel(this);
            TextureManager.InitializeSprites(this, player, backgroundLayer);
            SoundManager.SetMusicVolume(settings.MusicVolume);
        }

        public override void LoadContent()
        {
            DebugUtility.LoadContent();
            TextureManager.LoadTextures(backgroundLayer);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardManager.Update(player);
            SoundManager.PlayMusic();
            player.UpdatePlayerCamera(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
        }
    }
}
