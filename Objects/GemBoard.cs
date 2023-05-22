using Android.Gestures;
using Android.Net.Wifi.Aware;
using Android.Service.Autofill;
using Android.Text;
using Android.Views.TextClassifiers;
using Bobix.Definitions;
using Bobix.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mobile.Animations;
using Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Bobix.Objects
{
    //[SupportedOSPlatform("Android31.0")]
    public class GemBoard
    {
        private readonly GemFactory gemFactory;
        private Point? selectedCoords = null;
        private Gem[,] board;
        private int width, height, visibleHeight;

        private bool modified = false;

        private ContentManager content;
        private Texture2D background;
        private Vector2 backgroundScale;
        //private Texture2D gridBoxTexture;
        private Texture2D selectedTexture;
        private Vector2 position;

        private int scoreAccumulator = 0;

        private List<IDrawableAnimation> drawableAnimations;

        private readonly Stopwatch gameClock;
        private int lastMatchTime;

        private readonly Point windowSize;


        public GemBoard(ContentManager content, Stopwatch gameClock, GemFactory gemFactory, Vector2 position, Point windowSize)
        {
            this.content = content;
            this.gameClock = gameClock;
            this.gemFactory = gemFactory;
            this.position = new Vector2(position.X, position.Y - GameConstants.GemHeightOnScreen * GameConstants.BoardHeight);

            this.width = GameConstants.BoardWidth;
            this.height = 2 * GameConstants.BoardHeight;
            this.visibleHeight = GameConstants.BoardHeight;

            this.drawableAnimations = new List<IDrawableAnimation>();
            this.windowSize = windowSize;

            this.lastMatchTime = 0;
        }

        public void LoadContent()
        {
            background = content.Load<Texture2D>("background");
            backgroundScale = new Vector2(windowSize.X / background.Width, windowSize.Y / background.Height);

            selectedTexture = content.Load<Texture2D>("grid_box");
            //gridBoxTexture = content.Load<Texture2D>("selected");
        }

        public void Fill()
        {
            board = new Gem[height, width];
            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                {
                    board[i, j] = gemFactory.Create(this.GetPositionFromIndex(j, i), new Point(j, i));
                }
            modified = true;
            this.Update(true);
        }

        public void OnTap(Vector2 coords)
        {
            var gemCoords = this.GetGemAtCoords(coords);
            if (gemCoords == null)
            {
                selectedCoords = null;
                return;
            }

            if (selectedCoords == null)
            {
                selectedCoords = gemCoords;
                return;
            }


            if (selectedCoords == gemCoords)
                return;

            if ((selectedCoords?.Y == gemCoords?.Y && (selectedCoords?.X - gemCoords?.X == 1 || selectedCoords?.X - gemCoords?.X == -1)) ||
                (selectedCoords?.X == gemCoords?.X && (selectedCoords?.Y - gemCoords?.Y == 1 || selectedCoords?.Y - gemCoords?.Y == -1)))
            {
                this.Swap((int)selectedCoords?.X, (int)selectedCoords?.Y, (int)gemCoords?.X, (int)gemCoords?.Y);
                modified = true;
                selectedCoords = null;
                return;
            }

            selectedCoords = gemCoords;
        }

        public void OnSwipe(Vector2 coords, Direction direction)
        {
            selectedCoords = null;

            var gemCoords = this.GetGemAtCoords(coords);
            if (gemCoords == null)
                return;

            if (direction == Direction.UP)
            {
                if (gemCoords?.Y > visibleHeight)
                {
                    this.Swap((int)gemCoords?.X, (int)gemCoords?.Y, (int)gemCoords?.X, (int)gemCoords?.Y - 1);
                    modified = true;
                }
                return;
            }
            if (direction == Direction.DOWN)
            {
                if (gemCoords?.Y < height - 1)
                {
                    this.Swap((int)gemCoords?.X, (int)gemCoords?.Y, (int)gemCoords?.X, (int)gemCoords?.Y + 1);
                    modified = true;
                }
                return;
            }
            if (direction == Direction.LEFT)
            {
                if (gemCoords?.X > 0)
                {
                    this.Swap((int)gemCoords?.X, (int)gemCoords?.Y, (int)gemCoords?.X - 1, (int)gemCoords?.Y);
                    modified = true;
                }
                return;
            }
            if (direction == Direction.RIGHT)
            {
                if (gemCoords?.X < GameConstants.BoardWidth - 1)
                {
                    this.Swap((int)gemCoords?.X, (int)gemCoords?.Y, (int)gemCoords?.X + 1, (int)gemCoords?.Y);
                    modified = true;
                }
                return;
            }
        }

        public int GetAccumulatedScore()
        {
            int score = scoreAccumulator;
            scoreAccumulator = 0;
            return score;
        }

        public bool Update(bool isStartup = false)
        {
            if (isStartup == false)
            {
                for (int i = 0; i < height; ++i)
                    for (int j = 0; j < width; ++j)
                        board[i, j].StepAnimations();

                for (int i = 0; i < drawableAnimations.Count(); ++i)
                {
                    if (drawableAnimations.ElementAt(i).Step() == false)
                    {
                        drawableAnimations.RemoveAt(i);
                        --i;
                    }
                }

                if (gameClock.Elapsed.TotalSeconds - lastMatchTime >= 10)
                {
                    lastMatchTime = (int)gameClock.Elapsed.TotalSeconds;
                    var found = this.FindPossibleMatch();
                    if (found == null)
                    {
                        return false;
                    }
                    board[found.Value.Y, found.Value.X].StartHorizontalJiggle(3);
                }
            }
            else
            {
                this.lastMatchTime = (int)gameClock.Elapsed.TotalSeconds;
            }

            while (modified)
            {
                modified = false;
                for (int i = visibleHeight; i < height; ++i)
                    for (int j = 0; j < width; ++j)
                        if (this.RemoveMatchings(j, i, isStartup) == true)
                            modified = true;

                for (int i = 0; i < height; ++i)
                    for (int j = 0; j < width; ++j)
                        this.GemFall(j, i, isStartup);
            }
            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, backgroundScale, SpriteEffects.None, 0.0f);

            var selected = GetSelected();
            if (selected != null)
                spriteBatch.Draw(selectedTexture, selected.GetPosition(), null, new Color(255, 218, 185, 150), 0.0f, Vector2.Zero, GameConstants.GemScale, SpriteEffects.None, 0.0f);

            for (int i = visibleHeight; i < height; ++i)
                for (int j = 0; j < width; ++j)
                {
                    //spriteBatch.Draw(gridBoxTexture, GetPositionFromIndex(j, i), null, Color.White, 0.0f, Vector2.Zero, GameConstants.GemScale / 2.0f, SpriteEffects.None, 0.0f);
                    if (board[i, j] != null)
                        board[i, j].Draw(spriteBatch);
                }

            for (int i = 0; i < drawableAnimations.Count(); ++i)
            {
                drawableAnimations.ElementAt(i).Draw(spriteBatch);
            }
        }

        private Gem GetSelected()
        {
            if (selectedCoords == null)
                return null;
            return board[(int)selectedCoords?.Y, (int)selectedCoords?.X];
        }

        private void Swap(int xA, int yA, int xB, int yB)
        {
            var temp = board[yA, xA];
            board[yA, xA] = board[yB, xB];
            board[yB, xB] = temp;

            if (CheckMatching(xA, yA) || CheckMatching(xB, yB))
            {
                board[yA, xA].SetPosition(this.GetPositionFromIndex(xA, yA), new Point(xA, yA));
                board[yB, xB].SetPosition(this.GetPositionFromIndex(xB, yB), new Point(xB, yB));
                lastMatchTime = (int)gameClock.Elapsed.TotalSeconds;
                return;
            }


            temp = board[yA, xA];
            board[yA, xA] = board[yB, xB];
            board[yB, xB] = temp;

            if(yA == yB)
                board[yA, xA].StartHorizontalJiggle(1);
            else
                board[yA, xA].StartVerticalJiggle(1);
        }

        private Point? GetGemAtCoords(Vector2 coord)
        {
            var mappedCoord = (coord - this.position) / GameConstants.GemWidthOnScreen;
            int x = (int)mappedCoord.X, y = (int)mappedCoord.Y;

            if (x < 0 || x >= GameConstants.BoardWidth ||
                y < visibleHeight || y >= height)
                return null;

            return new Point(x, y);
        }

        private bool CheckMatching(int x, int y)
        {
            if (board[y, x] == null)
                return false;

            int matchings = 1;

            int p = x + 1;
            while (p < width && board[y, p] != null &&
                board[y, x].HasColor(board[y, p])) ++p;
            matchings += p - x - 1;

            p = x - 1;
            while (p >= 0 && board[y, p] != null &&
                board[y, x].HasColor(board[y, p])) --p;
            matchings += x - p - 1;

            if (matchings >= 3)
                return true;

            matchings = 1;
            p = y + 1;
            while (p < height && board[p, x] != null &&
                board[y, x].HasColor(board[p, x])) ++p;
            matchings += p - y - 1;

            p = y - 1;
            while (p >= visibleHeight && board[p, x] != null &&
                board[y, x].HasColor(board[p, x])) --p;
            matchings += y - p - 1;

            return matchings >= 3;
        }

        private bool RemoveMatchings(int x, int y, bool isStartup = false)
        {
            if (board[y, x] == null)
                return false;

            bool matchFound = false;
            int p = x + 1;
            while (p < width && board[y, p] != null && board[y, x] != null &&
                board[y, x].HasColor(board[y, p])) ++p;
            if (p - x >= 3)
            {
                matchFound = true;
                int multiplier = p - x == 5 ? 9 : (p - x == 4 ? 3 : 1);
                for (int i = x; i < p; ++i)
                    RemoveGem(i, y, multiplier, isStartup);
            }

            p = y + 1;
            while (p < height && board[p, x] != null && board[y, x] != null &&
                board[y, x].HasColor(board[p, x])) ++p;
            if (p - y >= 3)
            {
                matchFound = true;
                int multiplier = p - x == 5 ? 9 : (p - x == 4 ? 3 : 1);
                for (int i = y; i < p; ++i)
                    RemoveGem(x, i, multiplier, isStartup);
            }

            return matchFound;
        }

        private void RemoveGem(int x, int y, int multiplier, bool isStartup = false)
        {
            if (board[y, x] == null)
                return;

            if (isStartup == true)
            {
                board[y, x] = null;
                return;
            }

            scoreAccumulator += board[y, x].GetScoreValue() * multiplier;

            if (y >= visibleHeight)
            {
                drawableAnimations.Add(
                    new MinimizeAndSlideTo(
                        board[y, x].GetSprite(),
                        new Vector2(300, 100),
                        20 + RandomGenerator.GetInt(20),
                        RandomGenerator.GetInt(2) == 0
                        )
                    );
            }

            var type = board[y, x].GetGemType();
            var color = board[y, x].GetSprite().Color;
            board[y, x] = null;
            if (type == GemType.Grenade)
            {
                var affected = GemEffectsHelper.GetGrenadeArea(x, y, width, height, 3);
                for (int i = 0; i < affected.Count(); ++i)
                    RemoveGem(affected[i].X, affected[i].Y, multiplier);
            }
            else if (type == GemType.Heisenberg)
            {
                var affected = GemEffectsHelper.GetCrossArea(x, y, width, height);
                for (int i = 0; i < affected.Count(); ++i)
                    RemoveGem(affected[i].X, affected[i].Y, multiplier);
            }
            else if (type == GemType.Formula1)
            {
                var affected = GemEffectsHelper.GetHorizontalArea3(y, width, height);
                for (int i = 0; i < affected.Count(); ++i)
                    RemoveGem(affected[i].X, affected[i].Y, multiplier);
            }
            else if (type == GemType.Diamond)
            {
                var affected = GemEffectsHelper.GetVerticalArea3(x, width, height);
                for (int i = 0; i < affected.Count(); ++i)
                    RemoveGem(affected[i].X, affected[i].Y, multiplier);
            }
            else if (type == GemType.Punisher && color == GemColor.Blue)
            {
                var affected = GemEffectsHelper.GetGrenadeArea(x, y, width, height, 5);
                for (int i = 0; i < affected.Count(); ++i)
                    RemoveGem(affected[i].X, affected[i].Y, multiplier * 10);
            }
        }

        private void GemFall(int x, int y, bool isStartup = false)
        {
            int bottom = y + 1;
            while (bottom < height && board[bottom, x] == null) ++bottom;
            --bottom;

            if (bottom <= y)
                return;

            int i = 0;
            while (y - i >= 0)
            {
                board[bottom - i, x] = board[y - i, x];
                if (board[bottom - i, x] != null)
                    board[bottom - i, x].SetPosition(GetPositionFromIndex(x, bottom - i), new Point(x, bottom - i), isStartup);
                board[y - i, x] = null;
                ++i;
            }

            this.RefillColumn(x, bottom - y);
        }

        private Vector2 GetPositionFromIndex(int x, int y)
        {
            return new Vector2(
                position.X + x * GameConstants.GemWidthOnScreen,
                position.Y + y * GameConstants.GemHeightOnScreen
                );
        }

        private void RefillColumn(int column, int count)
        {
            for (int i = 0; i < count; ++i)
                board[i, column] = gemFactory.Create(this.GetPositionFromIndex(column, i), new Point(column, i));
        }

        private bool HasColor(int x, int y, Gem other)
        {
            if (x < 0 || y < visibleHeight || x >= width || y >= height)
                return false;

            return board[y, x].HasColor(other);
        }

        private Point? FindPossibleMatch()
        {
            for (int i = visibleHeight; i < height; ++i)
                for (int j = 0; j < width; ++j)
                {
                    var thisGem = board[i, j];

                    if (HasColor(j + 1, i + 1, thisGem))   
                    {
                        if (HasColor(j - 1, i + 1, thisGem))
                            return new Point(j, i);
                        if (HasColor(j + 2, i, thisGem))
                            return new Point(j + 1, i + 1);
                        if (HasColor(j, i + 2, thisGem))
                            return new Point(j + 1, i + 1);
                        if (HasColor(j + 2, i + 1, thisGem))
                            return new Point(j, i);
                        if (HasColor(j + 1, i + 2, thisGem))
                            return new Point(j, i);
                    }

                    if(HasColor(j - 1, i + 1, thisGem))
                    {
                        if (HasColor(j, i + 2, thisGem))
                            return new Point(j - 1, i + 1);
                        if (HasColor(j + 1, i, thisGem))
                            return new Point(j - 1, i + 1);
                        if (HasColor(j - 2, i + 1, thisGem))
                            return new Point(j, i);
                        if (HasColor(j - 1, i + 2, thisGem))
                            return new Point(j, i);
                    }

                    if(HasColor(j, i + 1, thisGem))
                    {
                        if (HasColor(j, i + 3, thisGem))
                            return new Point(j, i + 3);
                        if (HasColor(j + 1, i + 2, thisGem))
                            return new Point(j + 1, i + 2);
                        if (HasColor(j - 1, i + 2, thisGem))
                            return new Point(j - 1, i + 2);
                    }

                    if(HasColor(j + 3, i, thisGem))
                    {
                        if (HasColor(j + 1, i, thisGem))
                            return new Point(j + 3, i);
                        if (HasColor(j + 2, i, thisGem))
                            return new Point(j, i);
                    }

                    if (HasColor(j, i + 2, thisGem) && HasColor(j, i + 3, thisGem))
                        return new Point(j, i);

                    if (HasColor(j + 1, i, thisGem) && HasColor(j + 2, i + 1, thisGem))
                        return new Point(j + 2, i + 1);
                }

            return null;

        }
    }
}
