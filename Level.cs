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
    public class Level : IDisposable
    {
        public LayerList Layers { get; set; }
        public Player Player { get; private set; }
        public View Camera { get; private set; }
        public string Name { get; private set; }
        public string TrackFilename { get; private set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2i TileStartPosition { get; private set; }

        public Level()
        {
            Layers = new LayerList();
            Player = new Player();
            Name = "Unnamed";
            TrackFilename = string.Empty;
            Width = 0;
            Height = 0;
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
            Width = Layers[LayerList.PRIMARY_LAYER].Width;
            Height = Layers[LayerList.PRIMARY_LAYER].Height;

            // Test
            TileStartPosition = new Vector2i(4, 6);

            Player.Initialize(TileStartPosition);
            Player.SetPlayerCamera(new Vector2f(Game.DEFAULT_WINDOW_WIDTH / 2, Game.DEFAULT_WINDOW_HEIGHT / 2), new Vector2f(Game.DEFAULT_WINDOW_WIDTH, Game.DEFAULT_WINDOW_HEIGHT));
            Camera = Player.Camera;
        }

        public void Update()
        {
            Player.Update(this, Game.DEFAULT_WINDOW_WIDTH, Game.DEFAULT_WINDOW_HEIGHT);
            for (int i = 0; i < LayerList.LAYER_COUNT; ++i)
            {
                Layers[i].Update(Player.Camera);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Player.Dispose();
                Layers.Dispose();
                Camera.Dispose();
                Name = string.Empty;
                TrackFilename = string.Empty;
                Width = 0;
                Height = 0;
                TileStartPosition = new Vector2i(0, 0);
            }
        }
    }
}
