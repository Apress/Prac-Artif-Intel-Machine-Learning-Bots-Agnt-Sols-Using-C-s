using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using Practical.AI.SupervisedLearning.Extensors;

namespace Practical.AI.SupervisedLearning.SVM.GUI
{
    public partial class SvmGui : Form
    {
        private readonly MainViewModel _plot;

        public SvmGui(double [] weights, double bias, int model, IEnumerable<Tuple<double, double>> setA, IEnumerable<Tuple<double, double>> setB, IEnumerable<Tuple<double, double>> hyperplane = null)
        {
            InitializeComponent();

            _plot = new MainViewModel(weights, bias, model, setA, setB, hyperplane);
            var view = new OxyPlot.WindowsForms.PlotView
                           {
                               Width = Width,
                               Height = Height,
                               Parent = this,
                               BackColor = Color.WhiteSmoke,
                               Model = _plot.Model
                           };
        }
    }

    public class MainViewModel
    {

        public MainViewModel(double[] weights, double bias, int model, IEnumerable<Tuple<double, double>> setA, IEnumerable<Tuple<double, double>> setB, IEnumerable<Tuple<double, double>> hyperplane = null)
        {
            Model = new PlotModel { Title = "SVM by SMO" };
            var scatterPointsA = setA.Select(e => new ScatterPoint(e.Item1, e.Item2)).ToList();
            var scatterPointsB = setB.Select(e => new ScatterPoint(e.Item1, e.Item2)).ToList();
            var h = new List<ScatterPoint>();

            if (hyperplane != null)
                h = hyperplane.Select(e => new ScatterPoint(e.Item1, e.Item2)).ToList(); ;

            var scatterSeriesA = new ScatterSeries
                                    {
                                       MarkerFill = OxyColor.FromRgb(255, 0, 0),
                                       ItemsSource = scatterPointsA,
                                    };
            var scatterSeriesB = new ScatterSeries
                                    {
                                        MarkerFill = OxyColor.FromRgb(0, 0, 255),
                                        ItemsSource = scatterPointsB
                                    };

            var scatterSeriesH = new ScatterSeries
                                    {
                                        MarkerFill = OxyColor.FromRgb(0, 255, 255),
                                        ItemsSource = h
                                    };

            Model.Series.Add(scatterSeriesA);
            Model.Series.Add(scatterSeriesB);
            Model.Series.Add(scatterSeriesH);
            Model.Series.Add(GetFunction(weights, bias, model));
        }

        public FunctionSeries GetFunction(double [] w, double b, int model)
        {
            const int n = 10;
            var serie = new FunctionSeries();
            
            for (var x = 0.0; x < n; x += 0.01)
            {
                for (var y = 0.0; y < n; y += 0.01)
                {
                    //adding the points based x,y
                    var funVal = GetValue(x, y, w, b, model);

                    if (Math.Abs(funVal) <= 0.01)
                        serie.Points.Add(new DataPoint(x, y));
                }
            }
            
            return serie;
        }

        public double GetValue(double x, double y, double [] w, double b, int model)
        {
            w = w.RoundValues(5).ToArray();
            b = new [] {b}.RoundValues(5).ToArray().First();
            return w[0] * x  + w[1] * y + model * b;
        }

        public PlotModel Model { get;  set; }
    }

}
