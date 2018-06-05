using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Practical.AI.Agents;
using Practical.AI.MultiAgentSystems.Communication;
using Practical.AI.MultiAgentSystems.Communication.Acl;
using Practical.AI.MultiAgentSystems.GUI;
using Practical.AI.MultiAgentSystems.Negotiation;
using Practical.AI.MultiAgentSystems.Platform;
using Plan = Practical.AI.MultiAgentSystems.Planning.Plan;
using Timer = System.Windows.Forms.Timer;

namespace Practical.AI.MultiAgentSystems
{
    public class MasCleaningAgent
    {
        public Guid Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool TaskFinished { get; set; }
        public Timer ReactionTime { get; set; }
        public FibaAcl Language { get; set; }
        public CleaningAgentPlatform Platform { get; set; }
        public List<Tuple<int, int>> CleanedCells;
        public ContractRole Role { get; set; }
        public Color Color;
        public bool AwaitingBids { get; set; }
        public bool AwaitingTaskAssignment { get; set; }
        public bool AnnouncementMade { get; set; }
        public bool TaskDistributed { get; set; }
        public Plan Plan { get; set; }
        public bool InCleaningArea { get; set; }
        public List<Tuple<int, int>> AreaTobeCleaned;
        private readonly int[,] _room;
        private readonly Form _gui;
        private Messaging _messageBoardWin;
        private readonly List<Tuple<double, Tuple<int, int>>> _wishList;

        public MasCleaningAgent(Guid id, int[,] room, Form gui, int x, int y, Color color)
        {
            Id = id;
            X = x;
            Y = y;
            _room = room;
            CleanedCells = new List<Tuple<int, int>>();
            Role = ContractRole.None;
           _wishList = new List<Tuple<double, Tuple<int, int>>>();
            Color = color;
            _gui = gui;
            Run();
        }

        private void Run()
        {
            _messageBoardWin = new Messaging (Id.ToString())
                                      {
                                          StartPosition = FormStartPosition.WindowsDefaultLocation,
                                          BackColor = Color,
                                          Size = new Size(300, 300),
                                          Text = Id.ToString(),
                                          Enabled = true
                                      };

            Language = new FibaAcl(_messageBoardWin.Proxy);
            _messageBoardWin.Show();
                       
            ReactionTime = new Timer { Enabled = true, Interval = 1000 };
            ReactionTime.Tick += ReactionTimeOnTick;
        }

        private void ReactionTimeOnTick(object sender, EventArgs eventArgs)
        {
            // There's no area assigned for cleaning
            if (AreaTobeCleaned == null)
            {
                if (Role == ContractRole.Manager && AnnouncementMade && !TaskDistributed)
                {
                    ContractNet.Awarding(_messageBoardWin.Messages, Platform.Manager, Platform.Contractors, Platform.Task, Language);
                    TaskDistributed = true;
                }
                if (Role == ContractRole.Manager && !AnnouncementMade)
                {
                    ContractNet.Announcement(Platform.Task, Platform.Manager, Platform.Contractors,
                                             Language);
                    AnnouncementMade = true;
                    Thread.Sleep(2000);
                }
                if (Role == ContractRole.Contractor && AwaitingTaskAssignment || TaskDistributed)
                {
                    AreaTobeCleaned = SetSocialLaw(_messageBoardWin.Messages);
                }
                if (Role == ContractRole.Contractor && !AwaitingTaskAssignment)
                {
                    Thread.Sleep(2000);
                    ContractNet.Bidding(_messageBoardWin.Messages, Platform.Contractors);
                    AwaitingTaskAssignment = true;
                }
            }
            else
            {
                if (!InCleaningArea)
                {
                    if (Plan == null)
                    {
                        Plan = new Plan(TypesPlan.PathFinding, this);
                        Plan.BuildPlan(new Tuple<int, int>(X, Y), AreaTobeCleaned.First());
                    }
                    else if (Plan.Path.Count == 0)
                        InCleaningArea = true;
                }
                
                Action(Perceived());
            }
            _gui.Refresh();
        }

