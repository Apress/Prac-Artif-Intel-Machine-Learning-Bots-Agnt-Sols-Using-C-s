using System.Collections.Generic;
using System.Linq;
using Practical.AI.PropositionalLogic;

namespace Practical.AI.SupervisedLearning.DecisionTrees
{
    public class BinaryDecisionTree
    {
        private BinaryDecisionTree LeftChild { get; set; }
        private BinaryDecisionTree RightChild { get; set; }
        private int Value { get; set; }

        public BinaryDecisionTree()
        { }

        public BinaryDecisionTree(int value)
        {
            Value = value;
        }

        public BinaryDecisionTree(int value, BinaryDecisionTree lft, BinaryDecisionTree rgt)
        {
            Value = value;
            LeftChild = lft;
            RightChild = rgt;
        }

        public static BinaryDecisionTree FromFormula(Formula f) 
        {
            return TreeBuilder(f, f.Variables(), 0, "");
        }

        private static BinaryDecisionTree TreeBuilder(Formula f, IEnumerable<Variable> variables, int varIndex, string path)
        {
            if (!string.IsNullOrEmpty(path))
                variables.ElementAt(varIndex - 1).Value = path[path.Length - 1] != '0';

            if (varIndex == variables.Count())
                return new BinaryDecisionTree(f.Evaluate() ? 1 : 0);

            return new BinaryDecisionTree(varIndex, TreeBuilder(f, variables, varIndex + 1, path + "0"), 
                                                    TreeBuilder(f, variables, varIndex + 1, path + "1"));
        }

    }
}
