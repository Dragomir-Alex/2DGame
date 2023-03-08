using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame
{
    public class Settings
    {
        private uint musicVolume;
        private uint soundVolume;

        public uint MusicVolume
        {
            get { return musicVolume; }
            set
            {
                if (value < 0)
                {
                    musicVolume = 0;
                }
                else if (value > 100)
                {
                    musicVolume = 100;
                }
                else musicVolume = value;
            }
        }

        public uint SoundVolume
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

        public Settings()
        {
            musicVolume = 100;
            soundVolume = 100;
        }
    }
}
