using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.UnsupervisedLearning.Clustering.Methods
{
    public class KMeans
    {
        public int K { get; set; }
        public DataSet DataSet { get; set; }
        public List<Cluster> Clusters { get; set; }
        private static Random _random;
        private const int MaxIterations = 100;

        public KMeans(int k, DataSet dataSet)
        {
            K = k;
            DataSet = dataSet;
            Clusters = new List<Cluster>();
            _random = new Random();
        }

        public void Start()
        {
            InitializeCentroids();
            var i = 0;

            while (i < MaxIterations)
            {
                foreach (var obj in DataSet.Objects)
                {
                    var newCluster = MinDistCentroid(obj);
                    var oldCluster = obj.Cluster;
                    Clusters[newCluster].Add(obj);
                    if (oldCluster >= 0)
                        Clusters[oldCluster].Remove(obj);
                }

                UpdateCentroids();
                i++;
            }
        }

        private void InitializeCentroids()
        {
            RandomCentroids();
        }

        private void RandomCentroids()
        {
            var indices = Enumerable.Range(0, DataSet.Objects.Count).ToList();
            Clusters.Clear();

            for (var i = 0; i < K; i++)
            {
                var objIndex = _random.Next(0, indices.Count);
                Clusters.Add(new Cluster(DataSet.Objects[objIndex].Features, i));
                indices.RemoveAt(objIndex);
            }
        }

        private int MinDistCentroid(Element e)
        {
            var distances = new List<double>();

            for (var i = 0; i < Clusters.Count; i++)
                distances.Add(Distance.Euclidean(Clusters[i].Centroid.Features, e.Features));

            var minDist = distances.Min();
            return distances.FindIndex(0, d => d == minDist);
        }

        private void UpdateCentroids()
        {
            foreach (var cluster in Clusters)
                cluster.CalculateCentroid();
        } 
    }
}
