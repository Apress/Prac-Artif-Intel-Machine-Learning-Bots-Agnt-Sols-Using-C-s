using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.GameProgramming.UninformedSearch
{
    public class Dfs<T> : UninformedMethod<T>
    {
        public Dfs(Tree<T> tree):base(tree)
        {
        }

        public override List<T> Execute()
        {
            var path = new List<T>();
            var stack = new Stack<Tree<T>>();
            stack.Push(Tree);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                path.Add(current.State);

                for (var i = current.Children.Count - 1; i >= 0; i--)
                    stack.Push(current.Children[i]);
            }

            return path;
        }
    }
}
