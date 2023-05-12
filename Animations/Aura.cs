using Android.Icu.Number;
using Bobix.Definitions;
using Bobix.Helpers;
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
    public class Aura : IDrawableAnimation
    {
        private static readonly float scaleStep = GameConstants.GemScale * 0.026f;
        private Sprite sprite;
        private bool scaleUp = true;

        public Aura(Sprite sprite, Color color)
        {
            this.sprite = new Sprite()
            {
                Texture = sprite.Texture,
                Color = color,
                Scale = sprite.Scale * 2.0f,
                Rotation = sprite.Rotation + RandomGenerator.GetFloat(),

                Origin = new Vector2(GameConstants.GemTextureWidth / 2.0f, GameConstants.GemTextureHeight / 2.0f),
                Position = sprite.Position + new Vector2(GameConstants.GemTextureWidth / 2.0f, GameConstants.GemTextureHeight / 2.0f) * sprite.Scale
            };
        }

        public Sprite GetSprite() { return this.sprite; }

        public void SetPosition(Vector2 position) 
        {
            this.sprite.Position = position + new Vector2(GameConstants.GemTextureWidth / 2, GameConstants.GemTextureHeight / 2) * sprite.Scale / 2;
        }

        public bool Step()
        {
            sprite.Rotation += Aura.scaleStep;

            if (scaleUp)
            {
                sprite.Scale += 0.05f;
                if(sprite.Scale > GameConstants.GemScale * 2.4)
                    scaleUp = false;
            }
            else
            {
                sprite.Scale -= Aura.scaleStep;
                if (sprite.Scale < GameConstants.GemScale * 1.3)
                    scaleUp = true;
            }

            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite.Texture, sprite.Position, null, sprite.Color, sprite.Rotation, sprite.Origin, sprite.Scale, SpriteEffects.None, 0.0f);
        }
    }
}
