using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Helpers
{
    public class Equation
    {
        private bool isVertical = false;
        private float a, c;

        public Equation(Vector2 v1, Vector2 v2)
        {
            if (v1.X == v2.X)
            {
                isVertical = true;
                c = v1.X;
            }
            else
            {
                a = (v2.Y - v1.Y) / (v2.X - v1.X);
                c = v1.Y - a * v1.X;
            }
        }

        public bool IsVertical() { return isVertical; }
        public float GetY(float x) { return isVertical? float.PositiveInfinity : a * x + c; }
        public float GetX(float y) { return isVertical? c : (y - c) / a; }
    }
}
