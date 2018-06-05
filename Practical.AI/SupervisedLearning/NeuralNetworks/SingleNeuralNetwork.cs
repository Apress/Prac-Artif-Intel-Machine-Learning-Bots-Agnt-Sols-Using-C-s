using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Practical.AI.SupervisedLearning.SVM;

namespace Practical.AI.SupervisedLearning.NeuralNetworks
{
    public abstract class SingleNeuralNetwork
    {
        public List<TrainingSample> TrainingSamples { get; set; }
        public int Inputs { get; set; }
        public List<double> Weights { get; set; }
        public static readonly Random Random = new Random();
        protected readonly double LearningRate;
        protected double Bias = 0.5;

        protected SingleNeuralNetwork(IEnumerable<TrainingSample> trainingSamples, int inputs, double learningRate)
        {
            TrainingSamples = new List<TrainingSample>(trainingSamples);
            Inputs = inputs;
            Weights = new List<double>();
            for (var i = 0; i < Inputs; i++)
                Weights.Add(Random.NextDouble() - 0.5);
            LearningRate = learningRate;
        }

        public virtual void Training()
        {
        }

        public virtual double Predict(double[] features)
        {
            var result = 0.0;

            for (var i = 0; i < features.Length; i++)
                result += features[i] * Weights[i];

            return result > -Bias ? 1 : 0;
        }

        public List<double> PredictSet(IEnumerable<double[]> objects)
        {
            var result = new List<double>();

            foreach (var obj in objects)
                result.Add(Predict(obj));

            return result;
        }
    }
}