        private List<Tuple<int, int>> SetSocialLaw(List<string> messages)
        {
            if (!messages.Exists(m => FibaAcl.GetPerformative(m) == "inform"))
                return null;

            var informMsg = messages.First(m => FibaAcl.GetPerformative(m) == "inform");
            var content = FibaAcl.MessageToDict(FibaAcl.GetInnerMessage(informMsg));
            var directive = content["content"];
            var temp = directive.Substring(directive.IndexOf('(') + 1, directive.Length - directive.IndexOf('(') - 2);
            var pos = temp.Split(',');
            var posTuple = new Tuple<int, int>(int.Parse(pos[0]), int.Parse(pos[1]));
            var colsTuple = new Tuple<int, int>(posTuple.Item2, posTuple.Item2 + _room.GetLength(1) / Platform.Directory.Count - 1);

            var result = new List<Tuple<int, int>>();
            var startRow = _room.GetLength(0) - 1;
            var dx = -1;

            // Generate path to clean
            for (var col = colsTuple.Item1; col <= colsTuple.Item2; col++)
            {
                startRow = startRow == _room.GetLength(0) - 1 ? 0 : _room.GetLength(0) - 1;
                dx = dx == -1 ? 1 : -1;

                for (var row = startRow; row < _room.GetLength(0) && row >= 0; row+=dx)
                    result.Add(new Tuple<int, int>(row, col));
            }

            return result;
        }

        private void UpdateState()
        {
            if (!CleanedCells.Contains(new Tuple<int, int>(X, Y)))
                CleanedCells.Add(new Tuple<int, int>(X, Y));
        }

        // Function
        public void Clean()
        {
            _room[X, Y] -= 1;
            Thread.Sleep(200);
        }

        // Predicate
        public bool IsDirty()
        {
            return _room[X, Y] > 0;
        }

        public void Action(List<Tuple<TypesPercept, Tuple<int, int>>> percepts)
        {
            if (Plan.Path.Count > 0)
            {
                var nextAction = Plan.NextAction();
                var percept = percepts.Find(p => p.Item1 == nextAction);
                Move(percept.Item1);
                return;
            }

            if (percepts.Exists(p => p.Item1 == TypesPercept.Clean))
                UpdateState();
            if (percepts.Exists(p => p.Item1 == TypesPercept.Dirty))
            {
                Clean();
                return; 
            }

            if (AreaTobeCleaned.Count > 0)
            {
                var nextCell = AreaTobeCleaned.First();
                AreaTobeCleaned.RemoveAt(0);
                Move(GetMove(nextCell));
            }
            else
            {
                if (!TaskFinished)
                {
                    TaskFinished = true;
                    MessageBox.Show("Task Finished");
                }
            }
        }

        public void Move(TypesPercept p)
        {
            switch (p)
            {
                case TypesPercept.MoveUp:
                    X -= 1;
                    break;
                case TypesPercept.MoveDown:
                    X += 1;
                    break;
                case TypesPercept.MoveLeft:
                    Y -= 1;
                    break;
                case TypesPercept.MoveRight:
                    Y += 1;
                    break;
            }
        }

        public TypesPercept GetMove(Tuple<int, int> move)
        {
            if (move.Item1 > X)
                return TypesPercept.MoveDown;
            if (move.Item1 < X)
                return TypesPercept.MoveUp;
            if (move.Item2 > Y)
                return TypesPercept.MoveRight;
            if (move.Item2 < Y)
                return TypesPercept.MoveLeft;
            return TypesPercept.None;
        }

