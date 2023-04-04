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
        public Layers.Layer[] Layers { get; private set; }
        public uint[,] EventData { get; private set; }
        public string Name { get; private set; }
        public string TrackFilename { get; set; } // Private set after loading is done?
        public Sprite Tileset { get; private set; }
        public List<TileData> TilesList { get; private set; }

        public Level()
        {
            // Nullable? Idk.
            Layers = null;
            EventData = null;
            Name = "Unnamed";
            TrackFilename = null;
            Tileset = null;
            TilesList = null;
        }

        public void LoadData()
        {
            // TBA

            // Testing
            TilesList = new();
            TileData tiles = new TileData(new int[,]{
                { 0, 0, 0, 0, 0, 0, 0, 10, 10, 0, 0, 0, 0, 0, 0, 0, 10, 10, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 10, 10, 0, 0, 0, 0, 0, 0, 0, 10, 10, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 10, 10, 0, 0, 0, 0, 0, 0, 0, 10, 10, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 10, 10, 10, 10, 10, 0, 10, 10, 10, 10, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 10, 10, 10, 10, 10, 0, 10, 10, 10, 10, 0, 0 }
            });

            TileData tilesDet = new TileData(new int[,] {
                { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 },
                { 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 },
                { 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 },
                { 60, 61, 62, 63, 64, 65, 66, 67, 68, 69 },
                { 70, 71, 72, 73, 74, 75, 76, 77, 78, 79 },
                { 80, 81, 82, 83, 84, 85, 86, 87, 88, 89 },
                { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 },
            });

            TileData tilesBg = new TileData(new int[,] {
                { 100, 101, 102, 103, 104, 105, 106, 107 },
                { 110, 111, 112, 113, 114, 115, 116, 117 },
                { 120, 121, 122, 123, 124, 125, 126, 127 },
                { 130, 131, 132, 133, 134, 135, 136, 137 },
                { 140, 141, 142, 143, 144, 145, 146, 147 },
                { 150, 151, 152, 153, 154, 155, 156, 157 },
                { 160, 161, 162, 163, 164, 165, 166, 167 },
                { 170, 171, 172, 173, 174, 175, 176, 177 },
            });

            TilesList.Add(null);
            TilesList.Add(null);
            TilesList.Add(null);
            TilesList.Add(tiles);
            TilesList.Add(null);
            TilesList.Add(null);
            TilesList.Add(tilesDet);
            TilesList.Add(tilesBg);
            // Testing end
        }

        public void Initialize(string tilesetFilename, string trackFilename)
        {
            TrackFilename = trackFilename;
            SoundManager.SetCurrentTrack(TrackFilename);
            InitializeLayers(tilesetFilename);
        }

        public void InstantiateLayers()
        {
            Layers = new Layers.Layer[Game.LAYER_COUNT];
            for (int i = 0; i < Game.LAYER_COUNT; ++i)
            {
                Layers[i] = new DetailLayer();
            }
            Layers[Game.PRIMARY_LAYER] = new SpriteLayer();
        }

        public void Update(Player player)
        {
            foreach (var layer in Layers) { layer.Update(player.Camera); }
        }

        private void InitializeLayers(string tilesetFilename)
        {
            for (int i = 0; i < Game.LAYER_COUNT; ++i)
            {
                if (Layers[i] != null)
                {
                    if (i == Game.PRIMARY_LAYER)
                    {
                        ((SpriteLayer)Layers[i]).Initialize(tilesetFilename, TilesList[i]);
                    }
                    else
                    {
                        ((DetailLayer)Layers[i]).Initialize(tilesetFilename, TilesList[i]);
                    }
                }
            }

            // Test data
            ((DetailLayer)Layers[Game.LAYER_COUNT - 1]).AutoXSpeed = -1;
            ((DetailLayer)Layers[Game.LAYER_COUNT - 1]).AutoYSpeed = -1;
            ((DetailLayer)Layers[Game.LAYER_COUNT - 1]).RepeatX = true;
            ((DetailLayer)Layers[Game.LAYER_COUNT - 1]).RepeatY = true;

            ((DetailLayer)Layers[Game.LAYER_COUNT - 2]).XOffset = 0f;
            ((DetailLayer)Layers[Game.LAYER_COUNT - 2]).YOffset = 355f;
            ((DetailLayer)Layers[Game.LAYER_COUNT - 2]).RepeatX = true;
            ((DetailLayer)Layers[Game.LAYER_COUNT - 2]).RepeatY = false;
            ((DetailLayer)Layers[Game.LAYER_COUNT - 2]).XSpeed = 200f;
            ((DetailLayer)Layers[Game.LAYER_COUNT - 2]).YSpeed = 100f;
            // Test end
        }
    }
}
