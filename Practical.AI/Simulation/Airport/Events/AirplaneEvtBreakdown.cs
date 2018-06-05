using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace Practical.AI.Simulation.Airport.Events
{
    public class AirplaneEvtBreakdown : AirportEvent<TimeSpan>
    {
        public AirplaneEvtBreakdown(params double[] lambdas)
        {
            Distributions = new List<IDistribution>();
            DistributionValues = new double[lambdas.Length];
            Parameters = lambdas;
        }
    }
}
