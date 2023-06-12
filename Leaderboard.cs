using _2DGame.GameSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _2DGame
{
    public class Leaderboard
    {
        public List<HighScore> HighScores { get; private set; }
        public const int MAX_HIGHSCORES_COUNT = 5;
        public const string HIGHSCORES_FILE_PATH = "./Data/highscores.json";

        public Leaderboard()
        {
            HighScores = new();
        }

        public void Load()
        {
            if (File.Exists(HIGHSCORES_FILE_PATH))
            {
                string jsonString = File.ReadAllText(HIGHSCORES_FILE_PATH);
                HighScores = JsonSerializer.Deserialize<List<HighScore>>(jsonString)!;
            }
            else
            {
                Save();
            }
        }

        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(HighScores, options);
            File.WriteAllText(HIGHSCORES_FILE_PATH, jsonString);
        }

        public void Add(string name, int score)
        {
            if (HighScores.Count < MAX_HIGHSCORES_COUNT || score > HighScores[HighScores.Count - 1].Score)
            {
                HighScores.Add(new(name, score));
                HighScores.Sort(delegate (HighScore hs1, HighScore hs2) { return (hs2.Score.CompareTo(hs1.Score)); });
                if (HighScores.Count > MAX_HIGHSCORES_COUNT)
                    HighScores.RemoveAt(HighScores.Count - 1);
            }
        }

        public record HighScore(string Name, int Score);
    }
}
