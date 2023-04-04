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
        public List<TileData> Map { get; private set; }
        public const string MAPS_PATH = "./Data/Maps/";

        public Level()
        {
            // Nullable? Idk.
            Layers = null;
            EventData = null;
            Name = "Unnamed";
            TrackFilename = null;
            Tileset = null;
            Map = null;
        }

        public void LoadData(string mapFilename)
        {
            Map = new();
            for (int i = 0; i < Game.LAYER_COUNT; ++i)
            {
                TileData tileData = new TileData(MAPS_PATH + mapFilename, (i + 1).ToString());
                Map.Add(tileData);
            }
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
                        ((SpriteLayer)Layers[i]).Initialize(tilesetFilename, Map[i]);
                    }
                    else
                    {
                        ((DetailLayer)Layers[i]).Initialize(tilesetFilename, Map[i]);
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
