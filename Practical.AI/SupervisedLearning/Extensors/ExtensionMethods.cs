using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace Practical.AI.SupervisedLearning.Extensors
{
    public static class ExtensionMethods
    {
        public static void SetInitValue(this double[,] q, Dictionary<Tuple<int, int>, double> coefficients, double epsilon = 0.000001)
        {
            for (var i = 0; i < q.GetLength(0); i++)
            {
                for (var j = 0; j < q.GetLength(1); j++)
                {
                    q[i, j] = coefficients[new Tuple<int, int>(i, j)];
                    if (i == j)
                        q[i, j] -= epsilon;
                }
            }
        }

        public static IEnumerable<int> GetIndicesFromValues(this double[] toCompare, params double[] values)
        {
            var result = new List<int>();

            for (var i = 0; i < toCompare.Length; i++)
                if (values.Contains(toCompare[i]))
                    result.Add(i);

            return result;
        }

        public static IEnumerable<double> RoundValues(this double[] list, int decimals)
        {
            var result = new double[list.Length];

            for (var i = 0; i < list.Length; i++)
                result[i] = Math.Round(list[i], decimals);

            return result;
        }
    
        public static IEnumerable<string> GetColumn(this double[,] matrix, int columnIndex)
        {
            var result = new List<string>();

            for (var i = 0; i < matrix.GetLength(0); i++)
                result.Add(matrix[i, columnIndex].ToString());
            
            return result;
        }

        public static string GetMostFrequent(this string[] values)
        {
            var dicc = new Dictionary<string, int>();

            foreach (var v in values)
            {
                if (!dicc.ContainsKey(v))
                    dicc.Add(v, 1);
                else
                    dicc[v] += 1;
            }

            var maxVal = dicc.Max(e => e.Value); 
            return dicc.First(p => p.Value == maxVal).Key;
        }
    
        public static Dictionary<string, int> GetFreqPerDistinctElem(this string [,] values, int columnIndex, int [] rowIndex = null )
        {
            var freqDicc = new Dictionary<string, int>();

            for (var i = 0; i < (rowIndex == null ? values.GetLength(0) : rowIndex.Length); i++)
            {
                var row = rowIndex == null ? i : rowIndex[i];
                if (!freqDicc.ContainsKey(values[row, columnIndex]))
                    freqDicc.Add(values[row, columnIndex], 1);
                else
                    freqDicc[values[row, columnIndex]] += 1;
            }

            return freqDicc;
        }

        public static List<int> GetRowIndex(this string[,] values, int columnIndex, string toCompare, ComparisonType comparisonType)
        {
            var result = new List<int>();

            for (var i = 0; i < values.GetLength(0); i++)
            {
                switch (comparisonType)
                {
                        case ComparisonType.Equality:
                            if (values[i, columnIndex] == toCompare)
                                result.Add(i);
                            break;
                        case ComparisonType.NumericLessThan:
                            if (double.Parse(values[i, columnIndex]) < double.Parse(toCompare))
                                result.Add(i);
                            break;
                        case ComparisonType.NumericGreaterThan:
                            if (double.Parse(values[i, columnIndex]) > double.Parse(toCompare))
                                result.Add(i);
                            break;
                }
            }

            return result;
        }

        public static string[,] GetMatrix(this string[,] values, List<int> rowIndex)
        {
            var result = new string[rowIndex.Count, values.GetLength(1)];
            var j = 0;

            foreach (var i in rowIndex)
            {
                result.SetRow(j, values.GetRow(i));
                j++;
            }

            return result;
        } 
   
        public static IEnumerable<double> GetProbabilities(this Dictionary<string, int> dicc)
        {
            var probabilities = new List<double>();
            var sum = dicc.Values.Sum();

            foreach (var e in dicc)
                probabilities.Add((e.Value / (double) sum));

            return probabilities;
        } 
    }

    public enum ComparisonType
    {
        Equality, NumericGreaterThan, NumericLessThan
    }
}
