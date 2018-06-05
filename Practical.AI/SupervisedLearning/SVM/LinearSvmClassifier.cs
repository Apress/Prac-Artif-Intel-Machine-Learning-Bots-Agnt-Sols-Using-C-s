using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Accord.Math.Optimization;
using Practical.AI.SupervisedLearning.Extensors;
using Practical.AI.SupervisedLearning.SVM;

namespace Practical.AI.SupervisedLearning.SVM
{
    public class LinearSvmClassifier
    {
        public List<TrainingSample> TrainingSamples { get; set; }
        public double[] Weights;
        public double Bias;
        public List<Tuple<double, double>> SetA { get; set; }
        public List<Tuple<double, double>> SetB { get; set; }
        public List<Tuple<double, double>> Hyperplane { get; set; }
        public int ModelToUse = 1;
        private readonly double[] _alphas;
        private const double C = 0.5;
        private const double Epsilon = 0.001;
        private const double Tolerance = 0.001;

        public LinearSvmClassifier(IEnumerable<TrainingSample> trainingSamples)
        {
            TrainingSamples = new List<TrainingSample>(trainingSamples);
            Weights = new double[TrainingSamples.First().Features.Length];
            SetA = new List<Tuple<double, double>>();
            SetB = new List<Tuple<double, double>>();
            Hyperplane = new List<Tuple<double, double>>();
            _alphas = new double[TrainingSamples.Count];
        }

        public void Training()
        {
            var coefficients = new Dictionary<Tuple<int, int>, double>();
            ModelToUse = 1;

            for (var i = 0; i < TrainingSamples.Count; i++)
            {
                for (var j = 0; j < TrainingSamples.Count; j++)
                    coefficients.Add(new Tuple<int, int>(i, j),
                                     -(1) * TrainingSamples[i].Classification * TrainingSamples[j].Classification *
                                     TrainingSamples[i].Features.Dot(TrainingSamples[j].Features));
            }

            var q = new double[TrainingSamples.Count, TrainingSamples.Count];
            q.SetInitValue(coefficients);

            var d = Enumerable.Repeat(1.0, TrainingSamples.Count).ToArray();
            var objective = new QuadraticObjectiveFunction(q, d);
            
            // sum(ai * yi) = 0
            var constraints = new List<LinearConstraint>
                                  {
                                      new LinearConstraint(d)
                                          {
                                              VariablesAtIndices = Enumerable.Range(0, TrainingSamples.Count).ToArray(),
                                              ShouldBe = ConstraintType.EqualTo,
                                              Value = 0,
                                              CombinedAs = TrainingSamples.Select(t => t.Classification).ToArray().ToDouble()
                                          }
                                  };

            // 0 <= ai <= C
            for (var i = 0; i < TrainingSamples.Count; i++)
            {
                constraints.Add(new LinearConstraint(1)
                                    {
                                        VariablesAtIndices = new[] { i },
                                        ShouldBe = ConstraintType.GreaterThanOrEqualTo,
                                        Value = 0
                                    });
            }

            var solver = new GoldfarbIdnani(objective, constraints);

            if (solver.Maximize())
            {
                var solution = solver.Solution;
                UpdateWeightVector(solution);
                UpdateBias();
            }
            else
                Console.WriteLine("Error ...");
        }

        public void TrainingBySmo()
        {
            var numChanged = 0;
            var examAll = true;
            ModelToUse = -1;

            while (numChanged > 0 || examAll)
            {
                numChanged = 0;
                if (examAll)
                {
                    for (var i = 0; i < TrainingSamples.Count; i++)
                        numChanged += ExamineExample(i) ? 1 : 0;
                }
                else
                {
                    var subset = _alphas.GetIndicesFromValues(0, C);
                    foreach (var i in subset)
                        numChanged += ExamineExample(i) ? 1 : 0;
                }

                if (examAll)
                    examAll = false;
                else if (numChanged == 0)
                    examAll = true;
            }
        }

        private bool ExamineExample(int i1)
        {
            var yi = TrainingSamples[i1].Classification;
            var ai = _alphas[i1];
            var errorI = LFunctionValue(i1) - yi;

            var ri = yi * errorI;

            if ((ri < -Tolerance && ai < C) ||
            (ri > Tolerance && ai > 0))
            {
                for (var i2 = 0; i2 < TrainingSamples.Count; i2++)
                    if (TakeStep(i1, i2))
                        return true;
            }

            return false;
        }

