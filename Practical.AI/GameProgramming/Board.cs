using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.GameProgramming
{
    public class Board<T> : IEqualityComparer<Board<T>> 
    {
        public T[,] State { get; set; }
        public T Blank { get; set; }
        public string Path { get; set; }
        private readonly Tuple<int, int> _blankPos;
        private readonly int _n;

        public Board() { } 

        public Board(T[,] state, T blank, Tuple<int, int> blankPos, string path)
        {
            State = state;
            Blank = blank;
            _n = State.GetLength(0);
            _blankPos = blankPos;
            Path = path;
        }

        public List<Board<T>> Expand(bool backwards = false)
        {
            var result = new List<Board<T>>();

            var up = Move(GameProgramming.Move.Up, backwards);
            var down = Move(GameProgramming.Move.Down, backwards);
            var lft = Move(GameProgramming.Move.Left, backwards);
            var rgt = Move(GameProgramming.Move.Right, backwards);

            if (up._blankPos.Item1 >= 0 && (string.IsNullOrEmpty(Path) || Path.Last() != (backwards ? 'U' : 'D'))) 
                result.Add(up);
            if (down._blankPos.Item1 >= 0 && (string.IsNullOrEmpty(Path) || Path.Last() != (backwards ? 'D' : 'U')))
                result.Add(down);
            if (lft._blankPos.Item1 >= 0 && (string.IsNullOrEmpty(Path) || Path.Last() != (backwards ? 'L' : 'R')))
                result.Add(lft);
            if (rgt._blankPos.Item1 >= 0 && (string.IsNullOrEmpty(Path) || Path.Last() != (backwards ? 'R' : 'L')))
                result.Add(rgt);

            return result;
        }

        public Board<T> Move(Move move, bool backwards = false)
        {
            var newState = new T[_n, _n];
            Array.Copy(State, newState, State.GetLength(0) * State.GetLength(1));
            var newBlankPos = new Tuple<int, int>(-1, -1);
            var path = "";

            switch (move)
            {
                case GameProgramming.Move.Up:
                    if (_blankPos.Item1 - 1 >= 0)
                    {
                        var temp = newState[_blankPos.Item1 - 1, _blankPos.Item2];
                        newState[_blankPos.Item1 - 1, _blankPos.Item2] = Blank;
                        newState[_blankPos.Item1, _blankPos.Item2] = temp;
                        newBlankPos = new Tuple<int, int>(_blankPos.Item1 - 1, _blankPos.Item2);
                        path = backwards ? "D" : "U";
                    }
                    break;
                case GameProgramming.Move.Down:
                    if (_blankPos.Item1 + 1 < _n)
                    {
                        var temp = newState[_blankPos.Item1 + 1, _blankPos.Item2];
                        newState[_blankPos.Item1 + 1, _blankPos.Item2] = Blank;
                        newState[_blankPos.Item1, _blankPos.Item2] = temp;
                        newBlankPos = new Tuple<int, int>(_blankPos.Item1 + 1, _blankPos.Item2);
                        path = backwards ? "U" : "D";
                    }
                    break;
                case GameProgramming.Move.Left:
                    if (_blankPos.Item2 - 1 >= 0)
                    {
                        var temp = newState[_blankPos.Item1, _blankPos.Item2 - 1];
                        newState[_blankPos.Item1, _blankPos.Item2 - 1] = Blank;
                        newState[_blankPos.Item1, _blankPos.Item2] = temp;
                        newBlankPos = new Tuple<int, int>(_blankPos.Item1, _blankPos.Item2 - 1);
                        path = backwards ? "R" : "L";
                    }
                    break;
                case GameProgramming.Move.Right:
                    if (_blankPos.Item2 + 1 < _n)
                    {
                        var temp = newState[_blankPos.Item1, _blankPos.Item2 + 1];
                        newState[_blankPos.Item1, _blankPos.Item2 + 1] = Blank;
                        newState[_blankPos.Item1, _blankPos.Item2] = temp;
                        newBlankPos = new Tuple<int, int>(_blankPos.Item1, _blankPos.Item2 + 1);
                        path = backwards ? "L" : "R";
                    }
                    break;
            }

            return new Board<T>(newState, Blank, newBlankPos, Path + path);
        }

        public bool Equals(Board<T> x, Board<T> y)
        {
            if (x.State.GetLength(0) != y.State.GetLength(0) ||
                x.State.GetLength(1) != y.State.GetLength(1))
                return false;

            for (var i = 0; i < x.State.GetLength(0); i++)
            {
                for (var j = 0; j < x.State.GetLength(1); j++)
                {
                    if (!x.State[i, j].Equals(y.State[i, j]))
                        return false;
                }
            }

            return true;
        }

        public int GetHashCode(Board<T> obj)
        {
            return 0;
        }
    }

    public enum Move
    {
        Up, Down, Left, Right
    }
}
