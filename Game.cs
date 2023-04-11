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
using System.Drawing;
using Color = SFML.Graphics.Color;

namespace _2DGame
{
    public class Game : GameLoop
    {
        public const uint DEFAULT_WINDOW_WIDTH = 1280;
        public const uint DEFAULT_WINDOW_HEIGHT = 720;
        public const string WINDOW_TITLE = "Game";

        public Game() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.Black) { }
        private Player player;
        private View view;
        private Level level;
        private Settings settings;
        private bool debugMode;

        public override void Draw(GameTime gameTime)
        {
            TextureManager.DrawTextures(this, view, player, level.Layers);

            if (CurrentState == GameState.Paused)
            {
                DrawPausedMenu();
            }

            if (debugMode)
            {
                // Hitbox debug
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

                DebugUtility.DrawPerformanceData(this, Color.White);
                DebugUtility.DrawGameData(this, player, Color.White);
            }
        }

        public override void Instantiate()
        {
            player = new Player();
            level = new Level();
            settings = new Settings();
        }

        public override void Initialize()
        {
            // Level
            level.Initialize("aztec.png", "lush.ogg");

            // Settings
            settings.MusicVolume = 0;
            SoundManager.SetMusicVolume(settings.MusicVolume);

            // Camera
            player.SetPlayerCamera(new Vector2f(DEFAULT_WINDOW_WIDTH / 2, DEFAULT_WINDOW_HEIGHT / 2), new Vector2f(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
            view = player.Camera;

            // Textures
            TextureManager.InitializeSprites(this, player, level.Layers);

            // Player hitbox
            player.Initialize(level.TileStartPosition);
        }

        public override void LoadContent()
        {
            DebugUtility.LoadFont();
            TextureManager.LoadTextures(player);
            level.LoadData("test.tmx");
        }

        public override void ProcessInputs()
        {
            KeyboardManager.ProcessMenuKeys(this);

            if (CurrentState == GameState.Running)
            {
                KeyboardManager.ProcessPlayerKeys(player);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentState == GameState.Running)
            {
                SoundManager.PlayMusic();
                player.Update(level, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
                level.Update(player);
            }
        }

        public void ToggleDebugMode()
        {
            debugMode = !debugMode;
        }

        private void DrawPausedMenu()
        {
            Window.SetView(Window.DefaultView);

            RectangleShape rectangleBackground = new RectangleShape(new Vector2f(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));
            rectangleBackground.FillColor = new Color(0, 0, 0, 100);

            Text text = new Text("The game is paused\nPress [P] to resume", TextureManager.GameFont);
            text.CharacterSize = 60;
            text.Position = new Vector2f((int)(DEFAULT_WINDOW_WIDTH / 2 - text.GetGlobalBounds().Width / 2), (int)(DEFAULT_WINDOW_HEIGHT / 2 - text.GetGlobalBounds().Height));
            text.FillColor = Color.White;

            Window.Draw(rectangleBackground);
            Window.Draw(text);

            Window.SetView(view);
        }
    }
}
