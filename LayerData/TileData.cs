using System.IO.Compression;
using System.Xml;

namespace _2DGame.LayerData
{
    /// <summary>
    /// Stores the data for a map full of tiles in the form of tile IDs. Can be treated as a two
    /// dimensional array of <see cref="int"/>.
    /// </summary>
    public class TileData
    {
        // i = x + width * y;
        // x = i % width;
        // y = i / width;
#pragma warning disable CS1591
        public const int EMPTY_TILE = -1;
#pragma warning restore CS1591

        /// <summary>
        /// Get: returns the tile at <paramref name="position"/>.<br></br>
        /// Set: replaces the tile at <paramref name="position"/>.
        /// </summary>
        public int this[(int, int) position]
        {
            get
            {
                var (x, y) = position;
                return PositionIsInvalid(x, y) ? -1 : tiles[x, y];
            }
            set
            {
                var (x, y) = position;
                if (PositionIsInvalid(x, y))
                    return;

                tiles[x, y] = value;
            }
        }
        /// <summary>
        /// Replaces the tiles in a square of <paramref name="size"/> starting at
        /// <paramref name="position"/> from its top-left corner.
        /// </summary>
        public int this[(int, int) position, (int, int) size]
        {
            set
            {
                var (width, height) = size;
                var (x, y) = position;
                var xStep = width < 0 ? -1 : 1;
                var yStep = height < 0 ? -1 : 1;
                for (int i = x; i != x + width; i += xStep)
                    for (int j = y; j != y + height; j += yStep)
                        this[(i, j)] = value;
            }
        }

        /// <summary>
        /// Creates an empty <see cref="TileData"/> with a <paramref name="size"/>. All tiles are set
        /// to -1.
        /// </summary>
        public TileData((int, int) size)
        {
            var (width, height) = size;
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Values cannot be < 1.", nameof(size));

            tiles = new int[width, height];
            this[(0, 0), size] = -1;
        }
        /// <summary>
        /// Creates the <see cref="TileData"/> from a collection of <paramref name="tiles"/>.
        /// </summary>
        public TileData(int[,] tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            if (tiles.Length == 0)
                throw new ArgumentException("Total tiles cannot be < 1.", nameof(tiles));

            this.tiles = tiles;
        }
        /// <summary>
        /// Creates the <see cref="TileData"/> from a <paramref name="layerName"/> in a
        /// <see langword="Tiled Map"/> export file at <paramref name="tmxPath"/>.
        /// </summary>
        public TileData(string tmxPath, string layerName)
        {
            tiles = new int[0, 0];

            if (tmxPath == null)
                throw new ArgumentNullException(nameof(tmxPath));

            if (File.Exists(tmxPath) == false)
                throw new ArgumentException($"No tmx file was found at '{tmxPath}'.");

            var xml = new XmlDocument();
            xml.Load(tmxPath);

            var layers = xml.GetElementsByTagName("layer");
            var layer = default(XmlElement);
            var data = default(XmlNode);
            var layerFound = false;

            foreach (var element in layers)
            {
                layer = (XmlElement)element;
                data = layer.FirstChild;

                if (data == null || data.Attributes == null)
                    continue;

                var name = layer.Attributes["name"]?.Value;
                if (name == layerName)
                {
                    layerFound = true;
                    break;
                }
            }

            if (layerFound == false)
                throw new Exception($"File at '{tmxPath}' does not contain the layer '{layerName}'.");

            if (layer == null || data == null)
            {
                Error();
                return;
            }

            var dataStr = data.InnerText.Trim();
            var attributes = data.Attributes;
            var encoding = attributes?["encoding"]?.Value;
            var compression = attributes?["compression"]?.Value;
            _ = int.TryParse(layer?.Attributes?["width"]?.InnerText, out var mapWidth);
            _ = int.TryParse(layer?.Attributes?["height"]?.InnerText, out var mapHeight);

            tiles = new int[mapWidth, mapHeight];

            if (encoding == "csv")
                LoadFromCSV(dataStr);
            else if (encoding == "base64")
            {
                if (compression == null)
                    LoadFromBase64Uncompressed(dataStr);
                else if (compression == "gzip")
                    LoadFromBase64<GZipStream>(dataStr);
                else if (compression == "zlib")
                    LoadFromBase64<ZLibStream>(dataStr);
                else
                    throw new Exception($"Tile Layer Format encoding 'Base64' " +
                        $"with compression '{compression}' is not supported.");
            }
            else
                throw new Exception($"Tile Layer Format encoding" +
                    $"'{encoding}' is not supported.");

            void Error() => throw new Exception(
                $"Could not parse file at '{tmxPath}', layer '{layerName}'.");
        }

