﻿using _2DGame.Entities;
using _2DGame.Layers;
using _2DGame.Utility;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _2DGame
{
    public class Level : IDestroyable
    {
        public LayerList Layers { get; set; }
        public string Name { get; private set; }
        public string TrackFilename { get; private set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2i TileStartPosition { get; private set; }
        public GameEntityManager GameEntityManager { get; set; }

        public Level()
        {
            Layers = new LayerList();
            Name = "Unnamed";
            TrackFilename = string.Empty;
            Width = 0;
            Height = 0;
            TileStartPosition = new Vector2i(0, 0);
            GameEntityManager = new();
        }

        public void LoadData(string mapFilename, string entityDataFilename, string layerDataFilename)
        {
            Layers.Load(mapFilename, layerDataFilename);
            GameEntityManager.Load(entityDataFilename);
        }

        public void Initialize(string tilesetFilename, string trackFilename)
        {
            TrackFilename = trackFilename;
            SoundManager.SetCurrentTrack(TrackFilename);
            Layers.Initialize(tilesetFilename);
            Width = Layers[LayerList.PRIMARY_LAYER].Width;
            Height = Layers[LayerList.PRIMARY_LAYER].Height;
            GameEntityManager.Initialize();

            // Test
            TileStartPosition = new Vector2i(30, 50);
        }

        public void Update(Player player, GameLoop gameLoop)
        {
            for (int i = 0; i < LayerList.LAYER_COUNT; ++i)
            {
                Layers[i].Update(player.Camera);
            }
            GameEntityManager.Update(this, player, gameLoop);
        }

        public void Destroy()
        {
            Layers.Destroy();
            Name = string.Empty;
            TrackFilename = string.Empty;
        }
    }
}
