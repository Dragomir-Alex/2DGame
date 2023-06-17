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
        public const int WINDOW_ICON_WIDTH = 23;
        public const int WINDOW_ICON_HEIGHT = 23;

        public enum GameState
        {
            StartingUp, LoadingMenu, LoadingLevel, Menu, Level, Paused, GameOver
        }
        public bool IsFocused { get; private set; }
        public RenderWindow Window { get; protected set; }
        public RenderTexture RenderTexture { get; private set; }
        public GameTime GameTime { get; private set; }
        public Image Icon { get; private set; }
        public Color WindowClearColor { get; }
        public GameState CurrentState { get; set; }


        protected GameLoop(uint windowWidth, uint windowHeight, string windowTitle, Color windowClearColor)
        {
            WindowClearColor = windowClearColor;
            RenderTexture = new RenderTexture(windowWidth, windowHeight);
            GameTime = new GameTime();
            CurrentState = GameState.StartingUp;
            IsFocused = true;
            Icon = new(TextureManager.TEXTURES_PATH + "GameIcon.png");

            WindowSetup(windowWidth, windowHeight, windowTitle, Styles.Default);
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

        protected void WindowSetup(uint windowWidth, uint windowHeight, string windowTitle, Styles style)
        {
            Window = new RenderWindow(new VideoMode(windowWidth, windowHeight), windowTitle, style);
            Window.SetVerticalSyncEnabled(true);
            Window.SetIcon(23, 23, Icon.Pixels);

            Window.LostFocus += (_, __) =>
            {
                if (CurrentState == GameState.Level)
                    CurrentState = GameState.Paused;
                IsFocused = false;
            };
            Window.GainedFocus += (_, __) => { IsFocused = true; };
            Window.Closed += (_, __) => Window.Close();
        }
    }
}




