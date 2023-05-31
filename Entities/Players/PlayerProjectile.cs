using _2DGame.Animation;
using _2DGame.LayerData;
using _2DGame.Layers;
using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TransformableHitbox2D;

namespace _2DGame.Entities.Players
{
    public class PlayerProjectile : GameEntity, IAnimated
    {
        private Vector2f position = new();
        public override Vector2f Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateAllPositionProperties();
            }
        }
        public enum State { Starting, Active, Disappearing }
        public State CurrentState { get; set; }
        public AnimatedSprite Sprite { get; set; }
        public IAnimated.Direction CurrentDirection { get; set; }

        public const int HITBOX_WIDTH = 10;
        public const int HITBOX_HEIGHT = 6;
        public const int SPEED = 7;
        public const int DAMAGE = 1;
        public const int PROJECTILE_X_OFFSET = 40;
        public const int PROJECTILE_Y_OFFSET = 3;


        public PlayerProjectile() : base(2)
        {
            CurrentState = State.Starting;
            CurrentDirection = IAnimated.Direction.Right;
            Sprite = new AnimatedSprite(TextureManager.PlayerAnimations["ProjectileStart"]);
        }

        public PlayerProjectile(Vector2f startPosition, IAnimated.Direction direction) : this()
        {
            CurrentDirection = direction;
            InitializeFloatPosition(startPosition);
        }

        public override void Initialize(Vector2i startPosition)
        {
            Position = new Vector2f(startPosition.X * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2, startPosition.Y * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE / 2);
            InitializeHitbox();
        }

        public void InitializeFloatPosition(Vector2f startPosition)
        {
            Position = new Vector2f(startPosition.X, startPosition.Y);
            InitializeHitbox();
        }

        public override void OnEntityCollision(GameEntity entity) { }

        public override void Update(Level level, GameLoop gameLoop)
        {
            UpdatePosition((SpriteLayer)level.Layers[LayerList.PRIMARY_LAYER]);
            GameEntityCollision(level.GameEntityManager);
            UpdateProperties();
        }

        private void UpdatePosition(SpriteLayer spriteLayer)
        {
            if (CurrentState == State.Disappearing || CurrentState == State.Starting) { return; }

            int crtSpeed = (CurrentDirection == IAnimated.Direction.Right ? 1 : -1) * SPEED;
            int finalSpeed = crtSpeed;
            Vector2f crtPos = new Vector2f(Position.X, Position.Y);
            Vector2f posAfterXSpeed = new Vector2f(Position.X + crtSpeed, Position.Y);
            List<Tuple<Hitbox, int, int>> collidedTiles;

            Position = posAfterXSpeed;

            collidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
            if (collidedTiles.Count != 0)
            {
                CurrentState = State.Disappearing;

                for (int i = Math.Abs(crtSpeed); i >= 0; i--)
                {
                    Vector2f interpolatedPosition = new Vector2f(crtPos.X + i * Math.Sign(crtSpeed), crtPos.Y);
                    Position = interpolatedPosition;
                    List<Tuple<Hitbox, int, int>> newCollidedTiles = UtilityFunctions.GameEntityLevelCollision(this, spriteLayer);
                    if (newCollidedTiles.Count == 0)
                    {
                        finalSpeed = i * Math.Sign(crtSpeed);
                        break;
                    }
                }
            }

            Position = new Vector2f(crtPos.X + finalSpeed, crtPos.Y);
        }

        public void GameEntityCollision(GameEntityManager gameEntityManager)
        {
            if (CurrentState == State.Disappearing || CurrentState == State.Starting) { return; }

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

        public void UpdateProperties()
        {
            var currentAnimation = Sprite;

            if (CurrentState == State.Disappearing && Sprite.IsFinished())
            {
                SoundManager.PlaySound("Poof");
                IsActive = false;
            }
            else if (CurrentState == State.Starting && Sprite.IsFinished())
            {
                CurrentState = State.Active;
                Sprite = new AnimatedSprite(TextureManager.PlayerAnimations["ProjectileMiddle"]);
            }
            else if (CurrentState == State.Disappearing && Sprite.GetLastFrame() != TextureManager.PlayerAnimations["ProjectileEnd"].GetLastFrame())
            {
                Sprite = new AnimatedSprite(TextureManager.PlayerAnimations["ProjectileEnd"]);
            }

            if (currentAnimation != Sprite)
            {
                Sprite.Play();
            }

            // Debug.WriteLine(CurrentState.ToString() + " " + Sprite.GetCurrentFrame() + " " + Sprite.GetLastFrame() + " " + Sprite.IsFinished());

            UpdateSpritePosition();
        }

        private void UpdateAllPositionProperties()
        {
            UpdateSpritePosition();
            UpdateHitbox();
            UpdateTileCoordinates();
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
            if (Hitbox != null && Sprite != null)
            {
                TransformableHitbox2D.Transform transform = new();
                transform.Position = new Vector2(Position.X - (CurrentDirection == IAnimated.Direction.Left ? HITBOX_WIDTH : 0), Position.Y - HITBOX_HEIGHT / 2);
                Hitbox.Transform(transform);
            }
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
    }
}
