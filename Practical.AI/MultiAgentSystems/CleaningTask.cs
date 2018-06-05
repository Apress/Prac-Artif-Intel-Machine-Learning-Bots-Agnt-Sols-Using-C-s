using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.MultiAgentSystems
{
    public class CleaningTask
    {
        public int Count { get; set; }
        public int M { get; set; }
        public List<Tuple<int, int>> SubDivide { get; set; }
        public IEnumerable<string> SubTasks { get; set; } 

        public CleaningTask(int m, int agents)
        {
            M = m;
            Count = agents;
            SubDivide = new List<Tuple<int, int>>();
            Divide();
            SubTasks = BuildTasks();
        }

        /// <summary>
        /// For the division we assume that M % Count = 0, i.e.  
        /// the number of columns is always divisible by the number of agents.
        /// </summary>
        private void Divide()
        {
            var div = M / Count;

            for (var i = 0; i < M; i += div)
                SubDivide.Add(new Tuple<int, int>(i, i + div - 1));
        }

        private IEnumerable<string> BuildTasks()
        {
            var result = new string[SubDivide.Count];

            for (var i = 0; i < SubDivide.Count; i++)
                result[i] = "clean(" + SubDivide[i].Item1 + "," + SubDivide[i].Item2 + ")";

            return result;
        }
    }
}
