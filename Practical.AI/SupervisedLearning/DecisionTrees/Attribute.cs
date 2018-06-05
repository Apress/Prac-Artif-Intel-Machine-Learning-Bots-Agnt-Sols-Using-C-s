using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.SupervisedLearning.DecisionTrees
{
    public class Attribute
    {
        public string Name { get; set; }
        public string[] Values { get; set; }
        public TypeAttrib Type { get; set; }
        public TypeVal TypeVal { get; set; }

        public Attribute(string name, string [] values, TypeAttrib type, TypeVal typeVal)
        {
            Name = name;
            Values = values;
            Type = type;
            TypeVal = typeVal;
        }
    }

    public enum TypeAttrib
    {
        Goal, NonGoal
    }

    public enum TypeVal
    {
        Discrete, Continuous
    }
}
