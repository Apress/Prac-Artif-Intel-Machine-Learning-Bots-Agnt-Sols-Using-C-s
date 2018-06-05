using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Practical.AI.UnsupervisedLearning.Clustering;

namespace Practical.AI.UnsupervisedLearning
{
    public class DataSet
    {
        public List<Element> Objects { get; set; }
 
        public DataSet()
        {
            Objects = new List<Element>();
        }

        public void Load(List<Element> objects)
        {
            Objects = new List<Element>(objects);
        }
    }
}
