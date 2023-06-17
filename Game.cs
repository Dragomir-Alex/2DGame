using SFML.System;
using _2DGame.Utility;
using Color = SFML.Graphics.Color;
using _2DGame.MainMenu;
using _2DGame.LevelUI;
using _2DGame.Entities.Players;
using SFML.Graphics;
using System.Text.Json;
using _2DGame.GameSettings;
using _2DGame.UIElements;

namespace _2DGame
{
    public class Game : GameLoop
    {
        public const uint WINDOW_WIDTH = 1280;
        public const uint WINDOW_HEIGHT = 720;
        public const string WINDOW_TITLE = "Witchi";

        public Game() : base(WINDOW_WIDTH, WINDOW_HEIGHT, WINDOW_TITLE, Color.Black) { }
        private Level level;
        private Player player;
        private Menu menu;
        private LoadingScreen loadingScreen;
        private PauseScreen pauseScreen;
        private GameOverScreen gameOverScreen;
        private Scoreboard scoreboard;
        private HealthBar healthBar;
        private Leaderboard leaderboard;
        private bool debugMode;

        public override void Draw(GameTime gameTime)
        {
            switch (CurrentState)
            {
                case GameState.StartingUp:
                    RenderTexture.Draw(loadingScreen);
                    break;

                case GameState.LoadingMenu:
                    RenderTexture.Draw(loadingScreen);
                    break;

                case GameState.LoadingLevel:
                    RenderTexture.Draw(loadingScreen);
                    break;

                case GameState.Menu:
                    RenderTexture.SetView(RenderTexture.DefaultView);
                    RenderTexture.Draw(menu);
                    break;

                case GameState.Level:
                    TextureManager.DrawLevelTextures(this, level, player, false);

                    RenderTexture.SetView(RenderTexture.DefaultView);
                    RenderTexture.Draw(scoreboard);
                    RenderTexture.Draw(healthBar);
                    RenderTexture.SetView(player.Camera);

                    if (debugMode)
                    {
                        DebugUtility.DrawDebugInfo(this, player, level);
                    }
                    break;

                case GameState.Paused:
                    TextureManager.DrawLevelTextures(this, level, player, true);

                    RenderTexture.SetView(RenderTexture.DefaultView);
                    RenderTexture.Draw(scoreboard);
                    RenderTexture.Draw(healthBar);
                    RenderTexture.Draw(pauseScreen);
                    RenderTexture.SetView(player.Camera);

                    if (debugMode)
                    {
                        DebugUtility.DrawDebugInfo(this, player, level);
                    }
                    break;

                case GameState.GameOver:
                    TextureManager.DrawLevelTextures(this, level, player, true);

                    RenderTexture.SetView(RenderTexture.DefaultView);
                    RenderTexture.Draw(healthBar);
                    RenderTexture.Draw(gameOverScreen);
                    RenderTexture.SetView(player.Camera);

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
            leaderboard = new Leaderboard();
        }

        public override void Initialize()
        {
            LanguageManager.Initialize();

            gameOverScreen = new GameOverScreen();
            pauseScreen = new PauseScreen();

            loadingScreen.Initialize();

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
            TextureManager.LoadTilesetTexture();
            TextureManager.LoadMenuTextures();
            TextureManager.LoadHealthTexture();
            TextureManager.LoadHeartCollectibleTexture();
            TextureManager.LoadGemTexture();
            TextureManager.LoadFlyingEyeTextures();
            TextureManager.LoadMushroomTextures();
            TextureManager.LoadEndPortalTexture();
            Settings.Load();
        }

        public override void ProcessInputs()
        {
            switch (CurrentState)
            {
                case GameState.Menu:
                    KeyboardManager.ProcessMainMenuKeys(this, menu);
                    break;

                case GameState.Level:
                    KeyboardManager.ProcessPlayerKeys(this, player);
                    KeyboardManager.ProcessLevelKeys(this, player);
                    break;

                case GameState.Paused:
                    KeyboardManager.ProcessPauseScreenKeys(this, player);
                    break;

                case GameState.GameOver:
                    KeyboardManager.ProcessGameOverScreenKeys(this, leaderboard, gameOverScreen);
                    break;

                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            SoundManager.PlayMusic();
            XInputController.Update();

            switch (CurrentState)
            {
                case GameState.StartingUp:
                    Draw(gameTime);
                    Display();

                    menu.Initialize();
                    TextureManager.InitializeMenuSprites(menu, loadingScreen);
                    SoundManager.LoadSounds();
                    SoundManager.SetSoundVolume((uint)Settings.SoundVolume);
                    leaderboard.Load();
                    menu.UpdateLocalLeaderboard(leaderboard);
                    menu.UpdateGlobalLeaderboard(leaderboard);
                    CurrentState = GameState.Menu;
                    break;

                case GameState.LoadingMenu:
                    Draw(gameTime);
                    Display();

                    player.Reset();
                    Score.Reset();
                    menu.UpdateLocalLeaderboard(leaderboard);
                    menu.UpdateGlobalLeaderboard(leaderboard);

                    SoundManager.SetCurrentTrack(Menu.MENU_MUSIC_FILENAME);
                    CurrentState = GameState.Menu;
                    break;

                case GameState.LoadingLevel:
                    Draw(gameTime);
                    Display();

                    level.LoadData("level.tmx", "level_entitydata.json", "level_layerdata.json");
                    level.Initialize("level_music.ogg");
                    TextureManager.InitializeLevelSprites(level, this);

                    player.Initialize(level.TileStartPosition);
                    player.SetPlayerCamera(new Vector2f(WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2), new Vector2f(WINDOW_WIDTH, WINDOW_HEIGHT));

                    CurrentState = GameState.Level;
                    break;

                case GameState.Menu:
                    SoundManager.SetMusicVolume((uint)Settings.MusicVolume);
                    SoundManager.SetSoundVolume((uint)Settings.SoundVolume);
                    SoundManager.PlayMusic();
                    menu.Update(RenderTexture.DefaultView, GameTime.DeltaTime, TIME_UNTIL_UPDATE);
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
                    SoundManager.StopMusic();
                    gameOverScreen.Initialize(this, player.Health.CurrentHealth != 0);
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

        public void TogglePause()
        {
            if (CurrentState == GameState.Paused)
            {
                CurrentState = GameState.Level;
            }
            else if (CurrentState == GameState.Level)
            {
                CurrentState = GameState.Paused;
            }
        }
    }
}
