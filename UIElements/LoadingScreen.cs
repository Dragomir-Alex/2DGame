using _2DGame.LayerData;
using _2DGame.Layers;
using _2DGame.MainMenu;
using _2DGame.MainMenu.Buttons;
using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static _2DGame.MainMenu.Menu;

namespace _2DGame.UIElements
{
    public class LoadingScreen : Drawable
    {
        private Page LoadingPage { get; set; }

        public LoadingScreen()
        {
            LoadingPage = null;
        }

        public void Initialize()
        {
            int[,] tiles =
{
                { 3 }
            };

            TileData tileData = new TileData(tiles);

            LoadingPage = new Page();
            LoadingPage.Background = CreateMenuBackground(tileData);

            ButtonAction loadingPageAction = new ButtonAction(ButtonAction.Type.None, "", 0);
            Button loadingPageButton = new Button(LanguageStrings.LoadingString, 60, TextureManager.GameFontBold, Color.White, Color.White, loadingPageAction);
            loadingPageButton.ButtonText.Position = new Vector2f((int)(Game.WINDOW_WIDTH / 2), (int)(Game.WINDOW_HEIGHT / 1.5f));
            LoadingPage.AddButton(loadingPageButton);
        }

        public void InitializeSprites()
        {
            LoadingPage.InitializeSprites(null);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(LoadingPage);
        }
    }
}
