using Android.Hardware.Lights;
using Bobix.Definitions;
using Bobix.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mobile.Animations;
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
        private PositionSlideTo highScoreSlideTo = null;
        private Vector2 highScorePosition;

        private int newScore = 0;
        private int score = 0;
        private int highScore = 0;
        private bool highScoreModified = false;
        private Color highScoreColor = Color.MediumOrchid;

        public GameHud(ContentManager content)
        {
            this.content = content;
            this.highScorePosition = new Vector2(10, 120);
        }

        public void LoadContent()
        {
            hudFont = content.Load<SpriteFont>("hud_font");
        }

        public void Reset()
        {
            newScore = 0;
            score = 0;
            highScoreModified = false;
            highScorePosition = new Vector2(10, 120);
            highScoreSlideTo = null;
            highScoreColor = Color.MediumOrchid;
        }

        public void SetHighscore(int highScore)
        {
            this.highScore = highScore;
        }

        public void AddScore(int score) { newScore += score; }

        public int GetScore() { return score; }

        public void Update()
        {
            int portion = newScore / 16;

            score += portion;
            newScore -= portion;

            if (highScore < score)
            {
                highScore = score;
                highScoreModified = true;

                if (highScoreModified == true && highScoreSlideTo == null && highScore > 0)
                {
                    highScoreSlideTo = new PositionSlideTo(
                        highScorePosition,
                        highScorePosition + new Vector2(0.0f, 1400.0f),
                        10);
                }
            }

            if(highScoreSlideTo != null)
            {
                var newPos = highScoreSlideTo.Step();
                if (newPos != Vector2.Zero)
                {
                    highScorePosition = newPos;
                    highScoreColor *= 0.97f;
                }
                else
                    highScoreSlideTo = null;
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (score < 1_000_000)
            {
                spriteBatch.DrawString(hudFont, score + "mg", new Vector2(10, 10), Color.PeachPuff);
            }
            else
            {
                spriteBatch.DrawString(hudFont, Math.Round(score / 1000f, 0) + "g", new Vector2(10, 10), Color.Gold);
            }

            if ((highScoreModified && highScoreSlideTo == null) || highScore == 0)
                return;

            if (highScore < 1_000_000)
            {
                spriteBatch.DrawString(hudFont, highScore + "mg", highScorePosition, highScoreColor);
            }
            else
            {
                spriteBatch.DrawString(hudFont, Math.Round(highScore / 1000f, 0) + "g", highScorePosition, highScoreColor);
            }
        }
    }
}
