using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Accord.Math;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Practical.AI.SupervisedLearning.Extensors;

namespace Practical.AI.SupervisedLearning.DecisionTrees
{
    public class DecisionTree
    {
        public TrainingDataSet DataSet { get; set; }
        public string Value { get; set; }
        public List<DecisionTree> Children { get; set; }
        public string Edge { get; set; }

        public DecisionTree(TrainingDataSet dataSet)
        {
            DataSet = dataSet;
        }

        public static DecisionTree Learn(TrainingDataSet dataSet, DtTrainingAlgorithm algorithm)
        {
            if (dataSet == null)
                throw new Exception("Data Set cannot be null");

            switch (algorithm)
            {
                default:
                    return Id3(dataSet.Values, dataSet.NonGoalAttributes, "root");
            }
        }

        public DecisionTree(string value, string edge)
        {
            Value = value;
            Children = new List<DecisionTree>();
            Edge = edge;
        }

        public void Visualize()
        {
            var form = new Form();
            //create a viewer object 
            var viewer = new GViewer();
            //create a graph object 
            var graph = new Graph("Decision Tree");
            //create the graph content 

            CreateNodes(graph);

            //bind the graph to the viewer 
            viewer.Graph = graph;
            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();

            //show the form 
            form.ShowDialog();
        }

        private void CreateNodes(Graph graph)
        {
            var queue = new Queue<DecisionTree>();
            queue.Enqueue(this);
            graph.CreateLayoutSettings().EdgeRoutingSettings.EdgeRoutingMode = EdgeRoutingMode.StraightLine;
            var id = 0;

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                Node firstEnd;
                if (graph.Nodes.Any(n => n.LabelText == currentNode.Value))
                    firstEnd = graph.Nodes.First(n => n.LabelText == currentNode.Value);
                else
                    firstEnd = new Node((id++).ToString()) { LabelText = currentNode.Value };
                graph.AddNode(firstEnd);
               
                foreach (var decisionTree in currentNode.Children)
                {
                    var secondEnd = new Node((id++).ToString()) { LabelText = decisionTree.Value };
                    graph.AddNode(secondEnd);
                    graph.AddEdge(firstEnd.Id, decisionTree.Edge, secondEnd.Id);
                    queue.Enqueue(decisionTree);
                }
            }
        }

        public static DecisionTree Id3(string [,] values, List<Attribute> attributes, string edge)
        {
            // All training data has the same goal attribute
            var goalValues = values.GetColumn(values.GetLength(1) - 1);
            if (goalValues.DistinctCount() == 1)
                return new DecisionTree(goalValues.First(), edge);
            
            // There are no NonGoal attributes
            if (attributes.Count == 0) 
                return new DecisionTree(goalValues.GetMostFrequent(), edge);
            
            // Set as root the attribute providing the highest information gain
            var attrIndexPair = HighestGainAttribute(values, attributes);
            var attr = attrIndexPair["attrib"] as Attribute;
            var attrIndex = (int) attrIndexPair["index"];
            var threshold = (int) attrIndexPair["threshold"];
            var less = (List<int>) attrIndexPair["less"];
            var greater = (List<int>) attrIndexPair["greater"];
            var root = new DecisionTree(attr.Name, edge);
            var splittingVals = attr.TypeVal == TypeVal.Discrete ? attr.Values 
                                              : new [] { "less " + threshold, "greater " + threshold } ;

            foreach (var value in splittingVals)
            {
                List<int> subSetVi;

                if (attr.TypeVal == TypeVal.Discrete)
                    subSetVi = values.GetRowIndex(attrIndex, value, ComparisonType.Equality);
                else
                    subSetVi = value.Contains("less") ? less : greater;

                if (subSetVi.Count == 0)
                    root.Children.Add(new DecisionTree(goalValues.GetMostFrequent(), value));
                else
                {
                    var newAttrbs = new List<Attribute>(attributes);
                    newAttrbs.RemoveAt(attrIndex);
                    var newValues = values.GetMatrix(subSetVi).RemoveColumn(attrIndex);
                    root.Children.Add(Id3(newValues, newAttrbs, attr.Name + " : " + value));
                }
            }

            return root;
        }

