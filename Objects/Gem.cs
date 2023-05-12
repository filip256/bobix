using Android.Graphics.Drawables;
using Bobix.Definitions;
using Bobix.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mobile.Animations;
using Mobile.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using static Android.Icu.Text.Transliterator;
using static Android.Webkit.WebStorage;

namespace Bobix.Objects
{
    //[SupportedOSPlatform("Android31.0")]
    public class Gem
    {
        private Sprite sprite;
        private IAnimation? slideTo, auraSlideTo, jiggle, blink;
        private Aura? aura;

        private Point positionOnBoard;

        private GemType type;

        private int scoreValue;

        public Gem(GemType type, Color color, Texture2D texture, Rectangle textureRect, Texture2D? auraTexture, Vector2 position, Point positionOnBoard)
        {
            this.sprite = new Sprite();

            this.type = type;
            this.sprite.Color = color;

            this.sprite.Texture = texture;
            this.sprite.TextureRect = textureRect;

            this.positionOnBoard = positionOnBoard;
            this.sprite.Position = position;
            this.sprite.Scale = GameConstants.GemScale;

            this.scoreValue = 500;

            if (auraTexture != null)
            {
                this.aura = new Aura(
                    new Sprite()
                    {
                        Texture = auraTexture,
                        Color = color,
                        Position = position,
                        Scale = GameConstants.GemScale,
                        Rotation = 0.0f
                    },
                    type == GemType.Punisher && color == GemColor.Blue ? Color.DeepPink : Color.Goldenrod);
            }
        }

        public void StepAnimations()
        {
            if (slideTo != null)
            {
                if (slideTo.Step() == false)
                    slideTo = null;
            }
            if (jiggle != null)
            {
                if (jiggle.Step() == false)
                    jiggle = null;
            }
            if (blink != null)
            {
                if (blink.Step() == false)
                    blink = null;
            }
            if (aura != null)
            {
                aura.Step();
                if(auraSlideTo != null)
                {
                    if (auraSlideTo.Step() == false)
                        auraSlideTo = null;
                }
            }
        }

        public bool HasColor(Gem other)
        {
            return this.sprite.Color == other.sprite.Color;
        }
        public Sprite GetSprite() { return this.sprite; }
        public GemType GetGemType() { return this.type; }

        public void SetPosition(Vector2 position, Point positionOnBoard, bool isStartup = false)
        {
            if (isStartup == false)
            {
                jiggle = null;
                slideTo = new SlideTo(sprite, position);
                if (aura != null)
                    auraSlideTo = new SlideTo(aura.GetSprite(), position + (aura.GetSprite().Position - sprite.Position));
            }
            else
            {
                this.sprite.Position = position;
                if (aura != null)
                    this.aura.SetPosition(position);
            }
            this.positionOnBoard = positionOnBoard;
        }
        public Vector2 GetPosition() { return this.sprite.Position; }
        public Point GetPositionOnBoard() { return this.positionOnBoard; }

        public int GetScoreValue() { return this.scoreValue; }

        public void StartHorizontalJiggle(int times)
        {
            if(jiggle == null)
                jiggle = new HorizontalJiggle(sprite, times);
        }
        public void StartVerticalJiggle(int times)
        {
            if (jiggle == null)
                jiggle = new VerticalJiggle(sprite, times);
        }
        public void StartBlink()
        {
            blink = new Blink(sprite);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (aura != null)
                aura.Draw(spriteBatch);
            sprite.Draw(spriteBatch);
        }
    }
}
