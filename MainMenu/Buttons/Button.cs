using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.MainMenu.Buttons
{
    public class Button : Drawable
    {
        public bool IsHovered { get; set; }
        public bool IsActive { get; set; }
        private ButtonAction Action { get; set; }

        public Text ButtonText { get; set; }
        public Color DefaultColor { get; private set; }
        public Color HoverColor { get; private set; }

        public Button(string textString, uint fontSize, Font font, Color defaultColor, Color hoverColor, ButtonAction buttonAction)
        {
            IsHovered = false;
            IsActive = true;
            DefaultColor = defaultColor;
            HoverColor = hoverColor;
            Action = buttonAction;

            ButtonText = new Text(textString, font, fontSize)
            {
                FillColor = DefaultColor,
                OutlineThickness = 2
            };

            ButtonText.Origin = new Vector2f((int)(ButtonText.GetGlobalBounds().Width / 2), (int)(ButtonText.GetGlobalBounds().Height / 2));
        }

        public ButtonAction OnMouseClick()
        {
            if (IsActive)
            {
                return Action;
            }
            else return new ButtonAction(ButtonAction.Type.None, "", 0);
        }

        public void SetDisplayedString(string displayedString)
        {
            ButtonText.DisplayedString = displayedString;
            ButtonText.Origin = new Vector2f((int)(ButtonText.GetGlobalBounds().Width / 2), (int)(ButtonText.GetGlobalBounds().Height / 2));
        }

        public void Update()
        {
            if (IsHovered)
            {
                ButtonText.FillColor = HoverColor;
            }
            else
            {
                ButtonText.FillColor = DefaultColor;
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(ButtonText);
        }
    }
}