        private static Dictionary<string, dynamic> HighestGainAttribute(string [,] values, IEnumerable<Attribute> attributes)
        {
            Attribute result = null;
            var maxGain = double.MinValue;
            var index = -1;
            double threshold = -1.0;
            var i = 0;
            List<int> bestLess = null;
            List<int> bestGreater = null;

            foreach (var attr in attributes)
            {
                double gain = 0;
                Dictionary<string, dynamic> gainThreshold = null;

                if (attr.TypeVal == TypeVal.Discrete)
                    gain = Gain(values, i);
                if (attr.TypeVal == TypeVal.Continuous)
                {
                    gainThreshold = GainContinuos(values, i);
                    gain = gainThreshold["gain"];
                }

                if (gain > maxGain)
                {
                    maxGain = gain;
                    result = attr;
                    index = i;

                    if (gainThreshold != null)
                    {
                        threshold = gainThreshold["threshold"];
                        bestLess = gainThreshold["less"];
                        bestGreater = gainThreshold["greater"];
                    }
                }
                i++;
            }

            return new Dictionary<string, dynamic> {
                                                       { "attrib" , result },
                                                       { "index" , index },  
                                                       { "less" , bestLess }, 
                                                       { "greater" , bestGreater }, 
                                                       { "threshold" , threshold }, 
                                                   };
        }

        private static Dictionary<string, dynamic> GainContinuos(string[,] values, int i)
        {
            var column = values.GetColumn(i);
            var columnVals = column.Select(double.Parse).ToList();
            var bestGain = double.MinValue;
            var bestThreshold = 0.0;
            List<int> bestLess = null;
            List<int> bestGreater = null;

            columnVals.Sort();

            for (var j = 0; j < columnVals.Count - 1; j++)
            {
                if (columnVals[j] != columnVals[j + 1] && values[j, values.GetLength(1) - 1] != values[j + 1, values.GetLength(1) - 1])
                {
                    var threshold = (columnVals[j] + columnVals[j + 1])/2;
                    var less = values.GetRowIndex(i, threshold.ToString(), ComparisonType.NumericLessThan);
                    var greater = values.GetRowIndex(i, threshold.ToString(), ComparisonType.NumericGreaterThan);

                    var gain = GainRatio(values, i, threshold, less, greater);
                    if (gain > bestGain)
                    {
                        bestGain = gain;
                        bestThreshold = threshold;
                        bestLess = less;
                        bestGreater = greater;
                    }
                }
            }
             
            return new Dictionary<string, dynamic>
                       {
                           { "gain" , bestGain }, 
                           { "threshold" , bestThreshold }, 
                           { "less", bestLess }, 
                           { "greater", bestGreater }, 
                       };
        }

        private static double GainRatio(string[,] values, int attributeIndex, double threshold = -1, List<int> less = null, List<int> greater = null)
        {
            return Gain(values, attributeIndex, threshold, less, greater) / SplitInformation(values, attributeIndex);
        }

        private static double SplitInformation(string[,] values, int attributeIndex)
        {
            var freq = values.GetFreqPerDistinctElem(attributeIndex);
            var total = freq.Sum(t => t.Value);
            var result = 0.0;

            foreach (var f in freq)
                result += (double)f.Value / total * Math.Log((double)f.Value / total, 2);

            return -result;
        }

        private static double Gain(string [,] values, int attributeIndex, double threshold = -1, List<int> less = null, List<int> greater = null)
        {
            var impurityBeforeSplit = Entropy(values.GetFreqPerDistinctElem(values.GetLength(1) - 1).GetProbabilities());
            double impurityAfterSplit = 0;
            
            if (threshold >= 0)
            {
                var freq = new Dictionary<string, int> {  {"less", less.Count}, {"greater", greater.Count}  };

                for (var i = 0; i < freq.Count; i++)
                    impurityAfterSplit += SubsetEntropy(values, attributeIndex, freq, less, greater);
            }
            else 
                impurityAfterSplit = SubsetEntropy(values, attributeIndex);
            
            return impurityBeforeSplit - impurityAfterSplit;
        }

        private static double Entropy(IEnumerable<double> probs)
        {
            return -1 * probs.Sum(d => LogEntropy(d));
        }

        private static double LogEntropy(double p)
        {
            return p > 0 ? p * Math.Log(p, 2) : 0;
        }

        private static double SubsetEntropy(string[,] values, int columnIndex, Dictionary<string, int> freqContinous = null,
        List<int> less = null, List<int> greater = null)
        {
            var result = 0.0;
            var freqDicc = freqContinous ?? values.GetFreqPerDistinctElem(columnIndex);
            
            var total = freqDicc.Values.Sum();
            
            foreach (var key in freqDicc.Keys)
            {
                List<int> rowIndex;

                switch (key)
                {
                    case "less":
                        rowIndex = less;
                        break;
                    case "greater":
                        rowIndex = greater;
                        break;
                    default:
                        rowIndex = values.GetRowIndex(columnIndex, key, ComparisonType.Equality);
                        break;
                }

                var frequencyPerClass = values.GetFreqPerDistinctElem(values.GetLength(1) - 1, rowIndex.ToArray());
                result += (freqDicc[key] / (double) total) * Entropy(frequencyPerClass.GetProbabilities());
            }

            return result;
         }
    }

    public enum DtTrainingAlgorithm
    {
        Id3, 
    }
}
