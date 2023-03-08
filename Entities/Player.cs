using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Entities
{
    public class Player
    {
        public Sprite CharacterSprite { get; set; }
        public Vector2f Position; // Not sure what to do with these at the moment, make them vars?
        public Vector2f Velocity;
        public Vector2f MaxVelocity { get; set; }
        public View Camera { get; set; }
        private const float X_VELOCITY_GAIN = 0.3f;
        private const float Y_VELOCITY_GAIN = 0.3f;
        private const float X_VELOCITY_REDUCTION = 0.15f;
        private const float Y_VELOCITY_REDUCTION = 0.15f;

        public Player()
        {
            CharacterSprite = new Sprite();
            Position = new Vector2f(100f, 200f);
            Velocity = new Vector2f(0f, 0f);
            MaxVelocity = new Vector2f(5f, 5f);
            Camera = new View();
        }

        public void SetPlayerCamera(Vector2f center, Vector2f size)
        {
            Camera.Center = center;
            Camera.Size = size;
        }

        public void UpdatePlayerCamera(uint screenWidth, uint screenHeight)
        {
            if (Position.X + CharacterSprite.Texture.Size.X > screenWidth / 2)
                Camera.Center = new Vector2f(Position.X + CharacterSprite.Texture.Size.X, Position.Y + CharacterSprite.Texture.Size.Y);
            else
                Camera.Center = new Vector2f(screenWidth / 2, Position.Y + CharacterSprite.Texture.Size.Y);
        }

        public void UpdatePosition()
        {
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
        }

        public void GainPositiveXVelocity() { Velocity.X += X_VELOCITY_GAIN; }
        public void GainNegativeXVelocity() { Velocity.X -= X_VELOCITY_GAIN; }
        public void GainPositiveYVelocity() { Velocity.Y += Y_VELOCITY_GAIN; }
        public void GainNegativeYVelocity() { Velocity.Y -= Y_VELOCITY_GAIN; }

        public void UpdateVelocity()
        {
            if (Velocity.X < 0f)
            {
                if (Velocity.X + X_VELOCITY_REDUCTION > 0f)
                    Velocity.X = 0f;
                else Velocity.X += X_VELOCITY_REDUCTION;
            }

            if (Velocity.X > 0f)
            {
                if (Velocity.X - X_VELOCITY_REDUCTION < 0f)
                    Velocity.X = 0f;
                else
                    Velocity.X -= X_VELOCITY_REDUCTION;
            }

            if (Velocity.X > MaxVelocity.X)
                Velocity.X = MaxVelocity.X;

            if (Velocity.X < -MaxVelocity.X)
                Velocity.X = -MaxVelocity.X;

            if (Velocity.Y < 0f)
            {
                if (Velocity.Y + Y_VELOCITY_REDUCTION > 0f)
                    Velocity.Y = 0f;
                else
                    Velocity.Y += Y_VELOCITY_REDUCTION;
            }

            if (Velocity.Y > 0f)
            {
                if (Velocity.Y - Y_VELOCITY_REDUCTION < 0f)
                    Velocity.Y = 0f;
                else
                    Velocity.Y -= Y_VELOCITY_REDUCTION;
            }

            if (Velocity.Y > MaxVelocity.Y)
                Velocity.Y = MaxVelocity.Y;

            if (Velocity.Y < -MaxVelocity.Y)
                Velocity.Y = -MaxVelocity.Y;
        }
    }
}
