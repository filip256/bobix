using Android.Content;
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
    public class VerticalJiggle : IAnimation
    {
        private Sprite sprite;
        private int times = 3;
        private Vector2 initialPosition;
        private bool direction = false;

        public VerticalJiggle(Sprite sprite, int times = 3)
        {
            this.sprite = sprite;
            this.times = times;
            this.initialPosition = sprite.Position;
        }

        public bool Step()
        {
            if (times == 0)
            {
                sprite.Position = initialPosition;
                return false;
            }

            if (direction)
            {
                if (sprite.Position.Y < initialPosition.Y + 12)
                    sprite.Position = new Vector2(initialPosition.X, sprite.Position.Y + 4);
                else
                {
                    direction = false;
                    --times;
                }
            }
            else
            {
                if (sprite.Position.Y > initialPosition.Y - 12)
                    sprite.Position = new Vector2(initialPosition.X, sprite.Position.Y - 4);
                else
                    direction = true;
            }

            return true;
        }
    }
}
