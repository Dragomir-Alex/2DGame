using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _2DGame.GameSettings
{
    public static class Settings
    {
        private static int musicVolume = 50;
        private static int soundVolume = 50;

        public const string SETTINGS_FILENAME_PATH = "./Data/settings.json";
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

        public static string Language { get; set; } = "English";

        public static void Load()
        {
            if (File.Exists(SETTINGS_FILENAME_PATH))
            {
                string jsonString = File.ReadAllText(SETTINGS_FILENAME_PATH);
                SettingsJson settingsJson = JsonSerializer.Deserialize<SettingsJson>(jsonString)!;
                MusicVolume = settingsJson.MusicVolume;
                SoundVolume = settingsJson.SoundVolume;
                Language = settingsJson.Language;
            }
            else
            {
                Save();
            }
        }

        public static void Save()
        {
            var settingsJson = new SettingsJson(musicVolume, soundVolume, Language);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(settingsJson, options);
            File.WriteAllText(SETTINGS_FILENAME_PATH, jsonString);
        }
    }
}
