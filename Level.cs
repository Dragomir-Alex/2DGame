using _2DGame.Utility;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static _2DGame.Layers.BackgroundLayer;

namespace _2DGame
{
    public class Level
    {
        public Layers.Layer[] Layers { get; private set; }
        public uint[,] EventData { get; private set; }
        public string Name { get; private set; }
        public string TrackFilename { get; set; } // Private set after loading is done?
        public Sprite Tileset { get; private set; }

        public Level()
        {
            // Nullable? Idk.
            Layers = null;
            EventData = null;
            Name = "Unnamed";
            TrackFilename = null;
            Tileset = null;
        }

        public void LoadData()
        {
            // TBA
        }

        public void InitializeLevel(GameLoop gameLoop)
        {
            SoundManager.SetCurrentTrack(TrackFilename);
            // Load tile data
            // Load event data
        }
    }
}
