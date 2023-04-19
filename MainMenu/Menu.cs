﻿using _2DGame.ExternalLibraries;
using _2DGame.Layers;
using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.MainMenu
{
    public class Menu : Drawable
    {
        public const string MENU_MUSIC_FILENAME = "passage.ogg";
        public enum PageName { MainPage, Settings, Credits }
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

        private Page CreateMainPage(DetailLayer background)
        {
            Page mainPage = Pages[PageName.MainPage];
            mainPage.SetBackground(background);

            ButtonAction startButtonAction = new ButtonAction(ButtonAction.Type.StartLevel, "", 0); // Will need to do something about that last param...
            Button startButton = new Button("START GAME", 30, TextureManager.GameFontBold, Color.White, Color.Red, startButtonAction);
            startButton.ButtonText.Position = new Vector2f(Game.DEFAULT_WINDOW_WIDTH / 2 - startButton.ButtonText.GetGlobalBounds().Width / 2, Game.DEFAULT_WINDOW_HEIGHT / 1.5f);
            mainPage.AddButton(startButton);

            ButtonAction creditsButtonAction = new ButtonAction(ButtonAction.Type.ChangePage, "Credits", 0);
            Button creditsButton = new Button("CREDITS", 30, TextureManager.GameFontBold, Color.White, Color.Red, creditsButtonAction);
            creditsButton.ButtonText.Position = new Vector2f(Game.DEFAULT_WINDOW_WIDTH / 2 - startButton.ButtonText.GetGlobalBounds().Width / 2, Game.DEFAULT_WINDOW_HEIGHT / 1.5f + 75);
            mainPage.AddButton(creditsButton);

            ButtonAction quitButtonAction = new ButtonAction(ButtonAction.Type.QuitGame, "", 0);
            Button quitButton = new Button("CLOSE GAME", 30, TextureManager.GameFontBold, Color.White, Color.Red, quitButtonAction);
            quitButton.ButtonText.Position = new Vector2f(Game.DEFAULT_WINDOW_WIDTH / 2 - startButton.ButtonText.GetGlobalBounds().Width / 2, Game.DEFAULT_WINDOW_HEIGHT / 1.5f + 150);
            mainPage.AddButton(quitButton);

            return mainPage;
        }

        public void ProcessButtonAction(ButtonAction buttonAction, GameLoop gameLoop)
        {
            switch (buttonAction.ActionType)
            {
                case ButtonAction.Type.StartLevel:
                    gameLoop.CurrentState = GameLoop.GameState.LoadingLevel;
                    break;

                case ButtonAction.Type.ChangeSetting:
                    if (buttonAction.Target == "Sound")
                    {
                        Settings.SoundVolume += (uint)buttonAction.Parameter;
                    }
                    else if (buttonAction.Target == "Music")
                    {
                        Settings.MusicVolume += (uint)buttonAction.Parameter;
                    }
                    break;

                case ButtonAction.Type.ChangePage:
                    if (Enum.TryParse<PageName>(buttonAction.Target, out PageName pageName))
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