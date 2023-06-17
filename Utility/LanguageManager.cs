using _2DGame.GameSettings;
using _2DGame.MainMenu.Buttons;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Utility
{
    public static class LanguageManager
    {
        public static Dictionary<string, string> EnglishDictionary { get; private set; } = new();
        public static Dictionary<string, string> RomanianDictionary { get; private set; } = new();
        public static List<(Text, string)> LocalizedText { get; private set; } = new();

        public static void Initialize()
        {
            InitializeEnglishDictionary();
            InitializeRomanianDictionary();
        }

        private static void InitializeEnglishDictionary()
        {
            EnglishDictionary.Add("LoadingString", "Loading...");
            EnglishDictionary.Add("StartGameButtonString", "START GAME");
            EnglishDictionary.Add("SettingsButtonString", "SETTINGS");
            EnglishDictionary.Add("SettingsTitleString", "Settings");
            EnglishDictionary.Add("HighScoresButtonString", "HIGH SCORES");
            EnglishDictionary.Add("LocalHighScoresTitleString", "Local High Scores");
            EnglishDictionary.Add("GlobalHighScoresTitleString", "Global High Scores");
            EnglishDictionary.Add("CreditsButtonString", "CREDITS");
            EnglishDictionary.Add("CreditsTitleString", "Credits");
            EnglishDictionary.Add("CreditsString",
                "Dodo-Codes - TransformableHitbox2D and TileData libraries, coding help \nhttps://github.com/Dodo-Codes\n\n" +
                "Sampo 'Blade' Vesanen - 'Aztec 2' tileset \nhttps://www.jazz2online.com/downloads/7852/aztec-2\n\n" +
                "c1b0rg0101 - Witch sprite sheet \nhttps://c1b0rg0101.itch.io/little-witch-pixel-character-animation\n\n" +
                "LuizMelo - Enemy sprite sheets \nhttps://luizmelo.itch.io/monsters-creatures-fantasy\n\n" +
                "Lunarnia - Gem sprite sheet \nhttps://lunarnia.itch.io\n\n" +
                "LoadedInReverse - Life hearts texture \nhttps://loadedinreverse.itch.io/life-hearts-basic\n\n" +
                "Extra free assets, music and sounds from Freesound (freesound.org) and OpenGameArt (opengameart.org)");
            EnglishDictionary.Add("CloseGameButtonString", "CLOSE GAME");
            EnglishDictionary.Add("MusicVolumeButtonString", "Music Volume");
            EnglishDictionary.Add("SoundVolumeButtonString", "Sound Volume");
            EnglishDictionary.Add("LanguageButtonString", "Language");
            EnglishDictionary.Add("ScoreString", "Score: ");
            EnglishDictionary.Add("PausedMenuString", "The game is paused\n\nPress [P] to resume\nPress [Esc] to exit to menu");
            EnglishDictionary.Add("GameOverWonString", "GAME OVER!\n  You won!\n");
            EnglishDictionary.Add("GameOverLostString", "GAME OVER!\n You lost!\n");
            EnglishDictionary.Add("GameOverFinalScoreString", "Final score: ");
            EnglishDictionary.Add("GameOverTypeNameString", "Please type out your name:");
            EnglishDictionary.Add("GameOverPressKeyString", "Press the [Enter] key to continue.");
        }

        private static void InitializeRomanianDictionary()
        {
            RomanianDictionary.Add("LoadingString", "Se incarca...");
            RomanianDictionary.Add("StartGameButtonString", "INCEPE JOCUL");
            RomanianDictionary.Add("SettingsButtonString", "SETARI");
            RomanianDictionary.Add("SettingsTitleString", "Setari");
            RomanianDictionary.Add("HighScoresButtonString", "SCORURI MARI");
            RomanianDictionary.Add("LocalHighScoresTitleString", "Scoruri mari locale");
            RomanianDictionary.Add("GlobalHighScoresTitleString", "Scoruri mari globale");
            RomanianDictionary.Add("CreditsButtonString", "CREDITE");
            RomanianDictionary.Add("CreditsTitleString", "Credite");
            RomanianDictionary.Add("CreditsString",
                "Dodo-Codes - Bibliotecile TransformableHitbox2D si TileData, ajutor la realizarea codului \nhttps://github.com/Dodo-Codes\n\n" +
                "Sampo 'Blade' Vesanen - Tileset-ul 'Aztec 2' \nhttps://www.jazz2online.com/downloads/7852/aztec-2\n\n" +
                "c1b0rg0101 - Sprite sheet-ul vrajitoarei \nhttps://c1b0rg0101.itch.io/little-witch-pixel-character-animation\n\n" +
                "LuizMelo - Sprite sheet-urile inamicilor \nhttps://luizmelo.itch.io/monsters-creatures-fantasy\n\n" +
                "Lunarnia - Sprite sheet-ul cristalului \nhttps://lunarnia.itch.io\n\n" +
                "LoadedInReverse - Textura inimilor de viata \nhttps://loadedinreverse.itch.io/life-hearts-basic\n\n" +
                "Alte imagini, muzica si sunete gratuite de pe Freesound (freesound.org) si OpenGameArt (opengameart.org)");
            RomanianDictionary.Add("CloseGameButtonString", "INCHIDE JOCUL");
            RomanianDictionary.Add("MusicVolumeButtonString", "Volumul muzicii");
            RomanianDictionary.Add("SoundVolumeButtonString", "Volumul sunetului");
            RomanianDictionary.Add("LanguageButtonString", "Limba");
            RomanianDictionary.Add("ScoreString", "Scor: ");
            RomanianDictionary.Add("PausedMenuString", "Jocul este suspendat\n\nApasa tasta [P] pentru a continua\nApasa tasta [Esc] pentru a reveni\nla meniul principal");
            RomanianDictionary.Add("GameOverWonString", "JOC TERMINAT!\n  Ai castigat!\n");
            RomanianDictionary.Add("GameOverLostString", "JOC TERMINAT!\n  Ai pierdut!\n");
            RomanianDictionary.Add("GameOverFinalScoreString", "Scor final: ");
            RomanianDictionary.Add("GameOverTypeNameString", "Tasteaza-ti numele:");
            RomanianDictionary.Add("GameOverPressKeyString", "Apasa tasta [Enter] pentru a continua.");
        }

        public static void AddLocalizedText(Text text, string str)
        {
            LocalizedText.Add((text, str));
            text.DisplayedString = Settings.Language == "English" ? EnglishDictionary[str] : RomanianDictionary[str];
            text.Origin = new Vector2f((int)(text.GetGlobalBounds().Width / 2), (int)(text.GetGlobalBounds().Height / 2));
        }

        public static void UpdateAllButtons()
        {
            foreach (var text in LocalizedText)
            {
                text.Item1.DisplayedString = Settings.Language == "English" ? EnglishDictionary[text.Item2] : RomanianDictionary[text.Item2];
                text.Item1.Origin = new Vector2f((int)(text.Item1.GetGlobalBounds().Width / 2), (int)(text.Item1.GetGlobalBounds().Height / 2));
            }
        }
    }
}