        private bool TakeStep(int i, int j)
        {
            if (i == j)
                return false;

            var yi = TrainingSamples[i].Classification;
            var yj = TrainingSamples[j].Classification;

            // Checking bounds on aj
            var s = yi*yj;
            var errorI = LFunctionValue(i) - yi;

            // Computing L, H
            var l = Math.Max(0, _alphas[j] + _alphas[i] * s - (s + 1) / 2 * C);
            var h = Math.Min(C, _alphas[j] + _alphas[i] * s - (s - 1) / 2 * C);

            if (l == h)
                return false;

            double newAj;

            // Obtaining new value for aj
            var k12 = Kernel.Polynomial(2, TrainingSamples[i].Features, TrainingSamples[j].Features);
            var k11 = Kernel.Polynomial(2, TrainingSamples[i].Features, TrainingSamples[i].Features);
            var k22 = Kernel.Polynomial(2, TrainingSamples[j].Features, TrainingSamples[j].Features);
            var eta = 2*k12 - k11 - k22;
            var errorJ = LFunctionValue(j) - yj;

            if (eta < 0)
            {
                newAj = _alphas[j] - TrainingSamples[j].Classification*(errorI - errorJ)/eta;
                if (newAj < l)
                    newAj = l;
                else if (newAj > h)
                    newAj = h;
            }
            else
            {
                var c1 = eta/2;
                var c2 = yj * (errorI - errorJ) - eta * _alphas[j];
                var lObj = c1*Math.Pow(l, 2) + c2*l;
                var hObj = c1*Math.Pow(h, 2) + c2*h;

                if (lObj > hObj + Epsilon)
                    newAj = l;
                else if (lObj < hObj - Epsilon)
                    newAj = h;
                else
                    newAj = _alphas[j];
            }

            if (Math.Abs(newAj - _alphas[j]) < Epsilon * (newAj + _alphas[j] + Epsilon))
                return false;

            var newAi = _alphas[i] - s * (newAj - _alphas[j]);
            if (newAi < 0)
            {
                newAj += s*newAi;
                newAi = 0;
            }
            else if (newAi > C)
            {
                newAj += s * (newAi - C);
                newAi = C;
            }

            // Updating bias & weight vector
            UpdateBias(newAi, _alphas[i], newAj, _alphas[j], yi, yj, errorI, errorJ, k11, k12, k22);
            UpdateWeightVector(i, j, newAi, _alphas[i], newAj, _alphas[j], yi, yj);

            _alphas[i] = newAi;
            _alphas[j] = newAj;

            return true;
        }

        private void UpdateWeightVector(int i, int j, double newAi, double oldAi, 
        double newAj, double oldAj, double yi, double yj)
        {
            var t1 = yi * (newAi - oldAi);
            var t2 = yj * (newAj - oldAj);
            var objI = TrainingSamples[i].Features;
            var objJ = TrainingSamples[j].Features;

            for (var k = 0; k < objI.Length; k++)
                Weights[k] += t1 * objI[k] + t2 * objJ[k];
        }

        private void UpdateWeightVector(double [] alphas)
        {
            var len = TrainingSamples.First().Features.Length;

            for (var i = 0; i < len; i++)
            {
                for (var j = 0; j < TrainingSamples.Count; j++)
                    Weights[i] += TrainingSamples[j].Classification*alphas[j]*
                                  TrainingSamples[j].Features[i];
            }
        }

        private void UpdateBias()
        {
            var x = TrainingSamples.First().Features;
            Bias = 1;
            
            for (var i = 0; i < x.Length; i++)
                Bias -= Weights[i] * x[i];
        }

        private void UpdateBias(double newAi, double oldAi, double newAj, 
        double oldAj, double yi, double yj, double errorI, double errorJ, 
        double k11, double k12, double k22)
        {
            double b1, b2, bNew;

            if (newAi > 0 && newAi < C)
                bNew = Bias + errorI + yi*(newAi - oldAi)*k11 + yj*(newAj - oldAj)*k12;
            else
            {
                if (newAj > 0 && newAj < C)
                    bNew = Bias + errorJ + yi * (newAi - oldAi) * k12 + yj * (newAj - oldAj) * k22;
                else
                {
                    b1 = Bias + errorI + yi * (newAi - oldAi) * k11 + yj * (newAj - oldAj) * k12;
                    b2 = Bias + errorJ + yi * (newAi - oldAi) * k12 + yj * (newAj - oldAj) * k22;
                    bNew = (b1 + b2)/2;
                }
            }

            var deltaB = bNew - Bias;
            Bias = bNew;
        }

        private double LFunctionValue(int i)
        {
            var result = 0.0;

            for (int k = 0; k < TrainingSamples[i].Features.Length; k++)
                result += Weights[k] * TrainingSamples[i].Features[k];
            
            result -= Bias;
            return result;
        }

        public void Predict(IEnumerable<double[]> elems)
        {
            var roundWeights = Weights.RoundValues(2).ToArray();
            var roundBias = new [] {Bias}.RoundValues(2).ToArray();

            foreach (var e in elems) 
            {
                var @class = Math.Sign(e.Dot(roundWeights) +  ModelToUse * roundBias.First());
                if (@class >= 1)
                    SetA.Add(new Tuple<double, double>(e[0], e[1]));
                else if (@class <= -1)
                    SetB.Add(new Tuple<double, double>(e[0], e[1]));
                else
                    Hyperplane.Add(new Tuple<double, double>(e[0], e[1]));
            }
        }
    }

    public class TrainingSample
    {
        public int Classification { get; set; }
        public List<double> Classifications { get; set; }
        public double[] Features { get; set; }

        public TrainingSample(double [] features, int classification, double[] clasifications = null )
        {
            Features = new double[features.Length];
            Array.Copy(features, Features, features.Length);
            Classification = classification;
            if (clasifications != null)
                Classifications = new List<double>(clasifications);
        }
    }
}
