using _2DGame.LayerData;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace _2DGame.Layers
{
    public abstract class Layer : Drawable
    {
        protected float clock;

        public float XSpeed { get; set; }
        public float YSpeed { get; set; }
        public float AutoXSpeed { get; set; }
        public float AutoYSpeed { get; set; }
        public bool IsVisible { get; set; }

        [JsonIgnore]
        public float Width { get; protected set; }
        [JsonIgnore]
        public float Height { get; protected set; }
        [JsonIgnore]
        public int TileWidth { get; protected set; }
        [JsonIgnore]
        public int TileHeight { get; protected set; }
        [JsonIgnore]
        public TileData? TileIDs { get; set; }
        [JsonIgnore]
        public Tilemap LayerTilemap { get; set; }

        public Layer()
        {
            clock = 0f;
            XSpeed = 1f;
            YSpeed = 1f;
            AutoXSpeed = 0f;
            AutoYSpeed = 0f;
            Width = 0f;
            Height = 0f;
            TileWidth = 0;
            TileHeight = 0;
            IsVisible = true;
        }

        public abstract void Update(View view, float deltaTime, float timeUntilUpdate);
        public abstract void Draw(RenderTarget target, RenderStates states);
    }
}
