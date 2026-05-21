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
        private List<Point> _obstacles;
        
        private Point _nextDirection;
        private float _timer;
        private float _speed = GlobalSettings.InitialSpeed;
        
        private int _currentLevel = 0;
        private const int TotalLevels = 10;
        private const int ScoreToWinLevel = 5;
        private bool _gameWon = false;

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
            LoadLevel(_currentLevel);
            base.Initialize();
        }

        private void LoadLevel(int levelIndex)
        {
            _obstacles = Levels.GetObstacles(levelIndex);
            _snake.Reset();
            _food.Spawn(_snake.Body, _obstacles);
            _nextDirection = _snake.Direction;
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
            
            if ((_snake.IsDead || _gameWon) && kState.IsKeyDown(Keys.Enter))
            {
                _currentLevel = 0;
                _gameWon = false;
                LoadLevel(_currentLevel);
            }

            if ((kState.IsKeyDown(Keys.Up) || kState.IsKeyDown(Keys.W)) && _snake.Direction.Y == 0) _nextDirection = new Point(0, -1);
            else if ((kState.IsKeyDown(Keys.Down) || kState.IsKeyDown(Keys.S)) && _snake.Direction.Y == 0) _nextDirection = new Point(0, 1);
            else if ((kState.IsKeyDown(Keys.Left) || kState.IsKeyDown(Keys.A)) && _snake.Direction.X == 0) _nextDirection = new Point(-1, 0);
            else if ((kState.IsKeyDown(Keys.Right) || kState.IsKeyDown(Keys.D)) && _snake.Direction.X == 0) _nextDirection = new Point(1, 0);

            if (!_snake.IsDead && !_gameWon)
            {
                _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timer >= _speed)
                {
                    _snake.Direction = _nextDirection;
                    bool ateFood = (_snake.Body[0] + _snake.Direction) == _food.Position;
                    
                    _snake.Update(ateFood, _obstacles);
                    
                    if (ateFood)
                    {
                        if (_snake.Score >= ScoreToWinLevel)
                        {
                            _currentLevel++;
                            if (_currentLevel >= TotalLevels)
                            {
                                _gameWon = true;
                            }
                            else
                            {
                                LoadLevel(_currentLevel);
                            }
                        }
                        else
                        {
                            _food.Spawn(_snake.Body, _obstacles);
                        }
                    }
                    _timer = 0;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(25, 25, 25));
            _spriteBatch.Begin();

            // Отрисовка стен уровня
            foreach (var obstacle in _obstacles)
            {
                _spriteBatch.Draw(_pixel, new Rectangle(
                    obstacle.X * GlobalSettings.TileSize, 
                    obstacle.Y * GlobalSettings.TileSize, 
                    GlobalSettings.TileSize - 1, 
                    GlobalSettings.TileSize - 1), new Color(70, 80, 95));
            }

            _food.Draw(_spriteBatch, _pixel);
            _snake.Draw(_spriteBatch, _pixel);

            // Индикация уровня и счета
            DrawPixelText("LVL " + (_currentLevel + 1), 20, 20, 3, Color.LightBlue);
            DrawPixelText("SCORE " + _snake.Score, 140, 20, 3, Color.Gold * 0.8f);

            int centerX = Window.ClientBounds.Width / 2;
            int centerY = Window.ClientBounds.Height / 2;

            if (_snake.IsDead)
            {
                _spriteBatch.Draw(_pixel, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.Black * 0.7f);
                DrawPixelText("GAME OVER", centerX - 85, centerY - 40, 5, Color.Red);
                DrawPixelText("FINAL SCORE " + _snake.Score, centerX - 75, centerY + 10, 3, Color.White);
                DrawPixelText("PRESS ENTER TO RESTART", centerX - 140, centerY + 50, 2, Color.Gray);
            }
            else if (_gameWon)
            {
                _spriteBatch.Draw(_pixel, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.Black * 0.7f);
                DrawPixelText("YOU WIN", centerX - 70, centerY - 40, 5, Color.Green);
                DrawPixelText("ALL LEVELS COMPLETED", centerX - 130, centerY + 10, 3, Color.White);
                DrawPixelText("PRESS ENTER TO PLAY AGAIN", centerX - 150, centerY + 50, 2, Color.Gray);
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
                {'0', new[] {7, 5, 5, 5, 7}}, {'1', new[] {2, 6, 2, 2, 7}},
                {'2', new[] {7, 1, 7, 4, 7}}, {'3', new[] {7, 1, 7, 1, 7}},
                {'4', new[] {5, 5, 7, 1, 1}}, {'5', new[] {7, 4, 7, 1, 7}},
                {'6', new[] {7, 4, 7, 5, 7}}, {'7', new[] {7, 1, 1, 1, 1}},
                {'8', new[] {7, 5, 7, 5, 7}}, {'9', new[] {7, 5, 7, 1, 7}},
                {'A', new[] {7, 5, 7, 5, 5}}, {'C', new[] {7, 4, 4, 4, 7}},
                {'E', new[] {7, 4, 7, 4, 7}}, {'G', new[] {7, 4, 5, 5, 7}},
                {'M', new[] {5, 7, 5, 5, 5}}, {'O', new[] {7, 5, 5, 5, 7}},
                {'V', new[] {5, 5, 5, 5, 2}}, {'R', new[] {7, 5, 7, 6, 5}},
                {'S', new[] {7, 4, 7, 1, 7}}, {'P', new[] {7, 5, 7, 4, 4}},
                {'T', new[] {7, 2, 2, 2, 2}}, {'I', new[] {7, 2, 2, 2, 7}},
                {'N', new[] {5, 6, 5, 5, 5}}, {'U', new[] {5, 5, 5, 5, 7}},
                {'K', new[] {5, 5, 6, 5, 5}}, {'L', new[] {4, 4, 4, 4, 7}},
                {'W', new[] {5, 5, 5, 7, 5}}, {'Y', new[] {5, 5, 2, 2, 2}},
                {'F', new[] {7, 4, 6, 4, 4}}, {'B', new[] {6, 5, 6, 5, 6}},
                {'D', new[] {6, 5, 5, 5, 6}}
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
