using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.LevelUI
{
    public class PauseScreen : Drawable
    {
        private readonly RectangleShape rectangleBackground;
        private readonly Text text;

        public PauseScreen()
        {
            rectangleBackground = new RectangleShape(new Vector2f(Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT));
            rectangleBackground.FillColor = new Color(0, 0, 0, 100);

            text = new Text("", TextureManager.GameFont);
            text.CharacterSize = 60;
            text.Origin = new Vector2f((int)(text.GetGlobalBounds().Width / 2), (int)(text.GetGlobalBounds().Height / 2));
            text.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 2));
            text.FillColor = Color.White;
            LanguageManager.AddLocalizedText(text, "PausedMenuString");
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(rectangleBackground);
            target.Draw(text);
        }
    }
}
