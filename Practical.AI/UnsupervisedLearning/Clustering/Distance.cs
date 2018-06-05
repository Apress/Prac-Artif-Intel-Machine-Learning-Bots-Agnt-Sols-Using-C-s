using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.UnsupervisedLearning.Clustering
{
    public class Distance
    {
        public static double Euclidean(List<double> a, List<double> b)
        {
            var result = 0.0;

            for (var i = 0; i < a.Count; i++)
                result += Math.Pow(a[i] - b[i], 2);

            return Math.Sqrt(result);
        }
    }
}
