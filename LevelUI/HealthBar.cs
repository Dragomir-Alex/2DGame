using _2DGame.Entities;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.LevelUI
{
    public class HealthBar : Drawable
    {
        private readonly List<Sprite> heartSprites = new List<Sprite>();
        private Sprite heartSpriteCopy;

        public void Initialize(Texture texture, Player player)
        {
            if (texture != null)
            {
                for (int i = 0; i < player.Health.MaxHealth; ++i)
                {
                    Sprite heartSprite = new Sprite(texture);
                    heartSprite.Position = new Vector2f(15 + texture.Size.X * i + 10 * i, Game.DEFAULT_WINDOW_HEIGHT - texture.Size.Y - 10);
                    heartSprites.Add(heartSprite);
                }

                heartSpriteCopy = new Sprite(texture);
            }
        }

        public void Update(Player player)
        {
            for (int i = 1; i <= player.Health.MaxHealth; ++i)
            {
                if (i <= player.Health.CurrentHealth)
                {
                    heartSprites[i - 1].Color = new Color(heartSpriteCopy.Color.R, heartSpriteCopy.Color.G, heartSpriteCopy.Color.B, 255);
                }
                else
                {
                    heartSprites[i - 1].Color = new Color(0, 0, 0, 150);
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var heartSprite in heartSprites)
            {
                target.Draw(heartSprite, states);
            }
        }
    }
}
