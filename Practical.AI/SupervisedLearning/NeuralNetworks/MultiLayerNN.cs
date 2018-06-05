using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Practical.AI.SupervisedLearning.SVM;

namespace Practical.AI.SupervisedLearning.NeuralNetworks
{
    public class MultiLayerNetwork
    {
        public List<Layer> Layers { get; set; }
        public List<TrainingSample> TrainingSamples { get; set; }
        public int HiddenUnits { get; set; }
        public int OutputUnits { get; set; }
        public double LearningRate { get; set; }
        private double _maxError;

        public MultiLayerNetwork(IEnumerable<TrainingSample> trainingSamples, int inputs, int hiddenUnits, int outputs, double learningRate)
        {
            TrainingSamples = new List<TrainingSample>(trainingSamples);
            Layers = new List<Layer>();
            LearningRate = learningRate;
            HiddenUnits = hiddenUnits;
            OutputUnits = outputs;

            CreateLayers(inputs);
        }

        public void Training()
        {
            _maxError = double.MaxValue;

            while (Math.Abs(_maxError) > .001)
            {
                foreach (var trainingSample in TrainingSamples)
                {
                    Predict(trainingSample.Features);

                    // Error term for output layer ...
                    for (var i = 0; i < OutPutLayer.Units.Count; i++)
                    {
                        OutPutLayer.Units[i].ErrorTerm = FunctionDerivative(OutPutLayer.Units[i].ActivationValue, TypeFunction.Sigmoid) *
                                         (trainingSample.Classifications[i] - OutPutLayer.Units[i].ActivationValue);
                    }

                    // Error term for hidden layer ...
                    for (var i = 0; i < HiddenLayer.Units.Count; i++)
                    {
                        var outputUnitWeights = OutPutLayer.Units.Select(u => u.Weights[i]).ToList();
                        var product = (from j in Enumerable.Range(0, outputUnitWeights.Count)
                                       select outputUnitWeights[j]*OutPutLayer.Units[j].ErrorTerm).Sum();
                        HiddenLayer.Units[i].ErrorTerm = FunctionDerivative(HiddenLayer.Units[i].ActivationValue, TypeFunction.Sigmoid) * product;
                    }

                    UpdateWeight(trainingSample.Features, OutPutLayer);
                    UpdateWeight(trainingSample.Features, HiddenLayer);
                    _maxError = OutPutLayer.Units.Max(u => Math.Abs(u.ErrorTerm));
                }
            }
        }

        private double FunctionDerivative(double f, TypeFunction function)
        {
            switch (function)
            {
                case TypeFunction.Sigmoid:
                    return f*(1 - f);
                case TypeFunction.Tanh:
                    return 1 - Math.Pow(f, 2);
                case TypeFunction.ReLu:
                    return Math.Max(0, f);
                default:
                    return 0;
            }
        }

        private void UpdateWeight(double[] features, Layer layer)
        {
            var activationValues =
                           layer.Type == TypeofLayer.Hidden ? features : HiddenLayer.Units.Select(u => u.ActivationValue).ToArray();
            
            foreach (var unit in layer.Units)
            {
                for (var i = 0; i < unit.Weights.Count; i++)
                    unit.Weights[i] += LearningRate * unit.ErrorTerm * activationValues[i];
            }
        }

        public double Predict(double[] features)
        {
            for (var i = 0; i < Layers.Count; i++)
            {
                foreach (var unit in Layers[i].Units)
                {
                    var activationValues = 
                        i ==  0 ? features : HiddenLayer.Units.Select(u => u.ActivationValue).ToArray();
                        
                    unit.Predict(activationValues);
                }
            }

            return ReturnIndexByMax();
        }

        private int ReturnIndexByHalf()
        {
            var unit = OutPutLayer.Units.First();
            return unit.ActivationValue < 0.5 ? 0 : 1;
        }

        private int ReturnIndexByMax()
        {
            var max = OutPutLayer.Units.Max(u => u.ActivationValue);
            return OutPutLayer.Units.FindIndex(0, unit => unit.ActivationValue == max);
        }

        private void CreateLayers(int inputs)
        {
            Layers.Add(new Layer(HiddenUnits, TrainingSamples, LearningRate, inputs, TypeofLayer.Hidden));
            Layers.Add(new Layer(OutputUnits, TrainingSamples, LearningRate, HiddenUnits, TypeofLayer.OutPut));
        }

        public List<double> PredictSet(IEnumerable<double[]> objects)
        {
            var result = new List<double>();

            foreach (var obj in objects)
                result.Add(Predict(obj));

            return result;
        }

        public Layer OutPutLayer
        {
            get { return Layers.Last(); }
        }

        public Layer HiddenLayer
        {
            get { return Layers.First(); }
        }
    }

    public class Layer
    {
        public List<SigmoidUnit> Units { get; set; }
        public TypeofLayer Type { get; set; }

        public Layer(int number, List<TrainingSample> trainingSamples, double learningRate, int inputs, TypeofLayer typeofLayer)
        {
            Units = new List<SigmoidUnit>();
            Type = typeofLayer;
            for (var i = 0; i < number; i++)
                Units.Add(new SigmoidUnit(trainingSamples, inputs, learningRate));
        }
    }

    public enum TypeofLayer
    {
        Hidden, OutPut
    }

    public enum TypeFunction
    {
        Sigmoid, Tanh, ReLu
    }
}
