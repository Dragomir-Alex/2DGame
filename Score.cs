using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame
{
    public static class Score
    {
        public const int STRING_SIZE = 8;
        private static int value = 0;

        public static void Reset() { value = 0; }

        public static void Add(int score) { value += score; }

        public static string GetString()
        {
            StringBuilder stringBuilder = new StringBuilder("");
            stringBuilder.Append(value.ToString());

            while (stringBuilder.Length >= STRING_SIZE)
            {
                stringBuilder.Remove(0, 1);
            }

            while (stringBuilder.Length < STRING_SIZE)
            {
                stringBuilder.Insert(0, '0');
            }

            return stringBuilder.ToString();
        }
    }
}
