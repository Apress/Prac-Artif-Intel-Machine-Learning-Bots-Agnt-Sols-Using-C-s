using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.Reinforcement_Learning.Maze
{
    public class QAgent
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Dictionary<Tuple<int, int>, List<double>> QTable { get; set; }
        public double Randomness { get; set; }
        public double[,] Reward { get; set; }
        private readonly bool[,] _map;
        private readonly int _n;
        private readonly int _m;
        private const double K = 0.8;
        private readonly double _discountFactor;
        private static readonly Random Random = new Random();
        private readonly Dictionary<Tuple<int, int>, int> _freq; 

        public QAgent(int x, int y, double discountFactor, int n, int m, double [,] reward, bool [,] map, double randomness)
        {
            X = x;
            Y = y;
            Randomness = randomness;
            InitQTable(n, m);
            _n = n;
            _m = m;
            Reward = reward;
            _map = map;
            _discountFactor = discountFactor;
            _freq = new Dictionary<Tuple<int, int>, int> {{new Tuple<int, int>(0, 0), 1}};
        }

        private void InitQTable(int n, int m)
        {
            QTable = new Dictionary<Tuple<int, int>, List<double>>();
            
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < m; j++)
                    QTable.Add(new Tuple<int, int>(i, j), new List<double> { 0, 0, 0, 0}); 
            }
        }

        public void QLearning(bool actionByFreq = false)
        {
            var currentState = new Tuple<int, int>(X, Y);
            var action = SelectAction(actionByFreq);
            if (!_freq.ContainsKey(ActionToTuple(action)))
                _freq.Add(ActionToTuple(action), 1);
            else
                _freq[ActionToTuple(action)]++;
            ActionToTuple(action, true);
            
            var reward = Reward[currentState.Item1, currentState.Item2];

            QTable[currentState][(int) action] = reward + _discountFactor * QTable[new Tuple<int, int>(X, Y)].Max();
        }

        public void Reset()
        {
            X = 0;
            Y = 0;
            _freq.Clear();
        }

        private QAgentAction SelectAction(bool actionByFreq)
        {
            var bestValue = double.MinValue;
            var bestAction = QAgentAction.None;
            var availableActions = AvailableActions();

            if (actionByFreq)
                return FreqStrategy(availableActions);

            for (var i = 0; i < 4; i++)
            {
                if (!availableActions.Contains(ActionSelector(i)))
                    continue;

                var value = QTable[new Tuple<int, int>(X, Y)][i];
                if (value > bestValue)
                {
                    bestAction = ActionSelector(i);
                    bestValue = value;
                }
            }

            return bestAction;
        }

        private QAgentAction FreqStrategy(List<QAgentAction> availableActions)
        {
            var newPos = availableActions.Select(availableAction => ActionToTuple(availableAction)).ToList();
            var lowest = double.MaxValue;
            var i = 0;
            var bestIndex = 0;

            if (Random.NextDouble() <= 0.5)
                return availableActions[Random.Next(availableActions.Count)];

            foreach (var tuple in newPos)
            {
                if (!_freq.ContainsKey(tuple))
                {
                    bestIndex = i;
                    break;
                }

                if (_freq[tuple] <= lowest)
                {
                    lowest = _freq[tuple];
                    bestIndex = i;
                }

                i++;
            }
           
            return availableActions[bestIndex];
        }

        private List<QAgentAction> AvailableActions()
        {
            var result = new List<QAgentAction>();

            if (X - 1 >= 0 && _map[X - 1, Y])
                result.Add(QAgentAction.Up);

            if (X + 1 < _n && _map[X + 1, Y])
                result.Add(QAgentAction.Down);

            if (Y - 1 >= 0 && _map[X, Y - 1])
                result.Add(QAgentAction.Left);

            if (Y + 1 < _m && _map[X, Y + 1])
                result.Add(QAgentAction.Right);

            return result;
        }

        private double BoltzmannDist(int action)
        {
            var currentState = new Tuple<int, int>(X, Y);
            var actionsForState = QTable[currentState];

            return (Math.Exp(QTable[currentState][action]) / K) / (actionsForState.Sum(d => Math.Exp(d) / K));
        }

        public QAgentAction ActionSelector(int action)
        {
            switch (action)
            {
                case 0:
                    return QAgentAction.Up;
                case 1:
                    return QAgentAction.Down;
                case 2:
                    return QAgentAction.Left;
                case 3:
                    return QAgentAction.Right;
                default:
                    return QAgentAction.None;
            }
        }

        public Tuple<int, int> ActionToTuple(QAgentAction action, bool execute = false)
        {
            switch (action)
            {
                case QAgentAction.Up:
                    if (execute) X--;
                    return new Tuple<int, int>(X - 1, Y);
                case QAgentAction.Down:
                    if (execute) X++;
                    return new Tuple<int, int>(X + 1, Y);
                case QAgentAction.Left:
                    if (execute) Y--;
                    return new Tuple<int, int>(X, Y - 1);
                case QAgentAction.Right:
                    if (execute) Y++;
                    return new Tuple<int, int>(X, Y + 1);
                default:
                    return new Tuple<int, int>(-1, -1);
            }
        }
    }

    public enum QAgentAction
    {
        Up, Down, Left, Right, None
    }
}
