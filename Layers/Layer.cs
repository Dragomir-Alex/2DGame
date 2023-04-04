﻿using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Layers
{
    public abstract class Layer : Drawable
    {
        public float XSpeed { get; set; }
        public float YSpeed { get; set; }
        public float AutoXSpeed { get; set; }
        public float AutoYSpeed { get; set; }

        public Layer()
        {
            XSpeed = 1f;
            YSpeed = 1f;
            AutoXSpeed = 0f;
            AutoYSpeed = 0f;
        }

        public abstract void Update(View view);
        public abstract void Draw(RenderTarget target, RenderStates states);

        //public abstract void Draw(GameLoop gameLoop);
    }
}
