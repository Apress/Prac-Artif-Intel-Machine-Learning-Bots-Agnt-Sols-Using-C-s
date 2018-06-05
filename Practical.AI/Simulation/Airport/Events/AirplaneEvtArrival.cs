using System;
using System.Collections.Generic;

namespace Practical.AI.Simulation.Airport.Events
{
    public class AirplaneEvtArrival : AirportEvent<TimeSpan>
    {
        public AirplaneEvtArrival(params double[] lambdas) : base(lambdas)
        {
            Frames = new List<Tuple<TimeSpan, TimeSpan>>
                             {
                                 new Tuple<TimeSpan, TimeSpan>(new TimeSpan(0, 6, 0, 0), new TimeSpan(0, 14, 0, 0)),
                                 new Tuple<TimeSpan, TimeSpan>(new TimeSpan(0, 14, 0, 0), new TimeSpan(0, 22, 0, 0)),
                                 new Tuple<TimeSpan, TimeSpan>(new TimeSpan(0, 22, 0, 0), new TimeSpan(0, 6, 0, 0))
                             };
        }
    }
}
