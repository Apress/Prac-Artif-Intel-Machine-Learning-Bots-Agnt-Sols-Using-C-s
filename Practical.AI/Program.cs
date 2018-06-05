using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using Practical.AI.Agents.GUI;
using Practical.AI.FOL;
using Practical.AI.GameProgramming;
using Practical.AI.GameProgramming.UninformedSearch;
using Practical.AI.GameTheory;
using Practical.AI.MetaHeuristics;
using Practical.AI.MultiAgentSystems;
using Practical.AI.MultiAgentSystems.Communication;
using Practical.AI.MultiAgentSystems.GUI;
using Practical.AI.MultiAgentSystems.Platform;
using Practical.AI.PropositionalLogic;
using Practical.AI.Predicates;
using Practical.AI.Agents;
using Practical.AI.Reinforcement_Learning.Maze;
using Practical.AI.SupervisedLearning.NeuralNetworks;
using Practical.AI.SupervisedLearning.NeuralNetworks.HandwrittenDigitRecognition;
using Practical.AI.SupervisedLearning.SVM;
using Practical.AI.SupervisedLearning.SVM.GUI;
using Practical.AI.SupervisedLearning.DecisionTrees;
using Practical.AI.UnsupervisedLearning;
using Practical.AI.UnsupervisedLearning.Clustering.Methods;
using org.mariuszgromada.math.mxparser;
using Attribute = Practical.AI.SupervisedLearning.DecisionTrees.Attribute;

namespace Practical.AI
{
    class Program
    {
        static void Main()
        {
            #region Propositional Logic & FOL

            var p = new Variable(true) { Name = "p" };
            var q = new Variable(true) { Name = "q" }; 
            var r = new Variable(true) { Name = "r" };
            var u = new Variable(true) { Name = "u" };
            var m = new Variable(true) { Name = "m" };
            var n = new Variable(true) { Name = "n" };
            var l = new Variable(true) { Name = "l" };
            var t = new Variable(true) { Name = "t" };

            // p v q ^ p v 'q ^ 'p v q ^ 'p v 'r
            //var f1 = new And(new Or(p, q), new Or(p, new Not(q)));
            //var f2 = new And(new Or(new Not(p), q), new Or(new Not(p), new Not(r)));
            //var formula = new And(f1, f2);

            //var f1 = new Or(p, new Not(q));
            //var f2 = new Or(new Not(p), r);
            //var formula = new And(f1, new And(f2, q));

            var f1 = new Or(new Not(n), new Not(t));
            var f2 = new Or(m, new Or(q, n));
            var f3 = new Or(l, new Not(m));
            var f4 = new Or(l, new Not(q));
            var f5 = new Or(new Not(l), new Not(p));
            var f6 = new Or(r, new Or(p, n));
            var f7 = new Or(new Not(r), new Not(l));
            var formula = new And(f1, new And(f2, new And(f3, new And(f4, new And(f5, new And(f6, new And(f7, t)))))));

            // (p v q v r) ^ (p v q v 'r) ^ (p v 'q v r) ^ (p v 'q v 'r) ^ ('p v q v r) ^ ('p v q v 'r) ^ ('p v 'q v r)
            //var f1 = new Or(p, new Or(q, r));
            //var f2 = new Or(p, new Or(q, new Not(r)));
            //var f3 = new Or(p, new Or(new Not(q), r));
            //var f4 = new Or(p, new Or(new Not(q), new Not(r)));
            //var f5 = new Or(new Not(p), new Or(q, r));
            //var f6 = new Or(new Not(p), new Or(q, new Not(r)));
            //var f7 = new Or(new Not(p), new Or(new Not(q), r));
            //var formula = new And(f1, new And(f2, new And(f3, new And(f4, new And(f5, new And(f6, f7))))));

            // (p v q v 'r) ^ (p v q v r) ^ (p v 'q) ^ 'p
            //var f1 = new Or(p, new Or(q, new Not(r)));
            //var f2 = new Or(p, new Or(q, r));
            //var f3 = new Or(p, new Not(q));
            //var formula = new And(f1, new And(f2, new And(f3, new Not(p))));
            
            //var formula = new And(new Not(p), p);
            //var bdt = BinaryDecisionTree.FromFormula(formula);
            
            //var nnf = formula.ToNnf();
            //Console.WriteLine("NNF: " + nnf);

            //nnf = nnf.ToCnf();
            //var cnf = new Cnf(nnf as And);
            //cnf.SimplifyCnf();

            //Console.WriteLine("CNF: " + cnf);
            //Console.WriteLine("SAT: " + cnf.Dpll());

            p.Value = true;
            // Console.WriteLine(formula.Evaluate());

            #endregion

            #region Agents

            //var terrain = new [,]
            //                  {
            //                      {0, 0, 0},
            //                      {1, 1, 1},
            //                      {2, 2, 2}
            //                  };
            var terrain = new int[1000, 1];
            var random = new Random();

            //for (int i = 0; i < terrain.GetLength(0); i++)
            //{
            //      for (int j = 0; j < terrain.GetLength(1); j++)
            //     {
            //         if (i == terrain.GetLength(0) - 1)
            //            terrain[i, j] = 1;
            //     }   
            //}

            //var cleaningRobot = new CleaningAgent(terrain, 0, 0);
            //cleaningRobot.Print();
            //cleaningRobot.Start(200);
            //cleaningRobot.Print();


            //var johnny = new Dog("Johnny", 17.5, Gender.Male);
            //var jack = new Dog("Jack", 23.5, Gender.Male);
            //var jordan = new Dog("Jack", 21.2, Gender.Male);
            //var melissa = new Dog("Melissa", 19.7, Gender.Female);
            //var dogs = new List<Dog> { johnny, jack, jordan, melissa };

            //Predicate<Dog> maleFinder = (Dog d) => { return d.Sex == Gender.Male; };
            //Predicate<Dog> heavyDogsFinder = (Dog d) => { return d.Weight >= 22; };

            //var maleDogs = dogs.Find(maleFinder);
            //var heavyDogs = dogs.Find(heavyDogsFinder);

            var water = new List<Tuple<int, int>>
            {
                new Tuple<int, int> (1, 2),
                new Tuple<int, int> (3, 5),
            };

            var obstacles = new List<Tuple<int, int>>
            {
                new Tuple<int, int> (2, 2),
                new Tuple<int, int> (4, 5),
            };

            var beliefs = new List<Belief> {
                new Belief(TypesBelief.PotentialWaterSpots, water), 
                new Belief(TypesBelief.ObstaclesOnTerrain, obstacles), 
            };

            var marsTerrain = new [,]
                              {
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0.8, -1, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0.8, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0.8, 0, 0, 0, 0},
                                  {0, -1, 0, 0, 0, 0, 0, 0, 0, 0}
            };


