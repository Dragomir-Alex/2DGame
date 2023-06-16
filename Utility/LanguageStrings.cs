using _2DGame.GameSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Utility
{
    public static class LanguageStrings
    {
        public static string LoadingString { get; private set; }
        public static string StartGameButtonString { get; private set; }
        public static string SettingsButtonString { get; private set; }
        public static string SettingsTitleString { get; private set; }
        public static string HighScoresButtonString { get; private set; }
        public static string LocalHighScoresTitleString { get; private set; }
        public static string GlobalHighScoresTitleString { get; private set; }
        public static string CreditsButtonString { get; private set; }
        public static string CreditsTitleString { get; private set; }
        public static string CloseGameButtonString { get; private set; }
        public static string MusicVolumeButtonString { get; private set; }
        public static string SoundVolumeButtonString { get; private set; }
        public static string LanguageButtonString { get; private set; }
        public static string CreditsString { get; private set; }
        public static string ScoreString { get; private set; }
        public static string PausedMenuString { get; private set; }
        public static string GameOverLostString { get; private set; }
        public static string GameOverWonString { get; private set; }
        public static string GameOverTypeNameString { get; private set; }
        public static string GameOverFinalScoreString { get; private set; }
        public static string GameOverPressKeyString { get; private set; }

        public static void Initialize()
        {
            if (Settings.Language == "English") SetEnglish();
            else SetRomanian();
        }

        public static void SetEnglish()
        {
            LoadingString = "Loading...";
            StartGameButtonString = "START GAME";
            SettingsButtonString = "SETTINGS";
            SettingsTitleString = "Settings";
            HighScoresButtonString = "HIGH SCORES";
            LocalHighScoresTitleString = "Local High Scores";
            GlobalHighScoresTitleString = "Global High Scores";
            CreditsButtonString = "CREDITS";
            CreditsTitleString = "Credits";
            CreditsString =
                "Dodo-Codes - TransformableHitbox2D and TileData libraries, coding help \nhttps://github.com/Dodo-Codes\n\n" +
                "Sampo 'Blade' Vesanen - 'Aztec 2' tileset \nhttps://www.jazz2online.com/downloads/7852/aztec-2\n\n" +
                "c1b0rg0101 - Witch sprite sheet \nhttps://c1b0rg0101.itch.io/little-witch-pixel-character-animation\n\n" +
                "LuizMelo - Enemy sprite sheets \nhttps://luizmelo.itch.io/monsters-creatures-fantasy\n\n" +
                "Lunarnia - Gem sprite sheet \nhttps://lunarnia.itch.io\n\n" +
                "LoadedInReverse - Life hearts texture \nhttps://loadedinreverse.itch.io/life-hearts-basic\n\n" +
                "Extra free assets, music and sounds from Freesound (freesound.org) and OpenGameArt (opengameart.org)";
            CloseGameButtonString = "CLOSE GAME";
            MusicVolumeButtonString = "Music Volume";
            SoundVolumeButtonString = "Sound Volume";
            LanguageButtonString = "Language";
            ScoreString = "Score: ";
            PausedMenuString = "The game is paused\n\nPress [P] to resume\nPress [Esc] to exit to menu";
            GameOverWonString = "GAME OVER!\n  You won!\n";
            GameOverLostString = "GAME OVER!\n You lost!\n";
            GameOverFinalScoreString = "Final score: ";
            GameOverTypeNameString = "Please type out your name:";
            GameOverPressKeyString = "Press the [Enter] key to continue.";
        }

        public static void SetRomanian()
        {
            LoadingString = "Se incarca...";
            StartGameButtonString = "INCEPE JOCUL";
            SettingsButtonString = "SETARI";
            SettingsTitleString = "Setari";
            HighScoresButtonString = "SCORURI MARI";
            LocalHighScoresTitleString = "Scoruri mari locale";
            GlobalHighScoresTitleString = "Scoruri mari globale";
            CreditsButtonString = "CREDITE";
            CreditsTitleString = "Credite";
            CreditsString =
                "Dodo-Codes - Bibliotecile TransformableHitbox2D si TileData, ajutor la realizarea codului \nhttps://github.com/Dodo-Codes\n\n" +
                "Sampo 'Blade' Vesanen - Tileset-ul 'Aztec 2' \nhttps://www.jazz2online.com/downloads/7852/aztec-2\n\n" +
                "c1b0rg0101 - Sprite sheet-ul vrajitoarei \nhttps://c1b0rg0101.itch.io/little-witch-pixel-character-animation\n\n" +
                "LuizMelo - Sprite sheet-urile inamicilor \nhttps://luizmelo.itch.io/monsters-creatures-fantasy\n\n" +
                "Lunarnia - Sprite sheet-ul cristalului \nhttps://lunarnia.itch.io\n\n" +
                "LoadedInReverse - Textura inimilor de viata \nhttps://loadedinreverse.itch.io/life-hearts-basic\n\n" +
                "Alte imagini, muzica si sunete gratuite de pe Freesound (freesound.org) si OpenGameArt (opengameart.org)";
            CloseGameButtonString = "INCHIDE JOCUL";
            MusicVolumeButtonString = "Volumul muzicii";
            SoundVolumeButtonString = "Volumul sunetului";
            LanguageButtonString = "Limba";
            ScoreString = "Scor: ";
            PausedMenuString = "Jocul este suspendat\n\nApasa tasta [P] pentru a continua\nApasa tasta [Esc] pentru a reveni\nla meniul principal";
            GameOverWonString = "JOC TERMINAT!\n  Ai castigat!\n";
            GameOverLostString = "JOC TERMINAT!\n  Ai pierdut!\n";
            GameOverFinalScoreString = "Scor final: ";
            GameOverTypeNameString = "Tasteaza-ti numele:";
            GameOverPressKeyString = "Apasa tasta [Enter] pentru a continua.";
        }
    }
}
