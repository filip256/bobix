using Android.Views.ContentCaptures;
using Bobix.Definitions;
using Bobix.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Bobix.Helpers
{
    //[SupportedOSPlatform("Android31.0")]
    public class GemFactory
    {
        private ContentManager content;
        private Texture2D gemsTexture;
        private Texture2D auraTexture;

        public GemFactory(ContentManager content)
        {
            this.content = content;
        }

        public void LoadContent()
        {
            gemsTexture = content.Load<Texture2D>("gems_texture");
            auraTexture = content.Load<Texture2D>("aura_texture");
        }

        public Gem Create(Vector2 position, Point positionOnBoard)
        {
            var type = RandomGenerator.GetGemType();
            return new Gem(
                type, 
                RandomGenerator.GetGemColor(),
                this.gemsTexture,
                this.GetTextureRect(type),
                GameConstants.SpecialTypes.Contains(type) ? this.auraTexture : null,
                position,
                positionOnBoard);
        }

        private Rectangle GetTextureRect(GemType type)
        {
            return new Rectangle(
                (int)type * GameConstants.GemTextureWidth,
                0,
                GameConstants.GemTextureWidth,
                GameConstants.GemTextureHeight);
        }
    }
}
