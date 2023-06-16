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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Mouse;
using Button = _2DGame.MainMenu.Buttons.Button;

namespace _2DGame.MainMenu
{
    public class Menu : Drawable
    {
        private DetailLayer background;
        private Button localLeaderboardButtonRef;
        private Button globalLeaderboardButtonRef;
        private bool isUpdatingGlobalLeaderboards = false;

        public const string MENU_MUSIC_FILENAME = "menu_music.ogg";
        public enum PageName { MainPage, Settings, LocalHighScores, GlobalHighScores, Credits }
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
            background = CreateMenuBackground(tileData);

            CreateMainPage();
            CreateCreditsPage();
            CreateSettingsPage();
            CreateLocalHighScoresPage();
            CreateGlobalHighScoresPage();
        }

        public static DetailLayer CreateMenuBackground(TileData tileIDs)
        {
            DetailLayer background = new();
            background.AutoYSpeed = 1;
            background.RepeatX = true;
            background.RepeatY = true;
            background.Initialize(TextureManager.MenuBackgroundTexture, tileIDs);

            return background;
        }

        private void CreateMainPage()
        {
            Page mainPage = Pages[PageName.MainPage];
            mainPage.Background = background;

            ButtonAction startButtonAction = new ButtonAction(ButtonAction.Type.StartLevel, "", 0);
            Button startButton = new Button(LanguageStrings.StartGameButtonString, 30, TextureManager.GameFontBold, Color.White, Color.Red, startButtonAction);
            startButton.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 2.2f));
            mainPage.AddButton(startButton);

            ButtonAction settingsButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "Settings", 0);
            Button settingsButton = new Button(LanguageStrings.SettingsButtonString, 30, TextureManager.GameFontBold, Color.White, Color.Red, settingsButtonAction);
            settingsButton.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 2.2f + 60));
            mainPage.AddButton(settingsButton);

            ButtonAction highScoresButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "LocalHighScores", 0);
            Button highScoresButton = new Button(LanguageStrings.HighScoresButtonString, 30, TextureManager.GameFontBold, Color.White, Color.Red, highScoresButtonAction);
            highScoresButton.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 2.2f + 120));
            mainPage.AddButton(highScoresButton);

            ButtonAction creditsButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "Credits", 0);
            Button creditsButton = new Button(LanguageStrings.CreditsButtonString, 30, TextureManager.GameFontBold, Color.White, Color.Red, creditsButtonAction);
            creditsButton.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 2.2f + 180));
            mainPage.AddButton(creditsButton);

            ButtonAction quitButtonAction = new ButtonAction(ButtonAction.Type.QuitGame, "", 0);
            Button quitButton = new Button(LanguageStrings.CloseGameButtonString, 30, TextureManager.GameFontBold, Color.White, Color.Red, quitButtonAction);
            quitButton.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 2.2f + 240));
            mainPage.AddButton(quitButton);
        }

        private void CreateCreditsPage()
        {
            Page creditsPage = Pages[PageName.Credits];
            creditsPage.Background = background;

            Text title = new Text(LanguageStrings.CreditsTitleString, TextureManager.GameFontBold, 40);
            title.OutlineThickness = 2;
            title.FillColor = Color.White;
            title.Position = new Vector2f(Game.WINDOW_WIDTH / 2, Game.WINDOW_HEIGHT / 16);
            creditsPage.Title = title;

            ButtonAction creditsButtonAction = new ButtonAction(ButtonAction.Type.None, "", 0); 
            Button creditsButton = new Button(LanguageStrings.CreditsString, 18, TextureManager.GameFontBold, Color.White, Color.White, creditsButtonAction);
            creditsButton.ButtonText.LineSpacing = 0.8f;
            creditsButton.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 1.8));
            creditsPage.AddButton(creditsButton);

            ButtonAction backButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "MainPage", 0);
            Button backButton = new Button("<", 50, TextureManager.GameFontBold, Color.White, Color.Red, backButtonAction);
            backButton.ButtonText.Position = new Vector2f(50, (int)(Game.WINDOW_HEIGHT - 50));
            creditsPage.AddButton(backButton);
        }

        private void CreateSettingsPage()
        {
            Page settingsPage = Pages[PageName.Settings];
            settingsPage.Background = background;

            Text title = new Text(LanguageStrings.SettingsTitleString, TextureManager.GameFontBold, 40);
            title.OutlineThickness = 2;
            title.FillColor = Color.White;
            title.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 6));
            settingsPage.Title = title;

            ButtonAction leftArrowMusicAction = new ButtonAction(ButtonAction.Type.ChangeSetting, "MusicVolume", -10);
            ButtonAction rightArrowMusicAction = new ButtonAction(ButtonAction.Type.ChangeSetting, "MusicVolume", 10);
            LeftRightButtons musicVolumeButtons = new LeftRightButtons(LanguageStrings.MusicVolumeButtonString, "MusicVolume", 30, TextureManager.GameFont, TextureManager.GameFontBold, Color.White, Color.Red, leftArrowMusicAction, rightArrowMusicAction);

            musicVolumeButtons.SetPosition(new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 2.5)));
            musicVolumeButtons.SettingValue.SetDisplayedString(Settings.MusicVolume.ToString());

            settingsPage.AddButton(musicVolumeButtons.Label);
            settingsPage.AddButton(musicVolumeButtons.LeftArrow);
            settingsPage.AddButton(musicVolumeButtons.RightArrow);
            settingsPage.AddButton(musicVolumeButtons.SettingValue);

            ButtonAction leftArrowSoundAction = new ButtonAction(ButtonAction.Type.ChangeSetting, "SoundVolume", -10);
            ButtonAction rightArrowSoundAction = new ButtonAction(ButtonAction.Type.ChangeSetting, "SoundVolume", 10);
            LeftRightButtons soundVolumeButtons = new LeftRightButtons(LanguageStrings.SoundVolumeButtonString, "SoundVolume", 30, TextureManager.GameFont, TextureManager.GameFontBold, Color.White, Color.Red, leftArrowSoundAction, rightArrowSoundAction);

            soundVolumeButtons.SetPosition(new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 2.5) + 140));
            soundVolumeButtons.SettingValue.SetDisplayedString(Settings.SoundVolume.ToString());

            settingsPage.AddButton(soundVolumeButtons.Label);
            settingsPage.AddButton(soundVolumeButtons.LeftArrow);
            settingsPage.AddButton(soundVolumeButtons.RightArrow);
            settingsPage.AddButton(soundVolumeButtons.SettingValue);

            ButtonAction leftArrowLanguageAction = new ButtonAction(ButtonAction.Type.ChangeSetting, "Language", 0);
            ButtonAction rightArrowLanguageAction = new ButtonAction(ButtonAction.Type.ChangeSetting, "Language", 0);
            LeftRightButtons languageButtons = new LeftRightButtons(LanguageStrings.LanguageButtonString, "Language", 30, TextureManager.GameFont, TextureManager.GameFontBold, Color.White, Color.Red, leftArrowLanguageAction, rightArrowLanguageAction);

            languageButtons.SetPosition(new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 2.5) + 280));
            languageButtons.SettingValue.SetDisplayedString(Settings.Language);

            settingsPage.AddButton(languageButtons.Label);
            settingsPage.AddButton(languageButtons.LeftArrow);
            settingsPage.AddButton(languageButtons.RightArrow);
            settingsPage.AddButton(languageButtons.SettingValue);

            ButtonAction backButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "MainPage", 0);
            Button backButton = new Button("<", 50, TextureManager.GameFontBold, Color.White, Color.Red, backButtonAction);
            backButton.ButtonText.Position = new Vector2f(50, (int)(Game.WINDOW_HEIGHT - 50));
            settingsPage.AddButton(backButton);
        }

        private void CreateLocalHighScoresPage()
        {
            Page highScoresPage = Pages[PageName.LocalHighScores];
            highScoresPage.Background = background;

            Text title = new Text(LanguageStrings.LocalHighScoresTitleString, TextureManager.GameFontBold, 40);
            title.OutlineThickness = 2;
            title.FillColor = Color.White;
            title.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 6));
            highScoresPage.Title = title;

            ButtonAction leaderboardButtonAction = new ButtonAction(ButtonAction.Type.None, "", 0);
            Button leaderboardButton = new Button("", 30, TextureManager.GameFontBold, Color.White, Color.White, leaderboardButtonAction);
            leaderboardButton.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 2.5f));
            highScoresPage.AddButton(leaderboardButton);
            localLeaderboardButtonRef = leaderboardButton;

            ButtonAction globalHighScoresButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "GlobalHighScores", 0);
            Button globalHighScoresButton = new Button(">", 80, TextureManager.GameFontBold, Color.White, Color.Red, globalHighScoresButtonAction);
            globalHighScoresButton.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH - 50), (int)(Game.WINDOW_HEIGHT / 2));
            highScoresPage.AddButton(globalHighScoresButton);

            ButtonAction backButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "MainPage", 0);
            Button backButton = new Button("<", 50, TextureManager.GameFontBold, Color.White, Color.Red, backButtonAction);
            backButton.ButtonText.Position = new Vector2f(50, (int)(Game.WINDOW_HEIGHT - 50));
            highScoresPage.AddButton(backButton);
        }

        private void CreateGlobalHighScoresPage()
        {
            Page highScoresPage = Pages[PageName.GlobalHighScores];
            highScoresPage.Background = background;

            Text title = new Text(LanguageStrings.GlobalHighScoresTitleString, TextureManager.GameFontBold, 40);
            title.OutlineThickness = 2;
            title.FillColor = Color.White;
            title.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 6));
            highScoresPage.Title = title;

            ButtonAction leaderboardButtonAction = new ButtonAction(ButtonAction.Type.None, "", 0);
            Button leaderboardButton = new Button("", 30, TextureManager.GameFontBold, Color.White, Color.White, leaderboardButtonAction);
            leaderboardButton.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 2.5f));
            highScoresPage.AddButton(leaderboardButton);
            globalLeaderboardButtonRef = leaderboardButton;

            ButtonAction localHighScoresButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "LocalHighScores", 0);
            Button localHighScoresButton = new Button("<", 80, TextureManager.GameFontBold, Color.White, Color.Red, localHighScoresButtonAction);
            localHighScoresButton.ButtonText.Position = new Vector2f(50, (int)(Game.WINDOW_HEIGHT / 2));
            highScoresPage.AddButton(localHighScoresButton);

            ButtonAction backButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "MainPage", 0);
            Button backButton = new Button("<", 50, TextureManager.GameFontBold, Color.White, Color.Red, backButtonAction);
            backButton.ButtonText.Position = new Vector2f(50, (int)(Game.WINDOW_HEIGHT - 50));
            highScoresPage.AddButton(backButton);
        }

        public void UpdateLocalLeaderboard(Leaderboard leaderboard)
        {
            if (leaderboard == null || localLeaderboardButtonRef == null) return;

            StringBuilder leaderboardSB = new();
            for (int i = 0; i < leaderboard.LocalHighScores.Count; ++i)
            {
                leaderboardSB.AppendLine((i + 1) + ". " + leaderboard.LocalHighScores[i].Name + "   -   " + leaderboard.LocalHighScores[i].Score);
            }

            localLeaderboardButtonRef.SetDisplayedString(leaderboardSB.ToString());
            localLeaderboardButtonRef.ButtonText.Origin = new Vector2f((int)(localLeaderboardButtonRef.ButtonText.GetGlobalBounds().Width / 2), 0);
            localLeaderboardButtonRef.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 4));
        }

        public async Task UpdateGlobalLeaderboard(Leaderboard leaderboard)
        {
            if (leaderboard == null || globalLeaderboardButtonRef == null || isUpdatingGlobalLeaderboards) return;

            isUpdatingGlobalLeaderboards = true;

            await leaderboard.GetLeaderboardFromDatabase();

            StringBuilder leaderboardSB = new();
            for (int i = 0; i < leaderboard.GlobalHighScores.Count; ++i)
            {
                leaderboardSB.AppendLine((i + 1) + ". " + leaderboard.GlobalHighScores[i].Name + "   -   " + leaderboard.GlobalHighScores[i].Score);
            }

            globalLeaderboardButtonRef.SetDisplayedString(leaderboardSB.ToString());
            globalLeaderboardButtonRef.ButtonText.Origin = new Vector2f((int)(globalLeaderboardButtonRef.ButtonText.GetGlobalBounds().Width / 2), 0);
            globalLeaderboardButtonRef.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 4));

            isUpdatingGlobalLeaderboards = false;
        }

        public void ProcessButtonAction(ButtonAction buttonAction, GameLoop gameLoop)
        {
            switch (buttonAction.ActionType)
            {
                case ButtonAction.Type.StartLevel:
                    SoundManager.PlaySound("Click");
                    gameLoop.CurrentState = GameLoop.GameState.LoadingLevel;
                    break;

                case ButtonAction.Type.ChangeSetting:
                    SoundManager.PlaySound("Click");
                    if (buttonAction.Target == "SoundVolume")
                    {
                        Settings.SoundVolume += (int)buttonAction.Parameter;
                        foreach (var button in Pages[CurrentPage].Buttons)
                        {
                            if (button.OnMouseClick().ActionType == ButtonAction.Type.DisplayVariable &&
                                button.OnMouseClick().Target == "SoundVolume")
                            {
                                button.SetDisplayedString(Settings.SoundVolume.ToString());
                                break;
                            }
                        }
                        Settings.Save();
                    }
                    else if (buttonAction.Target == "MusicVolume")
                    {
                        SoundManager.PlaySound("Click");
                        Settings.MusicVolume += (int)buttonAction.Parameter;
                        foreach (var button in Pages[CurrentPage].Buttons)
                        {
                            if (button.OnMouseClick().ActionType == ButtonAction.Type.DisplayVariable &&
                                button.OnMouseClick().Target == "MusicVolume")
                            {
                                button.SetDisplayedString(Settings.MusicVolume.ToString());
                                break;
                            }
                        }
                        Settings.Save();
                    }
                    else if (buttonAction.Target == "Language")
                    {
                        SoundManager.PlaySound("Click");
                        Settings.Language = Settings.Language == "English" ? "Romana" : "English";
                        foreach (var button in Pages[CurrentPage].Buttons)
                        {
                            if (button.OnMouseClick().ActionType == ButtonAction.Type.DisplayVariable &&
                                button.OnMouseClick().Target == "Language")
                            {
                                button.SetDisplayedString(Settings.Language.ToString());
                                break;
                            }
                        }
                        Settings.Save();
                    }
                    break;

                case ButtonAction.Type.ChangePage:
                    SoundManager.PlaySound("Click");
                    if (Enum.TryParse(buttonAction.Target, out PageName pageName))
                    {
                        CurrentPage = pageName;
                    }
                    break;

                case ButtonAction.Type.QuitGame:
                    SoundManager.PlaySound("Click");
                    gameLoop.Window.Close();
                    break;

                default:
                    break;
            }
        }

        public void Update(View view, float deltaTime, float timeUntilUpdate)
        {
            Pages[CurrentPage].Update(view, deltaTime, timeUntilUpdate);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Pages[CurrentPage]);
        }
    }
}
