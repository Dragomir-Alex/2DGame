using _2DGame.Animation;
using _2DGame.LayerData;
using _2DGame.Layers;
using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using TransformableHitbox2D;

namespace _2DGame.Entities.Players
{
    public class Player : GameEntity, IAnimated
    {
        public enum State { Idle, Jumping, Falling, Walking, Attacking, Hit, Dead }

        public IAnimated.Direction CurrentDirection { get; set; }
        public State CurrentState { get; private set; }
        public State PreviousFrameState { get; private set; }

        private bool debugMode;
        private bool isGrounded;
        private bool attackedInCurrentAnimation;

        private Vector2f position = new();
        private readonly FrameTimer invincibilityFrames;

        public bool HasFinishedLevel { get; set; }
        public bool IsSpawningProjectile { get; private set; }
        public View Camera { get; set; }
        public Vector2f Velocity { get; set; }
        public Health Health { get; set; }
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
        public const int PROJECTILE_SPAWN_FRAME = 7;
        public const uint INVINCIBILITY_FRAME_COUNT = 120;

        public const float X_MAX_VELOCITY = 5f;
        public const float Y_MAX_VELOCITY = 12f;
        private const float X_VELOCITY_GAIN = 0.4f;
        private const float Y_VELOCITY_GAIN = 12f;
        private const float X_VELOCITY_REDUCTION = 0.15f;
        private const float Y_VELOCITY_REDUCTION = 0.15f;
        private const float X_KNOCKBACK_VELOCITY = 5f;
        private const float Y_KNOCKBACK_VELOCITY = 3f;
        public const float DEBUG_VELOCITY = 10f;
        private const float GRAVITY = 0.5f;

        public const int HITBOX_WIDTH = 20;
        public const int HITBOX_HEIGHT = 40;

