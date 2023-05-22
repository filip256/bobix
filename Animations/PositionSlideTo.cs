using Mobile.Helpers;
using Mobile.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Animations
{
    public class PositionSlideTo
    {
        private Vector2 position;
        private Vector2 destination;
        private int speed;

        public PositionSlideTo(Vector2 position, Vector2 destination, int speed)
        {
            this.position = position;
            this.destination = destination;
            this.speed = speed;
        }

        public Vector2 Step()
        {
            if (!Formula.Equals(position.X, destination.X))
            {
                position.X = position.X + speed;
                return position;
            }

            if (!Formula.Equals(position.Y, destination.Y))
            {
                position.Y = position.Y + speed;
                return position;
            }

            speed += speed / 5 + 50;

            return Vector2.Zero;
        }
    }
}
