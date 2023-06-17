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
                "LoadedInReverse - Life heart texture \nhttps://loadedinreverse.itch.io/life-hearts-basic\n\n" +
                "Extra free assets, music and sounds from Freesound (freesound.org) and OpenGameArt (opengameart.org)");
            EnglishDictionary.Add("InstructionsButtonString", "INSTRUCTIONS");
            EnglishDictionary.Add("InstructionsTitleString", "Instructions");
            EnglishDictionary.Add("InstructionsString",
                "The goal of this game is to get a high score and reach the portal at the end of the level.\n" +
                "You can collect gems to get points. Collecting pink hearts will restore your health.\n" +
                "Enemies can hurt you, so try to either avoid them or defeat them.\n" +
                "If you run out of health, you'll lose the game.");
            EnglishDictionary.Add("ControlsString",
                "Controls:\n\n" +
                "Jump -\t [ Up Arrow ]  /  [ X ]  /  [ W ]  /  [ L ]  /  ( D-pad Up )  /  ( A )\n" +
                "Move left -\t [ Left Arrow ]  /  [ A ]  /  ( D-pad Left )\n" +
                "Move right -\t [ Right Arrow ]  /  [ D ]  /  ( D-pad Right )\n" +
                "Attack -\t [ Z ]  /  [ K ]  /  ( X ) \n" +
                "Pause game -\t [ P ]  /  [ Esc ]  /  ( Menu )  /  ( Back )\n" +
                "Fullscreen mode -\t [ F11 ]");
            EnglishDictionary.Add("CloseGameButtonString", "CLOSE GAME");
            EnglishDictionary.Add("MusicVolumeButtonString", "Music Volume");
            EnglishDictionary.Add("SoundVolumeButtonString", "Sound Volume");
            EnglishDictionary.Add("LanguageButtonString", "Language");
            EnglishDictionary.Add("ScoreString", "Score: ");
            EnglishDictionary.Add("PausedMenuString",
                "The game is paused\n\n" +
                "Press [ P ] or ( Back ) to resume\n" +
                "Press [ Esc ] or ( Menu ) to exit to menu");
            EnglishDictionary.Add("GameOverWonString", "GAME OVER!\n  You won!\n");
            EnglishDictionary.Add("GameOverLostString", "GAME OVER!\n You lost!\n");
            EnglishDictionary.Add("GameOverFinalScoreString", "Final score: ");
            EnglishDictionary.Add("GameOverTypeNameString", "Please type out your name:");
            EnglishDictionary.Add("GameOverPressKeyString", "Press the [ Enter ] key to continue");
        }

        private static void InitializeRomanianDictionary()
        {
            RomanianDictionary.Add("LoadingString", "Se încarcă...");
            RomanianDictionary.Add("StartGameButtonString", "ÎNCEPE JOCUL");
            RomanianDictionary.Add("SettingsButtonString", "SETĂRI");
            RomanianDictionary.Add("SettingsTitleString", "Setări");
            RomanianDictionary.Add("HighScoresButtonString", "SCORURI MARI");
            RomanianDictionary.Add("LocalHighScoresTitleString", "Scoruri mari locale");
            RomanianDictionary.Add("GlobalHighScoresTitleString", "Scoruri mari globale");
            RomanianDictionary.Add("CreditsButtonString", "CREDITE");
            RomanianDictionary.Add("CreditsTitleString", "Credite");
            RomanianDictionary.Add("CreditsString",
                "Dodo-Codes - Bibliotecile TransformableHitbox2D și TileData, ajutor la realizarea codului \nhttps://github.com/Dodo-Codes\n\n" +
                "Sampo 'Blade' Vesanen - Tileset-ul 'Aztec 2' \nhttps://www.jazz2online.com/downloads/7852/aztec-2\n\n" +
                "c1b0rg0101 - Sprite sheet-ul vrăjitoarei \nhttps://c1b0rg0101.itch.io/little-witch-pixel-character-animation\n\n" +
                "LuizMelo - Sprite sheet-urile inamicilor \nhttps://luizmelo.itch.io/monsters-creatures-fantasy\n\n" +
                "Lunarnia - Sprite sheet-ul cristalului \nhttps://lunarnia.itch.io\n\n" +
                "LoadedInReverse - Textura inimii de viață \nhttps://loadedinreverse.itch.io/life-hearts-basic\n\n" +
                "Alte imagini, muzică și sunete gratuite de pe Freesound (freesound.org) și OpenGameArt (opengameart.org)");
            RomanianDictionary.Add("InstructionsButtonString", "INSTRUCȚIUNI");
            RomanianDictionary.Add("InstructionsTitleString", "Instrucțiuni");
            RomanianDictionary.Add("InstructionsString",
                "Scopul acestui joc este de a aduna cât mai multe puncte și de a ajunge la\n" +
                "portalul de la capătul nivelului.\n" +
                "Colectează cristale pentru a primi puncte. Inimile roz colectate\n" +
                "restituie o inimă de viata.\n" +
                "Inamicii te pot răni, așa că încearcă să îi eviți sau sa îi înfrângi.\n" +
                "Dacă rămâi fără inimi de viață, vei pierde jocul.");
            RomanianDictionary.Add("ControlsString",
                "Controale:\n\n" +
                "Salt -\t [ Săgeată sus ]  /  [ X ]  /  [ W ]  /  [ L ]  /  ( D-pad sus )  /  ( A )\n" +
                "Deplasare la stânga -\t [ Săgeată stânga ]  /  [ A ]  /  ( D-pad stânga )\n" +
                "Deplasare la dreapta -\t [ Săgeată dreapta ]  /  [ D ]  /  ( D-pad dreapta )\n" +
                "Atac -\t [ Z ]  /  [ K ]  /  ( X )\n" +
                "Suspendare joc -\t [ P ]  /  [ Esc ]  /  ( Menu )  /  ( Back )\n" +
                "Modul Ecran complet -\t [ F11 ]");
            RomanianDictionary.Add("CloseGameButtonString", "ÎNCHIDE JOCUL");
            RomanianDictionary.Add("MusicVolumeButtonString", "Volumul muzicii");
            RomanianDictionary.Add("SoundVolumeButtonString", "Volumul sunetului");
            RomanianDictionary.Add("LanguageButtonString", "Limba");
            RomanianDictionary.Add("ScoreString", "Scor: ");
            RomanianDictionary.Add("PausedMenuString",
                "Jocul este suspendat\n\n" +
                "Apasă [ P ] sau ( Back ) pentru a continua\n" +
                "Apasă [ Esc ] sau ( Menu ) pentru a reveni\n" +
                "la meniul principal");
            RomanianDictionary.Add("GameOverWonString", "JOC TERMINAT!\n  Ai câștigat!\n");
            RomanianDictionary.Add("GameOverLostString", "JOC TERMINAT!\n  Ai pierdut!\n");
            RomanianDictionary.Add("GameOverFinalScoreString", "Scor final: ");
            RomanianDictionary.Add("GameOverTypeNameString", "Tastează-ți numele:");
            RomanianDictionary.Add("GameOverPressKeyString", "Apasă tasta [ Enter ] pentru a continua");
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
