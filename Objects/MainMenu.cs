using Bobix.Definitions;
using Bobix.Helpers;
using Bobix.Objects;
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
    public class MainMenu
    {
        private ContentManager content;
        private Sprite title;
        private Button playButton;
        private Point windowSize;

        private GemFactory gemFactory;
        private List<Gem> gems;
        private List<MinimizeAndSlideTo> animations;

        public MainMenu(ContentManager content, GemFactory gemFactory, Point windowSize)
        {
            this.content = content;
            this.gemFactory = gemFactory;
            this.windowSize = windowSize;

            gems = new List<Gem>();
            animations = new List<MinimizeAndSlideTo>();
        }

        public void LoadContent()
        {
            title = new Sprite()
            {
                Texture = content.Load<Texture2D>("title_texture"),
                Scale = 12.0f
            };
            title.Position = new Vector2((windowSize.X - title.Texture.Width * title.Scale) / 2.0f, 50.0f);

            playButton = new Button(
                content.Load<Texture2D>("playbutton_texture"),
                new Vector2(60, 560),
                12.0f
            );

            for (int i = 0; i < 10; ++i)
            {
                gems.Add(gemFactory.Create(
                    new Vector2(i * 100, 50),
                    //new Vector2(
                    //    i * (GameConstants.GemTextureWidth * GameConstants.GemScale),
                    //    0 - GameConstants.GemTextureHeight * GameConstants.GemScale
                    //    ),
                    new Point(0, 0)
                    )
                );
                animations.Add(new MinimizeAndSlideTo(
                    gems.Last().GetSprite(),
                    gems.Last().GetSprite().Position + new Vector2(0.0f, 500),
                    5)
                );
            }
        }

        public void Update(Vector2 coords)
        {
            foreach (var a in animations)
                 a.Step();
            playButton.Hover(playButton.Contains(coords));
        }


        public bool PlayButtonContains(Vector2 coords)
        {
            return playButton.Contains(coords);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(var a in animations)
                a.Draw(spriteBatch);

            title.Draw(spriteBatch);
            playButton.Draw(spriteBatch);
        }
    }
}
