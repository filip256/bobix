using Mobile.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Animations
{
    public class TitleDance : IAnimation
    {
        private Sprite sprite;
        private float baseScale;
        private bool scaleDirection = true;
        private bool rotationDirection = true;

        public TitleDance(Sprite sprite, float baseScale)
        {
            this.sprite = sprite;
            this.baseScale = baseScale;
        }

        public bool Step()
        {
            if (rotationDirection)
            {
                if (sprite.Rotation < 0.1f)
                {
                    sprite.Rotation += 0.0075f;
                }
                else
                    rotationDirection = false;
            }
            else
            {
                if (sprite.Rotation > -0.1f)
                {
                    sprite.Rotation -= 0.0075f;
                }
                else
                    rotationDirection = true;
            }

            if (scaleDirection && (sprite.Rotation >= 0.055f || sprite.Rotation <= -0.055f))
            {
                if (sprite.Scale < baseScale + 0.72f)
                {
                    sprite.Scale += 0.18f;
                }
                else
                    scaleDirection = false;
            }
            else
            {
                if (sprite.Scale > baseScale)
                {
                    sprite.Scale -= 0.054f;
                }
                else
                    scaleDirection = true;
            }

            return true;
        }
    }
}
