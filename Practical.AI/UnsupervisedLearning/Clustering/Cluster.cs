using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.UnsupervisedLearning.Clustering
{
    public class Cluster
    {
        public List<Element> Objects { get; set; }
        public Element Centroid { get; set; }
        public int ClusterNo { get; set; }

        public Cluster()
        {
            Objects = new List<Element>();
            Centroid = new Element();
        }

        public Cluster(IEnumerable<double> centroid, int clusterNo)
        {
            Objects = new List<Element>();
            Centroid = new Element(centroid);
            ClusterNo = clusterNo;
        }

        public void Add(Element e)
        {
            Objects.Add(e);
            e.Cluster = ClusterNo;
        }

        public void Remove(Element e)
        {
            Objects.Remove(e);
        }

        public void CalculateCentroid()
        {
            var result = new List<double>();
            var toAvg = new List<Element>(Objects);
            var total = Total;
            if (Objects.Count == 0)
            {
                toAvg.Add(Centroid);
                total = 1;
            }

            var dimension = toAvg.First().Features.Count;

            for (var i = 0; i < dimension; i++)
                result.Add(toAvg.Select(o => o.Features[i]).Sum() / total);

            Centroid.Features = new List<double>(result);
        }

        public double AverageLinkageClustering(Cluster c)
        {
            var result = 0.0;

            foreach (var c1 in c.Objects)
                result += Objects.Sum(c2 => Distance.Euclidean(c1.Features, c2.Features));
            
            return result / (Total + c.Total);
        }

        public int Total
        {
            get { return Objects.Count; }
        }
    }

    public class Element
    {
        public List<double> Features { get; set; }
        public int Cluster { get; set; }

        public Element(int cluster = -1)
        {
            Features = new List<double>();
            Cluster = cluster;
        }

        public Element(IEnumerable<double> features)
        {
            Features = new List<double>(features);
            Cluster = -1;
        }
    }
}
