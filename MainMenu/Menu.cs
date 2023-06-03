using _2DGame.GameSettings;
using _2DGame.LayerData;
using _2DGame.Layers;
using _2DGame.MainMenu.Buttons;
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
using static SFML.Window.Mouse;
using Button = _2DGame.MainMenu.Buttons.Button;

namespace _2DGame.MainMenu
{
    public class Menu : Drawable
    {
        private Button leaderboardButtonRef;

        public const string MENU_MUSIC_FILENAME = "passage.ogg";
        public enum PageName { MainPage, Settings, HighScores, Credits }
        public Dictionary<PageName, Page> Pages { get; set; }
        public PageName CurrentPage { get; set; }

        public Menu()
        {
            Pages = new Dictionary<PageName, Page>();
            foreach (var pageName in (PageName[]) Enum.GetValues(typeof(PageName)))
            {
                Pages.Add(pageName, new Page());
            }
            CurrentPage = PageName.MainPage;
        }

        public void Initialize()
        {
            // First menu screen
            int[,] tiles =
            {
                { 1, 1, 0, 0 },
                { 1, 1, 0, 0 },
                { 0, 0, 1, 1 },
                { 0, 0, 1, 1 }
            };
            TileData tileData = new TileData(tiles);

            CreateMainPage(CreateMenuBackground("menu.png", tileData));
            CreateCreditsPage(CreateMenuBackground("menu.png", tileData));
            CreateSettingsPage(CreateMenuBackground("menu.png", tileData));
            CreateHighScoresPage(CreateMenuBackground("menu.png", tileData));
        }

        public static DetailLayer CreateMenuBackground(string tilesetFilename, TileData tileIDs)
        {
            DetailLayer background = new DetailLayer();
            background.AutoYSpeed = 1;
            background.RepeatX = true;
            background.RepeatY = true;
            background.Initialize(tilesetFilename, tileIDs);

            return background;
        }

        private void CreateMainPage(DetailLayer background)
        {
            Page mainPage = Pages[PageName.MainPage];
            mainPage.Background = background;

            ButtonAction startButtonAction = new ButtonAction(ButtonAction.Type.StartLevel, "", 0);
            Button startButton = new Button("START GAME", 30, TextureManager.GameFontBold, Color.White, Color.Red, startButtonAction);
            startButton.ButtonText.Position = new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - startButton.ButtonText.GetGlobalBounds().Width / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 2.2f));
            mainPage.AddButton(startButton);

