using System;
using System.Runtime.CompilerServices;
using _2DGame.Animation;
using _2DGame.Entities;
using _2DGame.Layers;
using _2DGame.LevelUI;
using _2DGame.MainMenu;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace _2DGame.Utility
{
    public static class TextureManager
    {
        public const string TEXTURES_PATH = "./Data/Assets/Textures/Sprites/";
        public const string TILESETS_PATH = "./Data/Assets/Textures/Tilesets/";
        public const string FONTS_PATH = "./Data/Assets/Fonts/";

        public const int PLAYER_SPRITE_WIDTH = 69;
        public const int PLAYER_SPRITE_HEIGHT = 56;

        public static Dictionary<string, Texture> PlayerTextures { get; private set; }
        public static Dictionary<string, AnimatedSprite> PlayerAnimations { get; private set; }

        public static Texture LogoTexture { get; private set; }
        public static Texture HealthTexture { get; private set; }

        public static Font GameFont { get; private set; }
        public static Font GameFontBold { get; private set; }
        public static Font DebugFont { get; private set; }

        public static void LoadFonts()
        {
            GameFont = new Font(FONTS_PATH + "8bitOperatorPlus-Regular.ttf");
            GameFontBold = new Font(FONTS_PATH + "8bitOperatorPlus-Bold.ttf");
            DebugFont = new Font(FONTS_PATH + "8bitOperatorPlus-Regular.ttf");
        }

        public static void LoadMenuTextures()
        {
            LogoTexture = new Texture(TEXTURES_PATH + "Logo.png");
        }

        public static void LoadPlayerTextures()
        {
            PlayerTextures = new Dictionary<string, Texture>
            {
                { "PlayerIdle", new Texture(TEXTURES_PATH + "PlayerIdle.png") },
                { "PlayerRun", new Texture(TEXTURES_PATH + "PlayerRun.png") },
                { "PlayerJump", new Texture(TEXTURES_PATH + "PlayerJump.png") },
                { "PlayerAttack", new Texture(TEXTURES_PATH + "PlayerAttack.png") },
                { "PlayerJumpAttack", new Texture(TEXTURES_PATH + "PlayerJumpAttack.png") },
                { "PlayerHit", new Texture(TEXTURES_PATH + "PlayerHit.png") },
                { "PlayerDeath", new Texture(TEXTURES_PATH + "PlayerDeath.png") }
            };
        }

        public static void LoadHealthTexture()
        {
            HealthTexture = new Texture(TEXTURES_PATH + "Heart.png");
        }

        public static void InitializeMenuSprites(Menu menu, LoadingScreen loadingScreen)
        {
            // Menu
            foreach (var page in menu.Pages)
            {

                if (page.Key == Menu.PageName.MainPage)
                {
                    page.Value.InitializeSprites(LogoTexture);
                    page.Value.LogoSprite.Position = new Vector2f(Game.DEFAULT_WINDOW_WIDTH / 2 - LogoTexture.Size.X / 2, Game.DEFAULT_WINDOW_HEIGHT / 6f);
                }
                else
                {
                    page.Value.InitializeSprites(null);
                }
            }

            // Loading Screen
            loadingScreen.InitializeSprites();
        }

        public static void InitializePlayerSprite(Player player, GameLoop gameLoop)
        {
            PlayerAnimations = new Dictionary<string, AnimatedSprite>();

            if (PlayerTextures["PlayerIdle"] != null)
            {
                PlayerAnimations.Add("PlayerIdle", new AnimatedSprite(PlayerTextures["PlayerIdle"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 18, gameLoop.Window, RenderStates.Default, 0, 12, false, true));
            }

            if (PlayerTextures["PlayerRun"] != null)
            {
                PlayerAnimations.Add("PlayerRun", new AnimatedSprite(PlayerTextures["PlayerRun"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 22, gameLoop.Window, RenderStates.Default, 0, 15, false, true));
            }

            if (PlayerTextures["PlayerJump"] != null)
            {
                PlayerAnimations.Add("PlayerJump", new AnimatedSprite(PlayerTextures["PlayerJump"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 30, gameLoop.Window, RenderStates.Default, 0, 8, false, false));
                PlayerAnimations.Add("PlayerFall", new AnimatedSprite(PlayerTextures["PlayerJump"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 25, gameLoop.Window, RenderStates.Default, 9, 13, false, false));
            }

            if (PlayerTextures["PlayerAttack"] != null)
            {
                PlayerAnimations.Add("PlayerAttack", new AnimatedSprite(PlayerTextures["PlayerAttack"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 10, gameLoop.Window, RenderStates.Default, 0, 9, false, false));
            }

            if (PlayerTextures["PlayerJumpAttack"] != null)
            {
                PlayerAnimations.Add("PlayerJumpAttack", new AnimatedSprite(PlayerTextures["PlayerJumpAttack"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 2, gameLoop.Window, RenderStates.Default, 0, 1, false, false));
            }

            if (PlayerTextures["PlayerHit"] != null)
            {
                PlayerAnimations.Add("PlayerHit", new AnimatedSprite(PlayerTextures["PlayerHit"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 4, gameLoop.Window, RenderStates.Default, 0, 3, false, false));
            }

            if (PlayerTextures["PlayerDeath"] != null)
            {
                PlayerAnimations.Add("PlayerDeath", new AnimatedSprite(PlayerTextures["PlayerDeath"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 10, gameLoop.Window, RenderStates.Default, 0, 9, false, false));
            }

            player.InitializeSprite();
        }

        public static void InitializeLevelSprites(Level level)
        {
            foreach (var layer in level.Layers)
            {
                if (layer is DetailLayer detailLayer)
                    detailLayer.InitializeSprite();
            }
        }

        public static void InitializeHealthBarSprites(HealthBar healthBar, Player player)
        {
            healthBar.Initialize(HealthTexture, player);
        }

        public static void DrawLevelTextures(GameLoop gameLoop, Level level, Player player, bool isPaused)
        {
            foreach (var layer in level.Layers.Reverse())
            {
                if (layer is DetailLayer)
                {
                    gameLoop.Window.SetView(gameLoop.Window.DefaultView); // For UI and background only
                    gameLoop.Window.Draw(layer);
                }
                else
                {
                    gameLoop.Window.SetView(player.Camera); // Player cameras
                    gameLoop.Window.Draw(layer);

                    if (isPaused)
                        player.Sprite.Pause();
                    else
                        player.Sprite.PlayWithoutLoop();

                    player.Sprite.Update(gameLoop.GameTime.DeltaTime, player.CurrentDirection != Player.PlayerDirection.Right);
                }
            }

            gameLoop.Window.SetView(player.Camera); // Back to player camera
        }
    }
}
