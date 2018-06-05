using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Practical.AI.GameProgramming;

namespace Practical.AI.GameTheory.AdversarialSearch
{
    public class Minimax
    {
        public int MaxDepth { get; set; }
        public bool Max { get; set; }
        private Tuple<int, int> _resultMove;

        public Minimax(int maxDepth, bool max)
        {
            MaxDepth = maxDepth;
            Max = max;
        }

        public Tuple<int, int> GetOptimalMove(OthelloBoard board, bool max)
        {
            Execute(board, max, 0);
            return _resultMove;
        } 

        public double Execute (OthelloBoard board, bool max, int depth)
        {
            if (depth == MaxDepth)
                return board.HeuristicUtility();

            var children = board.Expand(max ? 1 : 2);

            if (children.Count == 0)
                return board.HeuristicUtility();

            var result = !max ? double.MaxValue : double.MinValue;

            foreach (var othelloBoard in children)
            {
                var value = Execute(othelloBoard, !max, depth + 1);
                othelloBoard.UtilityValue = value;
                result = max ? Math.Max(value, result) : Math.Min(value, result);
            }

            if (depth == 0)
                _resultMove = children.First(c => c.UtilityValue == result).MoveFrom;

            return result;
        }
    }
}
