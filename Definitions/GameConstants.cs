using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bobix.Definitions
{
    public static class GameConstants
    {
        public static readonly string PlayerDataPath = "playerdata";
        public static readonly int GemTextureWidth = 64;
        public static readonly int GemTextureHeight = 64;
        public static float GemScale = 1.9f;

        public static float GemWidthOnScreen { get { return GemTextureWidth * GemScale; } }
        public static float GemHeightOnScreen { get { return GemTextureHeight * GemScale; } }

        public static readonly int BoardWidth = 8;
        public static readonly int BoardHeight = 12; // only the shown half

        public static readonly List<GemType> SpecialTypes = new List<GemType>() {
            GemType.Grenade,
            GemType.Heisenberg,
            GemType.Formula1,
            GemType.Diamond,
        };
    }
}
