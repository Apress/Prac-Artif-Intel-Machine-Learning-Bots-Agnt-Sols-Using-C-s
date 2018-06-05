using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Practical.AI.SupervisedLearning.SVM;

namespace Practical.AI.SupervisedLearning.NeuralNetworks
{
    public class Perceptron : SingleNeuralNetwork
    {
        public Perceptron(IEnumerable<TrainingSample> trainingSamples, int inputs, double learningRate)
            : base(trainingSamples, inputs, learningRate)
        { }

        public override void Training()
        {
            while (true)
            {
                var missclasification = false;
                
                foreach (var trainingSample in TrainingSamples)
                {
                    var output = Predict(trainingSample.Features);
                    var features = trainingSample.Features;
                    if (output != trainingSample.Classification)
                    {
                        missclasification = true;
                        for (var j = 0; j < Inputs; j++)
                            Weights[j] += LearningRate*(trainingSample.Classification - output)*features[j];
                        Bias += LearningRate * (trainingSample.Classification - output);
                    }
                }

                if (!missclasification)
                    break;
            }
        }
    }
}
