using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.GameSettings
{
    internal class SettingsJson
    {
        public int MusicVolume { get; set; }
        public int SoundVolume { get; set; }
        public string Language { get; set; }

        public SettingsJson(int musicVolume, int soundVolume, string language)
        {
            MusicVolume = musicVolume;
            SoundVolume = soundVolume;
            Language = language;
        }
    }
}
