using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnakeGame;
using System;
using System.Collections.Generic;

namespace SnakeGame
{
    public class Food
    {
        public Point Position { get; private set; }
        private Random _random = new Random();

        public void Spawn(List<Point> snakeBody, List<Point> obstacles)
{
    while (true)
    {
        Position = new Point(_random.Next(GlobalSettings.GridWidth), _random.Next(GlobalSettings.GridHeight));
        if (!snakeBody.Contains(Position) && !obstacles.Contains(Position)) break;
    }
}
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, new Rectangle(
                Position.X * GlobalSettings.TileSize,
                Position.Y * GlobalSettings.TileSize,
                GlobalSettings.TileSize - 1,
                GlobalSettings.TileSize - 1), Color.Red);
        }
    }
}
