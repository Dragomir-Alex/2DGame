﻿using _2DGame.Entities;
using _2DGame.Entities.Players;
using _2DGame.LayerData;
using SFML.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _2DGame.Layers
{
    public class LayerList : IEnumerable<Layer>
    {
        private DetailLayer[] loadedLayerData;
        private Layer[] layers;
        private List<TileData> map;

        public const int LAYER_COUNT = 8;
        public const int PRIMARY_LAYER = 3;
        public const string LEVELS_PATH = "./Data/Levels/";

        public Layer this[int i]
        {
            get { return layers[i]; }
            set { layers[i] = value; }
        }

        public LayerList()
        {
            Instantiate();
        }

        public void Load(string mapFilename, string layerDataFilename)
        {
            for (int i = 0; i < LAYER_COUNT; ++i)
            {
                TileData tileData = new TileData(LEVELS_PATH + mapFilename, (i + 1).ToString());
                map.Add(tileData);
            }

            string fileName = LEVELS_PATH + layerDataFilename;
            string jsonString = File.ReadAllText(fileName);
            loadedLayerData = JsonSerializer.Deserialize<DetailLayer[]>(jsonString)!;
        }

        public void Instantiate()
        {
            layers = new Layer[LAYER_COUNT];
            map = new();

            for (int i = 0; i < LAYER_COUNT; ++i)
            {
                layers[i] = new DetailLayer();
            }
            layers[PRIMARY_LAYER] = new SpriteLayer();
        }

        public void Initialize(Texture tileset)
        {
            for (int i = 0; i < LAYER_COUNT; ++i)
            {
                if (layers[i] != null)
                {
                    if (i == PRIMARY_LAYER)
                    {
                        ((SpriteLayer)layers[i]).Initialize(tileset, map[i]);
                    }
                    else
                    {
                        ((DetailLayer)layers[i]).Initialize(tileset, map[i]);

                        ((DetailLayer)layers[i]).XOffset = loadedLayerData[i].XOffset;
                        ((DetailLayer)layers[i]).YOffset = loadedLayerData[i].YOffset;
                        ((DetailLayer)layers[i]).RepeatX = loadedLayerData[i].RepeatX;
                        ((DetailLayer)layers[i]).RepeatY = loadedLayerData[i].RepeatY;
                        ((DetailLayer)layers[i]).XSpeed = Game.WINDOW_WIDTH * loadedLayerData[i].XSpeed;
                        ((DetailLayer)layers[i]).YSpeed = Game.WINDOW_HEIGHT * loadedLayerData[i].YSpeed;
                        ((DetailLayer)layers[i]).AutoXSpeed = loadedLayerData[i].AutoXSpeed;
                        ((DetailLayer)layers[i]).AutoYSpeed = loadedLayerData[i].AutoYSpeed;
                        ((DetailLayer)layers[i]).IsVisible = loadedLayerData[i].IsVisible;
                    }
                }
            }

            loadedLayerData = null;
        }

        public void Update(Player player, GameLoop gameLoop)
        {
            for (int i = 0; i < LAYER_COUNT; ++i)
            {
                this[i].Update(player.Camera, gameLoop.GameTime.DeltaTime, GameLoop.TIME_UNTIL_UPDATE);
            }
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
            return Cast<Layer>(layers.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