            var marsUnderneathTerrain = new [,]
                              {
                                  {false, false, false, false, false, false, false, false, false, false},
                                  {false, false, false, false, false, false, false, false, false, false},
                                  {false, false, false, false, false, false, false, false, false, false},
                                  {false, false, false, true, false, false, false, false, false, false},
                                  {false, false, false, false, false, false, false, false, false, false},
                                  {false, false, false, false, false, false, false, false, false, false},
                                  {false, false, false, false, false, false, false, false, false, false},
                                  {false, false, false, false, false, false, false, false, false, false},
                                  {false, false, false, false, false, false, false, false, false, false},
                                  {false, false, false, false, false, false, false, false, false, false},
            };

            var marsRocksCompound = new [,]
                              {
                                  {"", "", "", "", "", "", "", "", "", ""},
                                  {"", "", "", "", "", "", "", "", "", ""},
                                  {"", "", "", "", "", "", "", "", "", ""},
                                  {"", "", "", "", "", "", "", "", "", ""},
                                  {"AXY", "", "", "", "", "", "", "", "", ""},
                                  {"", "", "", "", "", "", "", "", "", ""},
                                  {"AXY", "", "", "", "", "", "", "", "", ""},
                                  {"", "", "", "", "", "", "", "", "", ""},
                                  {"", "", "", "", "", "", "", "", "", ""},
                                  {"AXY", "", "", "", "", "", "", "", "", ""}
            };

            var roverTerrain = new [,]
                              {
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0.8, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0.8, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0.8, 0, 0, 0, 0},
                                  {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            };

            var mars = new Mars(marsTerrain);
            var rover = new MarsRover(mars, roverTerrain, 7, 8, beliefs, 0.75, 2);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new MarsWorld(rover, mars, 10, 10));

            #endregion

            #region Multi-Agent Systems

            //var room = new [,]
            //               {
            //                   {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            //                   {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            //                   {0, 0, 0, 0, 0, 0, 1, 0, 0, 0},
            //                   {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            //                   {2, 0, 0, 1, 0, 0, 0, 0, 0, 0},
            //                   {0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            //                   {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            //                   {0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
            //                   {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            //                   {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            //               };

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            //const int N = 10;
            //const int M = 10;
            //var roomGui = new Room(N, M, room);
            
            //InitCommunicationService();

