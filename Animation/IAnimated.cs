using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Animation
{
    public interface IAnimated
    {
        public enum Direction { Left, Right };
        public AnimatedSprite Sprite { get; set; }
        public Direction CurrentDirection { get; set; }
    }
}
