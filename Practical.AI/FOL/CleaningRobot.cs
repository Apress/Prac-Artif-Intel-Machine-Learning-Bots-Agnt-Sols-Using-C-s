using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.FOL
{
    public class CleaningRobot
    {
        private readonly int[,] _terrain;
        private static Stopwatch _stopwatch;
        public int X { get; set; }
        public int Y { get; set; }
        private static Random _random;

        public CleaningRobot(int [,] terrain, int x, int y)
        {
            X = x;
            Y = y;
            _terrain = new int[terrain.GetLength(0), terrain.GetLength(1)];
            Array.Copy(terrain, _terrain, terrain.GetLength(0) * terrain.GetLength(1));
            _stopwatch = new Stopwatch();
            _random = new Random();
        }

        public void Start(int miliseconds)
        {
            _stopwatch.Start();

            do
            {
                if (IsDirty())
                    Clean();
                else
                    Move(SelectMove());

            } while (!IsTerrainClean() && !(_stopwatch.ElapsedMilliseconds > miliseconds));
        }

        // Function
        private Direction SelectMove()
        {
            var list = new List<Direction> { Direction.Down, Direction.Up, Direction.Right, Direction.Left };
            return list[_random.Next(0, list.Count)];
        }

        // Function
        public void Clean()
        {
            _terrain[X, Y] -= 1;
        }

        // Predicate
        public bool IsDirty()
        {
            return _terrain[X, Y] > 0;
        }

        // Function
        private void Move(Direction m)
        {
            switch (m)
            {
                case Direction.Up:
                    if (MoveAvailable(X - 1, Y))
                        X -= 1;
                        break;
                case Direction.Down:
                    if (MoveAvailable(X + 1, Y))
                        X += 1;
                        break;
                case Direction.Left:
                        if (MoveAvailable(X, Y - 1))
                            Y -= 1;
                        break;
                case Direction.Right:
                        if (MoveAvailable(X, Y + 1))
                            Y += 1;
                        break;
            }
        }

        // Predicate
        public bool MoveAvailable(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _terrain.GetLength(0) && y < _terrain.GetLength(1);
        }

        // Predicate
        public bool IsTerrainClean()
        {
            // For all cell in terrain; cell equals 0
            foreach (var c in _terrain)
                if (c > 0)
                    return false;

            return true;
        }

        public void Print()
        {
            var col = _terrain.GetLength(1);
            var i = 0;
            var line = "";
            Console.WriteLine("--------------");
            foreach (var c in _terrain)
            {
                line += string.Format("  {0}  ", c);
                i++;
                if (col == i)
                {
                    Console.WriteLine(line);
                    line = "";
                    i = 0;
                }
            }
        }
    }

    public enum Direction
    {
        Up, Down, Left, Right
    }

}
