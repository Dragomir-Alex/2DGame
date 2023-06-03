using _2DGame.Animation;
using _2DGame.LayerData;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using _2DGame.Entities.Players;
using TransformableHitbox2D;
using _2DGame.Utility;
using _2DGame.Layers;
using System.Diagnostics;

namespace _2DGame.Entities.Enemies
{
    public class FlyingEye : GameEntity, IEnemy, IAnimated
    {
        private Vector2f position, currentVelocityReduction;
        private bool isGrounded, biteSoundPlayed, flapSoundPlayed;
        private readonly FrameTimer invincibilityFrames;
        private float xPlayerDistance, yPlayerDistance;

        public enum State { Flying, Attacking, Hit, Dead }
        public State CurrentState { get; private set; }
        public State PreviousFrameState { get; private set; }

        public int AttackDamage { get; }
        public int ScoreOnDeath { get; }
        public Health Health { get; set; }
        public AnimatedSprite Sprite { get; set; }
        public IAnimated.Direction CurrentDirection { get; set; }

        public Vector2f Velocity { get; set; }
        public Vector2f Origin { get; private set; }
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
        public const float TRIGGER_DISTANCE = 5f * Tilemap.TILE_SIZE;
        public const float ATTACK_TRIGGER_DISTANCE = 1.5f * Tilemap.TILE_SIZE;
        public const uint INVINCIBILITY_FRAME_COUNT = 25;

        public const float MAX_VELOCITY = 3f;
        public const float FALL_MAX_VELOCITY = 5f;
        private const float VELOCITY_GAIN = 0.3f;
        private const float VELOCITY_REDUCTION = 0.2f;
        private const float GRAVITY = 0.5f;

        public const int HITBOX_WIDTH = 40;
        public const int HITBOX_HEIGHT = 30;

        public FlyingEye() : base(3)
        {
            currentVelocityReduction = new Vector2f();
            invincibilityFrames = new FrameTimer(INVINCIBILITY_FRAME_COUNT);
            isGrounded = false;
            biteSoundPlayed = false;
            flapSoundPlayed = false;
            CurrentState = State.Flying;
            PreviousFrameState = State.Flying;
            AttackDamage = 1;
            ScoreOnDeath = 100;
            Health = new Health(3);
            Velocity = new Vector2f();
        }

        public override void Initialize(Vector2i startPosition)
        {
            Position = new Vector2f(startPosition.X * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2, startPosition.Y * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2);
            Origin = Position;
            InitializeHitbox();
        }

        public override void OnEntityCollision(GameEntity gameEntity)
        {
            if (IsActive)
            {
                if (gameEntity is PlayerProjectile)
                {
                    if (CurrentState != State.Hit && CurrentState != State.Dead)
                    {
                        Health.Damage(PlayerProjectile.DAMAGE);
                        Velocity = new Vector2f();

                        if (Health.CurrentHealth > 0)
                        {
                            CurrentState = State.Hit;
                            SoundManager.Play2DSound("Hiss", position.X, position.Y);
                        }
                        else
                        {
                            CurrentState = State.Dead;
                            SoundManager.Play2DSound("Low Hiss", position.X, position.Y);
                            Score.Add(ScoreOnDeath);
                        }

                        invincibilityFrames.Reset();
                        invincibilityFrames.Start();

                        (gameEntity as PlayerProjectile).CurrentState = PlayerProjectile.State.Disappearing;
                    }
                }
                else if (gameEntity is Player)
                {
                    if (CurrentState != State.Hit && CurrentState != State.Dead)
                        (gameEntity as Player).Hurt(AttackDamage);
                }
            }
        }

