using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace Practical.AI.MetaHeuristics
{
    public class HillClimbing
    {
        public Function Function { get; set; }
        public double Step { get; set; }
        public double Radius { get; set; }
        private static readonly Random Random = new Random();

        public HillClimbing(Function function, double step, double radius)
        {
            Function = function;
            Step = step;
            Radius = radius;
        }

        public List<double> Execute()
        {
            var currentSolution = InitialSolution(Function.getArgumentsNumber());
            var bestEval = double.MaxValue;
            List<double> bestSolution = null;

            while (true)
            {
                var neighbors = Neighborhood(currentSolution, Function.getArgumentsNumber());
                var bestCurrentEval = double.MaxValue;
                List<double> bestCurrentSolution = null;

                foreach (var neighbor in neighbors)
                {
                    var eval = Function.calculate(neighbor.ToArray());
                    if (eval < bestCurrentEval)
                    {
                        bestCurrentEval = eval;
                        bestCurrentSolution = neighbor;
                    }
                }

                if (bestCurrentEval == bestEval)
                    break;

                if (bestCurrentEval < bestEval)
                {
                    bestEval = bestCurrentEval;
                    bestSolution = bestCurrentSolution;
                }
            }

            return bestSolution;
        }

        private List<double> InitialSolution(int dimension)
        {
            var result = new List<double>();

            for (var i = 0; i < dimension; i++)
                result.Add(Random.NextDouble()*100);

            return result;
        } 

        private IEnumerable<List<double>> Neighborhood(List<double> currentSolution, int dimension)
        {
            var result = new List<List<double>>();

            var newSolutions = NSpherePoints(currentSolution, dimension);
            result.AddRange(newSolutions);

            return result;
        }

        private IEnumerable<List<double>> NSpherePoints(List<double> currentSolution, int dimension)
        {
            var result = new List<List<double>>();
            var angles = Enumerable.Repeat(Step, dimension).ToList();

            while (angles.First() < 180)
            {
                for (var i = 0; i < dimension; i++)
                {
                    var newSolution = new List<double>(currentSolution);
                    var prod = 1.0;
                    for (var j = 0; j < i; j++)
                        prod *= Math.Sin(angles[j]);

                    newSolution[i] = i == dimension - 1 && i > 0
                                             ? Radius*(prod)*Math.Sin(angles[i])
                                             : Radius*(prod)*Math.Cos(angles[i]);

                    result.Add(newSolution);
                }
                angles = angles.Select(ang => ang + Step).ToList();
            }

            return result;
        }
    }
}
