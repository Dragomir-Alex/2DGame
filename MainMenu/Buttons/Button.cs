﻿using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.MainMenu.Buttons
{
    public class Button : IButton
    {
        public bool IsSelected { get; set; }
        public bool IsActive { get; set; }
        private ButtonAction Action { get; set; }

        public Text ButtonText { get; set; }
        public Color DefaultColor { get; private set; }
        public Color HoverColor { get; private set; }

        public Button(string textString, uint fontSize, Font font, Color defaultColor, Color hoverColor, ButtonAction buttonAction)
        {
            IsSelected = false;
            IsActive = true;
            DefaultColor = defaultColor;
            HoverColor = hoverColor;
            Action = buttonAction;

            ButtonText = new Text(textString, font, fontSize)
            {
                FillColor = DefaultColor
            };
        }

        public ButtonAction OnMouseClick()
        {
            if (IsActive)
            {
                return Action;
            }
            else return new ButtonAction(ButtonAction.Type.None, "", 0);
        }

        public void Update()
        {
            if (IsSelected)
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