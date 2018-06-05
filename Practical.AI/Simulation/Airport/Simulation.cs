using System;
using System.Collections.Generic;
using System.Linq;
using Practical.AI.Simulation.Airport.Events;
using Practical.AI.Simulation.Airport.Objects;

namespace Practical.AI.Simulation.Airport
{
    public class Simulation
    {
        public TimeSpan MaxTime { get; set; }
        private TimeSpan _currentTime;
        private readonly AirplaneEvtArrival _arrivalDistribution;
        private readonly AirplaneEvtProcessLoad _processLoadDistribution;
        private readonly AirplaneEvtBreakdown _airplaneBreakdown;
        private readonly bool [] _runways;
        private readonly int _planeArrivalInterval;
        private readonly Queue<Airplane> _waitingToLand;
        private readonly List<Airplane> _airplanes;
        private List<Airplane> _airplanesOnLand;
        private static readonly Random Random = new Random();

        public Simulation(TimeSpan startTime, TimeSpan maxTime, IEnumerable<Airplane> airplanes)
        {
            MaxTime = maxTime;
            _runways = new bool[5];
            _arrivalDistribution = new AirplaneEvtArrival(7, 10, 20);
            _processLoadDistribution = new AirplaneEvtProcessLoad(50, 60 , 75);
            _airplaneBreakdown = new AirplaneEvtBreakdown(80);
            _waitingToLand = new Queue<Airplane>();
            _airplanes = new List<Airplane>(airplanes);
            _airplanesOnLand = new List<Airplane>();
            _currentTime = startTime;
            // For 1st day set distribution values.
            _arrivalDistribution.SetDistributionValues(DistributionType.Poisson);
            _processLoadDistribution.SetDistributionValues(DistributionType.Exponential);
            _airplaneBreakdown.SetDistributionValues(DistributionType.Exponential);
            _planeArrivalInterval = (int) _arrivalDistribution.GetEvtFrequency(startTime);
        }

        public void Execute()
        {
            while (_currentTime < MaxTime)
            {
                Console.WriteLine(_currentTime);

                // Process airplanes on queue for landing
                foreach (var airplane in _waitingToLand)
                {
                    if (!TryToLand(airplane))
                        break;
                }

                // Plane arrival event
                if (_currentTime.Minutes % _planeArrivalInterval == 0 && _airplanes.Count > 0)
                {
                    var newPlane = _airplanes.First();
                    _airplanes.RemoveAt(0);
                    Console.WriteLine("Plane {0} arriving ...", newPlane.Id);

                    if (TryToLand(newPlane))
                        _airplanesOnLand.Add(newPlane);
                }

                // For updating list of airplanes on the ground
                var newAirplanesOnLand = new List<Airplane>();
                // Update airplane status for this minute
                foreach (var airplane in _airplanesOnLand)
                {
                    airplane.TimeToTakeOff--;
                    if (airplane.TimeToTakeOff <= 0)
                    {
                        _runways[airplane.RunwayOccupied] = false;
                        airplane.RunwayOccupied = -1;
                        Console.WriteLine("Plane {0} took off", airplane.Id);
                    }
                    else
                        newAirplanesOnLand.Add(airplane);

                    // Odds of having a breakdown
                    if (Random.NextDouble() < 0.15 && !airplane.BrokenDown)
                    {
                        airplane.BrokenDown = true;
                        airplane.TimeToTakeOff += _airplaneBreakdown.DistributionValues.First();
                        Console.WriteLine("Plane {0} broke down, take off time is now {1} mins", airplane.Id, Math.Round(airplane.TimeToTakeOff, 2));
                    }
                }

                _airplanesOnLand = new List<Airplane>(newAirplanesOnLand);

                // Add a minute
                _currentTime = _currentTime.Add(new TimeSpan(0, 0, 1, 0));
            }
        }

        public int RunwayAvailable()
        {
            return _runways.ToList().IndexOf(false);
        }

        public bool TryToLand(Airplane newPlane)
        {
            var runwayIndex = RunwayAvailable();
            if (runwayIndex >= 0)
            {
                _runways[runwayIndex] = true;
                newPlane.RunwayOccupied = runwayIndex;
                newPlane.TimeToTakeOff = _processLoadDistribution.SampleAt(newPlane.PassengersCount);
                Console.WriteLine("Plane {0} landed successfully", newPlane.Id);
                Console.WriteLine("Plane {0} time for take off {1} mins", newPlane.Id, Math.Round(newPlane.TimeToTakeOff, 2));
                return true;
            }
            
            _waitingToLand.Enqueue(newPlane);
            return false;
        }
    }
}
