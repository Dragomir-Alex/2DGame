using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.LevelUI
{
    public class Scoreboard : Drawable
    {
        private Text text;
        private RectangleShape rectangleBackground;

        public void Update()
        {
            text = new Text("", TextureManager.GameFont);
            text.CharacterSize = 30;
            text.FillColor = Color.White;
            LanguageManager.AddLocalizedText(text, "ScoreString");
            text.DisplayedString += Score.GetString();
            text.Origin = new();
            text.Position = new Vector2f((int)(Game.WINDOW_WIDTH - text.GetGlobalBounds().Width - 20), (int)(Game.WINDOW_HEIGHT - text.GetGlobalBounds().Height - 20));

            rectangleBackground = new RectangleShape(new Vector2f(text.GetGlobalBounds().Width + 10, text.GetGlobalBounds().Height + 10));
            rectangleBackground.Position = new Vector2f(text.GetGlobalBounds().Left - 5, text.GetGlobalBounds().Top - 5);
            rectangleBackground.FillColor = new Color(0, 0, 0, 100);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (rectangleBackground != null && text != null)
            {
                target.Draw(rectangleBackground);
                target.Draw(text);
            }
        }
    }
}
