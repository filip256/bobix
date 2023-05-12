using Microsoft.Xna.Framework;
using Mobile.Helpers;
using Mobile.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Animations
{
    public class SlideTo : IAnimation
    {
        private Sprite sprite;
        private Vector2 destination;

        public SlideTo(Sprite sprite, Vector2 destination)
        {
            this.sprite = sprite;
            this.destination = destination;
        }

        public bool Step()
        {
            bool modified = false;
            if (!Formula.Equals(sprite.Position.X, destination.X))
            {
                sprite.Position += new Vector2((destination.X - sprite.Position.X) / 10.0f, 0);
                modified = true;
            }

            if (!Formula.Equals(sprite.Position.Y, destination.Y))
            {
                sprite.Position += new Vector2(0, (destination.Y - sprite.Position.Y) / 10.0f);
                modified = true;
            }

            return modified;
        }
    }
}
