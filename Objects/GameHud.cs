using Android.Hardware.Lights;
using Bobix.Definitions;
using Bobix.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Objects
{
    public class GameHud
    {
        private ContentManager content;
        private SpriteFont hudFont;

        private int newScore = 0;
        private int score = 0;

        public GameHud(ContentManager content)
        {
            this.content = content;
        }

        public void LoadContent()
        {
            hudFont = content.Load<SpriteFont>("hud_font");
        }

        public void Reset()
        {
            newScore = 0;
            score = 0;
        }

        public void AddScore(int score) { newScore += score; }

        public void Update()
        {
            int portion = newScore / 16;

            score += portion;
            newScore -= portion;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(hudFont, "Score: ", new Vector2(10, 10), Color.Gray);

            if (score < 1_000_000)
            {
                spriteBatch.DrawString(hudFont, score + "mg", new Vector2(300, 10), Color.Yellow);
            }
            else
            {
                spriteBatch.DrawString(hudFont, Math.Round(score / 1000f, 2) + "g", new Vector2(300, 10), Color.Purple);
            }
        }
    }
}
