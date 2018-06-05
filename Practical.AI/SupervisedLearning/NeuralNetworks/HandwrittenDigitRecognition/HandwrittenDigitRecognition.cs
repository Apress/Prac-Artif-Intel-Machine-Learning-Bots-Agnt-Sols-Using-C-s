using System.Collections.Generic;
using Practical.AI.SupervisedLearning.SVM;

namespace Practical.AI.SupervisedLearning.NeuralNetworks.HandwrittenDigitRecognition
{
    public class HandwrittenDigitRecognitionNn : MultiLayerNetwork
    {
        public HandwrittenDigitRecognitionNn(IEnumerable<TrainingSample> trainingDataSet, int inputs, int hiddenUnits, int outputs, double learningRate) 
            :base(trainingDataSet, inputs, hiddenUnits, outputs, learningRate)
        {
        }
    }
}
