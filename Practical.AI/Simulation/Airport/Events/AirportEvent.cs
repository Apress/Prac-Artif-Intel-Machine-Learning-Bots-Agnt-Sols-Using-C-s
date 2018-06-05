using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace Practical.AI.Simulation.Airport.Events
{
    public abstract class AirportEvent<T> where T: IComparable
    {
        protected double[] Parameters;
        protected List<Tuple<T, T>> Frames;
        public double[] DistributionValues;
        public List<IDistribution> Distributions;

        protected AirportEvent(params double[] lambdas)
        {
            Distributions = new List<IDistribution>();
            DistributionValues = new double[lambdas.Length];
            Frames = new List<Tuple<T, T>>();
            Parameters = lambdas;
        } 

        public virtual void SetDistributionValues(DistributionType type)
        {
            // Setting every distribution as Poisson variables
            foreach (var lambda in Parameters)
            {
                switch (type)
                {
                    case DistributionType.Poisson:
                        Distributions.Add(new Poisson(lambda));
                        break;
                    case DistributionType.Exponential:
                        Distributions.Add(new Exponential(lambda));
                        break;
                }
            }
            // Sampling distributions
            for (var i = 0; i < Frames.Count; i++)
                DistributionValues[i] = type == DistributionType.Poisson 
                                        ? ((Poisson)Distributions[i]).Sample()
                                        : (1 - ((Exponential) Distributions[i]).Sample()) * Parameters[i];
        }


        public virtual double GetEvtFrequency(T elem)
        {
            for (var i = 0; i < Frames.Count; i++)
            {
                if (elem.CompareTo(Frames[i].Item1) >= 0 && elem.CompareTo(Frames[i].Item2) < 0)
                    return DistributionValues[i];
            }

            return -1;
        }
    }

    public enum DistributionType
    {
        Exponential, Poisson
    }
}
