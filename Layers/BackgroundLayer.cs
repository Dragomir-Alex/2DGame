using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Layers
{
    public class BackgroundLayer : Layer, Drawable
    {
        public enum BackgroundMode
        {
            Default, Parallax3D
        }
        public Texture? Texture { get; set; }
        public string TextureFilename { private get; set; }
        public Sprite? Sprite { get; set; }

        public BackgroundMode Mode { get; set; }

        public BackgroundLayer() : base()
        {
            Mode = BackgroundMode.Default;
            Texture = null;
        }

        public void InitializeSprite()
        {
            if (Texture != null)
            {
                this.Texture.Smooth = true;

                if (Mode == BackgroundMode.Default)
                    this.Texture.Repeated = true;

                this.Sprite = new Sprite();
                this.Sprite.Texture = this.Texture;

                if (Mode == BackgroundMode.Default)
                {
                    int TextureRectWidth = 3 * (int)this.Texture.Size.X * (int)Math.Ceiling((double)Game.DEFAULT_WINDOW_WIDTH / this.Texture.Size.X);
                    int TextureRectHeight = 3 * (int)this.Texture.Size.Y * (int)Math.Ceiling((double)Game.DEFAULT_WINDOW_HEIGHT / this.Texture.Size.Y);

                    this.Sprite.TextureRect = new IntRect(0, 0, TextureRectWidth, TextureRectHeight);
                    this.Sprite.Position = new Vector2f(-TextureRectWidth / 3, -TextureRectHeight / 3);
                }
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (Sprite != null)
            {
                switch (Mode)
                {
                    case BackgroundMode.Default:
                        Vector2f pos = this.Sprite.Position;

                        if (pos.X >= 0 || pos.X <= -(this.Sprite.TextureRect.Width / 3) * 2)
                            Sprite.Position = new Vector2f(-(this.Sprite.TextureRect.Width / 3), pos.Y);
                        if (pos.Y >= 0 || pos.Y <= -(this.Sprite.TextureRect.Height / 3) * 2)
                            Sprite.Position = new Vector2f(pos.X, -(this.Sprite.TextureRect.Width / 3));

                        UtilityFunctions.Move(this.Sprite, AutoXSpeed, AutoYSpeed);

                        target.Draw(this.Sprite);

                        break;
                    case BackgroundMode.Parallax3D:
                        break;
                }
            }
        }
    }
}
