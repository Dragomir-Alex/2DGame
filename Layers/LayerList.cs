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
            ((DetailLayer)Layers[LAYER_COUNT - 1]).AutoXSpeed = -1;
            ((DetailLayer)Layers[LAYER_COUNT - 1]).AutoYSpeed = -1;
            ((DetailLayer)Layers[LAYER_COUNT - 1]).RepeatX = true;
            ((DetailLayer)Layers[LAYER_COUNT - 1]).RepeatY = true;

            ((DetailLayer)Layers[LAYER_COUNT - 2]).XOffset = 0f;
            ((DetailLayer)Layers[LAYER_COUNT - 2]).YOffset = 355f;
            ((DetailLayer)Layers[LAYER_COUNT - 2]).RepeatX = true;
            ((DetailLayer)Layers[LAYER_COUNT - 2]).RepeatY = false;
            ((DetailLayer)Layers[LAYER_COUNT - 2]).XSpeed = 200f;
            ((DetailLayer)Layers[LAYER_COUNT - 2]).YSpeed = 100f;
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
