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
    public class GameOverMenu
    {
        private ContentManager content;

        private Vector2 position;
        private SpriteFont hudFont;
        private Texture2D background;
        private Button replayButton;
        private Button backButton;

        private int score = 0;
        private bool isNewHighScore = true;

        public GameOverMenu(ContentManager content, Vector2 position)
        {
            this.content = content;
            this.position = position;
        }

        public void LoadContent()
        {
            hudFont = content.Load<SpriteFont>("hud_font");
            background = content.Load<Texture2D>("menu_texture");

            replayButton = 
                new Button(content.Load<Texture2D>("replay_texture"), position + new Vector2(508, 570), 8.0f);

            backButton = 
                new Button(content.Load<Texture2D>("back_texture"), position + new Vector2(100, 570), 8.0f);
        }

        public void Update()
        {
        }

        public bool ReplayButtonContains(Vector2 coords)
        {
            return replayButton.Contains(coords);
        }
        public bool BackButtonContains(Vector2 coords)
        {
            return backButton.Contains(coords);
        }

        public void SetScore(int score, bool isNewHighScore)
        {
            this.score = score;
            this.isNewHighScore = isNewHighScore;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, position, null, Color.White, 0.0f, Vector2.Zero, 6.0f, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(hudFont, "GAME OVER", position + new Vector2(180, 50), new Color(255,86, 34));

            if (score < 1_000_000)
            {
                spriteBatch.DrawString(hudFont, score + "mg", position +  new Vector2(30, 280), Color.Gold);
            }
            else
            {
                spriteBatch.DrawString(hudFont, Math.Round(score / 1000f, 0) + "g", position +  new Vector2(30, 280), Color.Gold);
            }

            if(isNewHighScore)
                spriteBatch.DrawString(hudFont, "NEW HIGHSCORE!!!", position + new Vector2(30, 430), Color.MediumPurple);



            replayButton.Draw(spriteBatch);
            backButton.Draw(spriteBatch);
        }
    }
}