        // Function
        private List<Tuple<TypesPercept, Tuple<int, int>>> Perceived()
        {
            var result = new List<Tuple<TypesPercept, Tuple<int, int>>>();

            if (IsDirty())
                result.Add(new Tuple<TypesPercept, Tuple<int, int>>(TypesPercept.Dirty, new Tuple<int, int>(X, Y)));
            else
                result.Add(new Tuple<TypesPercept, Tuple<int, int>>(TypesPercept.Clean, new Tuple<int, int>(X, Y)));

            if (CleanedCells.Count == _room.GetLength(0) * _room.GetLength(1))
                result.Add(new Tuple<TypesPercept, Tuple<int, int>>(TypesPercept.Finished, new Tuple<int, int>(X, Y)));

            if (MoveAvailable(X - 1, Y))
                result.Add(new Tuple<TypesPercept, Tuple<int, int>>(TypesPercept.MoveUp, new Tuple<int, int>(X - 1, Y)));

            if (MoveAvailable(X + 1, Y))
                result.Add(new Tuple<TypesPercept, Tuple<int, int>>(TypesPercept.MoveDown, new Tuple<int, int>(X + 1, Y)));
            
            if (MoveAvailable(X, Y - 1))
                result.Add(new Tuple<TypesPercept, Tuple<int, int>>(TypesPercept.MoveLeft, new Tuple<int, int>(X, Y - 1)));
            
            if (MoveAvailable(X, Y + 1))
                result.Add(new Tuple<TypesPercept, Tuple<int, int>>(TypesPercept.MoveRight, new Tuple<int, int>(X, Y + 1)));
            
            return result;
        }

        // Predicate
        public bool MoveAvailable(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _room.GetLength(0) && y < _room.GetLength(1);
        }

        public void Print()
        {
            var col = _room.GetLength(1);
            var i = 0;
            var line = "";
            Console.WriteLine("--------------");
            foreach (var c in _room)
            {
                line += string.Format("  {0}  ", c);
                i++;
                if (col == i)
                {
                    Console.WriteLine(line);
                    line = "";
                    i = 0;
                }
            }
        }

        public void Bid(IEnumerable<string> tasks)
        {
            var n = _room.GetLength(0);
            _wishList.Clear();

            foreach (var task in tasks)
            {
                var innerMessage = FibaAcl.GetInnerMessage(task);
                var messageDict = FibaAcl.MessageToDict(innerMessage);
                var content = messageDict["content"];
                var subtask = content.Substring(0, content.IndexOf('('));
                var cols = new string[2];

                switch (subtask)
                {
                    case "clean":
                        var temp = content.Substring(content.IndexOf('(') + 1, content.Length - content.IndexOf('(') - 2);
                        cols = temp.Split(',');
                        break;
                }

                var colRange = new Tuple<int, int>(int.Parse(cols[0]), int.Parse(cols[1]));

                for (var i = colRange.Item1; i < colRange.Item2; i++)
                {
                    // Distance to extreme points for each column
                    var end1 = new Tuple<int, int>(0, i);
                    var end2 = new Tuple<int, int>(n - 1, i);

                    var dist1 = ManhattanDistance(end1, new Tuple<int, int>(X, Y));
                    var dist2 = ManhattanDistance(end2, new Tuple<int, int>(X, Y));

                    _wishList.Add(new Tuple<double, Tuple<int, int>>(dist1, end1));
                    _wishList.Add(new Tuple<double, Tuple<int, int>>(dist2, end2));
                }
            }

            _wishList.Sort(Comparison);

            foreach (var bid in _wishList)
                Language.Message(Performative.Proposal, Id.ToString(), Platform.Manager.Id.ToString(), bid.Item1 + "," + bid.Item2.Item1 + "," + bid.Item2.Item2);
        }

        private int Comparison(Tuple<double, Tuple<int, int>> tupleA, Tuple<double, Tuple<int, int>> tupleB)
        {
            if (tupleA.Item1 > tupleB.Item1)
                return 1;
            if (tupleA.Item1 < tupleB.Item1)
                return -1;
            return 0;
        }

        private int ManhattanDistance(Tuple<int, int> x, Tuple<int, int> y)
        {
            return Math.Abs(x.Item1 - y.Item1) + Math.Abs(x.Item2 - y.Item2);
        }
    }

    public enum TypesPercept
    {
        Dirty, Clean, Finished, MoveUp, MoveDown, MoveLeft, MoveRight, None 
    }
}
