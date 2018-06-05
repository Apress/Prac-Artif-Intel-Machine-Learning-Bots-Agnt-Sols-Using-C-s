using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.GameProgramming.UninformedSearch
{
    public abstract class UninformedMethod<T>
    {
        public Tree<T> Tree { get; set; }

        protected UninformedMethod(Tree<T> tree)
        {
            Tree = tree;
        }

        public abstract List<T> Execute();
    }
}
