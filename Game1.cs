using Android.Media.Metrics;
using Android.Views;
using Bobix.Definitions;
using Bobix.Helpers;
using Bobix.Objects;
using Java.Lang.Annotation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Mobile.Definitions;
using Mobile.Objects;
using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using static Android.Icu.Text.Transliterator;
using static Android.Renderscripts.Sampler;
using static Android.Webkit.WebStorage;

namespace Bobix
{
    //[SupportedOSPlatform("Android31.0")]
    public class Game1 : Game
    {
        private SpriteBatch spriteBatch;
        private GraphicsDeviceManager graphics;

        private GameState state;

        private MainMenu mainMenu;
        private GameHud gameHud;
        private GemBoard gemBoard;
        private GemFactory gemFactory;

        private Stopwatch gameClock;
        private Vector2 dragEndPosition = Vector2.Zero;
        private Vector2 dragStartPosition = Vector2.Zero;
        private bool isDragging = false;

        public Game1()
        {
            var wsize = new Point(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height);
            GameConstants.GemScale = 
                (float) (wsize.X - 100) / (GameConstants.GemTextureWidth * GameConstants.BoardWidth);
            var x = GameConstants.GemScale;
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            gemFactory = new GemFactory(Content);
            mainMenu = new MainMenu(Content, gemFactory, wsize);
            gameHud = new GameHud(Content);
            gameClock = new Stopwatch();
            gemBoard = new GemBoard(Content, gameClock, gemFactory, new Vector2(50, 100));
        }

        protected override void Initialize()
        {
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.FreeDrag | GestureType.DragComplete;

            state = GameState.InMainMenu;

            gameClock.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gemFactory.LoadContent();
            mainMenu.LoadContent();
            gameHud.LoadContent();
            gemBoard.LoadContent();
        }

        private void StartGame()
        {
            gemBoard.Fill();
            gameHud.Reset();
            state = GameState.InGame;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (state == GameState.InGame)
                this.UpdateGame();
            else
                this.UpdateMainMenu();

            base.Update(gameTime);
        }

        private void UpdateMainMenu()
        {
            while (TouchPanel.IsGestureAvailable)
            {
                var e = TouchPanel.ReadGesture();
                mainMenu.Update(e.Position);
                if (e.GestureType == GestureType.Tap)
                {
                    if (mainMenu.PlayButtonContains(e.Position))
                        this.StartGame();
                }
            }
        }

        private void UpdateGame()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                state = GameState.InMainMenu;
                return;
            }

            while (TouchPanel.IsGestureAvailable)
            {
                var e = TouchPanel.ReadGesture();
                if (e.GestureType == GestureType.Tap)
                {
                    gemBoard.OnTap(e.Position);
                }
                else if (e.GestureType == GestureType.FreeDrag)
                {
                    if (isDragging)
                    {
                        dragEndPosition = e.Position;
                    }
                    else
                    {
                        dragStartPosition = e.Position;
                        isDragging = true;
                    }
                }
                else if (e.GestureType == GestureType.DragComplete)
                {
                    isDragging = false;


                    var delta = dragEndPosition - dragStartPosition;
                    var distance = Math.Abs(delta.X * delta.X + delta.Y * delta.Y);
                    if (distance > GameConstants.GemWidthOnScreen * GameConstants.GemWidthOnScreen / 16.0 &&
                        distance < GameConstants.GemWidthOnScreen * GameConstants.GemWidthOnScreen * 9)
                    {
                        var angle = Math.Atan2(delta.Y, delta.X);
                        if (angle < 0.5235987756 && angle > -0.5235987756) // Right
                        {
                            gemBoard.OnSwipe(new Vector2(dragStartPosition.X - 75, dragStartPosition.Y), Direction.RIGHT);
                        }
                        else if (angle < -1.0471975512 && angle > -2.0943951024) // Up
                        {
                            gemBoard.OnSwipe(new Vector2(dragStartPosition.X, dragStartPosition.Y + 75), Direction.UP);
                        }
                        else if ((angle > 2.617993878 && angle < 3.141592654) || (angle < -2.617993878 && angle > -3.141592654)) // Left
                        {
                            gemBoard.OnSwipe(new Vector2(dragStartPosition.X + 75, dragStartPosition.Y), Direction.LEFT);
                        }
                        else if (angle > 1.0471975512 && angle < 2.0943951024) // Down
                        {
                            gemBoard.OnSwipe(new Vector2(dragStartPosition.X, dragStartPosition.Y - 75), Direction.DOWN);
                        }
                    }
                }
            }
            gemBoard.Update();

            gameHud.AddScore(gemBoard.GetAccumulatedScore());
            gameHud.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            if (state == GameState.InGame)
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(samplerState: SamplerState.AnisotropicWrap);
                gemBoard.Draw(spriteBatch);
                gameHud.Draw(spriteBatch);
                spriteBatch.End();
            }
            else
            {
                GraphicsDevice.Clear(Color.Purple);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                mainMenu.Draw(spriteBatch);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}