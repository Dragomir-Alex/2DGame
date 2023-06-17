using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.MainMenu.Buttons
{
    public class LeftRightButtons
    {
        public Button SettingValue { get; set; }
        public Button Label { get; set; }
        public Button LeftArrow { get; set; }
        public Button RightArrow { get; set; }

        public LeftRightButtons(string localizedTextString, string settingValueString, uint fontSize, Font font, Font boldFont, Color defaultColor, Color hoverColor, ButtonAction leftArrowAction, ButtonAction rightArrowAction)
        {
            ButtonAction NoAction = new ButtonAction(ButtonAction.Type.None, "", 0);
            ButtonAction SettingValueAction = new ButtonAction(ButtonAction.Type.DisplayVariable, settingValueString, 0);

            SettingValue = new Button(" - ", fontSize, boldFont, defaultColor, defaultColor, SettingValueAction);
            Label = new Button(localizedTextString, fontSize, boldFont, defaultColor, defaultColor, NoAction);
            LanguageManager.AddLocalizedText(Label.ButtonText, localizedTextString);
            LeftArrow = new Button("< ", (uint)(fontSize * 1.5), boldFont, defaultColor, hoverColor, leftArrowAction);
            RightArrow = new Button(" >", (uint)(fontSize * 1.5), boldFont, defaultColor, hoverColor, rightArrowAction);

            SetPosition(new());
        }

        public void SetPosition(Vector2f position)
        {
            SettingValue.ButtonText.Position = position;
            LeftArrow.ButtonText.Position = new Vector2f(SettingValue.ButtonText.Position.X - LeftArrow.ButtonText.GetGlobalBounds().Width - 60, SettingValue.ButtonText.Position.Y);
            Label.ButtonText.Position = new Vector2f(SettingValue.ButtonText.Position.X, SettingValue.ButtonText.Position.Y - SettingValue.ButtonText.GetGlobalBounds().Height - 40);
            RightArrow.ButtonText.Position = new Vector2f(SettingValue.ButtonText.Position.X + SettingValue.ButtonText.GetGlobalBounds().Width + 60, SettingValue.ButtonText.Position.Y);
        }
    }
}
