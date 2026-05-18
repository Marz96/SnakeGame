using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SnakeGame
{
    public class Snake
    {
        public List<Point> Body { get; private set; }
        public Point Direction { get; set; }
        public bool IsDead { get; private set; }
        public int Score => Body.Count - 3;

        public Snake()
        {
            Reset();
        }

        public void Reset()
        {
            Body = new List<Point> { new Point(10, 10), new Point(9, 10), new Point(8, 10) };
            Direction = new Point(1, 0);
            IsDead = false;
        }

        public void Update(bool grow)
        {
            if (IsDead) return;

            Point newHead = Body[0] + Direction;

            if (newHead.X < 0 || newHead.X >= GlobalSettings.GridWidth ||
                newHead.Y < 0 || newHead.Y >= GlobalSettings.GridHeight)
            {
                IsDead = true;
                return;
            }

            if (Body.Contains(newHead))
            {
                IsDead = true;
                return;
            }

            Body.Insert(0, newHead);
            if (!grow)
            {
                Body.RemoveAt(Body.Count - 1);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            for (int i = 0; i < Body.Count; i++)
            {
                Color color = (i == 0) ? Color.LimeGreen : Color.ForestGreen;
                spriteBatch.Draw(texture, new Rectangle(
                    Body[i].X * GlobalSettings.TileSize,
                    Body[i].Y * GlobalSettings.TileSize,
                    GlobalSettings.TileSize - 1,
                    GlobalSettings.TileSize - 1), color);
            }
        }
    }
}