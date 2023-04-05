using _2DGame.Entities;
using _2DGame.ExternalLibraries;
using _2DGame.Layers;
using _2DGame.Utility;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame
{
    public class Level
    {
        public LayerList Layers;
        public string Name { get; private set; }
        public string TrackFilename { get; set; } // Private set after loading is done?
        public Sprite Tileset { get; private set; }
        public Vector2i TileStartPosition { get; private set; }

        public Level()
        {
            Layers = new LayerList();
            Name = "Unnamed";
            TrackFilename = null;
            Tileset = null;
            TileStartPosition = new Vector2i(0, 0);
        }

        public void LoadData(string mapFilename)
        {
            Layers.Load(mapFilename);
        }

        public void Initialize(string tilesetFilename, string trackFilename)
        {
            TrackFilename = trackFilename;
            SoundManager.SetCurrentTrack(TrackFilename);
            Layers.Initialize(tilesetFilename);

            // Test
            TileStartPosition = new Vector2i(4, 6);
        }

        public void Update(Player player)
        {
            for (int i = 0; i < LayerList.LAYER_COUNT; ++i)
            {
                Layers[i].Update(player.Camera);
            }
        }
    }
}
