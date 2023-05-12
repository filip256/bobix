using Bobix.Definitions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bobix.Helpers
{
    public static class RandomGenerator
    {
        private static readonly Random rng = new Random((int)DateTime.Now.Ticks);

        public static int GetInt(int maxValue)
        {
            return rng.Next(maxValue);
        }

        public static float GetFloat()
        {
            return (float)rng.NextDouble();
        }

        public static GemType GetGemType()
        {
            var values = Enum.GetValues(typeof(GemType));
            var type = (GemType)values.GetValue(rng.Next(Enum.GetValues(typeof(GemType)).Length));

            if (GetInt(2) == 0 && GameConstants.SpecialTypes.Contains(type))
                return (GemType)values.GetValue(rng.Next(Enum.GetValues(typeof(GemType)).Length));

            return type;
        }

        public static Color GetGemColor()
        {
            switch(rng.Next(8))
            {
                case 0: return GemColor.Red;
                case 1: return GemColor.Green;
                case 2: return GemColor.Blue;
                case 3: return GemColor.Yellow;
                case 4: return GemColor.Orange;
                case 5: return GemColor.Purple;
                default: return GemColor.White;
            }
        }
    }
}
