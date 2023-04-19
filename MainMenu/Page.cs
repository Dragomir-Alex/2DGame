using _2DGame.Layers;
using InstilledBee.SFML.SimpleCollision;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.MainMenu
{
    public class Page : Drawable
    {
        public Text Title { get; set; }
        public DetailLayer Background { get; set; }
        public List<Button> Buttons { get; set; }
        public Texture? LogoTexture { get; set; }
        public Sprite? LogoSprite { get; set; }

        public Page()
        {
            Title = new Text();
            Background = new DetailLayer();
            Buttons = new List<Button>();
        }

        public Page(Text title, DetailLayer background)
        {
            Title = title;
            Background = background;
            Buttons = new List<Button>();
        }

        public void AddButton(Button button)
        {
            Buttons.Add(button);
        }

        public void InitializeSprites()
        {
            Background.InitializeSprite();
            if (LogoTexture != null)
            {
                LogoSprite = new Sprite(LogoTexture);
            }
        }

        public void Update(View view)
        {
            Background.Update(view);
            foreach (var button in Buttons)
            {
                button.Update();
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Background);
            target.Draw(Title);

            if (LogoSprite != null)
            {
                target.Draw(LogoSprite);
            }
            foreach (var button in Buttons)
            {
                target.Draw(button);
            }
        }
    }
}
