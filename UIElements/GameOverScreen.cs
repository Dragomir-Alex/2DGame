using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Text = SFML.Graphics.Text;

namespace _2DGame.LevelUI
{
    public class GameOverScreen : Drawable
    {
        private bool subscribed, initialized;
        private readonly RectangleShape rectangleBackground;
        private readonly Text topHintText;
        private readonly Text bottomHintText;
        private Text scoreText;
        private Text playerNameText;
        private Text messageTextWon;
        private Text messageTextLost;
        private bool won;
        private StringBuilder playerNameSB;
        public const int MAX_STRING_SIZE = 24;

        public GameOverScreen()
        {
            subscribed = false;
            initialized = false;
            playerNameSB = new StringBuilder();
            playerNameSB.Capacity = MAX_STRING_SIZE;

            rectangleBackground = new RectangleShape(new Vector2f(Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT));
            rectangleBackground.FillColor = new Color(0, 0, 0, 100);

            topHintText = new Text("", TextureManager.GameFont, 60);
            topHintText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2 - topHintText.GetGlobalBounds().Width / 2), 250);
            topHintText.FillColor = Color.White;
            LanguageManager.AddLocalizedText(topHintText, "GameOverTypeNameString");

            playerNameText = new Text("", TextureManager.GameFontBold, 50);
            playerNameText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), 350);
            playerNameText.FillColor = new Color(137, 207, 240);

            bottomHintText = new Text("", TextureManager.GameFont, 50);
            bottomHintText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2 - bottomHintText.GetGlobalBounds().Width / 2), 600);
            bottomHintText.FillColor = Color.White;
            LanguageManager.AddLocalizedText(bottomHintText, "GameOverPressKeyString");
        }

        public void Initialize(GameLoop gameLoop, bool won)
        {
            if (!initialized && gameLoop.CurrentState == GameLoop.GameState.GameOver)
            {
                Reset(gameLoop);
                initialized = true;

                if (!subscribed)
                {
                    TextEnteredSubscribe(gameLoop);
                    subscribed = true;
                }

                this.won = won;

                if (won)
                {
                    SoundManager.PlaySound("Victory");
                }
                else
                {
                    SoundManager.PlaySound("Loss");
                }

                messageTextWon = new Text("", TextureManager.GameFontBold, 60);
                messageTextWon.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), 50);
                messageTextWon.FillColor = Color.Green;
                LanguageManager.AddLocalizedText(messageTextWon, "GameOverWonString");
                messageTextWon.Origin = new Vector2f(messageTextWon.Origin.X, 0);

                messageTextLost = new Text("", TextureManager.GameFontBold, 60);
                messageTextLost.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), 50);
                messageTextLost.FillColor = Color.Red;
                LanguageManager.AddLocalizedText(messageTextLost, "GameOverLostString");
                messageTextLost.Origin = new Vector2f(messageTextLost.Origin.X, 0);

                scoreText = new Text("", TextureManager.GameFontBold, 60);
                scoreText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2 - scoreText.GetGlobalBounds().Width / 2), 475);
                scoreText.FillColor = Color.White;
                LanguageManager.AddLocalizedText(scoreText, "GameOverFinalScoreString");
                scoreText.DisplayedString += Score.GetString();
                scoreText.Origin = new Vector2f((int)(scoreText.GetGlobalBounds().Width / 2), (int)(scoreText.GetGlobalBounds().Height / 2));
            }
        }

        public void Update()
        {
            playerNameText.DisplayedString = playerNameSB.ToString();
            playerNameText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2 - playerNameText.GetGlobalBounds().Width / 2), 350);
        }

        public void TextEnteredSubscribe(GameLoop gameLoop) { gameLoop.Window.TextEntered += KeyboardKeyPressed; }

        public void Reset(GameLoop gameLoop)
        {
            if (subscribed)
            {
                gameLoop.Window.TextEntered -= KeyboardKeyPressed;
                subscribed = false;
            }

            initialized = false;
            playerNameSB.Clear();
            playerNameText.DisplayedString = "";
        }

        public string GetPlayerName() { return playerNameSB.ToString().Trim(); }

        private void KeyboardKeyPressed(object? sender, SFML.Window.TextEventArgs e)
        {
            int ascii = e.Unicode[0];
            
            if (ascii == 8 && playerNameSB.Length != 0)
            {
                playerNameSB.Remove(playerNameSB.Length - 1, 1);
            }

            if (((ascii >= 48 && ascii <= 57)
                || (ascii >= 65 && ascii <= 90)
                || (ascii >= 97 && ascii <= 122)
                || (ascii == 32 && playerNameSB.Length > 0)) && playerNameSB.Length < MAX_STRING_SIZE)
            {
                playerNameSB.Append(e.Unicode[0]);
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(rectangleBackground);
            target.Draw(playerNameText);
            target.Draw(topHintText);
            target.Draw(bottomHintText);

            if (initialized)
            {
                if (won) target.Draw(messageTextWon); else target.Draw(messageTextLost);
                target.Draw(scoreText);
            }
        }
    }
}
