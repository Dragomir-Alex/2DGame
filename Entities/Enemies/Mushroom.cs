using _2DGame.Animation;
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
using _2DGame.LayerData;

namespace _2DGame.Entities.Enemies
{
    public class Mushroom : GameEntity, IEnemy, IAnimated
    {
        private bool slapSoundPlayed, lowStepSoundPlayed;
        private Vector2f position;
        private readonly FrameTimer invincibilityFrames;
        private float playerDistance;
        private Hitbox normalHitbox, attackHitbox;

        public enum State { Idle, Running, Attacking, Hit, Dead }
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
        public const float TRIGGER_DISTANCE = 6f * Tilemap.TILE_SIZE;
        public const float ATTACK_TRIGGER_DISTANCE = 1.5f * Tilemap.TILE_SIZE;
        public const uint INVINCIBILITY_FRAME_COUNT = 25;
        public const int Y_SPRITE_OFFSET = -8;

        public const float MAX_VELOCITY = 3f;
        public const float FALL_MAX_VELOCITY = 7f;
        private const float VELOCITY_GAIN = 0.25f;
        private const float VELOCITY_REDUCTION = 0.2f;
        private const float GRAVITY = 1f;

        public const int HITBOX_ATTACK_RANGE = 20;
        public const int HITBOX_WIDTH = 30;
        public const int HITBOX_HEIGHT = 30;

        public Mushroom() : base(5)
        {
            slapSoundPlayed = false;
            lowStepSoundPlayed = false;
            invincibilityFrames = new FrameTimer(INVINCIBILITY_FRAME_COUNT);
            CurrentState = State.Idle;
            PreviousFrameState = State.Idle;
            AttackDamage = 1;
            ScoreOnDeath = 50;
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
                            SoundManager.Play2DSound("Growl", position.X, position.Y);
                        }
                        else
                        {
                            CurrentState = State.Dead;
                            SoundManager.Play2DSound("Low Growl", position.X, position.Y);
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
            playerDistance = (float)Math.Sqrt((float)Math.Pow(player.Position.X - Position.X, 2) + (float)Math.Pow(player.Position.Y - Position.Y, 2));

            if (CurrentState == State.Hit || CurrentState == State.Dead)
                return;

            if (playerDistance <= TRIGGER_DISTANCE && playerDistance >= ATTACK_TRIGGER_DISTANCE)
            {
                if (player.Position.X >= Position.X)
                {
                    GainPositiveXVelocity(VELOCITY_GAIN);
                    CurrentDirection = IAnimated.Direction.Right;
                }
                else
                {
                    GainNegativeXVelocity(VELOCITY_GAIN);
                    CurrentDirection = IAnimated.Direction.Left;
                }
            }
            else if (playerDistance <= ATTACK_TRIGGER_DISTANCE)
            {
                if (CurrentState != State.Attacking)
                {
                    Velocity = new Vector2f(Velocity.X / 2, Velocity.Y);
                }
            }
        }

        public void GainPositiveXVelocity(float xVelocityGain) { Velocity = new Vector2f(Velocity.X + xVelocityGain, Velocity.Y); }
        public void GainNegativeXVelocity(float xVelocityGain) { Velocity = new Vector2f(Velocity.X - xVelocityGain, Velocity.Y); }

        public override void Update(Level level, GameLoop gameLoop)
        {
            ApplyGravity();
            Velocity = UtilityFunctions.UpdateVelocity(Velocity, VELOCITY_REDUCTION, VELOCITY_REDUCTION, MAX_VELOCITY, MAX_VELOCITY);
            UpdatePosition((SpriteLayer)level.Layers[LayerList.PRIMARY_LAYER]);
            UpdateCurrentState();
            UpdateAnimatedSprite();
            invincibilityFrames.Update();
        }

        private void ApplyGravity()
        {
            if (Velocity.Y + GRAVITY >= FALL_MAX_VELOCITY)
                Velocity = new Vector2f(Velocity.X, FALL_MAX_VELOCITY);
            else Velocity = new Vector2f(Velocity.X, Velocity.Y + GRAVITY);
        }

        private void UpdateAllPositionProperties()
        {
            UpdateSpritePosition();
            UpdateHitbox();
            UpdateTileCoordinates();
        }

        private void InitializeHitbox()
        {
            Vector2[] normalHitboxArr = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(HITBOX_WIDTH, 0),
                new Vector2(HITBOX_WIDTH, HITBOX_HEIGHT),
                new Vector2(0, HITBOX_HEIGHT),
                new Vector2(0, 0)
            };

            Vector2[] attackHitboxArr = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(HITBOX_WIDTH + HITBOX_ATTACK_RANGE, 0),
                new Vector2(HITBOX_WIDTH + HITBOX_ATTACK_RANGE, HITBOX_HEIGHT),
                new Vector2(0, HITBOX_HEIGHT),
                new Vector2(0, 0)
            };

            normalHitbox = new Hitbox(normalHitboxArr);
            attackHitbox = new Hitbox(attackHitboxArr);