        public void OnPlayerDetection(Player player)
        {
            xPlayerDistance = Math.Abs(player.Position.X - Position.X);
            yPlayerDistance = Math.Abs(player.Position.Y - Position.Y);
            float positionToPlayerDistance = (float)Math.Sqrt((float)Math.Pow(player.Position.X - Position.X, 2) + (float)Math.Pow(player.Position.Y - Position.Y, 2));

            if (CurrentState == State.Hit || CurrentState == State.Dead)
                return;

            float xPlayerPercentage = (xPlayerDistance + yPlayerDistance != 0) ? (xPlayerDistance / (xPlayerDistance + yPlayerDistance)) : 0;
            float yPlayerPercentage = (xPlayerDistance + yPlayerDistance != 0) ? (yPlayerDistance / (xPlayerDistance + yPlayerDistance)) : 0;

            float xOriginDistance = Math.Abs(Origin.X - Position.X);
            float yOriginDistance = Math.Abs(Origin.Y - Position.Y);
            float xOriginPercentage = (xOriginDistance + yOriginDistance != 0) ? (xOriginDistance / (xOriginDistance + yOriginDistance)) : 0;
            float yOriginPercentage = (xOriginDistance + yOriginDistance != 0) ? (yOriginDistance / (xOriginDistance + yOriginDistance)) : 0;

            float originToPlayerDistance = (float)Math.Sqrt((float)Math.Pow(player.Position.X - Origin.X, 2) + (float)Math.Pow(player.Position.Y - Origin.Y, 2));
            float positionToOriginDistance = (float)Math.Sqrt((float)Math.Pow(Position.X - Origin.X, 2) + (float)Math.Pow(Position.Y - Origin.Y, 2));

            if (originToPlayerDistance <= TRIGGER_DISTANCE)
            {
                if (player.Position.X >= Position.X)
                {
                    GainPositiveXVelocity(VELOCITY_GAIN * xPlayerPercentage);
                    CurrentDirection = IAnimated.Direction.Right;
                }
                else
                {
                    GainNegativeXVelocity(VELOCITY_GAIN * xPlayerPercentage);
                    CurrentDirection = IAnimated.Direction.Left;
                }

                if (player.Position.Y >= Position.Y)
                {
                    GainPositiveYVelocity(VELOCITY_GAIN * yPlayerPercentage);
                }
                else
                {
                    GainNegativeYVelocity(VELOCITY_GAIN * yPlayerPercentage);
                }

                currentVelocityReduction = new Vector2f(VELOCITY_REDUCTION * xPlayerPercentage, VELOCITY_REDUCTION * yPlayerPercentage);
            }
            else
            {
                if (positionToOriginDistance < 2f)
                {
                    Position = Origin;
                    return;
                }

                if (Origin.X >= Position.X)
                {
                    GainPositiveXVelocity(VELOCITY_GAIN * xOriginPercentage);
                    CurrentDirection = IAnimated.Direction.Right;
                }
                else
                {
                    GainNegativeXVelocity(VELOCITY_GAIN * xOriginPercentage);
                    CurrentDirection = IAnimated.Direction.Left;
                }

                if (Origin.Y >= Position.Y)
                {
                    GainPositiveYVelocity(VELOCITY_GAIN * yOriginPercentage);
                }
                else
                {
                    GainNegativeYVelocity(VELOCITY_GAIN * yOriginPercentage);
                }

                currentVelocityReduction = new Vector2f(VELOCITY_REDUCTION * xOriginPercentage, VELOCITY_REDUCTION * yOriginPercentage);
            }
        }

        public void GainPositiveXVelocity(float xVelocityGain) { Velocity = new Vector2f(Velocity.X + xVelocityGain, Velocity.Y); }
        public void GainNegativeXVelocity(float xVelocityGain) { Velocity = new Vector2f(Velocity.X - xVelocityGain, Velocity.Y); }
        public void GainPositiveYVelocity(float yVelocityGain) { Velocity = new Vector2f(Velocity.X, Velocity.Y + yVelocityGain); }
        public void GainNegativeYVelocity(float yVelocityGain) { Velocity = new Vector2f(Velocity.X, Velocity.Y - yVelocityGain); }

        public override void Update(Level level, GameLoop gameLoop)
        {
            ApplyGravity();
            Velocity = UtilityFunctions.UpdateVelocity(Velocity, currentVelocityReduction.X, currentVelocityReduction.Y, MAX_VELOCITY, MAX_VELOCITY);
            UpdatePosition((SpriteLayer)level.Layers[LayerList.PRIMARY_LAYER]);
            UpdateCurrentState();
            UpdateAnimatedSprite();
            invincibilityFrames.Update();
        }

