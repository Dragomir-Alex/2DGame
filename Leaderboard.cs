using _2DGame.GameSettings;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _2DGame
{
    public class Leaderboard
    {
        public List<HighScore> LocalHighScores { get; private set; }
        public List<HighScore> GlobalHighScores { get; private set; }

        public const int MAX_HIGHSCORES_COUNT = 10;
        public const string HIGHSCORES_FILE_PATH = "./Data/highscores.json";
        public const string CONNECTION_STRING = "Host=3.72.246.58;Username=player;Password=1234;Database=postgres;SSL Mode=Require;Trust Server Certificate=true";

        public Leaderboard()
        {
            LocalHighScores = new();
            GlobalHighScores = new();
        }

        public void Load()
        {
            if (File.Exists(HIGHSCORES_FILE_PATH))
            {
                string jsonString = File.ReadAllText(HIGHSCORES_FILE_PATH);
                LocalHighScores = JsonSerializer.Deserialize<List<HighScore>>(jsonString)!;
            }
            else
            {
                Save();
            }
        }

        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(LocalHighScores, options);
            File.WriteAllText(HIGHSCORES_FILE_PATH, jsonString);
        }

        public async Task Add(string name, int score)
        {
            await AddToDatabase(name, score);

            if (LocalHighScores.Count < MAX_HIGHSCORES_COUNT || score > LocalHighScores[LocalHighScores.Count - 1].Score)
            {
                LocalHighScores.Add(new(name, score));
                LocalHighScores.Sort(delegate (HighScore hs1, HighScore hs2) { return (hs2.Score.CompareTo(hs1.Score)); });
                if (LocalHighScores.Count > MAX_HIGHSCORES_COUNT)
                    LocalHighScores.RemoveAt(LocalHighScores.Count - 1);
            }
        }

        private async Task AddToDatabase(string name, int score)
        {
            await using var dataSource = NpgsqlDataSource.Create(CONNECTION_STRING);

            await using (var cmd = dataSource.CreateCommand("INSERT INTO user_scores (user_name, score) VALUES ($1, $2)"))
            {
                cmd.CommandTimeout = 3;
                cmd.Parameters.AddWithValue(name);
                cmd.Parameters.AddWithValue(score);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task GetLeaderboardFromDatabase()
        {
            GlobalHighScores.Clear();

            await using var dataSource = NpgsqlDataSource.Create(CONNECTION_STRING);

            await using (var cmd = dataSource.CreateCommand("SELECT user_name, score FROM user_scores ORDER BY score DESC LIMIT 10"))
            {
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    cmd.CommandTimeout = 3;
                    while (await reader.ReadAsync())
                    {
                        GlobalHighScores.Add(new HighScore(reader.GetString(0), reader.GetInt32(1)));
                    }
                }
            }
        }

        public record HighScore(string Name, int Score);
    }
}
