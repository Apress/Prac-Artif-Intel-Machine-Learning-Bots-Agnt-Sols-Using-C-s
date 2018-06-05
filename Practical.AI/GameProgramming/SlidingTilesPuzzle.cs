using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.GameProgramming
{
    public class SlidingTilesPuzzle<T>
    {
        public Board<T> Board { get; set; }
        public Board<T> Goal { get; set; }

        public SlidingTilesPuzzle(Board<T> initial, Board<T> goal)
        {
            Board = initial;
            Goal = goal;
        }
    }

    
}