            //var clAgent1 = new MasCleaningAgent(Guid.NewGuid(), room, roomGui, 0, 0, Color.Teal);
            //var clAgent2 = new MasCleaningAgent(Guid.NewGuid(), room, roomGui, 1, 1, Color.Yellow);
            //var clAgent3 = new MasCleaningAgent(Guid.NewGuid(), room, roomGui, 1, 2, Color.Tomato);
            //var clAgent4 = new MasCleaningAgent(Guid.NewGuid(), room, roomGui, 2, 1, Color.LightSkyBlue);
            //var clAgent5 = new MasCleaningAgent(Guid.NewGuid(), room, roomGui, 2, 2, Color.Black);

            //roomGui.CleaningAgents = new List<MasCleaningAgent> { clAgent1, clAgent2, clAgent3, clAgent4, clAgent5 };
            //var platform = new CleaningAgentPlatform(roomGui.CleaningAgents, new CleaningTask(M, roomGui.CleaningAgents.Count));

            //Application.Run(roomGui);

            #endregion

            #region Simulation

            //var airplanes = new List<Airplane>
            //                    {
            //                        new Airplane(100), 
            //                        new Airplane(300), 
            //                        new Airplane(50), 
            //                        new Airplane(250), 
            //                        new Airplane(150), 
            //                        new Airplane(200), 
            //                        new Airplane(120)
            //                    };

            //var sim = new Simulation.Airport.Simulation(new TimeSpan(0, 13, 0, 0), new TimeSpan(0, 15, 0, 0), airplanes);
            //sim.Execute();

            #endregion

            #region SVM

            //var trainingSamples = new List<TrainingSample>
            //                          {
            //                              new TrainingSample(new double[] {1, 1}, 1),
            //                              new TrainingSample(new double[] {1, 0}, 1),
            //                              new TrainingSample(new double[] {2, 2}, -1),
            //                              new TrainingSample(new double[] {2, 3}, -1),
            //                          };

