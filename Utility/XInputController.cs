using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Utility
{
    public static class XInputController
    {
        private static readonly Controller controller;
        public static Gamepad GamePad { get; set; }

        static XInputController()
        {
            controller = new Controller(UserIndex.One);
        }

        public static void Update()
        {
            if (!controller.IsConnected)
                return;

            GamePad = controller.GetState().Gamepad;
        }
    }
}
