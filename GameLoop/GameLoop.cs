﻿using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace _2DGame
{
    public abstract class GameLoop
    {
        public const int TARGET_FPS = 60;
        public const float TIME_UNTIL_UPDATE = 1f / TARGET_FPS;

        public RenderWindow Window { get; protected set; }
        public GameTime GameTime { get; protected set; }
        public Color WindowClearColor { get; protected set; }
        public bool IsPaused { get; protected set; }



        protected GameLoop(uint windowWidth, uint windowHeight, string windowTitle, Color windowClearColor)
        {
            WindowClearColor = windowClearColor;
            Window = new RenderWindow(new VideoMode(windowWidth, windowHeight), windowTitle);
            Window.SetVerticalSyncEnabled(true);
            GameTime = new GameTime();

            Window.Closed += WindowClosed;
        }

        public void Run()
        {
            Instantiate();
            LoadContent();
            Initialize();

            float totalTimeBeforeUpdate = 0f;
            float previousTimeElapsed = 0f;
            float deltaTime = 0f;
            float totalTimeElapsed = 0f;

            Clock clock = new Clock();

            while (Window.IsOpen)
            {
                Window.DispatchEvents();

                totalTimeElapsed = clock.ElapsedTime.AsSeconds();
                deltaTime = totalTimeElapsed - previousTimeElapsed;
                previousTimeElapsed = totalTimeElapsed;
                totalTimeBeforeUpdate += deltaTime;

                if (totalTimeBeforeUpdate >= TIME_UNTIL_UPDATE)
                {
                    GameTime.Update(totalTimeBeforeUpdate, clock.ElapsedTime.AsSeconds());
                    totalTimeBeforeUpdate = 0f;

                    ProcessInputs();

                    if (!IsPaused)
                    {
                        Update(GameTime);
                    }

                    Window.Clear(WindowClearColor);
                    Draw(GameTime);
                    Window.Display();
                }
            }
        }

        public abstract void Instantiate();
        public abstract void LoadContent();
        public abstract void Initialize();
        public abstract void ProcessInputs();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);

        public void TogglePause()
        {
            IsPaused = !IsPaused;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Window.Close();
        }
    }
}

