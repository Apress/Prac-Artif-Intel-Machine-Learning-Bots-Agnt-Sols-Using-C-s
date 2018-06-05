using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.GameProgramming.UninformedSearch
{
    /// <summary>
    /// Depth Limited Search
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Dls<T>: UninformedMethod<T>
    {
        public int DepthLimit { get; set; }
        public T Value { get; set; }

        public Dls(Tree<T> tree, int depthLimit, T value) : base(tree)
        {
            DepthLimit = depthLimit;
            Value = value;
        }

        public override List<T> Execute()
        {
            var path = new List<T>();
            if (RecursiveDfs(Tree, 0, path))
                return path;
            return null;
        }

        private bool RecursiveDfs(Tree<T> tree, int depth, ICollection<T> path)
        {
            if (tree.State.Equals(Value))
                return true;

            if (depth == DepthLimit || tree.IsLeaf)
                return false;

            path.Add(tree.State);

            if (tree.Children.Any(child => RecursiveDfs(child, depth + 1, path)))
                return true;
            
            path.Remove(tree.State);
            return false;
        }
    }
}
