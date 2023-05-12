using Android.Gestures;
using Android.Icu.Number;
using Bobix.Definitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Icu.Text.Transliterator;

namespace Mobile.Objects
{
    public class Sprite
    {
        public Texture2D Texture { get; set; }
        public Rectangle? TextureRect { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; } = Vector2.Zero;

        public float Rotation { get; set; } = 0.0f;
        public float Scale { get; set; } = 1.0f;

        public Color Color { get; set; } = Color.White;
        public SpriteEffects Effect { get; set; } = SpriteEffects.None;
        public float LayerDepth { get; set; } = 0.0f;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, TextureRect, Color, Rotation, Origin, Scale, Effect, LayerDepth);
        }
    }
}
