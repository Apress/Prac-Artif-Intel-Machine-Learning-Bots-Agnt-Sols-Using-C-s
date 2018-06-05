using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.SupervisedLearning.DecisionTrees
{
    public class TrainingDataSet
    {
        public string [,] Values { get; set; }
        public Attribute GoalAttribute { get; set; }
        public List<Attribute> NonGoalAttributes { get; set; }

        public TrainingDataSet(string [,] values, IEnumerable<Attribute> nonGoal, Attribute goal)
        {
            Values = new string[values.GetLength(0), values.GetLength(1)];
            Array.Copy(values, Values, values.GetLength(0) * values.GetLength(1));
            NonGoalAttributes = new List<Attribute>(nonGoal);
            GoalAttribute = goal;

            if (NonGoalAttributes.Count + 1 != Values.GetLength(1))
                throw new Exception("Number of attributes must coincide");
        }
    }
}