            ButtonAction settingsButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "Settings", 0);
            Button settingsButton = new Button("SETTINGS", 30, TextureManager.GameFontBold, Color.White, Color.Red, settingsButtonAction);
            settingsButton.ButtonText.Position = new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - settingsButton.ButtonText.GetGlobalBounds().Width / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 2.2f + 60));
            mainPage.AddButton(settingsButton);

            ButtonAction highScoresButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "HighScores", 0);
            Button highScoresButton = new Button("HIGH SCORES", 30, TextureManager.GameFontBold, Color.White, Color.Red, highScoresButtonAction);
            highScoresButton.ButtonText.Position = new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - highScoresButton.ButtonText.GetGlobalBounds().Width / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 2.2f + 120));
            mainPage.AddButton(highScoresButton);

            ButtonAction creditsButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "Credits", 0);
            Button creditsButton = new Button("CREDITS", 30, TextureManager.GameFontBold, Color.White, Color.Red, creditsButtonAction);
            creditsButton.ButtonText.Position = new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - creditsButton.ButtonText.GetGlobalBounds().Width / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 2.2f + 180));
            mainPage.AddButton(creditsButton);

            ButtonAction quitButtonAction = new ButtonAction(ButtonAction.Type.QuitGame, "", 0);
            Button quitButton = new Button("CLOSE GAME", 30, TextureManager.GameFontBold, Color.White, Color.Red, quitButtonAction);
            quitButton.ButtonText.Position = new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - quitButton.ButtonText.GetGlobalBounds().Width / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 2.2f + 240));
            mainPage.AddButton(quitButton);
        }

        private void CreateCreditsPage(DetailLayer background)
        {
            Page creditsPage = Pages[PageName.Credits];
            creditsPage.Background = background;

            Text title = new Text("Credits", TextureManager.GameFontBold, 40);
            title.FillColor = Color.White;
            title.Position = new Vector2f(Game.DEFAULT_WINDOW_WIDTH / 2 - title.GetGlobalBounds().Width / 2, Game.DEFAULT_WINDOW_HEIGHT / 4);
            creditsPage.Title = title;

            ButtonAction creditsButtonAction = new ButtonAction(ButtonAction.Type.None, "", 0); 
            Button creditsButton = new Button("Dodo-Codes - TransformableHitbox2D and TileData libraries, coding help\nBlade - Tileset\n\nMore TBA!", 20, TextureManager.GameFontBold, Color.White, Color.White, creditsButtonAction);
            creditsButton.ButtonText.Position = new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - creditsButton.ButtonText.GetGlobalBounds().Width / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 2.5f));
            creditsPage.AddButton(creditsButton);

            ButtonAction backButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "MainPage", 0);
            Button backButton = new Button("<", 50, TextureManager.GameFontBold, Color.White, Color.Red, backButtonAction);
            backButton.ButtonText.Position = new Vector2f(30, (int)(Game.DEFAULT_WINDOW_HEIGHT - backButton.ButtonText.GetGlobalBounds().Height - 30));
            creditsPage.AddButton(backButton);
        }

        private void CreateSettingsPage(DetailLayer background)
        {
            Page settingsPage = Pages[PageName.Settings];
            settingsPage.Background = background;

            Text title = new Text("Settings", TextureManager.GameFontBold, 40);
            title.FillColor = Color.White;
            title.Position = new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - title.GetGlobalBounds().Width / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 6));
            settingsPage.Title = title;

            ButtonAction leftArrowMusicAction = new ButtonAction(ButtonAction.Type.ChangeSetting, "MusicVolume", -10);
            ButtonAction rightArrowMusicAction = new ButtonAction(ButtonAction.Type.ChangeSetting, "MusicVolume", 10);
            LeftRightButtons musicVolumeButtons = new LeftRightButtons("Music Volume", "MusicVolume", 30, TextureManager.GameFont, TextureManager.GameFontBold, Color.White, Color.Red, leftArrowMusicAction, rightArrowMusicAction);

            var musicVolumeButtonsSize = musicVolumeButtons.GetSize();
            musicVolumeButtons.SetPosition(new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - musicVolumeButtonsSize.X / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 2.5)));
            musicVolumeButtons.SettingValue.ButtonText.DisplayedString = Settings.MusicVolume.ToString();

            settingsPage.AddButton(musicVolumeButtons.Label);
            settingsPage.AddButton(musicVolumeButtons.LeftArrow);
            settingsPage.AddButton(musicVolumeButtons.RightArrow);
            settingsPage.AddButton(musicVolumeButtons.SettingValue);

            ButtonAction leftArrowSoundAction = new ButtonAction(ButtonAction.Type.ChangeSetting, "SoundVolume", -10);
            ButtonAction rightArrowSoundAction = new ButtonAction(ButtonAction.Type.ChangeSetting, "SoundVolume", 10);
            LeftRightButtons soundVolumeButtons = new LeftRightButtons("Sound Volume", "SoundVolume", 30, TextureManager.GameFont, TextureManager.GameFontBold, Color.White, Color.Red, leftArrowSoundAction, rightArrowSoundAction);

            var soundVolumeButtonsSize = soundVolumeButtons.GetSize();
            soundVolumeButtons.SetPosition(new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - soundVolumeButtonsSize.X / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 2.5) + 150));
            soundVolumeButtons.SettingValue.ButtonText.DisplayedString = Settings.SoundVolume.ToString();

            settingsPage.AddButton(soundVolumeButtons.Label);
            settingsPage.AddButton(soundVolumeButtons.LeftArrow);
            settingsPage.AddButton(soundVolumeButtons.RightArrow);
            settingsPage.AddButton(soundVolumeButtons.SettingValue);

            ButtonAction backButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "MainPage", 0);
            Button backButton = new Button("<", 50, TextureManager.GameFontBold, Color.White, Color.Red, backButtonAction);
            backButton.ButtonText.Position = new Vector2f(30, (int)(Game.DEFAULT_WINDOW_HEIGHT - backButton.ButtonText.GetGlobalBounds().Height - 30));
            settingsPage.AddButton(backButton);
        }

        private void CreateHighScoresPage(DetailLayer background)
        {
            Page highScoresPage = Pages[PageName.HighScores];
            highScoresPage.Background = background;

            Text title = new Text("High Scores", TextureManager.GameFontBold, 40);
            title.FillColor = Color.White;
            title.Position = new Vector2f(Game.DEFAULT_WINDOW_WIDTH / 2 - title.GetGlobalBounds().Width / 2, Game.DEFAULT_WINDOW_HEIGHT / 4);
            highScoresPage.Title = title;

            ButtonAction leaderboardButtonAction = new ButtonAction(ButtonAction.Type.None, "", 0);
            Button leaderboardButton = new Button("", 30, TextureManager.GameFontBold, Color.White, Color.White, leaderboardButtonAction);
            leaderboardButton.ButtonText.Position = new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - leaderboardButton.ButtonText.GetGlobalBounds().Width / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 2.5f));
            highScoresPage.AddButton(leaderboardButton);
            leaderboardButtonRef = leaderboardButton;

            ButtonAction backButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "MainPage", 0);
            Button backButton = new Button("<", 50, TextureManager.GameFontBold, Color.White, Color.Red, backButtonAction);
            backButton.ButtonText.Position = new Vector2f(30, (int)(Game.DEFAULT_WINDOW_HEIGHT - backButton.ButtonText.GetGlobalBounds().Height - 30));
            highScoresPage.AddButton(backButton);
        }

        public void UpdateLeaderboard(Leaderboard leaderboard)
        {
            if (leaderboard == null || leaderboardButtonRef == null) return;

            StringBuilder leaderboardSB = new();
            for (int i = 0; i < leaderboard.HighScores.Count; ++i)
            {
                leaderboardSB.AppendLine((i + 1) + ". " + leaderboard.HighScores[i].Name + "   -   " + leaderboard.HighScores[i].Score);
            }

            leaderboardButtonRef.ButtonText.DisplayedString = leaderboardSB.ToString();
            leaderboardButtonRef.ButtonText.Position = new Vector2f((int)(Game.DEFAULT_WINDOW_WIDTH / 2 - leaderboardButtonRef.ButtonText.GetGlobalBounds().Width / 2), (int)(Game.DEFAULT_WINDOW_HEIGHT / 2.5f));
        }

        public void ProcessButtonAction(ButtonAction buttonAction, GameLoop gameLoop)
        {
            switch (buttonAction.ActionType)
            {
                case ButtonAction.Type.StartLevel:
                    gameLoop.CurrentState = GameLoop.GameState.LoadingLevel;
                    break;

                case ButtonAction.Type.ChangeSetting:
                    if (buttonAction.Target == "SoundVolume")
                    {
                        Settings.SoundVolume += (int)buttonAction.Parameter;
                        foreach (var button in Pages[CurrentPage].Buttons)
                        {
                            if (button.OnMouseClick().ActionType == ButtonAction.Type.DisplayVariable &&
                                button.OnMouseClick().Target == "SoundVolume")
                            {
                                button.ButtonText.DisplayedString = Settings.SoundVolume.ToString();
                                break;
                            }
                        }
                        Settings.Save();
                    }
                    else if (buttonAction.Target == "MusicVolume")
                    {
                        Settings.MusicVolume += (int)buttonAction.Parameter;
                        foreach (var button in Pages[CurrentPage].Buttons)
                        {
                            if (button.OnMouseClick().ActionType == ButtonAction.Type.DisplayVariable &&
                                button.OnMouseClick().Target == "MusicVolume")
                            {
                                button.ButtonText.DisplayedString = Settings.MusicVolume.ToString();
                                break;
                            }
                        }
                        Settings.Save();
                    }
                    break;

                case ButtonAction.Type.ChangePage:
                    if (Enum.TryParse(buttonAction.Target, out PageName pageName))
                    {
                        CurrentPage = pageName;
                    }
                    break;

                case ButtonAction.Type.QuitGame:
                    gameLoop.Window.Close();
                    break;

                default:
                    break;
            }
        }

        public void Update(View view)
        {
            Pages[CurrentPage].Update(view);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Pages[CurrentPage]);
        }
    }
}