        public Player() : base(0)
        {
            debugMode = false;
            isGrounded = false;
            attackedInCurrentAnimation = false;
            IsSpawningProjectile = false;
            HasFinishedLevel = false;

            invincibilityFrames = new FrameTimer(INVINCIBILITY_FRAME_COUNT);

            CurrentState = State.Falling;
            PreviousFrameState = State.Falling;
            CurrentDirection = IAnimated.Direction.Right;

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
            collidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
            if (collidedTiles.Count != 0)
            {
                for (int i = Math.Abs((int)Velocity.X); i >= 0; i--)
                {
                    Vector2f interpolatedPosition = new Vector2f(crtPos.X + i * Math.Sign(Velocity.X), crtPos.Y);
                    Position = interpolatedPosition;
                    List<Tuple<Hitbox, int, int>> newCollidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
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
            collidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
            if (collidedTiles.Count != 0)
            {
                for (int i = Math.Abs((int)Velocity.Y); i >= 0; i--)
                {
                    Vector2f interpolatedPosition = new Vector2f(crtPos.X, crtPos.Y + i * Math.Sign(Velocity.Y));
                    Position = interpolatedPosition;
                    List<Tuple<Hitbox, int, int>> newCollidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
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
            collidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
            if (collidedTiles.Count != 0)
            {
                for (int i = Math.Min(Math.Abs((int)Velocity.X), Math.Abs((int)Velocity.Y)); i >= 0; i--)
                {
                    Vector2f interpolatedPosition = new Vector2f(crtPos.X + i * Math.Sign(Velocity.X), crtPos.Y + i * Math.Sign(Velocity.Y));
                    Position = interpolatedPosition;
                    List<Tuple<Hitbox, int, int>> newCollidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
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

        public void GameEntityCollision(GameEntityManager gameEntityManager)
        {
            foreach (var entity in gameEntityManager.OnScreenGameEntities)
            {
                if (entity.IsActive && entity.Hitbox != null)
                {
                    if (entity.Hitbox.Overlaps(Hitbox))
                    {
                        entity.OnEntityCollision(this);
                    }
                }
            }
        }

        public void GainPositiveXVelocity() { Velocity = new Vector2f(Velocity.X + X_VELOCITY_GAIN, Velocity.Y); }
        public void GainNegativeXVelocity() { Velocity = new Vector2f(Velocity.X - X_VELOCITY_GAIN, Velocity.Y); }
        public void GainPositiveYVelocity() { Velocity = new Vector2f(Velocity.X, Velocity.Y + Y_VELOCITY_GAIN); }
        public void GainNegativeYVelocity() { Velocity = new Vector2f(Velocity.X, Velocity.Y - Y_VELOCITY_GAIN); }

        public void SetXVelocity(float xVelocity) { Velocity = new Vector2f(xVelocity, Velocity.Y); }
        public void SetYVelocity(float yVelocity) { Velocity = new Vector2f(Velocity.X, yVelocity); }

        public void Hurt(int damageAmount)
        {
            if (CurrentState != State.Hit && CurrentState != State.Dead && !invincibilityFrames.IsRunning && !debugMode)
            {
                Health.Damage(damageAmount);

                if (Health.CurrentHealth > 0)
                {
                    CurrentState = State.Hit;
                    invincibilityFrames.Reset();
                    invincibilityFrames.Start();

                    Velocity = new Vector2f((int)((Velocity.X < 0 ? 1 : -1) * X_KNOCKBACK_VELOCITY),
                        (int)((Velocity.Y < 0 ? 1 : -1) * Y_KNOCKBACK_VELOCITY));
                }
                else
                {
                    CurrentState = State.Dead;
                    SoundManager.PlaySound("Vanish");
                }

                SoundManager.PlaySound("Hurt");
            }
        }

        private void ApplyGravity()
        {
            if (!debugMode && CurrentState != State.Dead)
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
            if (CurrentState == State.Dead) return;

            PreviousFrameState = CurrentState;

            if (CurrentState == State.Hit
                && invincibilityFrames.CurrentTime >= INVINCIBILITY_FRAME_COUNT - INVINCIBILITY_FRAME_COUNT / 4)
            {
                return;
            }

            if (CurrentState != State.Attacking && Velocity.X < 1f && Velocity.X > -1f && Velocity.Y < 1f && Velocity.Y > -1f && isGrounded
                || CurrentState == State.Attacking && Sprite.IsFinished())
            {
                CurrentState = State.Idle;
            }
            else if (CurrentState != State.Attacking && Velocity.X != 0f && Velocity.Y < 1f && Velocity.Y > -1f && isGrounded)
            {
                CurrentState = State.Walking;
            }
            else if (Velocity.Y <= -1f)
            {
                CurrentState = State.Jumping;
            }
            else if (Velocity.Y >= 1f)
            {
                CurrentState = State.Falling;
            }

            if ((PreviousFrameState == State.Falling || PreviousFrameState == State.Jumping)
                && (CurrentState == State.Idle || CurrentState == State.Walking || CurrentState == State.Attacking))
            {
                SoundManager.PlaySound("Land");
            }
        }

        public void UpdateAnimatedSprite(GameLoop gameLoop)
        {
            var currentAnimation = Sprite;

            if (CurrentState == State.Idle) { Sprite = TextureManager.PlayerAnimations["Idle"]; }
            else if (CurrentState == State.Jumping) { Sprite = TextureManager.PlayerAnimations["Jump"]; }
            else if (CurrentState == State.Falling) { Sprite = TextureManager.PlayerAnimations["Fall"]; }
            else if (CurrentState == State.Walking) { Sprite = TextureManager.PlayerAnimations["Run"]; }
            else if (CurrentState == State.Attacking) { Sprite = TextureManager.PlayerAnimations["Attack"]; }
            else if (CurrentState == State.Hit) { Sprite = TextureManager.PlayerAnimations["Hit"]; }
            else if (CurrentState == State.Dead) { Sprite = TextureManager.PlayerAnimations["Death"]; }

            if (currentAnimation != Sprite)
            {
                currentAnimation.Restart();
                Sprite.Play();

            }
            else if (CurrentState == State.Walking) { Sprite.SetFPS((int)Math.Abs(Velocity.X) * 5); }

            if (CurrentState == State.Attacking && Sprite.GetCurrentFrame() == PROJECTILE_SPAWN_FRAME && !attackedInCurrentAnimation)
            {
                IsSpawningProjectile = true;
                attackedInCurrentAnimation = true;
            }
            else
            {
                IsSpawningProjectile = false;
            }

            if (CurrentState != State.Attacking)
            {
                attackedInCurrentAnimation = false;
            }

            if (CurrentState == State.Walking && (Sprite.GetCurrentFrame() == 1 || Sprite.GetCurrentFrame() == 9)) // Steps
            {
                SoundManager.PlaySound("Step");
            }

            if (CurrentState == State.Dead && Sprite.IsFinished())
            {
                gameLoop.CurrentState = GameLoop.GameState.GameOver;
            }

            UpdateSpritePosition();
        }

        public override void Update(Level level, GameLoop gameLoop)
        {
            ApplyGravity();
            if (CurrentState == State.Dead)
            {
                Velocity = new();
            }
            else if (!debugMode)
            {
                Velocity = UtilityFunctions.UpdateVelocity(Velocity, X_VELOCITY_REDUCTION, Y_VELOCITY_REDUCTION, X_MAX_VELOCITY, Y_MAX_VELOCITY);
            }

            UpdatePosition((SpriteLayer)level.Layers[LayerList.PRIMARY_LAYER]);
            GameEntityCollision(level.GameEntityManager);
            UpdatePlayerCamera(Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT, level);
            UpdateCurrentState();
            UpdateAnimatedSprite(gameLoop);
            invincibilityFrames.Update();
        }

        private void UpdateAllPositionProperties()
        {
            UpdateSpritePosition();
            UpdateHitbox();
            UpdateTileCoordinates();
        }

        public override void Reset()
        {
            base.Reset();
            CurrentState = State.Falling;
            Velocity = new Vector2f(0, 0);
            Health.Reset();

            debugMode = false;
            attackedInCurrentAnimation = false;
            IsSpawningProjectile = false;
            HasFinishedLevel = false;
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
            UpdateHitbox();
        }

        private void UpdateSpritePosition()
        {
            if (Sprite != null)
            {
                /*int sign = CurrentDirection == IAnimated.Direction.Left ? 1 : -1;
                Sprite.Position = new Vector2f(Position.X + sign * HITBOX_WIDTH + sign * 12, Position.Y - HITBOX_HEIGHT + 7); // Magic numbers :)*/
                Sprite.Position = new Vector2f(Position.X, Position.Y - 6); // Magic numbers :)
            }
        }

        protected override void UpdateHitbox()
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
            Sprite = TextureManager.PlayerAnimations["Fall"];
            Sprite.Play();
            UpdateSpritePosition();
        }

        public void UpButtonAction()
        {
            if (debugMode)
            {
                SetYVelocity(-DEBUG_VELOCITY);
            }
            else if (isGrounded && CurrentState != State.Attacking && CurrentState != State.Hit && CurrentState != State.Dead)
            {
                SetYVelocity(0);
                GainNegativeYVelocity();
                isGrounded = false;
                SoundManager.PlaySound("Jump");
            }
        }

        public void DownButtonAction()
        {
            if (debugMode)
            {
                SetYVelocity(DEBUG_VELOCITY);
            }
        }

        public void LeftButtonAction()
        {
            if (debugMode)
            {
                SetXVelocity(-DEBUG_VELOCITY);
            }
            else if (CurrentState != State.Attacking && CurrentState != State.Hit && CurrentState != State.Dead)
            {
                if (Velocity.X > 0f)
                {
                    Velocity = new Vector2f(-Velocity.X / 1.5f, Velocity.Y);
                }
                else
                {
                    GainNegativeXVelocity();
                }
            }

            if (CurrentState != State.Hit && CurrentState != State.Dead)
            {
                CurrentDirection = IAnimated.Direction.Left;
            }
        }

        public void RightButtonAction()
        {
            if (debugMode)
            {
                SetXVelocity(DEBUG_VELOCITY);
            }
            else if (CurrentState != State.Attacking && CurrentState != State.Hit && CurrentState != State.Dead)
            {
                if (Velocity.X < 0f)
                {
                    Velocity = new Vector2f(-Velocity.X / 1.5f, Velocity.Y);
                }
                else
                {
                    GainPositiveXVelocity();
                }
            }

            if (CurrentState != State.Hit && CurrentState != State.Dead)
            {
                CurrentDirection = IAnimated.Direction.Right;
            }
        }

        public void AttackButtonAction()
        {
            if ( (CurrentState != State.Attacking && CurrentState != State.Hit && CurrentState != State.Dead && isGrounded) || debugMode)
            {
                CurrentState = State.Attacking;
                SetXVelocity(Velocity.X / 2.5f);
            }
        }

        public override void OnEntityCollision(GameEntity entity) { }
    }
}
