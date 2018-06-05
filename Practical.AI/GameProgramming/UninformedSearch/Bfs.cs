using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.GameProgramming.UninformedSearch
{
    public class Bfs<T>: UninformedMethod<T>
    {
        public Bfs(Tree<T> tree):base(tree)
        { }

        public override List<T> Execute()
        {
            var queue = new Queue<Tree<T>>();
            queue.Enqueue(Tree);
            var path = new List<T>();
            
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                path.Add(current.State);

                foreach (var c in current.Children)
                    queue.Enqueue(c);
            }

            return path;
        }
    }
}
