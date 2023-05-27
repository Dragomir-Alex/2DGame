﻿using _2DGame.Utility;
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
            text = new Text("Score: " + Score.GetString(), TextureManager.GameFont);
            text.CharacterSize = 30;
            text.Position = new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH - text.GetGlobalBounds().Width - 20), (int)(Game.DEFAULT_WINDOW_HEIGHT - text.GetGlobalBounds().Height - 20));
            text.FillColor = Color.White;

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