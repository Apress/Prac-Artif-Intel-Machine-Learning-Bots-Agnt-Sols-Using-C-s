using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.GameProgramming.UninformedSearch
{
    /// <summary>
    /// Iterative Deepening Search
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Ids<T> : UninformedMethod<T>
    {
        public Dls<T> Dls { get; set; }
        public int MaxDepthSearch { get; set; }
        public int DepthGoalReached { get; set; }
        public T Value { get; set; }

        public Ids(Tree<T> tree, int maxDepthSearch, T value)
            : base(tree)
        {
            MaxDepthSearch = maxDepthSearch;
            Value = value;
        }

        public override List<T> Execute()
        {
            for (var depth = 1; depth < MaxDepthSearch; depth++)
            {
                Dls = new Dls<T>(Tree, depth, Value);
                DepthGoalReached = depth;
                var path = Dls.Execute();
                if (path != null)
                    return path;
            }

            DepthGoalReached = -1;
            return null;
        }

    }
}
