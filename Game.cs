using SFML.System;
using _2DGame.Utility;
using Color = SFML.Graphics.Color;
using _2DGame.MainMenu;
using _2DGame.LevelUI;
using _2DGame.Entities.Players;

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
        private GameOverScreen gameOverScreen;
        private Scoreboard scoreboard;
        private HealthBar healthBar;
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
                    TextureManager.DrawLevelTextures(this, level, player, false);

                    Window.SetView(Window.DefaultView);
                    Window.Draw(scoreboard);
                    Window.Draw(healthBar);
                    Window.SetView(player.Camera);

                    if (debugMode)
                    {
                        DebugUtility.DrawDebugInfo(this, player, level);
                    }
                    break;

                case GameState.Paused:
                    TextureManager.DrawLevelTextures(this, level, player, true);

                    Window.SetView(Window.DefaultView);
                    Window.Draw(scoreboard);
                    Window.Draw(healthBar);
                    Window.Draw(pauseScreen);
                    Window.SetView(player.Camera);

                    if (debugMode)
                    {
                        DebugUtility.DrawDebugInfo(this, player, level);
                    }
                    break;

                case GameState.GameOver:
                    TextureManager.DrawLevelTextures(this, level, player, true);

                    Window.SetView(Window.DefaultView);
                    Window.Draw(healthBar);
                    Window.Draw(gameOverScreen);
                    Window.SetView(player.Camera);

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
            scoreboard = new Scoreboard();
            healthBar = new HealthBar();
        }

        public override void Initialize()
        {
            gameOverScreen = new GameOverScreen();
            pauseScreen = new PauseScreen();

            loadingScreen.Initialize();

            Settings.MusicVolume = 0;
            SoundManager.SetCurrentTrack(Menu.MENU_MUSIC_FILENAME);
            SoundManager.SetMusicVolume((uint)Settings.MusicVolume);
            SoundManager.SetSoundVolume((uint)Settings.SoundVolume);

            TextureManager.InitializePlayerSprites(player, this);
            TextureManager.InitializeHealthBarSprites(healthBar, player);
            TextureManager.InitializeFlyingEyeSprites(this);
            TextureManager.InitializeMushroomSprites(this);
        }

        public override void LoadContent()
        {
            TextureManager.LoadFonts();
            TextureManager.LoadPlayerTextures();
            TextureManager.LoadMenuTextures();
            TextureManager.LoadHealthTexture();
            TextureManager.LoadHeartCollectibleTexture();
            TextureManager.LoadGemTexture();
            TextureManager.LoadFlyingEyeTextures();
            TextureManager.LoadMushroomTextures();
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

                case GameState.GameOver:
                    KeyboardManager.ProcessGameOverScreenKeys(this, player, gameOverScreen);
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

                    menu.Initialize();
                    TextureManager.InitializeMenuSprites(menu, loadingScreen);
                    SoundManager.LoadSounds();
                    SoundManager.SetSoundVolume((uint)Settings.SoundVolume);
                    CurrentState = GameState.Menu;
                    break;

                case GameState.LoadingMenu:
                    Draw(gameTime);
                    Window.Display();

                    level.Destroy();
                    level = null;
                    player.Reset();
                    Score.Reset();

                    SoundManager.SetCurrentTrack(Menu.MENU_MUSIC_FILENAME);
                    CurrentState = GameState.Menu;
                    break;

                case GameState.LoadingLevel:
                    Draw(gameTime);
                    Window.Display();

                    level = new Level();
                    level.LoadData("level.tmx", "level_entitydata.json", "level_layerdata.json");
                    level.Initialize("aztec2.png", "lush.ogg");
                    TextureManager.InitializeLevelSprites(level, this);

                    player.Initialize(level.TileStartPosition);
                    player.SetPlayerCamera(new Vector2f(DEFAULT_WINDOW_WIDTH / 2, DEFAULT_WINDOW_HEIGHT / 2), new Vector2f(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT));

                    CurrentState = GameState.Level;
                    break;

                case GameState.Menu:
                    SoundManager.SetMusicVolume((uint)Settings.MusicVolume);
                    SoundManager.SetSoundVolume((uint)Settings.SoundVolume);
                    SoundManager.PlayMusic();
                    menu.Update(Window.DefaultView);
                    break;

                case GameState.Level:
                    SoundManager.SetMusicVolume((uint)Settings.MusicVolume);
                    SoundManager.SetSoundVolume((uint)Settings.SoundVolume);
                    SoundManager.SetListenerPosition(player);
                    SoundManager.PlayMusic();
                    player.Update(level, this);
                    level.Update(player, this);
                    scoreboard.Update();
                    healthBar.Update(player);
                    break;

                case GameState.Paused:
                    SoundManager.SetMusicVolume((uint)Settings.MusicVolume / 2);
                    SoundManager.SetSoundVolume((uint)Settings.SoundVolume / 2);
                    break;

                case GameState.GameOver:
                    SoundManager.SetMusicVolume((uint)Settings.MusicVolume / 2);
                    SoundManager.SetSoundVolume((uint)Settings.SoundVolume / 2);
                    gameOverScreen.Initialize(player.Health.CurrentHealth != 0);
                    gameOverScreen.Update();
                    gameOverScreen.TextEnteredSubscribe(this);
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