            Hitbox = normalHitbox;
            UpdateHitbox();
        }

        private void UpdateSpritePosition()
        {
            if (Sprite != null)
            {
                Sprite.Position = new Vector2f(Position.X, Position.Y + Y_SPRITE_OFFSET);
            }
        }

        protected override void UpdateHitbox()
        {
            Transform transform = new();

            if (Hitbox != null)
            {

                transform.Position = new Vector2(Position.X - HITBOX_WIDTH / 2 + HITBOX_ATTACK_RANGE * (CurrentDirection == IAnimated.Direction.Right ? 1 : -1), Position.Y - HITBOX_HEIGHT / 2);
                attackHitbox.Transform(transform);

                transform.Position = new Vector2(Position.X - HITBOX_WIDTH / 2, Position.Y - HITBOX_HEIGHT / 2);
                normalHitbox.Transform(transform);

                if (CurrentState == State.Attacking && Sprite.GetCurrentFrame() == 7)
                {
                    Hitbox = attackHitbox;
                }
                else
                {
                    Hitbox = normalHitbox;
                }
            }
        }

        private void UpdatePosition(SpriteLayer spriteLayer)
        {
            if (Hitbox == attackHitbox) { Hitbox = normalHitbox; }

            Vector2f crtPos = new Vector2f(Position.X, Position.Y);
            Vector2f posAfterXVelocity = new Vector2f(Position.X + (int)Velocity.X, Position.Y);
            Vector2i finalVelocity = new Vector2i((int)Velocity.X, (int)Velocity.Y);
            List<Tuple<Hitbox, int, int>> collidedTiles;

            Position = posAfterXVelocity;
            if (Hitbox == attackHitbox) { Hitbox = normalHitbox; }

            // X Axis test
            collidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
            if (collidedTiles.Count != 0)
            {
                for (int i = Math.Abs((int)Velocity.X); i >= 0; i--)
                {
                    Vector2f interpolatedPosition = new Vector2f(crtPos.X + i * Math.Sign(Velocity.X), crtPos.Y);
                    Position = interpolatedPosition;
                    if (Hitbox == attackHitbox) { Hitbox = normalHitbox; }
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
            if (Hitbox == attackHitbox) { Hitbox = normalHitbox; }

            // Y Axis test
            collidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
            if (collidedTiles.Count != 0)
            {
                for (int i = Math.Abs((int)Velocity.Y); i >= 0; i--)
                {
                    Vector2f interpolatedPosition = new Vector2f(crtPos.X, crtPos.Y + i * Math.Sign(Velocity.Y));
                    Position = interpolatedPosition;
                    if (Hitbox == attackHitbox) { Hitbox = normalHitbox; }
                    List<Tuple<Hitbox, int, int>> newCollidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
                    if (newCollidedTiles.Count == 0)
                    {
                        finalVelocity.Y = i * Math.Sign(Velocity.Y);
                        Velocity = new Vector2f(Velocity.X, 0f);
                        break;
                    }
                }
            }

            Position = new Vector2f(crtPos.X + finalVelocity.X, crtPos.Y + finalVelocity.Y);
            if (Hitbox == attackHitbox) { Hitbox = normalHitbox; }

            // Corner test
            collidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
            if (collidedTiles.Count != 0)
            {
                for (int i = Math.Min(Math.Abs((int)Velocity.X), Math.Abs((int)Velocity.Y)); i >= 0; i--)
                {
                    Vector2f interpolatedPosition = new Vector2f(crtPos.X + i * Math.Sign(Velocity.X), crtPos.Y + i * Math.Sign(Velocity.Y));
                    Position = interpolatedPosition;
                    if (Hitbox == attackHitbox) { Hitbox = normalHitbox; }
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

            if (CurrentState == State.Hit && !invincibilityFrames.IsFinished())
            {
                return;
            }

            if (playerDistance <= TRIGGER_DISTANCE && playerDistance >= ATTACK_TRIGGER_DISTANCE)
            {
                CurrentState = State.Running;
            }
            else if (playerDistance < ATTACK_TRIGGER_DISTANCE)
            {
                CurrentState = State.Attacking;
            }
            else
            {
                CurrentState = State.Idle;
            }

        }

        private void UpdateAnimatedSprite()
        {
            var currentAnimation = Sprite;
            AnimatedSprite newAnimation = currentAnimation;

            if (CurrentState == State.Idle) { newAnimation = new AnimatedSprite(TextureManager.MushroomAnimations["Idle"]); }
            else if (CurrentState == State.Running) { newAnimation = new AnimatedSprite(TextureManager.MushroomAnimations["Run"]); }
            else if (CurrentState == State.Attacking) { newAnimation = new AnimatedSprite(TextureManager.MushroomAnimations["Attack"]); }
            else if (CurrentState == State.Hit) { newAnimation = new AnimatedSprite(TextureManager.MushroomAnimations["Hit"]); }
            else if (CurrentState == State.Dead) { newAnimation = new AnimatedSprite(TextureManager.MushroomAnimations["Death"]); }

            if (currentAnimation.NotEqual(newAnimation))
            {
                Sprite = newAnimation;
                Sprite.Play();
            }

            if (CurrentState == State.Attacking && Sprite.GetCurrentFrame() == 6)
            {
                if (!slapSoundPlayed)
                {
                    SoundManager.Play2DSound("Slap", position.X, position.Y);
                    slapSoundPlayed = true;
                }
            }
            else
            {
                slapSoundPlayed = false;
            }

            if (CurrentState == State.Running && (Sprite.GetCurrentFrame() == 1 || Sprite.GetCurrentFrame() == 5))
            {
                if (!lowStepSoundPlayed)
                {
                    SoundManager.Play2DSound("Low Step", position.X, position.Y);
                    lowStepSoundPlayed = true;
                }
            }
            else
            {
                lowStepSoundPlayed = false;
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
                CurrentState = State.Idle;
            }
        }
    }
}
