using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SnakeGame
{
    public enum GameState
    {
        MainMenu,
        Playing
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private Snake _snake;
        private Food _food;
        private Point _nextDirection;
        private float _timer;
        
        // Настройки уровней и скорости
        private float _speed;
        private int _currentLevel = 1;
        private const int MaxLevels = 4;

        // Список точек препятствий текущего уровня
        private List<Point> _obstacles = new List<Point>();

        private GameState _currentState = GameState.MainMenu;
        private MouseState _oldMouseState;
        private KeyboardState _oldKeyboardState;

        // Компактные кнопки (ширина 120, высота 40)
        private Rectangle _startButtonRect;
        private Rectangle _restartButtonRect;

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
            
            _oldMouseState = Mouse.GetState();
            _oldKeyboardState = Keyboard.GetState();

            int screenWidth = GlobalSettings.GridWidth * GlobalSettings.TileSize;
            int screenHeight = GlobalSettings.GridHeight * GlobalSettings.TileSize;
            
            // Центрируем компактные кнопки по оси X
            _startButtonRect = new Rectangle((screenWidth - 120) / 2, (screenHeight - 40) / 2 + 30, 120, 40);
            _restartButtonRect = new Rectangle((screenWidth - 120) / 2, (screenHeight - 40) / 2 + 45, 120, 40);

            ResetGameLogic();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        // Генерация стен для каждого уровня
        private void GenerateObstaclesForLevel(int level)
        {
            _obstacles.Clear();
            int w = GlobalSettings.GridWidth;
            int h = GlobalSettings.GridHeight;

            switch (level)
            {
                case 1:
                    break;
                case 2:
                    for (int i = 5; i < 15; i++)
                    {
                        _obstacles.Add(new Point(i, 5));
                        _obstacles.Add(new Point(5, i));
                        _obstacles.Add(new Point(w - 1 - i, 5));
                        _obstacles.Add(new Point(w - 6, i));
                        _obstacles.Add(new Point(i, h - 6));
                        _obstacles.Add(new Point(5, h - 1 - i));
                        _obstacles.Add(new Point(w - 1 - i, h - 6));
                        _obstacles.Add(new Point(w - 6, h - 1 - i));
                    }
                    break;
                case 3:
                    for (int x = 8; x < w - 8; x++)
                    {
                        _obstacles.Add(new Point(x, h / 3));
                        _obstacles.Add(new Point(x, (h / 3) * 2));
                    }
                    break;
                case 4:
                    for (int x = 10; x < w - 10; x++) _obstacles.Add(new Point(x, h / 2));
                    for (int y = 5; y < h - 5; y++) if (y != h / 2) _obstacles.Add(new Point(w / 2, y));
                    break;
            }
        }

        private void SpawnFoodSafely()
        {
            List<Point> occupied = new List<Point>(_snake.Body);
            occupied.AddRange(_obstacles);
            _food.Spawn(occupied);
        }

        private void ResetGameLogic()
        {
            _snake.Reset();
            _currentLevel = 1;
            _speed = GlobalSettings.InitialSpeed;
            _timer = 0;
            _nextDirection = _snake.Direction;
            GenerateObstaclesForLevel(_currentLevel);
            SpawnFoodSafely();
        }

        protected override void Update(GameTime gameTime)
        {
            var kState = Keyboard.GetState();
            var mState = Mouse.GetState();

            if (kState.IsKeyDown(Keys.Escape)) Exit();

            switch (_currentState)
            {
                case GameState.MainMenu:
                    bool isStartClicked = _startButtonRect.Contains(mState.Position) && 
                                          mState.LeftButton == ButtonState.Pressed && 
                                          _oldMouseState.LeftButton == ButtonState.Released;

                    if (isStartClicked || (kState.IsKeyDown(Keys.Enter) && _oldKeyboardState.IsKeyUp(Keys.Enter)))
                    {
                        ResetGameLogic();
                        _currentState = GameState.Playing;
                    }
                    break;

                case GameState.Playing:
                    if (_snake.IsDead)
                    {
                        bool isRestartClicked = _restartButtonRect.Contains(mState.Position) && 
                                                mState.LeftButton == ButtonState.Pressed && 
                                                _oldMouseState.LeftButton == ButtonState.Released;

                        if (isRestartClicked || (kState.IsKeyDown(Keys.Enter) && _oldKeyboardState.IsKeyUp(Keys.Enter)))
                        {
                            ResetGameLogic();
                        }
                        break;
                    }

                    if ((kState.IsKeyDown(Keys.Up) || kState.IsKeyDown(Keys.W)) && _snake.Direction.Y == 0) _nextDirection = new Point(0, -1);
                    else if ((kState.IsKeyDown(Keys.Down) || kState.IsKeyDown(Keys.S)) && _snake.Direction.Y == 0) _nextDirection = new Point(0, 1);
                    else if ((kState.IsKeyDown(Keys.Left) || kState.IsKeyDown(Keys.A)) && _snake.Direction.X == 0) _nextDirection = new Point(-1, 0);
                    else if ((kState.IsKeyDown(Keys.Right) || kState.IsKeyDown(Keys.D)) && _snake.Direction.X == 0) _nextDirection = new Point(1, 0);

                    _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_timer >= _speed)
                    {
                        _snake.Direction = _nextDirection;
                        Point nextHeadPos = _snake.Body[0] + _snake.Direction;

                        if (_obstacles.Contains(nextHeadPos))
                        {
                            _snake.Direction = new Point(-_snake.Direction.X, -_snake.Direction.Y);
                            _snake.Update(false);
                        }
                        else
                        {
                            bool ateFood = nextHeadPos == _food.Position;
                            _snake.Update(ateFood);

                            if (ateFood)
                            {
                                int newLevel = 1 + (_snake.Score / 5);
                                if (newLevel > MaxLevels) newLevel = MaxLevels;

                                if (newLevel != _currentLevel)
                                {
                                    _currentLevel = newLevel;
                                    GenerateObstaclesForLevel(_currentLevel);
                                }

                                SpawnFoodSafely();

                                _speed = GlobalSettings.InitialSpeed - (_currentLevel - 1) * 0.02f;
                                if (_speed < 0.05f) _speed = 0.05f;
                            }
                        }
                        _timer = 0;
                    }
                    break;
            }

            _oldMouseState = mState;
            _oldKeyboardState = kState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(25, 25, 25));
            _spriteBatch.Begin();

            int centerX = Window.ClientBounds.Width / 2;
            int centerY = Window.ClientBounds.Height / 2;

            switch (_currentState)
            {
                case GameState.MainMenu:
                    DrawPixelText("SNAKE GAME", centerX - 110, centerY - 60, 5, Color.LimeGreen);

                    DrawRectangleBorder(_startButtonRect, 3, Color.LimeGreen);
                    DrawPixelText("START", _startButtonRect.X + 30, _startButtonRect.Y + 10, 3, Color.LimeGreen);
                    break;

                case GameState.Playing:
                    foreach (var obstacle in _obstacles)
                    {
                        _spriteBatch.Draw(_pixel, new Rectangle(
                            obstacle.X * GlobalSettings.TileSize,
                            obstacle.Y * GlobalSettings.TileSize,
                            GlobalSettings.TileSize - 1,
                            GlobalSettings.TileSize - 1), Color.Gray);
                    }

                    _food.Draw(_spriteBatch, _pixel);
                    _snake.Draw(_spriteBatch, _pixel);

                    DrawPixelText("SCORE " + _snake.Score, 20, 20, 2, Color.Gold * 0.8f);
                    DrawPixelText("LEVEL " + _currentLevel, 20, 45, 2, Color.LightBlue * 0.8f);

                    if (_snake.IsDead)
                    {
                        _spriteBatch.Draw(_pixel, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.Black * 0.75f);

                        DrawPixelText("GAME OVER", centerX - 90, centerY - 80, 5, Color.Red);
                        DrawPixelText("FINAL SCORE " + _snake.Score, centerX - 72, centerY - 20, 2, Color.White);
                        DrawPixelText("LEVEL REACHED " + _currentLevel, centerX - 80, centerY + 5, 2, Color.LightBlue);

                        DrawRectangleBorder(_restartButtonRect, 3, Color.Orange);
                        DrawPixelText("RESTART", _restartButtonRect.X + 18, _restartButtonRect.Y + 10, 3, Color.Orange);
                    }
                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawRectangleBorder(Rectangle rect, int thickness, Color color)
        {
            _spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            _spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            _spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
            _spriteBatch.Draw(_pixel, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
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
                {'K', new[] {5, 5, 6, 5, 5}},
                {'H', new[] {5, 5, 7, 5, 5}},
                {'D', new[] {6, 5, 5, 5, 6}},
                {'F', new[] {7, 4, 6, 4, 4}},
                {'L', new[] {4, 4, 4, 4, 7}},
                {'B', new[] {6, 5, 6, 5, 6}}
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
