using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Helpers
{
    public static class Formula
    {
        public static float SquaredDistance(float xA, float yA, float xB, float yB)
        {
            return Math.Abs((xA - xB) * (xA - xB) + (yA - yB) * (yA - yB));
        }

        public static float SquaredDistance(Point A, Point B)
        {
            return Math.Abs((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y));
        }

        public static bool Equals(float x, float y, float precision = 0.001f)
        {
            var diff = Math.Abs(x - y);
            return diff <= precision ||
                   diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * precision;
        }
    }
}
