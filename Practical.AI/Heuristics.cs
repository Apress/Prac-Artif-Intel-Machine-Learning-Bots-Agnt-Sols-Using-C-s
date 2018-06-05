using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Practical.AI.PropositionalLogic;

namespace Practical.AI
{
    public class Heuristics
    {
        public static Formula ChooseLiteral(Cnf cnf)
        {
            return cnf.Clauses.First().Literals.First();
        }
    }
}
