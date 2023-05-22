using Android.App;
using Android.Content;
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
using System.IO.IsolatedStorage;
using System.IO;

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
        private GameOverMenu gameOverMenu;
        private GemFactory gemFactory;

        private Stopwatch gameClock;
        private Vector2 dragEndPosition = Vector2.Zero;
        private Vector2 dragStartPosition = Vector2.Zero;
        private bool isDragging = false;

        private int highScore;

        public Game1()
        {
            var wsize = new Point(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height);
            GameConstants.GemScale = 
                (float) (wsize.X - 100) / (GameConstants.GemTextureWidth * GameConstants.BoardWidth);
            var x = GameConstants.GemScale;
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            gemFactory = new GemFactory(Content);
            gameClock = new Stopwatch();
            mainMenu = new MainMenu(Content, gameClock, gemFactory, wsize);
            gameHud = new GameHud(Content);
            gameOverMenu = new GameOverMenu(Content, new Vector2((wsize.X - 768) / 2.0f, 500));
            gemBoard = new GemBoard(Content, gameClock, gemFactory, new Vector2(50, 300), wsize);

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.ApplyChanges();
            
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

            this.LoadHighscore();
            gameHud.SetHighscore(highScore);

            gemFactory.LoadContent();
            mainMenu.LoadContent();
            gameHud.LoadContent();
            gemBoard.LoadContent();
            gameOverMenu.LoadContent();
        }

        private void LoadHighscore()
        {
            var store = IsolatedStorageFile.GetUserStoreForApplication();

            if (store.FileExists(GameConstants.PlayerDataPath) == false)
            {
                highScore = 0;
                return;
            }

            var fs = store.OpenFile(GameConstants.PlayerDataPath, FileMode.Open);
            using StreamReader sr = new StreamReader(fs);
            highScore = Convert.ToInt32(sr.ReadLine());
        }

        private void UpdateHighscore(int newScore)
        {
            highScore = newScore;
            var store = IsolatedStorageFile.GetUserStoreForApplication();

            var fs = store.CreateFile(GameConstants.PlayerDataPath);
            using StreamWriter sw = new StreamWriter(fs);
            sw.Write(highScore.ToString());
        }

        private void StartGame()
        {
            gemBoard.Fill();
            gameHud.Reset();
            gameHud.SetHighscore(highScore);
            state = GameState.InGame;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (state == GameState.InGame)
                this.UpdateGame();
            else if (state == GameState.InMainMenu)
                this.UpdateMainMenu();
            else
                this.UpdateGameOverMenu();

            base.Update(gameTime);
        }

        private void UpdateGameOverMenu()
        {
            gameOverMenu.Update();
            while (TouchPanel.IsGestureAvailable)
            {
                var e = TouchPanel.ReadGesture();
                if (e.GestureType == GestureType.Tap)
                {
                    if (gameOverMenu.ReplayButtonContains(e.Position))
                    {
                        this.StartGame();
                    }
                    else if (gameOverMenu.BackButtonContains(e.Position))
                        state = GameState.InMainMenu;
                }
            }
        }

        private void UpdateMainMenu()
        {
            mainMenu.Update();
            while (TouchPanel.IsGestureAvailable)
            {
                var e = TouchPanel.ReadGesture();
                if (e.GestureType == GestureType.Tap)
                {
                    if (mainMenu.PlayButtonContains(e.Position))
                        this.StartGame();
                    else if (mainMenu.ExitButtonContains(e.Position))
                        this.Exit();
                    else if (mainMenu.GithubButtonContains(e.Position))
                    {
                        var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://github.com/filip256/bobix"));
                        intent.AddFlags(ActivityFlags.NewTask);
                        Application.Context.StartActivity(intent);
                    }
                    else
                    {
                        mainMenu.Tap(e.Position);
                    }
                }
            }
        }

        private void UpdateGame()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                state = GameState.InGameOver;
                var score = gameHud.GetScore();
                gameOverMenu.SetScore(score, score > highScore);
                if (score > highScore)
                    this.UpdateHighscore(score);
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
                            gemBoard.OnSwipe(new Vector2(dragStartPosition.X - 70, dragStartPosition.Y), Direction.RIGHT);
                        }
                        else if (angle < -1.0471975512 && angle > -2.0943951024) // Up
                        {
                            gemBoard.OnSwipe(new Vector2(dragStartPosition.X, dragStartPosition.Y + 70), Direction.UP);
                        }
                        else if ((angle > 2.617993878 && angle < 3.141592654) || (angle < -2.617993878 && angle > -3.141592654)) // Left
                        {
                            gemBoard.OnSwipe(new Vector2(dragStartPosition.X + 70, dragStartPosition.Y), Direction.LEFT);
                        }
                        else if (angle > 1.0471975512 && angle < 2.0943951024) // Down
                        {
                            gemBoard.OnSwipe(new Vector2(dragStartPosition.X, dragStartPosition.Y - 70), Direction.DOWN);
                        }
                    }
                }
            }
            
            if(gemBoard.Update() == false)
            {
                state = GameState.InGameOver;
                var score = gameHud.GetScore();
                gameOverMenu.SetScore(score, score > highScore);
                if (score > highScore)
                    this.UpdateHighscore(score);
                return;
            };

            gameHud.AddScore(gemBoard.GetAccumulatedScore());
            gameHud.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            if (state == GameState.InGame)
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                gemBoard.Draw(spriteBatch);
                gameHud.Draw(spriteBatch);
                spriteBatch.End();
            }
            else if(state == GameState.InMainMenu)
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                mainMenu.Draw(spriteBatch);
                spriteBatch.End();
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                gemBoard.Draw(spriteBatch);
                //gameHud.Draw(spriteBatch);
                gameOverMenu.Draw(spriteBatch);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void Exit()
        {
            base.Exit();
        }
    }
}