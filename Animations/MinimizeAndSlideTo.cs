using Android.Icu.Number;
using Bobix.Definitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mobile.Helpers;
using Mobile.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Icu.Text.Transliterator;
using static Android.Webkit.WebStorage;

namespace Mobile.Animations
{
    public class MinimizeAndSlideTo : IDrawableAnimation
    {
        private Texture2D texture;
        private Rectangle? textureRect;
        private Vector2 position;
        private Color color;
        private float scale;
        private float rotation;
        private Vector2 destination;
        private Vector2 origin;

        private int speed;
        private float rotationSpeed;
        private Equation equation;
        private bool reverseSpin;

        public MinimizeAndSlideTo(Sprite sprite, Vector2 destination, int speed, bool reverseSpin)
        {
            this.texture = sprite.Texture;
            this.textureRect = sprite.TextureRect;
            this.color = sprite.Color;
            this.scale = sprite.Scale;
            this.rotation = sprite.Rotation;
            this.destination = destination;

            this.origin = sprite.TextureRect == null ?
                new Vector2(sprite.Texture.Bounds.Width / 2, sprite.Texture.Bounds.Height / 2) : 
                new Vector2(sprite.TextureRect.Value.Width / 2, sprite.TextureRect.Value.Height / 2);
            this.position = sprite.Position - origin;

            this.speed = speed;
            this.rotationSpeed = speed / 300.0f;
            this.equation = new Equation(position, destination);

            this.reverseSpin = reverseSpin;
        }

        public bool Step()
        {
            if (Math.Abs(position.Y - destination.Y) < speed)
                return false;

            position.Y += position.Y < destination.Y? speed : - speed;
            position.X = equation.GetX(position.Y);

            rotation += rotationSpeed * (reverseSpin ? -1 : 1);

            return true;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, textureRect, color, rotation, origin, scale, SpriteEffects.None, 0.0f);
        }
    }
}
