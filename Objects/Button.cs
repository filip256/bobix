using Android.Icu.Number;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Objects
{
    public class Button
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 size;
        private float scale;
        private Color color = Color.White;

        public Button(Texture2D texture, Vector2 position, float scale)
        {
            this.texture = texture;
            this.position = position;
            this.scale = scale;

            this.size = new Vector2(texture.Bounds.Width * scale, texture.Bounds.Height * scale);
        }

        public void Hover(bool value)
        {
            if (value) 
                color = Color.DarkOrange;
            else
                color = Color.White;
        }

        public bool Contains(Vector2 point)
        {
            int halfHeight = (int)(size.Y / 2);

            return (point.X >= position.X + halfHeight && point.Y >= position.Y &&
                    point.X <= position.X + halfHeight + size.X - size.Y && 
                    point.Y <= position.Y + size.Y) ||
                Formula.SquaredDistance(point.X, point.Y, position.X + halfHeight, position.Y + halfHeight) <= halfHeight * halfHeight ||
                Formula.SquaredDistance(point.X, point.Y, position.X + size.X - halfHeight, position.Y + halfHeight) <= halfHeight * halfHeight;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

    }
}
