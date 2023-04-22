using _2DGame.Entities;
using _2DGame.ExternalLibraries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Layers
{
    public class LayerList : IEnumerable<Layer>, IDestroyable
    {
        private Layer[] Layers { get; set; }
        private List<TileData> Map { get; set; }

        public const int LAYER_COUNT = 8;
        public const int PRIMARY_LAYER = 3;
        public const string MAPS_PATH = "./Data/Maps/";

        public Layer this[int i]
        {
            get { return Layers[i]; }
            set { Layers[i] = value; }
        }

        public LayerList()
        {
            Instantiate();
        }

        public void Load(string mapFilename)
        {
            Map = new();
            for (int i = 0; i < LayerList.LAYER_COUNT; ++i)
            {
                TileData tileData = new TileData(MAPS_PATH + mapFilename, (i + 1).ToString());
                Map.Add(tileData);
            }
        }

        public void Instantiate()
        {
            Layers = new Layer[LAYER_COUNT];
            Map = new();

            for (int i = 0; i < LAYER_COUNT; ++i)
            {
                Layers[i] = new DetailLayer();
            }
            Layers[PRIMARY_LAYER] = new SpriteLayer();
        }

        public void Initialize(string tilesetFilename)
        {
            for (int i = 0; i < LayerList.LAYER_COUNT; ++i)
            {
                if (Layers[i] != null)
                {
                    if (i == LayerList.PRIMARY_LAYER)
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
            ((DetailLayer)Layers[0]).XSpeed = Game.DEFAULT_WINDOW_WIDTH * 1.25f;
            ((DetailLayer)Layers[0]).YSpeed = Game.DEFAULT_WINDOW_HEIGHT * 1.25f;
            ((DetailLayer)Layers[0]).XOffset = 3000;
            ((DetailLayer)Layers[0]).YOffset = 2200;

            ((DetailLayer)Layers[1]).XSpeed = Game.DEFAULT_WINDOW_WIDTH / 2f;
            ((DetailLayer)Layers[1]).YSpeed = Game.DEFAULT_WINDOW_HEIGHT / 2f;
            ((DetailLayer)Layers[1]).XOffset = 1600;
            ((DetailLayer)Layers[1]).YOffset = 1042;

            ((DetailLayer)Layers[2]).XSpeed = Game.DEFAULT_WINDOW_WIDTH / 2f;
            ((DetailLayer)Layers[2]).YSpeed = Game.DEFAULT_WINDOW_HEIGHT / 2f;
            ((DetailLayer)Layers[2]).XOffset = 1600;
            ((DetailLayer)Layers[2]).YOffset = 1042;

            ((DetailLayer)Layers[4]).YOffset = 700f;
            ((DetailLayer)Layers[4]).RepeatX = true;
            ((DetailLayer)Layers[4]).XSpeed = 550f;
            ((DetailLayer)Layers[4]).YSpeed = 75f;

            ((DetailLayer)Layers[5]).YOffset = 650f;
            ((DetailLayer)Layers[5]).RepeatX = true;
            ((DetailLayer)Layers[5]).XSpeed = 350f;
            ((DetailLayer)Layers[5]).YSpeed = 50f;

            ((DetailLayer)Layers[6]).YOffset = 700f;
            ((DetailLayer)Layers[6]).RepeatX = true;
            ((DetailLayer)Layers[6]).XSpeed = 150f;
            ((DetailLayer)Layers[6]).YSpeed = 25f;

            ((DetailLayer)Layers[7]).YOffset = 160f;
            ((DetailLayer)Layers[7]).RepeatX = true;
            ((DetailLayer)Layers[7]).AutoXSpeed = 1f;
            // Test end
        }

        IEnumerator<T> Cast<T>(IEnumerator iterator)
        {
            while (iterator.MoveNext())
            {
                yield return (T)iterator.Current;
            }
        }

        public IEnumerator<Layer> GetEnumerator()
        {
            return Cast<Layer>(Layers.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Destroy()
        {
            foreach (var layer in Layers)
            {
                layer.Destroy();
            }
            Layers = Array.Empty<Layer>();
            Map.Clear();
        }
    }
}