        private void ApplyGravity()
        {
            if (CurrentState == State.Dead && Sprite.GetCurrentFrame() > 2)
            {
                if (Velocity.Y + GRAVITY >= FALL_MAX_VELOCITY)
                    Velocity = new Vector2f(Velocity.X, FALL_MAX_VELOCITY);
                else Velocity = new Vector2f(Velocity.X, Velocity.Y + GRAVITY);

                if (Velocity.Y >= 1f)
                    isGrounded = false;
            }
        }

        private void UpdateAllPositionProperties()
        {
            UpdateSpritePosition();
            UpdateHitbox();
            UpdateTileCoordinates();
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
                Sprite.Position = new Vector2f(Position.X, Position.Y);
            }
        }

        protected override void UpdateHitbox()
        {
            Transform transform = new();
            transform.Position = new Vector2(Position.X - HITBOX_WIDTH / 2, Position.Y - HITBOX_HEIGHT / 2);
            if (Hitbox != null)
            {
                Hitbox.Transform(transform);
            }
        }

        private void UpdatePosition(SpriteLayer spriteLayer)
        {
            Vector2f crtPos = new Vector2f(Position.X, Position.Y);
            Vector2f posAfterXVelocity = new Vector2f(Position.X + (int)Velocity.X, Position.Y);
            Vector2i finalVelocity = new Vector2i((int)Velocity.X, (int)Velocity.Y);
            List<Tuple<Hitbox, int, int>> collidedTiles;

            Position = posAfterXVelocity;

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

        private void UpdateCurrentState()
        {
            PreviousFrameState = CurrentState;

            if (Health.CurrentHealth == 0)
            {
                CurrentState = State.Dead;
                return;
            }

            if (CurrentState == State.Hit && !invincibilityFrames.IsFinished()) { 
                return; }

            if (xPlayerDistance < ATTACK_TRIGGER_DISTANCE && yPlayerDistance < ATTACK_TRIGGER_DISTANCE)
            {
                CurrentState = State.Attacking;
            }
            else
            {
                CurrentState = State.Flying;
            }

        }

        private void UpdateAnimatedSprite()
        {
            var currentAnimation = Sprite;
            AnimatedSprite newAnimation = currentAnimation;

            if (CurrentState == State.Flying) { newAnimation = new AnimatedSprite(TextureManager.FlyingEyeAnimations["Fly"]); }
            else if (CurrentState == State.Attacking) { newAnimation = new AnimatedSprite(TextureManager.FlyingEyeAnimations["Attack"]); }
            else if (CurrentState == State.Hit) { newAnimation = new AnimatedSprite(TextureManager.FlyingEyeAnimations["Hit"]); }
            else if (CurrentState == State.Dead)
            {
                if (!isGrounded)
                {
                    newAnimation = new AnimatedSprite(TextureManager.FlyingEyeAnimations["DeathFall"]);
                }
                else
                {
                    newAnimation = new AnimatedSprite(TextureManager.FlyingEyeAnimations["DeathLand"]);
                }
/*                Sprite = newAnimation;
                Sprite.Play();*/
            }

            if (currentAnimation.NotEqual(newAnimation))
            {
                Sprite = newAnimation;
                Sprite.Play();
            }

            if (CurrentState == State.Attacking && Sprite.GetCurrentFrame() == 6)
            {
                if (!biteSoundPlayed)
                {
                    SoundManager.Play2DSound("Bite", position.X, position.Y);
                    biteSoundPlayed = true;
                }
            }
            else
            {
                biteSoundPlayed = false;
            }
            
            if (CurrentState == State.Flying && Sprite.GetCurrentFrame() == 6)
            {
                if (!flapSoundPlayed)
                {
                    SoundManager.Play2DSound("Flap", Position.X, Position.Y);
                    flapSoundPlayed = true;
                }
            }
            else
            {
                flapSoundPlayed = false;
            }

            UpdateSpritePosition();
        }

        public override void Reset()
        {
            if (CurrentState != State.Dead)
            {
                Position = Origin;
                Velocity = new();
                invincibilityFrames.Reset();
                CurrentState = State.Flying;
            }
        }
    }
}
