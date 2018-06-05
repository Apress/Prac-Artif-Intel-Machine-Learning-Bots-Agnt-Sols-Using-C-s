using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.Agents
{
    public class MarsRover
    {
        public Mars Mars { get; set; }
        public List<Belief> Beliefs { get; set; }
        public Queue<Desire> Desires { get; set; }
        public Stack<Intention> Intentions { get; set; }
        public List<Plan> PlanLibrary { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int SenseRadius { get; set; }
        public double RunningOverThreshold { get; set; }
        // Identifies the last part of the terrain seen by the Rover
        public List<Tuple<int, int>> CurrentTerrain { get; set; }
        public Plan CurrentPlan { get; set; }
        public List<Tuple<int, int>> WaterFound { get; set; }
        private double[,] _terrain;
        private static Random _random;
        private Dictionary<Tuple<int, int>, int> _perceivedCells;
        private int _wanderTimes;
        private const int WanderThreshold = 10;

        public MarsRover(Mars mars, double [,] terrain, int x, int y, IEnumerable<Belief> initialBeliefs, double runningOver, int senseRadious)
        {
            Mars = mars;
            X = x;
            Y = y;
            _terrain = new double[terrain.GetLength(0), terrain.GetLength(1)];
            Array.Copy(terrain, _terrain, terrain.GetLength(0) * terrain.GetLength(1));
            Beliefs = new List<Belief>(initialBeliefs);
            Desires = new Queue<Desire>();
            Intentions = new Stack<Intention>();
            PlanLibrary = new List<Plan>
                              {
                                  new  Plan(TypesPlan.PathFinding, this), 
                              };
            WaterFound = new List<Tuple<int, int>>();
            RunningOverThreshold = runningOver;
            SenseRadius = senseRadious;
            CurrentTerrain = new List<Tuple<int, int>>();
            _random = new Random();
            _perceivedCells = new Dictionary<Tuple<int, int>, int>();
        }

        /// <summary>
        /// Percepts function
        /// </summary>
        /// <returns></returns>
        public List<Percept> GetPercepts()
        {
            var result = new List<Percept>();

            if (MoveAvailable(X - 1, Y))
                result.Add(new Percept(new Tuple<int,int>(X - 1, Y), TypePercept.MoveUp));

            if (MoveAvailable(X + 1, Y))
                result.Add(new Percept(new Tuple<int, int>(X + 1, Y), TypePercept.MoveDown));

            if (MoveAvailable(X, Y - 1))
                result.Add(new Percept(new Tuple<int, int>(X, Y - 1), TypePercept.MoveLeft));

            if (MoveAvailable(X, Y + 1))
                result.Add(new Percept(new Tuple<int, int>(X, Y + 1), TypePercept.MoveRight));

            result.AddRange(LookAround());

            return result;
        }
        
        public IEnumerable<Percept> GetCurrentTerrain()
        {
            var R = SenseRadius;
            CurrentTerrain.Clear();
            var result = new List<Percept>();

            for (var i = X - R > 0 ? X - R : 0; i <= X + R; i++)
            {
                for (var j = Y; Math.Pow((j - Y), 2) + Math.Pow((i - X), 2) <= Math.Pow(R, 2); j--)
                {
                    if (j < 0 || i >= _terrain.GetLength(0)) break;
                    // In the circle
                    result.AddRange(CheckTerrain(Mars.TerrainAt(i, j), new Tuple<int, int>(i, j)));
                    CurrentTerrain.Add(new Tuple<int, int>(i, j));
                    UpdatePerceivedCellsDicc(new Tuple<int, int>(i, j));
                }
                for (var j = Y + 1; (j - Y) * (j - Y) + (i - X) * (i - X) <= R * R; j++)
                {
                    if (j >= _terrain.GetLength(1) || i >= _terrain.GetLength(0)) break;
                    // In the circle
                    result.AddRange(CheckTerrain(Mars.TerrainAt(i, j), new Tuple<int, int>(i, j)));
                    CurrentTerrain.Add(new Tuple<int, int>(i, j));
                    UpdatePerceivedCellsDicc(new Tuple<int, int>(i, j));
                }
            }

            return result;
        }

        private void UpdatePerceivedCellsDicc(Tuple<int, int> position)
        {
            if (!_perceivedCells.ContainsKey(position))
                _perceivedCells.Add(position, 0);
            _perceivedCells[position]++;
        }

        /// <summary>
        /// Look around the rover.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Percept> LookAround()
        {
            return GetCurrentTerrain();
        }

        /// <summary>
        /// Check a given cell in the terrain
        /// </summary>
        /// <param name="cell">Value of the cell</param>
        /// <param name="position">Its coordenates</param>
        /// <returns></returns>
        private IEnumerable<Percept> CheckTerrain(double cell, Tuple<int, int> position)
        {
            var result = new List<Percept>();

            if (cell > RunningOverThreshold)
                result.Add(new Percept(position, TypePercept.Obstacle));
            else if (cell < 0)
                result.Add(new Percept(position, TypePercept.WaterSpot));

            _terrain[position.Item1, position.Item2] = cell;

            return result;
        }

        /// <summary>
        /// Determines whether a move to cell (x, y) is available.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool MoveAvailable(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _terrain.GetLength(0) && y < _terrain.GetLength(1) && _terrain[x, y] < RunningOverThreshold;
        }

        public TypesAction Action(List<Percept> percepts)
        {
            // Reactive Layer
            if (Mars.WaterAt(X, Y) && !WaterFound.Contains(new Tuple<int, int>(X, Y)))
                return TypesAction.Dig;

            var waterPercepts = percepts.FindAll(p => p.Type == TypePercept.WaterSpot);

            if (waterPercepts.Count > 0)
            {
                foreach (var waterPercept in waterPercepts)
                {
                    var belief = Beliefs.FirstOrDefault(b => b.Name == TypesBelief.PotentialWaterSpots);
                    List<Tuple<int, int>> pred;
                    if (belief != null)
                        pred = belief.Predicate as List<Tuple<int, int>>;
                    else
                    {
                        pred = new List<Tuple<int, int>> {waterPercept.Position};
                        Beliefs.Add(new Belief(TypesBelief.PotentialWaterSpots, pred));
                    }
                    if (!WaterFound.Contains(waterPercept.Position))
                        pred.Add(waterPercept.Position);
                    else
                    {
                        pred.RemoveAll(
                            t => t.Item1 == waterPercept.Position.Item1 && t.Item2 == waterPercept.Position.Item2);
                        if (pred.Count == 0)
                            Beliefs.RemoveAll(b => (b.Predicate as List<Tuple<int, int>>).Count == 0);
                    }
                }

                if (waterPercepts.Any(p => !WaterFound.Contains(p.Position)))
                    CurrentPlan = null;
            }

            if (Beliefs.Count == 0)
            {
                if (_wanderTimes == WanderThreshold)
                {
                    _wanderTimes = 0;
                    InjectBelief();
                }
                _wanderTimes++;
                return RandomMove(percepts);
            }
            if (CurrentPlan == null || CurrentPlan.FulFill())
            {
                // Deliberative Layer
                Brf(percepts);
                Options();
                Filter();
            }

            return CurrentPlan.NextAction();
        }

        private void InjectBelief()
        {
            var halfC = _terrain.GetLength(1) / 2;
            var halfR = _terrain.GetLength(0) / 2;

            var firstSector = _perceivedCells.Where(k => k.Key.Item1 < halfR && k.Key.Item2 < halfC).ToList();
            var secondSector = _perceivedCells.Where(k => k.Key.Item1 < halfR && k.Key.Item2 >= halfC).ToList();

            var thirdSector = _perceivedCells.Where(k => k.Key.Item1 >= halfR && k.Key.Item2 < halfC).ToList();
            var fourthSector = _perceivedCells.Where(k => k.Key.Item1 >= halfR && k.Key.Item2 >= halfC).ToList();

            var freq1stSector = SetRelativeFreq(firstSector);
            var freq2ndSector = SetRelativeFreq(secondSector);
            var freq3rdSector = SetRelativeFreq(thirdSector);
            var freq4thSector = SetRelativeFreq(fourthSector);

            var min = Math.Min(freq1stSector, Math.Min(freq2ndSector, Math.Min(freq3rdSector, freq4thSector)));

            if (min == freq1stSector)
                Beliefs.Add(new Belief(TypesBelief.PotentialWaterSpots, new List<Tuple<int, int>> { new Tuple<int, int>(0, 0) }));
            else if (min == freq2ndSector)
                Beliefs.Add(new Belief(TypesBelief.PotentialWaterSpots, new List<Tuple<int, int>> { new Tuple<int, int>(0, _terrain.GetLength(1) - 1) }));
            else if (min == freq3rdSector)
                Beliefs.Add(new Belief(TypesBelief.PotentialWaterSpots, new List<Tuple<int, int>> { new Tuple<int, int>(_terrain.GetLength(0) - 1, 0) }));
            else
                Beliefs.Add(new Belief(TypesBelief.PotentialWaterSpots, new List<Tuple<int, int>> { new Tuple<int, int>(_terrain.GetLength(0) - 1, _terrain.GetLength(1) - 1) }));
        }

        private double SetRelativeFreq(List<KeyValuePair<Tuple<int, int>, int>> cells)
        {
            var result = 0.0;

            foreach (var cell in cells) 
                result += RelativeFrequency(cell.Value, cells.Count);

            return result;
        }

        private double RelativeFrequency(int absFreq, int n)
        {
            return (double) absFreq/n;
        }

        private TypesAction RandomMove(List<Percept> percepts)
        {
            var moves = percepts.FindAll(p => p.Type.ToString().Contains("Move"));
            var selectedMove = moves[_random.Next(0, moves.Count)];

            switch (selectedMove.Type)
            {
                case TypePercept.MoveUp:
                    return TypesAction.MoveUp;
                case TypePercept.MoveDown:
                    return TypesAction.MoveDown;
                case TypePercept.MoveRight:
                    return TypesAction.MoveRight;
                case TypePercept.MoveLeft:
                    return TypesAction.MoveLeft;
            }

            return TypesAction.None;
        }

        public void ExecuteAction(TypesAction action, List<Percept> percepts)
        {
            switch (action)
            {
                case TypesAction.MoveUp:
                    X -= 1;
                    break;
                case TypesAction.MoveDown:
                    X += 1;
                    break;
                case TypesAction.MoveLeft:
                    Y -= 1;
                    break;
                case TypesAction.MoveRight:
                    Y += 1;
                    break;
                case TypesAction.Dig:
                    WaterFound.Add(new Tuple<int, int>(X, Y));
                    break;
            }
        }

        /// <summary>
        /// Beliefs revision function
        /// </summary>
        /// <param name="percepts"></param>
        public void Brf(List<Percept> percepts)
        {
            var newBeliefs = new List<Belief>();

            foreach (var b in Beliefs)
            {
                switch (b.Name)
                {
                    case TypesBelief.PotentialWaterSpots:
                        var waterSpots = new List<Tuple<int, int>>(b.Predicate);
                        waterSpots = UpdateBelief(TypesBelief.PotentialWaterSpots, waterSpots);
                        if (waterSpots.Count > 0)
                            newBeliefs.Add(new Belief(TypesBelief.PotentialWaterSpots, waterSpots));
                        break;
                    case TypesBelief.ObstaclesOnTerrain:
                        var obstacleSpots = new List<Tuple<int, int>>(b.Predicate);
                        obstacleSpots = UpdateBelief(TypesBelief.ObstaclesOnTerrain, obstacleSpots);
                        if (obstacleSpots.Count > 0)
                            newBeliefs.Add(new Belief(TypesBelief.ObstaclesOnTerrain, obstacleSpots));
                        break;
                }
            }          

            Beliefs = new List<Belief>(newBeliefs);
        }

        /// <summary>
        /// Updates set of beliefs.
        /// </summary>
        /// <param name="belief"> </param>
        /// <param name="beliefPos"></param>
        /// <returns></returns>
        private List<Tuple<int, int>> UpdateBelief(TypesBelief belief, IEnumerable<Tuple<int, int>> beliefPos)
        {
            var result = new List<Tuple<int, int>>();

            foreach (var spot in beliefPos)
            {
                 if (CurrentTerrain.Contains(new Tuple<int, int>(spot.Item1, spot.Item2)))
                 {
                    switch (belief)
                    {
                        case TypesBelief.PotentialWaterSpots:
                            if (_terrain[spot.Item1, spot.Item2] >= 0)
                                continue;
                            break;
                        case TypesBelief.ObstaclesOnTerrain:
                            if (_terrain[spot.Item1, spot.Item2] < RunningOverThreshold)
                                continue;
                            break;
                    }
                 }
                 result.Add(spot);
            }
                        
            return result;
        }

        /// <summary>
        /// Generates desires.
        /// </summary>
        public void Options()
        {
            Desires.Clear();

             foreach (var b in Beliefs)
             {
                  if (b.Name == TypesBelief.PotentialWaterSpots)
                  {
                      var waterPos = b.Predicate as List<Tuple<int, int>>;
                      waterPos.Sort(delegate(Tuple<int, int> tupleA, Tuple<int, int> tupleB)
                                        {
                                            var distA = ManhattanDistance(tupleA, new Tuple<int, int>(X, Y));
                                            var distB = ManhattanDistance(tupleB, new Tuple<int, int>(X, Y));
                                            if (distA < distB)
                                                return 1;
                                            if (distA > distB)
                                                return -1;
                                            return 0;
                                        });
                      foreach (var wPos in waterPos)
                          Desires.Enqueue(new Desire(TypesDesire.FindWater, new Desire(TypesDesire.GotoLocation, new Desire(TypesDesire.Dig, wPos))));
                  }
             }
        }

        /// <summary>
        /// Determines which desires will become intentions or which intentions should remain or be deleted.
        /// </summary>
        /// <param name="percepts"></param>
        private void Filter()
        {
            Intentions.Clear();

             foreach (var desire in Desires)
            {
                if (desire.SubDesires.Count > 0)
                {
                    var primaryDesires = desire.GetSubDesires();
                    primaryDesires.Reverse();
                    foreach (var d in primaryDesires)
                        Intentions.Push(Intention.FromDesire(d));
                }
                else
                    Intentions.Push(Intention.FromDesire(desire));
            }

            if (Intentions.Any() && !ExistsPlan())
                ChoosePlan();
        }

        private void ChoosePlan()
        {
            var primaryIntention = Intentions.Pop();
            var location = primaryIntention.Predicate as Tuple<int, int>;

            switch (primaryIntention.Name)
            {
                case TypesDesire.Dig:
                    CurrentPlan = PlanLibrary.First(p => p.Name == TypesPlan.PathFinding);
                    CurrentPlan.BuildPlan(new Tuple<int, int>(X, Y), location);
                    break;
            }
        }

        public bool ExistsPlan()
        {
            return CurrentPlan != null && CurrentPlan.Path.Count > 0;
        }

        public int ManhattanDistance(Tuple<int, int> x, Tuple<int, int> y)
        {
            return Math.Abs(x.Item1 - y.Item1) + Math.Abs(x.Item2 - y.Item2);
        }
    }

    /// <summary>
    /// Represents Mars environment.
    /// </summary>
    public class Mars
    {
        private readonly double[,] _terrain;

        public Mars(double[,] terrain)
        {
            _terrain = new double[terrain.GetLength(0), terrain.GetLength(1)];
            Array.Copy(terrain, _terrain, terrain.GetLength(0) * terrain.GetLength(1));
        }

        public double TerrainAt(int x, int y)
        {
            return _terrain[x, y];
        }

        public bool WaterAt(int x, int y)
        {
            return _terrain[x, y] < 0;
        }
    }

    public class Belief
    {
        public TypesBelief Name { get; set; }
        public dynamic Predicate;
        
        public Belief(TypesBelief name, dynamic predicate)
        {
            Name = name;
            Predicate = predicate;
        }

        public override string ToString()
        {
            var result = "";
            var coord = Predicate as List<Tuple<int, int>>;

            foreach (var c in coord)
                result += Name + " (" + c.Item1 + "," + c.Item2 + ")" + "\n";
            
            return result;
        }
    }

    public class Desire
    {
        public TypesDesire Name { get; set; }
        public dynamic Predicate;
        public List<Desire> SubDesires { get; set; }

        public Desire() { SubDesires = new List<Desire>(); }

        public Desire(TypesDesire name)
        {
            Name = name;
            SubDesires = new List<Desire>();
        } 

        public Desire(TypesDesire name, dynamic predicate)
        {
            Name = name;
            Predicate = predicate;
            SubDesires = new List<Desire>();
        }

        public Desire(TypesDesire name, IEnumerable<Desire> subDesires)
        {
            Name = name;
            SubDesires = new List<Desire>(subDesires);
        }

        public Desire(TypesDesire name, params Desire[] subDesires)
        {
            Name = name;
            SubDesires = new List<Desire>(subDesires);
        }

        public List<Desire> GetSubDesires()
        {
            if (SubDesires.Count == 0)
                return new List<Desire>() { this };

            var result = new List<Desire>();

            foreach (var desire in SubDesires)
                result.AddRange(desire.GetSubDesires());

            return result;
        }

        public override string ToString()
        {
            return Name.ToString() + "\n";
        }
    }

    public class Intention: Desire
    {
        public static Intention FromDesire(Desire desire)
        {
            var result = new Intention
                             {
                                 Name = desire.Name,
                                 SubDesires = new List<Desire>(desire.SubDesires),
                                 Predicate = desire.Predicate
                             };

            return result;
        }
    }

    public class Plan
    {
        public TypesPlan Name { get; set; }
        public List<Tuple<int, int>> Path { get; set; }
        private MarsRover _rover;

        public Plan(TypesPlan name, MarsRover rover)
        {
            Name = name;
            Path = new List<Tuple<int, int>>();
            _rover = rover;
        }

        public TypesAction NextAction()
        {
            if (Path.Count == 0)
                return TypesAction.None;
            
            var next = Path.First();
            Path.RemoveAt(0);

            if (_rover.X > next.Item1)
                 return TypesAction.MoveUp;
            if (_rover.X < next.Item1)
                 return TypesAction.MoveDown;
            if (_rover.Y < next.Item2)
                return TypesAction.MoveRight;
            if(_rover.Y > next.Item2)
                return TypesAction.MoveLeft;

            return TypesAction.None;
        }

        public void BuildPlan(Tuple<int, int> source, Tuple<int, int> dest)
        {
            switch (Name)
            {
                    case TypesPlan.PathFinding:
                        Path = PathFinding(source.Item1, source.Item2, dest.Item1, dest.Item2).Item2;
                        break;
            }
        }

        private Tuple<Tuple<int, int>, List<Tuple<int, int>>> PathFinding(int x1, int y1, int x2, int y2)
        {
            var queue = new Queue<Tuple<Tuple<int, int>, List<Tuple<int, int>>>>();
            queue.Enqueue(new Tuple<Tuple<int, int>, List<Tuple<int, int>>>(new Tuple<int, int>(x1, y1), new List<Tuple<int, int>>()));
            var hashSetVisitedCells = new HashSet<Tuple<int, int>>();

            while(queue.Count > 0)
            {
                var currentCell = queue.Dequeue();
                var currentPath = currentCell.Item2;
                hashSetVisitedCells.Add(currentCell.Item1);
                var x = currentCell.Item1.Item1;
                var y = currentCell.Item1.Item2;

                if (x == x2 && y == y2)
                    return currentCell;

                // Up
                if (_rover.MoveAvailable(x - 1, y) && !hashSetVisitedCells.Contains(new Tuple<int, int>(x - 1, y)))
                {
                    var pathUp = new List<Tuple<int, int>>(currentPath);
                    pathUp.Add(new Tuple<int, int>(x - 1, y));
                    queue.Enqueue(new Tuple<Tuple<int, int>, List<Tuple<int, int>>>(new Tuple<int, int>(x - 1, y), pathUp));   
                }
                // Down
                if (_rover.MoveAvailable(x + 1, y) && !hashSetVisitedCells.Contains(new Tuple<int, int>(x + 1, y)))
                {
                    var pathDown = new List<Tuple<int, int>>(currentPath);
                    pathDown.Add(new Tuple<int, int>(x + 1, y));
                    queue.Enqueue(new Tuple<Tuple<int, int>, List<Tuple<int, int>>>(new Tuple<int, int>(x + 1, y), pathDown));
                }
                // Left
                if (_rover.MoveAvailable(x, y - 1) && !hashSetVisitedCells.Contains(new Tuple<int, int>(x, y - 1)))
                {
                    var pathLeft = new List<Tuple<int, int>>(currentPath);
                    pathLeft.Add(new Tuple<int, int>(x, y - 1));
                    queue.Enqueue(new Tuple<Tuple<int, int>, List<Tuple<int, int>>>(new Tuple<int, int>(x, y - 1), pathLeft));
                }
                // Right
                if (_rover.MoveAvailable(x, y + 1) && !hashSetVisitedCells.Contains(new Tuple<int, int>(x, y + 1)))
                {
                    var pathRight = new List<Tuple<int, int>>(currentPath);
                    pathRight.Add(new Tuple<int, int>(x, y + 1));
                    queue.Enqueue(new Tuple<Tuple<int, int>, List<Tuple<int, int>>>(new Tuple<int, int>(x, y + 1), pathRight));
                }
            }

            return null;
        }

        public bool FulFill()
        {
            return Path.Count == 0;
        }
    }

    public class Percept
    {
        public TypePercept Type { get; set; }
        public Tuple<int, int> Position { get; set; }

        public Percept(Tuple<int, int> position, TypePercept percept)
        {
            Position = position;
            Type = percept;
        }
    }

    public enum TypePercept
    {
        WaterSpot, Obstacle, MoveUp, MoveDown, MoveLeft, MoveRight
    }

    public enum TypesBelief
    {
        PotentialWaterSpots, ObstaclesOnTerrain
    }

    public enum TypesDesire
    {
        FindWater, GotoLocation, Dig
    }

    public enum TypesPlan
    {
        PathFinding
    }

    public enum TypesAction
    {
        MoveUp, MoveDown, MoveLeft, MoveRight, Dig,
        None
    }
}
