using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.MainMenu
{
    public class LeftRightButtons
    {
        public Button SettingValue { get; set; }
        public Button Label { get; set; }
        public Button LeftArrow { get; set; }
        public Button RightArrow { get; set; }

        public LeftRightButtons(string textString, string settingValueString, uint fontSize, Font font, Font boldFont, Color defaultColor, Color hoverColor, ButtonAction leftArrowAction, ButtonAction rightArrowAction)
        {
            ButtonAction NoAction = new ButtonAction(ButtonAction.Type.None, "", 0);
            ButtonAction SettingValueAction = new ButtonAction(ButtonAction.Type.DisplayVariable, settingValueString, 0);

            SettingValue = new Button(" - ", fontSize, boldFont, defaultColor, defaultColor, SettingValueAction);

            Label = new Button(textString, fontSize, boldFont, defaultColor, defaultColor, NoAction);
            Label.ButtonText.Position = new Vector2f(SettingValue.ButtonText.Position.X - (int)(Math.Abs(Label.ButtonText.GetGlobalBounds().Width - SettingValue.ButtonText.GetGlobalBounds().Width) / 2), SettingValue.ButtonText.Position.Y - SettingValue.ButtonText.GetGlobalBounds().Height - 40);

            LeftArrow = new Button("< ", (uint)(fontSize * 1.5), boldFont, defaultColor, hoverColor, leftArrowAction);
            LeftArrow.ButtonText.Position = new Vector2f(SettingValue.ButtonText.Position.X - LeftArrow.ButtonText.GetGlobalBounds().Width - 60, SettingValue.ButtonText.Position.Y - (int)(Math.Abs(LeftArrow.ButtonText.GetGlobalBounds().Height - SettingValue.ButtonText.GetGlobalBounds().Height) / 2));

            RightArrow = new Button(" >", (uint)(fontSize * 1.5), boldFont, defaultColor, hoverColor, rightArrowAction);
            RightArrow.ButtonText.Position = new Vector2f(SettingValue.ButtonText.Position.X + SettingValue.ButtonText.GetGlobalBounds().Width + 60, SettingValue.ButtonText.Position.Y - (int)(Math.Abs(RightArrow.ButtonText.GetGlobalBounds().Height - SettingValue.ButtonText.GetGlobalBounds().Height) / 2));
        }

        public void SetPosition(Vector2f position)
        {
            float xMove = (int)(position.X - LeftArrow.ButtonText.Position.X);
            float yMove = (int)(position.Y - LeftArrow.ButtonText.Position.Y);

            LeftArrow.ButtonText.Position = new Vector2f(LeftArrow.ButtonText.Position.X + xMove, LeftArrow.ButtonText.Position.Y + yMove);
            Label.ButtonText.Position = new Vector2f(Label.ButtonText.Position.X + xMove, Label.ButtonText.Position.Y + yMove);
            SettingValue.ButtonText.Position = new Vector2f(SettingValue.ButtonText.Position.X + xMove, SettingValue.ButtonText.Position.Y + yMove);
            RightArrow.ButtonText.Position = new Vector2f(RightArrow.ButtonText.Position.X + xMove, RightArrow.ButtonText.Position.Y + yMove);
        }

        public Vector2f GetPosition()
        {
            return LeftArrow.ButtonText.Position; // LeftArrow is the pivot
        }

        public Vector2f GetSize()
        {
            return new Vector2f(RightArrow.ButtonText.GetGlobalBounds().Width + RightArrow.ButtonText.Position.X - LeftArrow.ButtonText.Position.X, LeftArrow.ButtonText.GetGlobalBounds().Height + Label.ButtonText.GetGlobalBounds().Height);
        }
    }
}
