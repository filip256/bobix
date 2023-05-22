using Bobix.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Hardware.Camera;

namespace Mobile.Helpers
{
    public static class GemEffectsHelper
    {
        public static List<Point> GetGrenadeArea(int x, int y, int width, int height, int radius)
        {
            List<Point> area = new List<Point>();
            int startX = Math.Max(x - radius, 0);
            int startY = Math.Max(y - radius, 0);
            int endX = Math.Min(x + radius + 1, width);
            int endY = Math.Min(y + radius + 1, height);

            for (int i = startY; i < endY; ++i)
                for (int j = startX; j < endX; ++j)
                    if (i != y && j != x)
                    {
                        var a = Formula.SquaredDistance(j, i, x, y);
                        if(a <= radius * radius)
                            area.Add(new Point(j, i));
                    }

            return area;
        }

        public static List<Point> GetCrossArea(int x, int y, int width, int height)
        {
            List<Point> area = new List<Point>();

            int i = x - 1, j = y - 1;
            while (i >= 0 && j >= 0)
            {
                area.Add(new Point(i, j));
                i--; j--;
            }

            i = x + 1; j = y - 1;
            while (i < width && j >= 0)
            {
                area.Add(new Point(i, j));
                i++; j--;
            }

            i = x - 1; j = y + 1;
            while (i >= 0 && j < height)
            {
                area.Add(new Point(i, j));
                i--; j++;
            }

            i = x + 1; j = y + 1;
            while (i < width && j < height)
            {
                area.Add(new Point(i, j));
                i++; j++;
            }

            return area;
        }

        public static List<Point> GetVerticalArea3(int x, int width, int height)
        {
            List<Point> area = new List<Point>();

            for (int i = 0; i < height; ++i)
            {
                area.Add(new Point(x, i));
                //if (x > 0)
                //    area.Add(new Point(x - 1, i));
                //if (x < width - 1)
                //    area.Add(new Point(x + 1, i));
            }
            return area;
        }

        public static List<Point> GetHorizontalArea3(int y, int width, int height)
        {
            List<Point> area = new List<Point>();

            for (int i = 0; i < width; ++i)
            {
                area.Add(new Point(i, y));
                if (y > 0)
                    area.Add(new Point(i, y - 1));
                if (y < height - 1)
                    area.Add(new Point(i, y + 1));
            }
            return area;
        }

        public static List<Point> GetRandomArea(int count, int width, int height)
        {
            List<Point> area = new List<Point>();
            for (int i = 0; i < count; ++i)
                area.Add(new Point(
                    RandomGenerator.GetInt(width),
                    RandomGenerator.GetInt(height)
                    ));

            return area;
        }
    }
}