            //var svmClassifier = new LinearSvmClassifier(trainingSamples);
            //svmClassifier.Training();
            //svmClassifier.Predict(new List<double[]>
            //                          {
            //                              new double[] {1, 1},
            //                              new double[] {1, 0},
            //                              new double[] {2, 2},
            //                              new double[] {2, 3}, 
            //                              new double[] {2, 0}, 
            //                              new []   {2.5, 1.5}, 
            //                              new []   {0.5, 1.5}, 
            //                          });

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new SvmGui(svmClassifier.Weights, svmClassifier.Bias, svmClassifier.ModelToUse, svmClassifier.SetA, svmClassifier.SetB, svmClassifier.Hyperplane));

            #endregion

            #region Decision Trees

            //var values = new [,]
            //                 {
            //                     { "sunny", "12", "high", "weak", "no" },
            //                     { "sunny", "12", "high", "strong", "no" },
            //                     { "cloudy", "14", "high", "weak", "yes" },
            //                     { "rainy", "12", "high", "weak", "yes" },
            //                     { "rainy", "20", "normal", "weak", "yes" },
            //                     { "rainy", "20", "normal", "strong", "no" },
            //                     { "cloudy", "20", "normal", "strong", "yes" },
            //                     { "sunny", "12", "high", "weak", "no" },
            //                     { "sunny", "14", "normal", "weak", "yes" },
            //                     { "rainy", "20", "normal", "weak", "yes" },
            //                     { "sunny", "14", "normal", "strong", "yes" },
            //                     { "cloudy", "20", "high", "strong", "yes" },
            //                     { "cloudy", "20", "normal", "weak", "yes" },
            //                     { "rainy", "14", "high", "strong", "no" },
            //                 };

            //var attribs = new List<Attribute>
            //                  {
            //                      new Attribute("Outlook", new[] { "sunny", "cloudy", "rainy" }, TypeAttrib.NonGoal, TypeVal.Discrete),
            //                      new Attribute("Temperature", new[] { "12", "14", "20" }, TypeAttrib.NonGoal, TypeVal.Continuous),
            //                      new Attribute("Humidity", new[] { "high", "normal" }, TypeAttrib.NonGoal, TypeVal.Discrete),
            //                      new Attribute("Wind", new[] { "weak", "strong" }, TypeAttrib.NonGoal, TypeVal.Discrete),
            //                  };

            //var goalAttrib = new Attribute("Play Baseball", new[] { "yes", "no" }, TypeAttrib.Goal, TypeVal.Discrete);
            //var trainingDataSet = new TrainingDataSet(values, attribs, goalAttrib);
            //var dtree = DecisionTree.Learn(trainingDataSet, DtTrainingAlgorithm.Id3);
            //dtree.Visualize();
            
            #endregion

            #region Neural Networks
            
            //var trainingSamples = new List<TrainingSample>
            //                          {
            //                              new TrainingSample(new double[] {1, 1}, 0, new List<double> { 0 } ),
            //                              new TrainingSample(new double[] {1, 0}, 0, new List<double> { 0 } ),
            //                              new TrainingSample(new double[] {0, 1}, 0, new List<double> { 0 } ),
            //                              new TrainingSample(new double[] {0, 0}, 0, new List<double> { 0 } ),
            //                              new TrainingSample(new double[] {1, 2}, 1, new List<double> { 0 } ),
            //                              new TrainingSample(new double[] {2, 2}, 1, new List<double> { 1 } ),
            //                              new TrainingSample(new double[] {2, 3}, 1, new List<double> { 1 } ),
            //                              new TrainingSample(new double[] {0, 3}, 1, new List<double> { 1 } ),
            //                              new TrainingSample(new double[] {0, 2}, 1, new List<double> { 1 } ),
            //                          };

            //var trainingSamplesXor = new List<TrainingSample>
            //                          {
            //                              new TrainingSample(new double[] {0, 0}, -1, new List<double> { 0 } ),
            //                              new TrainingSample(new double[] {1, 1}, -1, new List<double> { 0 } ),
            //                              new TrainingSample(new double[] {0, 1}, -1, new List<double> { 1 } ),
            //                              new TrainingSample(new double[] {1, 0}, -1, new List<double> { 1 } ),
            //                          };

            //var perceptron = new Perceptron(trainingSamples, 2, 0.01);
            //var adaline = new Adaline(trainingSamples, 2, 0.01);
            //var multilayer = new MultiLayerNetwork(trainingSamplesXor, 2, 3, 1, 0.01);

            //adaline.Training();

            //var toPredict = new List<double[]>
            //                  {
            //                      new double[] {1, 1},
            //                      new double[] {1, 0},
            //                      new double[] {0, 0},
            //                      new double[] {0, 1},
            //                      new double[] {2, 0},
            //                      new[] {2.5, 2},
            //                      new[] {0.5, 1.5},
            //                  };

            //var predictions = adaline.PredictSet(toPredict);
            
            //for (var i = 0; i < predictions.Count; i++)
            //    Console.WriteLine("Data: ( {0} , {1} ) Classified as: {2}", toPredict[i][0], toPredict[i][1], predictions[i]);

            #endregion

            #region Handwritten Digit Recognition

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new HandwrittenRecognitionGui());

            #endregion

            #region Clustering


                //var elements = new List<UnsupervisedLearning.Clustering.Element>
                //                   {
                //                       new UnsupervisedLearning.Clustering.Element(new double[] {1, 2}),
                //                       new UnsupervisedLearning.Clustering.Element(new double[] {1, 3}),
                //                       new UnsupervisedLearning.Clustering.Element(new double[] {3, 3}),
                //                       new UnsupervisedLearning.Clustering.Element(new double[] {3, 4}),
                //                       new UnsupervisedLearning.Clustering.Element(new double[] {6, 6}),
                //                       new UnsupervisedLearning.Clustering.Element(new double[] {6, 7})
                //                   };
                //var dataSet = new DataSet();
                //dataSet.Load(elements);

                //var kMeans = new KMeans(3, dataSet);
                //kMeans.Start();

                //foreach (var cluster in kMeans.Clusters)
                //{
                //    Console.WriteLine("Cluster No {0}", cluster.ClusterNo);
                //    foreach (var obj in cluster.Objects)
                //        Console.WriteLine("({0}, {1}) in {2}", obj.Features[0], obj.Features[1], obj.Cluster);
                //    Console.WriteLine("--------------");
                //}

            #endregion

            #region MetaHeuristics

            //var f = new Function("f", "(x1)^2", "x1");
            //var hillClimbing = new HillClimbing(f, 5, 4);
            //var result = hillClimbing.Execute();

            //Console.WriteLine("Result: {0}", result[0]);

            //var map = new double[,] {
            //    {1, 2, 3, 1, 5},
            //    {5, 1, 1, 1, 8},
            //    {1, 7, 2, 1, 9},
            //    {1, 1, 6, 1, 8},
            //    {1, 1, 4, 1, 2},
            //};

            //var ga = new GeneticAlgorithmTsp(100, new Tsp(map), 100);
            //var best = ga.Execute();

            //Console.WriteLine("Solution:");
            //foreach (var d in best.Ordering)
            //    Console.Write("{0},", d);
            //Console.WriteLine('\n' + "Fitness: {0}", best.Fitness);

            #endregion

            #region Game Programming

            //var tree = new Tree<string> { State = "A" };
            //tree.Children.Add(new Tree<string> { State = "B", 
            //    Children = new List<Tree<string>>
            //                   {
            //                       new Tree<string>("E")
            //                   } });
            //tree.Children.Add(new Tree<string> { State = "C",  
            // Children = new List<Tree<string>>
            //                   {
            //                       new Tree<string>("F")
            //                   } 
            //});
            //tree.Children.Add(new Tree<string> { State = "D" });

            //var bfs = new Bfs<string>(tree);
            //var dfs = new Dfs<string>(tree);
            //var dls = new Dls<string>(tree, 21, "E");
            //var ids = new Ids<string>(tree, 10, "F");
            ////var path = bfs.Execute();
            ////var path = dfs.Execute();
            ////var path = dls.Execute();
            ////var path = ids.Execute();

            //var state = new[,]
            //                {
            //                    {6, 4, 7},
            //                    {8, 5, 0},
            //                    {3, 2, 1}
            //                };

            ////var state = new[,]
            ////                {
            ////                    {1, 0, 2},
            ////                    {4, 5, 3},
            ////                    {7, 8, 6}
            //                //};
            
            //var goalState = new[,]
            //                {
            //                    {1, 2, 3},
            //                    {4, 5, 6},
            //                    {7, 8, 0}
            //                };

            //var board = new Board<int>(state, 0, new Tuple<int, int>(1, 2), "");
            //var goal = new Board<int>(goalState, 0, new Tuple<int, int>(2, 2), "");
            //var slidingTilesPuzzle = new SlidingTilesPuzzle<int>(board, goal);
            //var bidirectionalSearch = new Bs<int>(slidingTilesPuzzle);
            //var stopWatch = new Stopwatch();
            //stopWatch.Start();
            //var path = bidirectionalSearch.BidirectionalBfs();
            //stopWatch.Stop();

            //foreach (var e in path)
            //    Console.Write(e + ", ");
            //Console.WriteLine('\n' + "Total steps: " + path.Length);
            //Console.WriteLine("Elapsed Time: " + stopWatch.ElapsedMilliseconds / 1000 + " secs");

            //board = new Board<int>(state, 0, new Tuple<int, int>(1, 2), "");

            //for (var i = 0; i < path.Length; i++)
            //{
            //    if (path[i] == 'R')
            //        board = board.Move(Move.Right);
            //    if (path[i] == 'D')
            //        board = board.Move(Move.Down);
            //    if (path[i] == 'U')
            //        board = board.Move(Move.Up);
            //    if (path[i] == 'L')
            //        board = board.Move(Move.Left);
            //}  

            #endregion

            #region Game Theory

            //var board = new OthelloBoard(8, 8);

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new OthelloGui(board));

            #endregion

            #region Reinforcement Learning

            var map = new [,]
                          {
                              {true, false, true, false, true},
                              {true, true, true, false, true},
                              {true, false, true, false, true},
                              {true, false, true, true, true},
                              {true, true, true, false, true}
                          };

            var reward = new [,]
                          {
                              {-0.01, -0.01, -0.01, -0.01, -0.01},
                              {-0.01, -0.01, -0.01, -0.01, -0.01},
                              {-0.01, -0.01, -0.01, -0.01, -0.01},
                              {-0.01, -0.01, -0.01, -0.01, -0.01},
                              {-0.01, -0.01, -0.01, -0.01, 1},

                          };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MazeGui(5, 5, map, reward));

            #endregion

            Console.Read();
        }

        private static void InitCommunicationService()
        {
            // Step 1 Create a URI to serve as the base address.
            var baseAddress = new Uri("http://localhost:9090/");

            // Step 2 Create a ServiceHost instance
            var selfHost = new ServiceHost(typeof(AgentCommunicationService), baseAddress);

            try
            {
                // Step 3 Add a service endpoint.
                var binding = new WSDualHttpBinding(WSDualHttpSecurityMode.None);
                
                selfHost.AddServiceEndpoint(typeof(IAgentCommunicationService),
                    binding, "AgentCommunicationService");

                // Step 4 Enable Metadata Exchange and Add MEX endpoint
                var smb = new ServiceMetadataBehavior { HttpGetEnabled = true };
                selfHost.Description.Behaviors.Add(smb);
                selfHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                    MetadataExchangeBindings.CreateMexHttpBinding(), baseAddress + "mex");

                // Step 5 Start the service.
                selfHost.Open();
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Listening at: {0}", baseAddress);
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }
        }
    }
}


