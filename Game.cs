using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using SFML.Audio;
using _2DGame.Utility;
using _2DGame.Entities;
using _2DGame.Layers;
using System.Diagnostics;
using InstilledBee.SFML.SimpleCollision;
using TransformableHitbox2D;
using _2DGame.ExternalLibraries;

namespace _2DGame
{
    public class Game : GameLoop
    {
        public const uint DEFAULT_WINDOW_WIDTH = 1280;
        public const uint DEFAULT_WINDOW_HEIGHT = 720;
        public const string WINDOW_TITLE = "Game";
        public const uint LAYER_COUNT = 8;
        public const uint PRIMARY_LAYER = 3;
        public const uint BACKGROUND_LAYER = 7;

        public Game() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.Black) { }
        private Player player;
        private View view;
        private Layer[] layers;
        private Level level;
        private Settings settings;

        public override void Draw(GameTime gameTime)
        {
            TextureManager.DrawTextures(this, view, player, (BackgroundLayer)layers[BACKGROUND_LAYER]);
            Window.Draw((SpriteLayer)layers[PRIMARY_LAYER]);

            // Debug start
            CircleShape shape = new CircleShape(2);
            shape.FillColor = new Color(100, 250, 50);
            shape.Position = player.Position;
            Window.Draw(shape);

            foreach (var line in player.CharacterHitbox.Lines)
            {
                CircleShape shape2 = new CircleShape(2);
                shape2.FillColor = new Color(200, 50, 50);
                shape2.Position = new Vector2f(line.A.X, line.A.Y);
                Window.Draw(shape2);
            }
            //Debug end

            DebugUtility.DrawPerformanceData(this, Color.White);
            DebugUtility.DrawGameData(this, player, Color.White);
        }

        public override void Instantiate()
        {
            player = new Player();
            layers = new Layer[LAYER_COUNT];
            InstantiateLayers();
            level = new Level();
            settings = new Settings();
        }

        public override void Initialize()
        {
            settings.MusicVolume = 0;

            // Testing
            List<TileData> tilesList = new();
            TileData tiles = new TileData(new int[,]{
                { 0, 0, 0, 0, 0, 0, 0, 10, 10, 0, 0, 0, 0, 0, 0, 0, 10, 10, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 10, 10, 0, 0, 0, 0, 0, 0, 0, 10, 10, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 10, 10, 0, 0, 0, 0, 0, 0, 0, 10, 10, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 10, 10, 10, 10, 10, 0, 10, 10, 10, 10, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 10, 10, 10, 10, 10, 0, 10, 10, 10, 10, 0, 0 }
            });

            TileData tilesBg = new TileData(new int[,] {
                { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 },
                { 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 },
                { 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 },
                { 60, 61, 62, 63, 64, 65, 66, 67, 68, 69 },
                { 70, 71, 72, 73, 74, 75, 76, 77, 78, 79 },
                { 80, 81, 82, 83, 84, 85, 86, 87, 88, 89 },
                { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 },
            });

            tilesList.Add(null);
            tilesList.Add(null);
            tilesList.Add(null);
            tilesList.Add(tiles);
            tilesList.Add(null);
            tilesList.Add(null);
            tilesList.Add(null);

            InitializeLayers("aztec.png", tilesList);

            player.SetPlayerCamera(new Vector2f(DEFAULT_WINDOW_WIDTH / 2, DEFAULT_WINDOW_HEIGHT / 2), new Vector2f(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
            view = player.Camera;

            level.TrackFilename = "lush.ogg";
            level.InitializeLevel(this);

            TextureManager.InitializeSprites(this, player, (BackgroundLayer)layers[BACKGROUND_LAYER]);
            SoundManager.SetMusicVolume(settings.MusicVolume);

            player.InitializeHitbox();
        }

        public override void LoadContent()
        {
            DebugUtility.LoadFont();
            TextureManager.LoadTextures(player, (BackgroundLayer)layers[BACKGROUND_LAYER]);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardManager.Update(player, (SpriteLayer)layers[PRIMARY_LAYER]);
            SoundManager.PlayMusic();
            player.UpdatePlayerCamera(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
        }

        private void InstantiateLayers()
        {
            for (int i = 0; i < LAYER_COUNT - 1; ++i)
            {
                layers[i] = new SpriteLayer();
            }
            layers[LAYER_COUNT - 1] = new BackgroundLayer();
        }

        private void InitializeLayers(string tilesetFilename, List<TileData> tilesList)
        {
            for (int i = 0; i < LAYER_COUNT - 1; ++i)
            {
                if (i == PRIMARY_LAYER)
                {
                    (layers[i] as SpriteLayer).Initialize(tilesetFilename, tilesList[i], true);
                }
                else
                {
                    (layers[i] as SpriteLayer).Initialize(tilesetFilename, tilesList[i], false);
                }
            }

            layers[LAYER_COUNT - 1].AutoXSpeed = -1;
            layers[LAYER_COUNT - 1].AutoYSpeed = -1;
        }
    }
}
