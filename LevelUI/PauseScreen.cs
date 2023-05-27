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
            rectangleBackground = new RectangleShape(new Vector2f(Game.DEFAULT_WINDOW_WIDTH, Game.DEFAULT_WINDOW_HEIGHT));
            rectangleBackground.FillColor = new Color(0, 0, 0, 100);

            text = new Text("The game is paused\n\nPress [P] to resume\nPress [Esc] to exit to menu", TextureManager.GameFont);
            text.CharacterSize = 60;
            text.Position = new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - text.GetGlobalBounds().Width / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 2 - text.GetGlobalBounds().Height / 2));
            text.FillColor = Color.White;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(rectangleBackground);
            target.Draw(text);
        }
    }
}
