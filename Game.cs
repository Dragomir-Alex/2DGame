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
using _2DGame.MainMenu;

namespace _2DGame
{
    public class Game : GameLoop
    {
        public const uint DEFAULT_WINDOW_WIDTH = 1280;
        public const uint DEFAULT_WINDOW_HEIGHT = 720;
        public const string WINDOW_TITLE = "Game";

        public Game() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, Color.Black) { }
        private Level level;
        private Menu menu;
        private bool debugMode;

        public override void Draw(GameTime gameTime)
        {
            //TextureManager.DrawTextures(this, level);
            Window.Draw(menu);

            if (CurrentState == GameState.Paused)
            {
                DrawPauseMenu();
            }

            if (debugMode)
            {
                // Hitbox debug
                CircleShape shape = new CircleShape(2);
                shape.FillColor = new Color(100, 250, 50);
                shape.Position = level.Player.Position;
                Window.Draw(shape);

                foreach (var line in level.Player.CharacterHitbox.Lines)
                {
                    CircleShape shape2 = new CircleShape(2);
                    shape2.FillColor = new Color(200, 50, 50);
                    shape2.Position = new Vector2f(line.A.X, line.A.Y);
                    Window.Draw(shape2);
                }

                DebugUtility.DrawPerformanceData(this, Color.White);
                DebugUtility.DrawGameData(this, level.Player, Color.White);
            }
        }

        public override void Instantiate()
        {
            menu = new Menu();
            level = new Level();
        }

        public override void Initialize()
        {
            // Menu
            menu.Initialize();

            // Level
            level.Initialize("aztec.png", "lush.ogg");

            // Settings
            Settings.MusicVolume = 0;
            SoundManager.SetMusicVolume(Settings.MusicVolume);

            // Textures
            TextureManager.InitializeSprites(this, level, menu);
        }

        public override void LoadContent()
        {
            DebugUtility.LoadFont();
            TextureManager.LoadTextures(level, menu);
            level.LoadData("test.tmx");
        }

        public override void ProcessInputs()
        {
            KeyboardManager.ProcessMainMenuKeys(this, menu);

            KeyboardManager.ProcessOtherKeys(this);

            if (CurrentState == GameState.Level)
            {
                KeyboardManager.ProcessPlayerKeys(level.Player);
            }
        }

        public override void Update(GameTime gameTime)
        {
            menu.Update(Window.DefaultView);
            if (CurrentState == GameState.Level)
            {
                SoundManager.PlayMusic();
                level.Update();
            }
        }

        public void ToggleDebugMode()
        {
            debugMode = !debugMode;
        }

        private void DrawPauseMenu()
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

            Window.SetView(level.Camera);
        }
    }
}
