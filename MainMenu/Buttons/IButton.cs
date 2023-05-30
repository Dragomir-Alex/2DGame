using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.MainMenu.Buttons
{
    public interface IButton : Drawable
    {
        public bool IsSelected { get; set; }
        public bool IsActive { get; set; }

        public ButtonAction OnMouseClick();
    }
}