        /// <summary>
        /// Returns a new <see cref="TileData"/> created from a collection of <paramref name="tiles"/>.
        /// </summary>
        public static implicit operator TileData(int[,] tiles) => new(tiles);
        /// <summary>
        /// Returns the tile data of a <paramref name="tileData"/> - <see cref="tiles"/>.
        /// </summary>
        public static implicit operator int[,](TileData tileData) => tileData.tiles;

        /// <summary>
        /// Calculates the index that corresponds to a <paramref name="position"/> and returns it.
        /// </summary>
        public int PositionToIndex((int, int) position)
        {
            var (x, y) = position;
            return x * tiles.GetLength(0) + y;
        }
        /// <summary>
        /// Calculates the position that corresponds to an <paramref name="index"/> and returns it.
        /// </summary>
        public (int, int) IndexToPosition(int index)
        {
            var w = tiles.GetLength(0);
            return (index % w, index / w);
        }

        #region Backend
        private readonly int[,] tiles;

        public int Height() { return tiles.GetLength(1); }
        public int Width() { return tiles.GetLength(0); }
        public int GetTile((int, int) position)
        {
            var (x, y) = position;
            return tiles[x, y];
        }

        private (int, int) IndexToCoords(int index)
        {
            var w = tiles.GetLength(0);
            var h = tiles.GetLength(1);
            index = index < 0 ? 0 : index;
            index = index > w * h - 1 ? w * h - 1 : index;

            return (index % w, index / w);
        }
        private bool PositionIsInvalid(int x, int y)
        {
            return x < 0 || y < 0 || x >= tiles.GetLength(0) || y >= tiles.GetLength(1);
        }
        private void LoadFromCSV(string dataStr)
        {
            var values = dataStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < values.Length; i++)
            {
                var value = int.Parse(values[i].Trim());
                var (x, y) = IndexToCoords(i);
                tiles[x, y] = value - 1;
            }
        }
        private void LoadFromBase64Uncompressed(string dataStr)
        {
            var bytes = Convert.FromBase64String(dataStr);
            LoadFromByteArray(bytes);
        }
        private void LoadFromBase64<T>(string dataStr) where T : Stream
        {
            var buffer = Convert.FromBase64String(dataStr);
            using var msi = new MemoryStream(buffer);
            using var mso = new MemoryStream();

            using var compStream = Activator.CreateInstance(
                typeof(T), msi, CompressionMode.Decompress) as T;

            if (compStream == null)
                return;

            CopyTo(compStream, mso);
            var bytes = mso.ToArray();
            LoadFromByteArray(bytes);
        }
        private void LoadFromByteArray(byte[] bytes)
        {
            var size = bytes.Length / sizeof(int);
            for (var i = 0; i < size; i++)
            {
                var (x, y) = IndexToCoords(i);
                var value = BitConverter.ToInt32(bytes, i * sizeof(int));
                tiles[x, y] = value - 1;
            }
        }

        private static void CopyTo(Stream src, Stream dest)
        {
            var bytes = new byte[4096];

            int i;
            while ((i = src.Read(bytes, 0, bytes.Length)) != 0)
                dest.Write(bytes, 0, i);
        }
        #endregion
    }
}