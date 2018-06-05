using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace Practical.AI.Simulation.Airport.Events
{
    public class AirplaneEvtProcessLoad : AirportEvent<int>
    {
        public AirplaneEvtProcessLoad(params double[] lambdas) : base(lambdas)
        {
            Frames = new List<Tuple<int, int>>
                             {
                                 new Tuple<int, int>(0, 150),
                                 new Tuple<int, int>(150, 300),
                                 new Tuple<int, int>(300, 450)
                             };
        }

        public double SampleAt(int elem)
        {
            for (var i = 0; i < Frames.Count; i++)
            {
                if (elem.CompareTo(Frames[i].Item1) >= 0 && elem.CompareTo(Frames[i].Item2) < 0)
                    return  (1 - ((Exponential) Distributions[i]).Sample()) * Parameters[i]; 
            }

            return -1;
        }
    }
}
