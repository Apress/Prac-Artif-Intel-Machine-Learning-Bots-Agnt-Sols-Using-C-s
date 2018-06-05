using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.GameProgramming.UninformedSearch
{
    /// <summary>
    /// Bidirectional Search
    /// </summary>
    public class Bs<T> 
    {
        public SlidingTilesPuzzle<T> Game { get; set; }
 
        public Bs(SlidingTilesPuzzle<T> game)
        {
            Game = game;
        }

        public string BidirectionalBfs()
        {
            var queueForward = new Queue<Board<T>>();
            queueForward.Enqueue(Game.Board);

            var queueBackward = new Queue<Board<T>>();
            queueBackward.Enqueue(Game.Goal);

            while (queueForward.Count > 0 && queueBackward.Count > 0)
            {
                var currentForward = queueForward.Dequeue();
                var currentBackward = queueBackward.Dequeue();

                var expansionForward = currentForward.Expand();
                var expansionBackward = currentBackward.Expand(true);

                foreach (var c in expansionForward)
                {
                    if (c.Path.Length == 1 && c.Equals(c, Game.Goal))
                        return c.Path;
                    queueForward.Enqueue(c);
                }

                foreach (var c in expansionBackward)
                    queueBackward.Enqueue(c);

                var path = SolutionMet(queueForward, expansionBackward);

                if (path != null)
                    return path;
            }

            return null;
        }

        private string SolutionMet(Queue<Board<T>> expansionForward, List<Board<T>> expansionBackward)
        {
            for (var i = 0; i < expansionBackward.Count; i++)
            {
                if (expansionForward.Contains(expansionBackward[i], new Board<T>()))
                {
                    var first = expansionForward.First(b => b.Equals(b, expansionBackward[i]));
                    return first.Path + new string(expansionBackward[i].Path.Reverse().ToArray());
                }
            }

            return null;
         }
    }
}
