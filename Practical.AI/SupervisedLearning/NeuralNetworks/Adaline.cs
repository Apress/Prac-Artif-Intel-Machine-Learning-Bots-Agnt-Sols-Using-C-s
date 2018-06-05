using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Practical.AI.SupervisedLearning.SVM;

namespace Practical.AI.SupervisedLearning.NeuralNetworks
{
    public class Adaline : SingleNeuralNetwork
    {
        public Adaline(IEnumerable<TrainingSample> trainingSamples, int inputs, double learningRate)
            : base(trainingSamples, inputs, learningRate)
        { }

        public override void Training()
        {
            double error;
          
            do
            {
                error = 0.0;

                foreach (var trainingSample in TrainingSamples)
                {
                    var output = LinearFunction(trainingSample.Features);
                    var errorT = Math.Pow(trainingSample.Classification - output, 2);

                    if (Math.Abs(errorT) < 0.001)
                        continue;

                    for (var j = 0; j < Inputs; j++)
                        Weights[j] +=  LearningRate * (trainingSample.Classification - output) * trainingSample.Features[j];
                    
                    error = Math.Max(error, Math.Abs(errorT));
                }
            }
            while (error > 0.25);
        }

        public double LinearFunction(double [] values)
        {
            return (from i in Enumerable.Range(0, Weights.Count)
                             select Weights[i] * values[i]).Sum();
        }

        public override double Predict(double[] features)
        {
            var sum = LinearFunction(features);
            return sum > 0.5 ? 1 : 0;
        }
    }
}
