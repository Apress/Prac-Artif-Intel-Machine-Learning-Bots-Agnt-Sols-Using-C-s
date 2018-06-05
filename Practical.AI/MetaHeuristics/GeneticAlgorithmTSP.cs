using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.MetaHeuristics
{
    public class GeneticAlgorithmTsp
    {
        public int Iterations { get; set; }
        public Tsp Tsp { get; set; }
        public List<Solution> Population { get; set; }
        public int Size;
        private static readonly Random Random = new Random();

        public GeneticAlgorithmTsp(int iterations, Tsp tsp, int size)
        {
            Iterations = iterations;
            Tsp = tsp;
            Population = new List<Solution>();
            Size = size;
        }

        public Solution Execute()
        {
            InitialPopulation();
            var i = 0;

            while (i < Iterations)
            {
                var selected = Selection();
                var offSprings = OffSprings(selected as List<Solution>);

                NewPopulation(offSprings);
                i++;
            }

            return Population.First();
        }

        private void NewPopulation(IEnumerable<Solution> offSprings)
        {
            Population.AddRange(offSprings);
            Population.Sort((solutionA, solutionB) => solutionA.Fitness >= solutionB.Fitness ? 1 : -1);
            Population = Population.GetRange(0, Size);
        }

        private IEnumerable<Solution> OffSprings(List<Solution> selected)
        {
            var result = new List<Solution>();

            for (var i = 0; i < selected.Count - 1; i++)
            {
                result.Add(Random.NextDouble() <= 0.4
                               ? selected[i].Mutate(Random)
                               : selected[i].CrossOver(Random, selected[Random.Next(0, selected.Count)]));
            }

            return result;
        }

        private IEnumerable<Solution> Selection()
        {
            Population.Sort((solutionA, solutionB) => solutionA.Fitness >= solutionB.Fitness ? 1 : -1);
            return Population.GetRange(0, Size / 2);
        }

        private void InitialPopulation()
        {
            var i = 0;

            while (i < Size)
            {
                Population.Add(RandomSolution(Tsp.Map.GetLength(0)));
                i++;
            }
        }
        
        private Solution RandomSolution(int n)
        {
            var result = new List<int>();
            var range = Enumerable.Range(0, n).ToList();

            while (range.Count > 0)
            {
                var index = Random.Next(0, range.Count);
                result.Add(range[index]);
                range.RemoveAt(index);
            }

            return new Solution(result);
        } 
    }

    public class Tsp
    {
        public static double[,] Map { get; set; }

        public Tsp(double [,] map)
        {
            Map = map;
        }

        public static void Evaluate(Solution solution)
        {
            var result = 0.0;

            for (var i = 0; i < solution.Ordering.Count - 1; i++)
                result += Map[solution.Ordering[i], solution.Ordering[i + 1]];

            solution.Fitness = result;
        }
    }

    public class Solution
    {
        public List<int> Ordering { get; set; }
        public double Fitness { get; set; }

        public Solution(IEnumerable<int> ordering)
        {
            Ordering = new List<int>(ordering);
            Tsp.Evaluate(this);
        }

        public Solution Mutate(Random random)
        {
            var i = random.Next(0, Ordering.Count);
            var j = random.Next(0, Ordering.Count);

            if (i == j)
                return this;

            var newOrdering = new List<int>(Ordering);
            var temp = newOrdering[i];
            newOrdering[i] = newOrdering[j];
            newOrdering[j] = temp;

            return new Solution(newOrdering);
        }

        public Solution CrossOver(Random random, Solution solution)
        {
            var ordinal = Ordinal();
            var ordinalSol = solution.Ordinal();

            var parentA = new List<int>(ordinal);
            var parentB = new List<int>(ordinalSol);
            var cut = parentA.Count/2;

            var firstHalf = parentA.GetRange(0, cut);
            var secondHalf = parentB.GetRange(cut, parentB.Count - cut);

            firstHalf.AddRange(secondHalf);
            return DecodeOrdinal(firstHalf);
        }

        public List<int> Ordinal()
        {
            var result = new List<int>();
            var canonic = new List<int>(Canonic);

            foreach (var currentVal in Ordering)
            {
                var indexCanonical = canonic.IndexOf(currentVal);
                result.Add(indexCanonical);
                canonic.RemoveAt(indexCanonical);
            }

            return result;
        }

        public Solution DecodeOrdinal(List<int> ordinal)
        {
            var result = new List<int>();
            var canonic = new List<int>(Canonic);

            for (var i = 0; i < ordinal.Count; i++)
            {
                var indexCanonical = ordinal[i];
                result.Add(canonic[indexCanonical]);
                canonic.RemoveAt(indexCanonical);
            }

            return new Solution(result);
        }

        public List<int> Canonic
        {
            get { return Enumerable.Range(0, Ordering.Count).ToList(); }
        } 
    }
}
