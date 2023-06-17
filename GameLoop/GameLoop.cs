using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using _2DGame.Utility;

namespace _2DGame
{
    public abstract class GameLoop
    {
        public const int TARGET_FPS = 60;
        public const float TIME_UNTIL_UPDATE = 1f / TARGET_FPS;

        public enum GameState
        {
            StartingUp, LoadingMenu, LoadingLevel, Menu, Level, Paused, GameOver
        }
        public bool IsFocused { get; private set; }
        public RenderWindow Window { get; private set; }
        public RenderTexture RenderTexture { get; private set; }
        public GameTime GameTime { get; private set; }
        public Color WindowClearColor { get; }
        public GameState CurrentState { get; set; }


        protected GameLoop(uint windowWidth, uint windowHeight, string windowTitle, Color windowClearColor)
        {
            WindowClearColor = windowClearColor;
            Window = new RenderWindow(new VideoMode(windowWidth, windowHeight), windowTitle);
            Window.SetVerticalSyncEnabled(true);
            RenderTexture = new RenderTexture(windowWidth, windowHeight);
            GameTime = new GameTime();
            CurrentState = GameState.StartingUp;
            IsFocused = true;

            Image image = new(TextureManager.TEXTURES_PATH + "GameIcon.png");
            Window.SetIcon(23, 23, image.Pixels);

            Window.LostFocus += (s, e) =>
            {
                if (CurrentState == GameState.Level)
                    CurrentState = GameState.Paused;
                IsFocused = false;
            };

            Window.GainedFocus += (s, e) =>
            {
                IsFocused = true;
            };

            Window.Closed += (s, e) =>
            {
                Window.Close();
            };
        }

        public void Run()
        {
            Instantiate();
            LoadContent();
            Initialize();

            float totalTimeBeforeUpdate = 0f;
            float previousTimeElapsed = 0f;
            float deltaTime;
            float totalTimeElapsed;

            Clock clock = new();

            while (Window.IsOpen)
            {
                Window.DispatchEvents();

                totalTimeElapsed = clock.ElapsedTime.AsSeconds();
                deltaTime = totalTimeElapsed - previousTimeElapsed;
                previousTimeElapsed = totalTimeElapsed;
                totalTimeBeforeUpdate += deltaTime;

                if (totalTimeBeforeUpdate >= TIME_UNTIL_UPDATE)
                {
                    ProcessInputs();

                    GameTime.Update(totalTimeBeforeUpdate, clock.ElapsedTime.AsSeconds());
                    totalTimeBeforeUpdate = 0f;

                    Update(GameTime);
                    Display();
                }
            }
        }

        public abstract void Instantiate();
        public abstract void LoadContent();
        public abstract void Initialize();
        public abstract void ProcessInputs();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);

        public void Display()
        {
            var viewSize = RenderTexture.Size;

            Vertex[] vertices = new Vertex[4]
            {
                    new(new Vector2f(0, 0), Color.White, new Vector2f(0, 0)),
                    new(new Vector2f(viewSize.X, 0), Color.White, new Vector2f(viewSize.X, 0)),
                    new(new Vector2f(viewSize.X, viewSize.Y), Color.White, new Vector2f(viewSize.X, viewSize.Y)),
                    new(new Vector2f(0, viewSize.Y), Color.White, new Vector2f(0, viewSize.Y))
            };

            Window.Clear(WindowClearColor);
            RenderTexture.Clear(WindowClearColor);
            Draw(GameTime);
            RenderTexture.Display();
            Window.Draw(vertices, PrimitiveType.Quads, new(RenderTexture.Texture));
            Window.Display();
        }
    }
}




