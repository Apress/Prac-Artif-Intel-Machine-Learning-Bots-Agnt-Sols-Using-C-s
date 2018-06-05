using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.Predicates
{
    public class Dog
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public Gender Sex { get; set; }

        public Dog(string name, double weight, Gender sex)
        {
            Name = name;
            Weight = weight;
            Sex = sex;
        }
    }

    public enum Gender {
         Male, Female
    }
}
