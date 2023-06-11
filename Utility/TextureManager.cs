using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using _2DGame.Animation;
using _2DGame.Entities.Collectibles;
using _2DGame.Entities.Players;
using _2DGame.Layers;
using _2DGame.LevelUI;
using _2DGame.MainMenu;
using _2DGame.UIElements;
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
        public const int PROJECTILE_SPRITE_WIDTH = 27;
        public const int PROJECTILE_SPRITE_HEIGHT = 16;
        public const int FLYING_EYE_SPRITE_WIDTH = 50;
        public const int FLYING_EYE_SPRITE_HEIGHT = 50;
        public const int MUSHROOM_SPRITE_WIDTH = 100;
        public const int MUSHROOM_SPRITE_HEIGHT = 50;

        public static Dictionary<string, Texture> PlayerTextures { get; private set; }
        public static Dictionary<string, AnimatedSprite> PlayerAnimations { get; private set; }
        public static Dictionary<string, Texture> FlyingEyeTextures { get; private set; }
        public static Dictionary<string, AnimatedSprite> FlyingEyeAnimations { get; private set; }
        public static Dictionary<string, Texture> MushroomTextures { get; private set; }
        public static Dictionary<string, AnimatedSprite> MushroomAnimations { get; private set; }

        public static Texture TilesetTexture { get; private set; }
        public static Texture MenuBackgroundTexture { get; private set; }
        public static Texture LogoTexture { get; private set; }
        public static Texture HealthTexture { get; private set; }
        public static Texture HeartCollectibleTexture { get; private set; }
        public static Texture GemTexture { get; private set; }
        public static Texture EndPortalTexture { get; private set; }

        public static Font GameFont { get; private set; }
        public static Font GameFontBold { get; private set; }
        public static Font DebugFont { get; private set; }

        public static void LoadFonts()
        {
            GameFont = new Font(FONTS_PATH + "8bitOperatorPlus-Regular.ttf");
            GameFontBold = new Font(FONTS_PATH + "8bitOperatorPlus-Bold.ttf");
            DebugFont = new Font(FONTS_PATH + "8bitOperatorPlus-Regular.ttf");
        }

        public static void LoadTilesetTexture()
        {
            TilesetTexture = new Texture(TILESETS_PATH + "aztec2.png");
        }

        public static void LoadMenuTextures()
        {
            LogoTexture = new Texture(TEXTURES_PATH + "Logo.png");
            MenuBackgroundTexture = new Texture(TILESETS_PATH + "menu.png");
        }

        public static void LoadPlayerTextures()
        {
            PlayerTextures = new Dictionary<string, Texture>
            {
                { "Idle", new Texture(TEXTURES_PATH + "PlayerIdle.png") },
                { "Run", new Texture(TEXTURES_PATH + "PlayerRun.png") },
                { "Jump", new Texture(TEXTURES_PATH + "PlayerJump.png") },
                { "Attack", new Texture(TEXTURES_PATH + "PlayerAttack.png") },
                { "Hit", new Texture(TEXTURES_PATH + "PlayerHit.png") },
                { "Death", new Texture(TEXTURES_PATH + "PlayerDeath.png") },
                { "Projectile", new Texture(TEXTURES_PATH + "MagicSpell.png") }
            };
        }

        public static void LoadFlyingEyeTextures()
        {
            FlyingEyeTextures = new Dictionary<string, Texture>
            {
                { "Fly", new Texture(TEXTURES_PATH + "FlyingEyeFly.png") },
                { "Attack", new Texture(TEXTURES_PATH + "FlyingEyeAttack.png") },
                { "Hit", new Texture(TEXTURES_PATH + "FlyingEyeHit.png") },
                { "Death", new Texture(TEXTURES_PATH + "FlyingEyeDeath.png") }
            };
        }

        public static void LoadMushroomTextures()
        {
            MushroomTextures = new Dictionary<string, Texture>
            {
                { "Idle", new Texture(TEXTURES_PATH + "MushroomIdle.png") },
                { "Run", new Texture(TEXTURES_PATH + "MushroomRun.png") },
                { "Attack", new Texture(TEXTURES_PATH + "MushroomAttack.png") },
                { "Hit", new Texture(TEXTURES_PATH + "MushroomHit.png") },
                { "Death", new Texture(TEXTURES_PATH + "MushroomDeath.png") }
            };
        }

        public static void LoadHealthTexture()
        {
            HealthTexture = new Texture(TEXTURES_PATH + "Heart.png");
        }

        public static void LoadHeartCollectibleTexture()
        {
            HeartCollectibleTexture = new Texture(TEXTURES_PATH + "HeartCollectible.png");
        }

        public static void LoadGemTexture()
        {
            GemTexture = new Texture(TEXTURES_PATH + "Gem.png");
        }
        public static void LoadEndPortalTexture()
        {
            EndPortalTexture = new Texture(TEXTURES_PATH + "EndPortal.png");
        }

        public static void InitializeMenuSprites(Menu menu, LoadingScreen loadingScreen)
        {
            // Menu
            foreach (var page in menu.Pages)
            {

                if (page.Key == Menu.PageName.MainPage)
                {
                    page.Value.InitializeSprites(LogoTexture);
                    page.Value.LogoSprite.Position = new Vector2f(Game.WINDOW_WIDTH / 2 - LogoTexture.Size.X / 2, Game.WINDOW_HEIGHT / 6f);
                }
                else
                {
                    page.Value.InitializeSprites(null);
                }
            }

            // Loading Screen
            loadingScreen.InitializeSprites();
        }

        public static void InitializePlayerSprites(Player player, GameLoop gameLoop)
        {
            PlayerAnimations = new Dictionary<string, AnimatedSprite>();

            if (PlayerTextures["Idle"] != null)
            {
                PlayerAnimations.Add("Idle", new AnimatedSprite(PlayerTextures["Idle"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 18, gameLoop.RenderTexture, RenderStates.Default, 0, 12, false, true));
            }

            if (PlayerTextures["Run"] != null)
            {
                PlayerAnimations.Add("Run", new AnimatedSprite(PlayerTextures["Run"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 22, gameLoop.RenderTexture, RenderStates.Default, 0, 15, false, true));
            }

            if (PlayerTextures["Jump"] != null)
            {
                PlayerAnimations.Add("Jump", new AnimatedSprite(PlayerTextures["Jump"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 30, gameLoop.RenderTexture, RenderStates.Default, 0, 8, false, false));
                PlayerAnimations.Add("Fall", new AnimatedSprite(PlayerTextures["Jump"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 25, gameLoop.RenderTexture, RenderStates.Default, 9, 13, false, false));
            }

            if (PlayerTextures["Attack"] != null)
            {
                PlayerAnimations.Add("Attack", new AnimatedSprite(PlayerTextures["Attack"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 20, gameLoop.RenderTexture, RenderStates.Default, 0, 9, false, false));
            }

            if (PlayerTextures["Hit"] != null)
            {
                PlayerAnimations.Add("Hit", new AnimatedSprite(PlayerTextures["Hit"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 15, gameLoop.RenderTexture, RenderStates.Default, 0, 4, false, false));
            }

            if (PlayerTextures["Death"] != null)
            {
                PlayerAnimations.Add("Death", new AnimatedSprite(PlayerTextures["Death"], PLAYER_SPRITE_WIDTH, PLAYER_SPRITE_HEIGHT, 12, gameLoop.RenderTexture, RenderStates.Default, 0, 11, false, false));
            }

            if (PlayerTextures["Projectile"] != null)
            {
                PlayerAnimations.Add("ProjectileStart", new AnimatedSprite(PlayerTextures["Projectile"], PROJECTILE_SPRITE_WIDTH, PROJECTILE_SPRITE_HEIGHT, 20, gameLoop.RenderTexture, RenderStates.Default, 0, 3, false, false));
                PlayerAnimations.Add("ProjectileMiddle", new AnimatedSprite(PlayerTextures["Projectile"], PROJECTILE_SPRITE_WIDTH, PROJECTILE_SPRITE_HEIGHT, 1, gameLoop.RenderTexture, RenderStates.Default, 3, 3, false, true));
                PlayerAnimations.Add("ProjectileEnd", new AnimatedSprite(PlayerTextures["Projectile"], PROJECTILE_SPRITE_WIDTH, PROJECTILE_SPRITE_HEIGHT, 20, gameLoop.RenderTexture, RenderStates.Default, 3, 7, false, false));
            }

            player.InitializeSprite();
        }

        public static void InitializeFlyingEyeSprites(GameLoop gameLoop)
        {
            FlyingEyeAnimations = new Dictionary<string, AnimatedSprite>();

            if (FlyingEyeTextures["Fly"] != null)
            {
                FlyingEyeAnimations.Add("Fly", new AnimatedSprite(FlyingEyeTextures["Fly"], FLYING_EYE_SPRITE_WIDTH, FLYING_EYE_SPRITE_HEIGHT, 10, gameLoop.RenderTexture, RenderStates.Default, 0, 7, false, true));
            }

            if (FlyingEyeTextures["Attack"] != null)
            {
                FlyingEyeAnimations.Add("Attack", new AnimatedSprite(FlyingEyeTextures["Attack"], FLYING_EYE_SPRITE_WIDTH, FLYING_EYE_SPRITE_HEIGHT, 15, gameLoop.RenderTexture, RenderStates.Default, 0, 7, false, true));
            }

            if (FlyingEyeTextures["Hit"] != null)
            {
                FlyingEyeAnimations.Add("Hit", new AnimatedSprite(FlyingEyeTextures["Hit"], FLYING_EYE_SPRITE_WIDTH, FLYING_EYE_SPRITE_HEIGHT, 10, gameLoop.RenderTexture, RenderStates.Default, 0, 4, false, false));
            }

            if (FlyingEyeTextures["Death"] != null)
            {
                FlyingEyeAnimations.Add("DeathFall", new AnimatedSprite(FlyingEyeTextures["Death"], FLYING_EYE_SPRITE_WIDTH + 10, FLYING_EYE_SPRITE_HEIGHT, 10, gameLoop.RenderTexture, RenderStates.Default, 0, 3, false, false));
                FlyingEyeAnimations.Add("DeathLand", new AnimatedSprite(FlyingEyeTextures["Death"], FLYING_EYE_SPRITE_WIDTH + 10, FLYING_EYE_SPRITE_HEIGHT, 20, gameLoop.RenderTexture, RenderStates.Default, 3, 5, false, false));
            }
        }

        public static void InitializeMushroomSprites(GameLoop gameLoop)
        {
            MushroomAnimations = new Dictionary<string, AnimatedSprite>();

            if (MushroomTextures["Idle"] != null)
            {
                MushroomAnimations.Add("Idle", new AnimatedSprite(MushroomTextures["Idle"], MUSHROOM_SPRITE_WIDTH, MUSHROOM_SPRITE_HEIGHT, 8, gameLoop.RenderTexture, RenderStates.Default, 0, 3, false, true));
            }

            if (MushroomTextures["Run"] != null)
            {
                MushroomAnimations.Add("Run", new AnimatedSprite(MushroomTextures["Run"], MUSHROOM_SPRITE_WIDTH, MUSHROOM_SPRITE_HEIGHT, 12, gameLoop.RenderTexture, RenderStates.Default, 0, 7, false, true));
            }

            if (MushroomTextures["Attack"] != null)
            {
                MushroomAnimations.Add("Attack", new AnimatedSprite(MushroomTextures["Attack"], MUSHROOM_SPRITE_WIDTH, MUSHROOM_SPRITE_HEIGHT, 15, gameLoop.RenderTexture, RenderStates.Default, 0, 7, false, true));
            }

            if (MushroomTextures["Hit"] != null)
            {
                MushroomAnimations.Add("Hit", new AnimatedSprite(MushroomTextures["Hit"], MUSHROOM_SPRITE_WIDTH, MUSHROOM_SPRITE_HEIGHT, 10, gameLoop.RenderTexture, RenderStates.Default, 0, 4, false, false));
            }

            if (MushroomTextures["Death"] != null)
            {
                MushroomAnimations.Add("Death", new AnimatedSprite(MushroomTextures["Death"], MUSHROOM_SPRITE_WIDTH, MUSHROOM_SPRITE_HEIGHT, 10, gameLoop.RenderTexture, RenderStates.Default, 0, 5, false, false));
            }
        }

        public static void InitializeLevelSprites(Level level, GameLoop gameLoop)
        {
            foreach (var layer in level.Layers)
            {
                if (layer is DetailLayer detailLayer)
                    detailLayer.InitializeSprite();
            }

            foreach (var entity in level.GameEntityManager.GameEntities)
            {
                switch (entity.ID)
                {
                    case 1:
                        (entity as IAnimated).Sprite = new AnimatedSprite(GemTexture, Gem.WIDTH, Gem.HEIGHT, 8, gameLoop.RenderTexture, RenderStates.Default, 0, 6, true, true);
                        (entity as IAnimated).Sprite.Color = new Color((entity as IAnimated).Sprite.Color.R, (entity as IAnimated).Sprite.Color.G, (entity as IAnimated).Sprite.Color.B, 210);
                        break;

                    case 3:
                        (entity as IAnimated).Sprite = new AnimatedSprite(FlyingEyeAnimations["Fly"]);
                        break;

                    case 4:
                        (entity as IAnimated).Sprite = new AnimatedSprite(HeartCollectibleTexture, Heart.WIDTH, Heart.HEIGHT, 8, gameLoop.RenderTexture, RenderStates.Default, 0, 5, true, true);
                        break;

                    case 5:
                        (entity as IAnimated).Sprite = new AnimatedSprite(TextureManager.MushroomAnimations["Idle"]);
                        break;

                    case 6:
                        (entity as IAnimated).Sprite = new AnimatedSprite(EndPortalTexture, EndPortal.WIDTH, EndPortal.HEIGHT, 10, gameLoop.RenderTexture, RenderStates.Default, 0, 3, true, true);
                        break;

                    default:
                        break;
                }
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
                    gameLoop.RenderTexture.SetView(gameLoop.RenderTexture.DefaultView); // For UI and background only
                    gameLoop.RenderTexture.Draw(layer);
                }
                else
                {
                    gameLoop.RenderTexture.SetView(player.Camera); // Player cameras
                    gameLoop.RenderTexture.Draw(layer);

                    if (isPaused) player.Sprite.Pause();
                    else player.Sprite.PlayWithoutLoop();
                    player.Sprite.Update(gameLoop.GameTime.DeltaTime, player.CurrentDirection != IAnimated.Direction.Right);

                    foreach (var gameEntity in level.GameEntityManager.OnScreenGameEntities)
                    {
                        if (gameEntity.IsActive)
                        {
                            if (gameEntity is IAnimated)
                            {
                                if (isPaused) (gameEntity as IAnimated).Sprite.Pause();
                                else (gameEntity as IAnimated).Sprite.PlayWithoutLoop();
                                (gameEntity as IAnimated).Sprite.Update(gameLoop.GameTime.DeltaTime, (gameEntity as IAnimated).CurrentDirection != IAnimated.Direction.Right);
                            }
                        }
                    }
                }
            }

            gameLoop.RenderTexture.SetView(player.Camera); // Back to player camera
        }
    }
}
