using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mobile.Helpers;
using Mobile.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Animations
{
    public class Blink : IAnimation
    {
        private Sprite sprite;
        private int times = 3;
        private Color initialColor;
        private bool direction;

        public Blink(Sprite sprite)
        {
            this.sprite = sprite;
            this.initialColor = sprite.Color;
        }

        ~Blink()
        {
            sprite.Color = initialColor;
        }

        public bool Step()
        {
            if (times == 0)
            {
                sprite.Color = initialColor;
                return false;
            }

            if (direction)
            {
                if (sprite.Color.A > 100)
                    sprite.Color = new Color(sprite.Color.R, sprite.Color.G, sprite.Color.B - 10, sprite.Color.A - 10);
                else
                    direction = false;
            }
            else
            {
                if (sprite.Color.A < 251)
                    sprite.Color = new Color(sprite.Color.R, sprite.Color.G, sprite.Color.B + 10, sprite.Color.A + 10);
                else
                    direction = true;
            }

            --times;
            return true;
        }
    }
}
