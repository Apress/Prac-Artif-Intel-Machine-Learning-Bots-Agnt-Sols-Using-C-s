using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.GameProgramming
{
    public class Tree<T>
    {
        public T State { get; set; }
        public List<Tree<T>> Children { get; set; } 

        public Tree() 
        {
            Children = new List<Tree<T>>();
        }

        public Tree(T state, IEnumerable<Tree<T>> children)
        {
            State = state;
            Children = new List<Tree<T>>(children);
        }

        public Tree(T state)
        {
            State = state;
            Children = new List<Tree<T>>();
        }

        public bool IsLeaf
        {
            get
            {
                return Children.Count == 0;
            }
        }
    }
}
