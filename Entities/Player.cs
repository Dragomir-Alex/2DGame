using _2DGame.Animation;
using _2DGame.LayerData;
using _2DGame.Layers;
using _2DGame.Utility;
using InstilledBee.SFML.SimpleCollision;
using NetTopologySuite.Mathematics;
using NetTopologySuite.Triangulate;
using SFML.Graphics;
using SFML.System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using TransformableHitbox2D;

namespace _2DGame.Entities
{
    public class Player : GameEntity
    {
        public enum PlayerDirection { Left, Right };
        public enum PlayerState { Idle, Jumping, Falling, Walking }

        public PlayerDirection CurrentDirection { get; private set; }
        public PlayerState CurrentState { get; private set; }

        private bool debugMode;
        private bool isGrounded;

        private Vector2f position = new();

        public View Camera { get; set; }
        public Vector2f Velocity { get; set; }
        public Health Health { get; set; }
        public Texture? Texture { get; set; }
        public AnimatedSprite Sprite { get; set; }
        public override Vector2f Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateAllPositionProperties();
            }
        }

        public const int MAX_HEALTH = 5;

        public const float X_MAX_VELOCITY = 5f;
        public const float Y_MAX_VELOCITY = 12f;
        private const float X_VELOCITY_GAIN = 0.4f;
        private const float Y_VELOCITY_GAIN = 12f;
        private const float X_VELOCITY_REDUCTION = 0.15f;
        private const float Y_VELOCITY_REDUCTION = 0.15f;
        private const float GRAVITY = 0.5f;

        public const int HITBOX_WIDTH = 20;
        public const int HITBOX_HEIGHT = 40;

        public Player() : base()
        {
            debugMode = false;
            isGrounded = false;

            CurrentState = PlayerState.Falling;
            CurrentDirection = PlayerDirection.Right;

            Texture = null;
            Velocity = new Vector2f(0f, 0f);
            Camera = new View();
            Health = new Health(MAX_HEALTH);
        }

        public override void Initialize(Vector2i startPosition)
        {
            Position = new Vector2f(startPosition.X * Tilemap.TILE_SIZE, startPosition.Y * Tilemap.TILE_SIZE);
            InitializeHitbox();
        }

        public void SetPlayerCamera(Vector2f center, Vector2f size)
        {
            Camera.Center = center;
            Camera.Size = size;
        }

        private void UpdatePlayerCamera(uint screenWidth, uint screenHeight, Level level)
        {
            float xCenter = (int)Position.X + HITBOX_WIDTH;
            float yCenter = (int)Position.Y + HITBOX_HEIGHT;

            if ((int)Position.X + HITBOX_WIDTH <= screenWidth / 2) // Left
            {
                xCenter = (float)screenWidth / 2;

            }
            else if ((int)Position.X + HITBOX_WIDTH >= level.Width - screenWidth / 2) // Right
            {
                xCenter = (int)level.Width - screenWidth / 2;
            }

            if ((int)Position.Y + HITBOX_HEIGHT <= screenHeight / 2) // Top
            {
                yCenter = (float)screenHeight / 2;

            }
            else if ((int)Position.Y + HITBOX_HEIGHT >= level.Height - screenHeight / 2) // Bottom
            {
                yCenter = (int)level.Height - screenHeight / 2;
            }

            Camera.Center = new Vector2f(xCenter, yCenter);
        }

        private void UpdatePosition(SpriteLayer spriteLayer)
        {
            PlayerBorderCollision(spriteLayer);

            Vector2f crtPos = new Vector2f(Position.X, Position.Y);
            Vector2f posAfterXVelocity = new Vector2f(Position.X + (int)Velocity.X, Position.Y);
            Vector2i finalVelocity = new Vector2i((int)Velocity.X, (int)Velocity.Y);
            List<Tuple<Hitbox, int, int>> collidedTiles;

            Position = posAfterXVelocity;

            if (debugMode)
            {
                Velocity = new();
                Position = new Vector2f(crtPos.X + finalVelocity.X, crtPos.Y + finalVelocity.Y);
                return;
            }

            // X Axis test
            collidedTiles = PlayerLevelCollision(spriteLayer);
            if (collidedTiles.Count != 0)
            {
                for (int i = Math.Abs((int)Velocity.X); i >= 0; i--)
                {
                    Vector2f interpolatedPosition = new Vector2f(crtPos.X + i * Math.Sign(Velocity.X), crtPos.Y);
                    Position = interpolatedPosition;
                    List<Tuple<Hitbox, int, int>> newCollidedTiles = PlayerLevelCollision(spriteLayer);
                    if (newCollidedTiles.Count == 0)
                    {
                        finalVelocity.X = i * Math.Sign(Velocity.X);
                        Velocity = new Vector2f(0f, Velocity.Y);
                        break;
                    }
                }
            }

            Position = new Vector2f(crtPos.X + finalVelocity.X, crtPos.Y + (int)Velocity.Y);

            // Y Axis test
            collidedTiles = PlayerLevelCollision(spriteLayer);
            if (collidedTiles.Count != 0)
            {
                for (int i = Math.Abs((int)Velocity.Y); i >= 0; i--)
                {
                    Vector2f interpolatedPosition = new Vector2f(crtPos.X, crtPos.Y + i * Math.Sign(Velocity.Y));
                    Position = interpolatedPosition;
                    List<Tuple<Hitbox, int, int>> newCollidedTiles = PlayerLevelCollision(spriteLayer);
                    if (newCollidedTiles.Count == 0)
                    {
                        finalVelocity.Y = i * Math.Sign(Velocity.Y);
                        Velocity = new Vector2f(Velocity.X, 0f);
                        break;
                    }
                }
            }

            if (finalVelocity.Y == 0 && collidedTiles.Count != 0)
                isGrounded = true;

            Position = new Vector2f(crtPos.X + finalVelocity.X, crtPos.Y + finalVelocity.Y);

            // Corner test
            collidedTiles = PlayerLevelCollision(spriteLayer);
            if (collidedTiles.Count != 0)
            {
                for (int i = Math.Min(Math.Abs((int)Velocity.X), Math.Abs((int)Velocity.Y)); i >= 0; i--)
                {
                    Vector2f interpolatedPosition = new Vector2f(crtPos.X + i * Math.Sign(Velocity.X), crtPos.Y + i * Math.Sign(Velocity.Y));
                    Position = interpolatedPosition;
                    List<Tuple<Hitbox, int, int>> newCollidedTiles = PlayerLevelCollision(spriteLayer);
                    if (newCollidedTiles.Count == 0)
                    {
                        finalVelocity.X = i * Math.Sign(Velocity.X);
                        finalVelocity.Y = i * Math.Sign(Velocity.Y);
                        Velocity = new Vector2f(Velocity.X, 0f);
                        break;
                    }
                }
            }

            Position = new Vector2f(crtPos.X + finalVelocity.X, crtPos.Y + finalVelocity.Y);
        }

        public void PlayerBorderCollision(SpriteLayer spriteLayer)
        {
            if (Position.X - HITBOX_WIDTH / 2 < 0) // Left
            {
                Position = new Vector2f((float)HITBOX_WIDTH / 2, Position.Y);
            }
            else if (Position.X + HITBOX_WIDTH / 2 > spriteLayer.Width) // Right
            {
                Position = new Vector2f(spriteLayer.Width - HITBOX_WIDTH / 2, Position.Y);
            }

            if (Position.Y - HITBOX_HEIGHT / 2 < 0) // Top
            {
                Position = new Vector2f(Position.X, (float)HITBOX_HEIGHT / 2);
            }
            else if (Position.Y + HITBOX_HEIGHT / 2 > spriteLayer.Height) // Bottom
            {
                Position = new Vector2f(Position.X, spriteLayer.Height - HITBOX_HEIGHT / 2);
            }
        }

        public List<Tuple<Hitbox, int, int>> PlayerLevelCollision(SpriteLayer spriteLayer)
        {
            List<Tuple<Hitbox, int, int>> collidedTiles = new();

            for (int i = TileCoordinates.X - 2; i <= TileCoordinates.X + 2; ++i)
            {
                for (int j = TileCoordinates.Y - 2; j <= TileCoordinates.Y + 2; ++j)
                {
                    if (spriteLayer.LayerTilemap.TileHitboxData.ContainsKey((j, i)))
                    {
                        if (spriteLayer.LayerTilemap.TileHitboxData[(j, i)].Overlaps(Hitbox))
                        {
                            var newTile = new Tuple<Hitbox, int, int>(spriteLayer.LayerTilemap.TileHitboxData[(j, i)], j, i);
                            collidedTiles.Add(newTile);
                        }
                    }
                }
            }

            return collidedTiles;
        }

        public void GainPositiveXVelocity() { Velocity = new Vector2f(Velocity.X + X_VELOCITY_GAIN, Velocity.Y); }
        public void GainNegativeXVelocity() { Velocity = new Vector2f(Velocity.X - X_VELOCITY_GAIN, Velocity.Y); }
        public void GainPositiveYVelocity() { Velocity = new Vector2f(Velocity.X, Velocity.Y + Y_VELOCITY_GAIN); }
        public void GainNegativeYVelocity() { Velocity = new Vector2f(Velocity.X, Velocity.Y - Y_VELOCITY_GAIN); }

        public void SetXVelocity(float xVelocity) { Velocity = new Vector2f(xVelocity, Velocity.Y); }
        public void SetYVelocity(float yVelocity) { Velocity = new Vector2f(Velocity.X, yVelocity); }

        private void UpdateVelocity()
        {
            ApplyGravity();

            if (Velocity.X < 0f)
            {
                if (Velocity.X + X_VELOCITY_REDUCTION > 0f)
                    Velocity = new Vector2f(0f, Velocity.Y);
                else
                    Velocity = new Vector2f(Velocity.X + X_VELOCITY_REDUCTION, Velocity.Y);
            }

            if (Velocity.X > 0f)
            {
                if (Velocity.X - X_VELOCITY_REDUCTION < 0f)
                    Velocity = new Vector2f(0f, Velocity.Y);
                else
                    Velocity = new Vector2f(Velocity.X - X_VELOCITY_REDUCTION, Velocity.Y);
            }

            if (Velocity.X >= X_MAX_VELOCITY)
                Velocity = new Vector2f(X_MAX_VELOCITY, Velocity.Y);

            if (Velocity.X <= -X_MAX_VELOCITY)
                Velocity = new Vector2f(-X_MAX_VELOCITY, Velocity.Y);

            if (Velocity.Y < 0f)
            {
                if (Velocity.Y + Y_VELOCITY_REDUCTION > 0f)
                    Velocity = new Vector2f(Velocity.X, 0);
                else
                    Velocity = new Vector2f(Velocity.X, Velocity.Y + Y_VELOCITY_REDUCTION);
            }

            if (Velocity.Y > 0f)
            {
                if (Velocity.Y - Y_VELOCITY_REDUCTION < 0f)
                    Velocity = new Vector2f(Velocity.X, 0);
                else
                    Velocity = new Vector2f(Velocity.X, Velocity.Y - Y_VELOCITY_REDUCTION);
            }

            if (Velocity.Y >= Y_MAX_VELOCITY)
                Velocity = new Vector2f(Velocity.X, Y_MAX_VELOCITY);

            if (Velocity.Y <= -Y_MAX_VELOCITY)
                Velocity = new Vector2f(Velocity.X, -Y_MAX_VELOCITY);
        }

        private void ApplyGravity()
        {
            if (!debugMode)
            {
                if (Velocity.Y + GRAVITY >= Y_MAX_VELOCITY)
                    Velocity = new Vector2f(Velocity.X, Y_MAX_VELOCITY);
                else Velocity = new Vector2f(Velocity.X, Velocity.Y + GRAVITY);

                if (Velocity.Y >= 1f)
                    isGrounded = false;
            }
        }

        public void UpdateCurrentState()
        {
            if (Velocity.X < 1f && Velocity.X > -1f && Velocity.Y < 1f && Velocity.Y > -1f && isGrounded)
            {
                CurrentState = PlayerState.Idle;
            }
            else if (Velocity.X != 0f && Velocity.Y < 1f && Velocity.Y > -1f && isGrounded)
            {
                CurrentState = PlayerState.Walking;
            }
            else if (Velocity.Y <= -1f)
            {
                CurrentState = PlayerState.Jumping;
            }
            else if (Velocity.Y >= 1f)
            {
                CurrentState = PlayerState.Falling;
            }
        }

        public void UpdateAnimatedSprite()
        {
            var currentAnimation = Sprite;

            if (CurrentState == PlayerState.Idle) { Sprite = TextureManager.PlayerAnimations["PlayerIdle"]; }
            if (CurrentState == PlayerState.Walking) { Sprite = TextureManager.PlayerAnimations["PlayerRun"]; }
            if (CurrentState == PlayerState.Jumping) { Sprite = TextureManager.PlayerAnimations["PlayerJump"]; }
            if (CurrentState == PlayerState.Falling) { Sprite = TextureManager.PlayerAnimations["PlayerFall"]; }

            if (currentAnimation != Sprite)
            {
                currentAnimation.Restart();
                Sprite.Play();
            }


            UpdateSpritePosition();
        }

        public override void Update(Level level, GameLoop gameLoop)
        {
            UpdateVelocity();
            UpdatePosition((SpriteLayer)level.Layers[LayerList.PRIMARY_LAYER]);
            UpdatePlayerCamera(Game.DEFAULT_WINDOW_WIDTH, Game.DEFAULT_WINDOW_HEIGHT, level);
            UpdateCurrentState();
            UpdateAnimatedSprite();
        }

        private void UpdateAllPositionProperties()
        {
            UpdateSpritePosition();
            UpdateHitboxPosition();
            UpdateTileCoordinates();
        }

        public override void Reset()
        {
            base.Reset();
            Velocity = new Vector2f(0, 0);
            Health.Reset();
        }

        public void ToggleDebugMode()
        {
            debugMode = !debugMode;
        }

        private void InitializeHitbox()
        {
            Vector2[] vector2Arr = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(HITBOX_WIDTH, 0),
                new Vector2(HITBOX_WIDTH, HITBOX_HEIGHT),
                new Vector2(0, HITBOX_HEIGHT),
                new Vector2(0, 0)
            };
            Hitbox = new Hitbox(vector2Arr);
            UpdateHitboxPosition();
        }

        private void UpdateSpritePosition()
        {
            if (Sprite != null)
            {
                int sign = (CurrentDirection == PlayerDirection.Left) ? 1 : -1;
                Sprite.Position = new Vector2f(Position.X + sign * HITBOX_WIDTH + sign * 12, Position.Y - HITBOX_HEIGHT + 8); // Magic numbers :)
            }
        }

        protected override void UpdateHitboxPosition()
        {
            TransformableHitbox2D.Transform transform = new();
            transform.Position = new Vector2(Position.X - HITBOX_WIDTH / 2, Position.Y - HITBOX_HEIGHT / 2);
            if (Hitbox != null)
            {
                Hitbox.Transform(transform);
            }
        }

        public void InitializeSprite()
        {
            Sprite = TextureManager.PlayerAnimations["PlayerFall"];
            Sprite.Play();
            UpdateSpritePosition();
        }

        public void UpButtonAction()
        {
            if (debugMode)
            {
                SetYVelocity(-Player.Y_MAX_VELOCITY - 1);
            }
            else if (isGrounded)
            {
                GainNegativeYVelocity();
                isGrounded = false;
            }
        }

        public void DownButtonAction()
        {
            if (debugMode)
            {
                SetYVelocity(Player.Y_MAX_VELOCITY + 1);
            }
        }

        public void LeftButtonAction()
        {
            if (debugMode)
            {
                SetXVelocity(-Player.X_MAX_VELOCITY - 1);
            }
            else
            {
                GainNegativeXVelocity();
            }
            CurrentDirection = PlayerDirection.Left;
        }

        public void RightButtonAction()
        {
            if (debugMode)
            {
                SetXVelocity(Player.X_MAX_VELOCITY + 1);
            }
            else
            {
                GainPositiveXVelocity();
            }
            CurrentDirection = PlayerDirection.Right;
        }
    }
}
