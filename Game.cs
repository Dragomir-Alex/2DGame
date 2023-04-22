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
        private Player player;
        private Menu menu;
        private LoadingScreen loadingScreen;
        private PauseScreen pauseScreen;
        private bool debugMode;

        public override void Draw(GameTime gameTime)
        {
            switch (CurrentState)
            {
                case GameState.StartingUp:
                    Window.Draw(loadingScreen);
                    break;

                case GameState.LoadingMenu:
                    Window.Draw(loadingScreen);
                    break;

                case GameState.LoadingLevel:
                    Window.Draw(loadingScreen);
                    break;

                case GameState.Menu:
                    Window.SetView(Window.DefaultView);
                    Window.Draw(menu);
                    break;

                case GameState.Level:
                    TextureManager.DrawLevelTextures(this, level, player);
                    if (debugMode)
                    {
                        DebugUtility.DrawDebugInfo(this, player);
                    }
                    break;

                case GameState.Paused:
                    TextureManager.DrawLevelTextures(this, level, player);

                    Window.SetView(Window.DefaultView);
                    Window.Draw(pauseScreen);
                    Window.SetView(player.Camera);

                    if (debugMode)
                    {
                        DebugUtility.DrawDebugInfo(this, player);
                    }
                    break;

                default:
                    break;
            }
        }

        public override void Instantiate()
        {
            menu = new Menu();
            level = new Level();
            player = new Player();
            loadingScreen = new LoadingScreen();
        }

        public override void Initialize()
        {
            loadingScreen.Initialize();
            pauseScreen = new PauseScreen();

            Settings.MusicVolume = 0;
            SoundManager.SetCurrentTrack(Menu.MENU_MUSIC_FILENAME);
            SoundManager.SetMusicVolume((uint)Settings.MusicVolume);

            TextureManager.InitializePlayerSprite(player);
        }

        public override void LoadContent()
        {
            TextureManager.LoadFonts();
            TextureManager.LoadPlayerTextures(player);
        }

        public override void ProcessInputs()
        {
            switch (CurrentState)
            {
                case GameState.Menu:
                    KeyboardManager.ProcessMainMenuKeys(this, menu);
                    break;

                case GameState.Level:
                    KeyboardManager.ProcessPlayerKeys(player);
                    KeyboardManager.ProcessLevelKeys(this, player);
                    break;

                case GameState.Paused:
                    KeyboardManager.ProcessPauseScreenKeys(this, player);
                    break;

                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            SoundManager.PlayMusic();

            switch (CurrentState)
            {
                case GameState.StartingUp:
                    Draw(gameTime);
                    Window.Display();

                    TextureManager.LoadMenuTextures(menu);
                    menu.Initialize();
                    TextureManager.InitializeMenuSprites(menu, loadingScreen);
                    CurrentState = GameState.Menu;
                    break;

                case GameState.LoadingMenu:
                    Draw(gameTime);
                    Window.Display();

                    level.Destroy();
                    level = null;
                    player.Reset();

                    SoundManager.SetCurrentTrack(Menu.MENU_MUSIC_FILENAME);
                    CurrentState = GameState.Menu;
                    break;

                case GameState.LoadingLevel:
                    Draw(gameTime);
                    Window.Display();

                    level = new Level();
                    level.LoadData("level.tmx");
                    level.Initialize("aztec2.png", "lush.ogg");
                    TextureManager.InitializeLevelSprites(level);

                    player.Initialize(level.TileStartPosition);
                    player.SetPlayerCamera(new Vector2f(DEFAULT_WINDOW_WIDTH / 2, DEFAULT_WINDOW_HEIGHT / 2), new Vector2f(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));

                    CurrentState = GameState.Level;
                    break;

                case GameState.Menu:
                    SoundManager.SetMusicVolume((uint)Settings.MusicVolume);
                    SoundManager.PlayMusic();
                    menu.Update(Window.DefaultView);
                    break;

                case GameState.Level:
                    SoundManager.SetMusicVolume((uint)Settings.MusicVolume);
                    SoundManager.PlayMusic();
                    player.Update(level, DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT);
                    level.Update(player);
                    break;

                case GameState.Paused:
                    SoundManager.SetMusicVolume((uint)Settings.MusicVolume / 2);
                    break;

                default:
                    break;
            }
        }

        public void ToggleDebugMode()
        {
            debugMode = !debugMode;
        }
    }
}
