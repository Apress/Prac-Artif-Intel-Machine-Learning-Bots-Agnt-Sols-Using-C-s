using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.Agents
{
    public class CleaningAgent
    {
        private readonly int[,] _terrain;
        private static Stopwatch _stopwatch;
        public int X { get; set; }
        public int Y { get; set; }
        public bool TaskFinished { get; set; }
        // Internal data structure for keeping state
        private List<Tuple<int, int>> _cleanedCells;
        private Random _random;

        public CleaningAgent(int [,] terrain, int x, int y)
        {
            X = x;
            Y = y;
            _terrain = new int[terrain.GetLength(0), terrain.GetLength(1)];
            Array.Copy(terrain, _terrain, terrain.GetLength(0) * terrain.GetLength(1));
            _stopwatch = new Stopwatch();
            _cleanedCells = new List<Tuple<int, int>>();
            _random = new Random();
        }

        public void Start(int miliseconds)
        {
            _stopwatch.Start();

            do
            {
                Action(Perceived());
            }
            while (!TaskFinished && !(_stopwatch.ElapsedMilliseconds > miliseconds));
        }

        private void UpdateState()
        {
            if (!_cleanedCells.Contains(new Tuple<int, int>(X, Y)))
                _cleanedCells.Add(new Tuple<int, int>(X, Y));
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

        public void Action(List<P> percepts)
        {
            if (percepts.Contains(P.Clean))
                UpdateState();
            if (percepts.Contains(P.Dirty))
                Clean();
            else if (percepts.Contains(P.Finished))
                TaskFinished = true;
            else if (percepts.Contains(P.MoveUp) && !_cleanedCells.Contains(new Tuple<int, int>(X - 1, Y)))
                Move(P.MoveUp);
            else if (percepts.Contains(P.MoveDown) && !_cleanedCells.Contains(new Tuple<int, int>(X + 1, Y)))
                Move(P.MoveDown);
            else if (percepts.Contains(P.MoveLeft) && !_cleanedCells.Contains(new Tuple<int, int>(X, Y - 1)))
                Move(P.MoveLeft);
            else if (percepts.Contains(P.MoveRight) && !_cleanedCells.Contains(new Tuple<int, int>(X, Y + 1)))
                Move(P.MoveRight);
            else
                RandomAction(percepts);
        }

        private void RandomAction(List<P> percepts)
        {
            var p = percepts[_random.Next(1, percepts.Count)];
            Move(p);
        }

        private void Move(P p)
        {
            switch (p)
            {
                case P.MoveUp:
                    X -= 1;
                    break;
                case P.MoveDown:
                    X += 1;
                    break;
                case P.MoveLeft:
                    Y -= 1;
                    break;
                case P.MoveRight:
                    Y += 1;
                    break;
            }
        }

        // Function
        private List<P> Perceived()
        {
            var result = new List<P>();

            if (IsDirty())
                result.Add(P.Dirty);
            else
                result.Add(P.Clean);

            if (_cleanedCells.Count == _terrain.GetLength(0) * _terrain.GetLength(1))
                result.Add(P.Finished); 

            if (MoveAvailable(X - 1, Y))
                result.Add(P.MoveUp);

            if (MoveAvailable(X + 1, Y))
                result.Add(P.MoveDown);
            
            if (MoveAvailable(X, Y - 1))
                result.Add(P.MoveLeft);
            
            if (MoveAvailable(X, Y + 1))
                result.Add(P.MoveRight);
            
            return result;
        }

        // Predicate
        public bool MoveAvailable(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _terrain.GetLength(0) && y < _terrain.GetLength(1);
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

    public enum P
    {
        Dirty, Clean, Finished, MoveUp, MoveDown, MoveLeft, MoveRight  
    }

}
