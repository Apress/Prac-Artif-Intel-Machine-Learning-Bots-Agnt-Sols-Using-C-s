using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Practical.AI.SupervisedLearning.SVM;

namespace Practical.AI.SupervisedLearning.NeuralNetworks
{
    public class TanhUnit : SingleNeuralNetwork
    {
        public double ActivationValue { get; set; }
        public double ErrorTerm { get; set; }

        public TanhUnit(IEnumerable<TrainingSample> trainingSamples, int inputs, double learningRate)
            : base(trainingSamples, inputs, learningRate)
        { }

        public override double Predict(double [] features)
        {
            var result = 0.0;

            for (var i = 0; i < features.Length; i++)
                result += features[i] * Weights[i];
            
            ActivationValue = Math.Tanh(result);
            return ActivationValue;
        }
    }
}
