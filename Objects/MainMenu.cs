using System.Diagnostics;
using Bobix.Definitions;
using Bobix.Helpers;
using Bobix.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mobile.Animations;
using Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Java.Lang.Annotation;
using Android.Animation;

namespace Mobile.Objects
{
    public class MainMenu
    {
        private ContentManager content;
        private Texture2D background;
        private Vector2 backgroundScale;
        private Sprite title;
        private Button playButton;
        private Button exitButton;
        private Point windowSize;

        private GemFactory gemFactory;
        private List<Gem> gems;
        private List<MinimizeAndSlideTo> animations;

        private TitleDance titleAnimation;

        private Texture2D gitLogo;
        private Vector2 gitLogoPosition;

        private readonly Stopwatch gameClock;
        private long initializationTimeDelta;

        public MainMenu(ContentManager content, Stopwatch gameClock, GemFactory gemFactory, Point windowSize)
        {
            this.content = content;
            this.gemFactory = gemFactory;
            this.windowSize = windowSize;
            this.gameClock = gameClock;

            gems = new List<Gem>();
            animations = new List<MinimizeAndSlideTo>();
        }

        public void LoadContent()
        {
            background = content.Load <Texture2D>("background");
            backgroundScale = new Vector2(windowSize.X / background.Width, windowSize.Y / background.Height);

            title = new Sprite()
            {
                Texture = content.Load<Texture2D>("title_texture"),
                Scale = 12.0f
            };
            title.Origin = new Vector2(title.Texture.Width / 2.0f, title.Texture.Height / 2.0f);
            title.Position = new Vector2(windowSize.X / 2.0f, 300.0f);

            titleAnimation = new TitleDance(title, title.Scale);

            var texture = content.Load<Texture2D>("playbutton_texture");
            playButton = new Button(
                texture,
                new Vector2((windowSize.X - texture.Width * 10.0f) / 2.0f, 700),
                10.0f
            );

            texture = content.Load<Texture2D>("exitbutton_texture");
            exitButton = new Button(
                texture,
                new Vector2((windowSize.X - texture.Width * 10.0f) / 2.0f, 920),
                10.0f
            );

            gitLogo = content.Load<Texture2D>("github_logo");
            gitLogoPosition = new Vector2(windowSize.X - gitLogo.Width * GameConstants.GemScale - 30,
                                         windowSize.Y - gitLogo.Height * GameConstants.GemScale - 230);


            for (int i = 0; i < 30; ++i)
            {
                animations.Add(this.CreateAnimation(i, -1000.0f, 12, 30));
            }

            initializationTimeDelta = gameClock.ElapsedMilliseconds;        
        }

        private MinimizeAndSlideTo CreateAnimation(int i, float y, int baseSpeed, int maxRandomSpeed)
        {
            var gem = gemFactory.Create(
                    new Vector2(
                        i / 3.0f * GameConstants.GemWidthOnScreen,
                        y - GameConstants.GemHeightOnScreen
                        ),
                    new Point(0, 0)
                    );

            return new MinimizeAndSlideTo(
                gem.GetSprite(),
                gem.GetSprite().Position + new Vector2(0.0f, windowSize.Y + GameConstants.GemHeightOnScreen - y + 50),
                baseSpeed + RandomGenerator.GetInt(maxRandomSpeed),
                RandomGenerator.GetInt(2) == 0);
        }

        public void Update()
        {
            if (gameClock.ElapsedMilliseconds - initializationTimeDelta < 400)
                return;

            if (gameClock.ElapsedMilliseconds - initializationTimeDelta > 1300)
                titleAnimation.Step();
            for(int i = 0; i < animations.Count; ++i)
            {
                if (animations[i].Step() == false)
                {
                    animations[i] = this.CreateAnimation(i, -10 - GameConstants.GemHeightOnScreen, 12, 12);
                }
            }
        }

        public void Tap(Vector2 position)
        {
            var idx = (int)(position.X * 3.0f / GameConstants.GemWidthOnScreen);
            if(idx >= 0 && idx < animations.Count())
            {
                if(Math.Abs(position.Y + 70 - animations[idx].GetPosition().Y) < GameConstants.GemHeightOnScreen)
                    animations[idx] = this.CreateAnimation(idx, -10 - GameConstants.GemHeightOnScreen, 12, 12);
                if(idx >= 1 &&
                    Math.Abs(position.Y + 70 - animations[idx - 1].GetPosition().Y) < GameConstants.GemHeightOnScreen)
                    animations[idx - 1] = this.CreateAnimation(idx - 1, -10 - GameConstants.GemHeightOnScreen, 12, 12);
                if (idx < animations.Count() - 1 &&
                    Math.Abs(position.Y + 70 - animations[idx + 1].GetPosition().Y) < GameConstants.GemHeightOnScreen)
                    animations[idx + 1] = this.CreateAnimation(idx + 1, -10 - GameConstants.GemHeightOnScreen, 12, 12);
            }
        }

        public bool GithubButtonContains(Vector2 coords)
        {
            var x = Formula.SquaredDistance(coords.X, coords.Y,
                gitLogoPosition.X + gitLogo.Width * GameConstants.GemScale  / 2.0f, gitLogoPosition.Y + gitLogo.Height * GameConstants.GemScale  / 2.0f);
            return Formula.SquaredDistance(coords.X, coords.Y, 
                gitLogoPosition.X + gitLogo.Width * GameConstants.GemScale / 2.0f, gitLogoPosition.Y + gitLogo.Height * GameConstants.GemScale / 2.0f) <= 3800;
        }

        public bool PlayButtonContains(Vector2 coords)
        {
            return playButton.Contains(coords);
        }

        public bool ExitButtonContains(Vector2 coords)
        {
            return exitButton.Contains(coords);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, backgroundScale, SpriteEffects.None, 0.0f);
            
            for (int i = 0; i < animations.Count; ++i)
                animations[i].Draw(spriteBatch);
            title.Draw(spriteBatch);
            playButton.Draw(spriteBatch);
            exitButton.Draw(spriteBatch);
            spriteBatch.Draw(gitLogo, gitLogoPosition, null, Color.White, 0.0f, Vector2.Zero, GameConstants.GemScale, SpriteEffects.None, 0.0f);

        }
    }
}
