using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TransformableHitbox2D;

namespace _2DGame.Entities
{
    public class JsonEntity
    {
        public uint ID { get; set; }
        public TileCoordinates TileCoordinates { get; set; }
    }
}
