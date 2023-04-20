using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame
{
    public static class Settings
    {
        private static int musicVolume = 100;
        private static int soundVolume = 100;

        public static int MusicVolume
        {
            get { return musicVolume; }
            set
            {
                if (value <= 0)
                {
                    musicVolume = 0;
                }
                else if (value >= 100)
                {
                    musicVolume = 100;
                }
                else musicVolume = value;
            }
        }

        public static int SoundVolume
        {
            get { return soundVolume; }
            set
            {
                if (value < 0)
                {
                    soundVolume = 0;
                }
                else if (value > 100)
                {
                    soundVolume = 100;
                }
                else soundVolume = value;
            }
        }
    }
}
