using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.MainMenu.Buttons
{
    public class ButtonAction
    {
        public enum Type { ChangeSetting, ChangePage, DisplayVariable, StartLevel, QuitGame, None }
        public Type ActionType { get; private set; }
        public string Target { get; private set; }
        public float Parameter { get; private set; }

        public ButtonAction(Type actionType, string target, float parameter)
        {
            ActionType = actionType;
            Target = target;
            Parameter = parameter;
        }
    }
}
