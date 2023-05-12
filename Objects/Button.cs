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
            Rectangle r = new Rectangle((int)(position.X + halfHeight), (int)position.Y, (int)(size.X - size.Y), (int)size.Y);
            Point c1 = new Point((int)(position.X + halfHeight), (int)(position.Y + halfHeight));
            Point c2 = new Point((int)(position.X + size.X - halfHeight), (int)(position.Y + halfHeight));

            return r.Contains(point) ||
                Formula.SquaredDistance(point.X, point.Y, c1.X, c1.Y) <= halfHeight * halfHeight ||
                Formula.SquaredDistance(point.X, point.Y, c2.X, c2.Y) <= halfHeight * halfHeight;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

    }
}
