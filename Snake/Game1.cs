using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SnakeGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private Snake _snake;
        private Food _food;
        private Point _nextDirection;
        private float _timer;
        private float _speed = GlobalSettings.InitialSpeed;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GlobalSettings.GridWidth * GlobalSettings.TileSize;
            _graphics.PreferredBackBufferHeight = GlobalSettings.GridHeight * GlobalSettings.TileSize;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _snake = new Snake();
            _food = new Food();
            _food.Spawn(_snake.Body);
            _nextDirection = _snake.Direction;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            var kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.Escape)) Exit();

            if (_snake.IsDead && kState.IsKeyDown(Keys.Enter))
            {
                _snake.Reset();
                _food.Spawn(_snake.Body);
                _nextDirection = _snake.Direction;
            }

            if ((kState.IsKeyDown(Keys.Up) || kState.IsKeyDown(Keys.W)) && _snake.Direction.Y == 0) _nextDirection = new Point(0, -1);
            else if ((kState.IsKeyDown(Keys.Down) || kState.IsKeyDown(Keys.S)) && _snake.Direction.Y == 0) _nextDirection = new Point(0, 1);
            else if ((kState.IsKeyDown(Keys.Left) || kState.IsKeyDown(Keys.A)) && _snake.Direction.X == 0) _nextDirection = new Point(-1, 0);
            else if ((kState.IsKeyDown(Keys.Right) || kState.IsKeyDown(Keys.D)) && _snake.Direction.X == 0) _nextDirection = new Point(1, 0);

            if (!_snake.IsDead)
            {
                _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timer >= _speed)
                {
                    _snake.Direction = _nextDirection;
                    bool ateFood = (_snake.Body[0] + _snake.Direction) == _food.Position;
                    _snake.Update(ateFood);
                    if (ateFood) _food.Spawn(_snake.Body);
                    _timer = 0;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(25, 25, 25));
            _spriteBatch.Begin();

            _food.Draw(_spriteBatch, _pixel);
            _snake.Draw(_spriteBatch, _pixel);

            DrawPixelText(_snake.Score.ToString(), 20, 20, 3, Color.Gold * 0.8f);

            if (_snake.IsDead)
            {
                _spriteBatch.Draw(_pixel, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.Black * 0.7f);

                int centerX = Window.ClientBounds.Width / 2;
                int centerY = Window.ClientBounds.Height / 2;

                DrawPixelText("GAME OVER", centerX - 85, centerY - 40, 5, Color.Red);
                DrawPixelText("SCORE " + _snake.Score, centerX - 45, centerY + 10, 3, Color.White);
                DrawPixelText("PRESS ENTER TO RESTART", centerX - 140, centerY + 50, 2, Color.Gray);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawPixelText(string text, int x, int y, int size, Color color)
        {
            int currentX = x;
            foreach (char c in text.ToUpper())
            {
                if (c == ' ') { currentX += size * 4; continue; }
                DrawCharacter(c, currentX, y, size, color);
                currentX += size * 4;
            }
        }

        private void DrawCharacter(char c, int x, int y, int s, Color color)
        {
            var glyphs = new Dictionary<char, int[]> {
                {'0', new[] {7, 5, 5, 5, 7}},
                {'1', new[] {2, 6, 2, 2, 7}},
                {'2', new[] {7, 1, 7, 4, 7}},
                {'3', new[] {7, 1, 7, 1, 7}},
                {'4', new[] {5, 5, 7, 1, 1}},
                {'5', new[] {7, 4, 7, 1, 7}},
                {'6', new[] {7, 4, 7, 5, 7}},
                {'7', new[] {7, 1, 1, 1, 1}},
                {'8', new[] {7, 5, 7, 5, 7}},
                {'9', new[] {7, 5, 7, 1, 7}},
                {'A', new[] {7, 5, 7, 5, 5}},
                {'C', new[] {7, 4, 4, 4, 7}},
                {'E', new[] {7, 4, 7, 4, 7}},
                {'G', new[] {7, 4, 5, 5, 7}},
                {'M', new[] {5, 7, 5, 5, 5}},
                {'O', new[] {7, 5, 5, 5, 7}},
                {'V', new[] {5, 5, 5, 5, 2}},
                {'R', new[] {7, 5, 7, 6, 5}},
                {'S', new[] {7, 4, 7, 1, 7}},
                {'P', new[] {7, 5, 7, 4, 4}},
                {'T', new[] {7, 2, 2, 2, 2}},
                {'I', new[] {7, 2, 2, 2, 7}},
                {'N', new[] {5, 6, 5, 5, 5}},
                {'U', new[] {5, 5, 5, 5, 7}},
                {'K', new[] {5, 5, 6, 5, 5}}
            };

            if (!glyphs.ContainsKey(c)) return;

            int[] rows = glyphs[c];
            for (int row = 0; row < 5; row++)
            {
                int rowData = rows[row];
                for (int col = 0; col < 3; col++)
                {
                    if (((rowData >> (2 - col)) & 1) != 0)
                    {
                        _spriteBatch.Draw(_pixel, new Rectangle(x + col * s, y + row * s, s, s), color);
                    }
                }
            }
        }
    }
}