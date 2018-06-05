using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Practical.AI.SupervisedLearning.SVM;

namespace Practical.AI.SupervisedLearning.NeuralNetworks.HandwrittenDigitRecognition
{
    public partial class HandwrittenRecognitionGui : Form
    {
        private bool _mouseIsDown;
        private Bitmap _bitmap;
        private const int NnInputs = 900;
        private const int NnHidden = 3;
        private const int NnOutputs = 3;
        private HandwrittenDigitRecognitionNn _handwrittenDigitRecogNn;
        private bool _weightsLoaded;

        public HandwrittenRecognitionGui()
        {
            InitializeComponent();
            _bitmap = new Bitmap(paintBox.Width, paintBox.Height);
        }

        private void PaintBoxPaint(object sender, PaintEventArgs e)
        {
        }

        private void PaintBoxMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _mouseIsDown = true;
        }

        private void PaintBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseIsDown)
            {
                var point = paintBox.PointToClient(Cursor.Position);
                DrawPoint((point.X), (point.Y), Color.FromArgb(255, 255, 255, 255));
            }
        }

        private void PaintBoxMouseUp(object sender, MouseEventArgs e)
        {
            _mouseIsDown = false;
        }

        public void DrawPoint(int x, int y, Color color)
        {
            var pen = new Pen(color);
            var gPaintBox = paintBox.CreateGraphics();
            var gImg = Graphics.FromImage(_bitmap);
            gPaintBox.DrawRectangle(pen, x, y, 1, 1);
            gImg.DrawRectangle(pen, x, y, 1, 1);
        }

        private void ReadWeights()
        {
            _handwrittenDigitRecogNn = new HandwrittenDigitRecognitionNn(new List<TrainingSample>(), NnInputs, NnHidden, NnOutputs, 0.002);
            var weightsFile = new StreamReader("weights.txt");
            var currentLayer = _handwrittenDigitRecogNn.HiddenLayer;
            var weights = new List<double>();
            var j = 0;

            while (!weightsFile.EndOfStream)
            {
                var currentLine = weightsFile.ReadLine();

                // End of weights for current unit.
                if (currentLine == "*")
                {
                    currentLayer.Units[j].Weights = new List<double>(weights);
                    j++;
                    weights.Clear();
                    continue;
                }

                // End of layer.
                if (currentLine == "-")
                {
                    currentLayer = _handwrittenDigitRecogNn.OutPutLayer;
                    j = 0;
                    weights.Clear();
                    continue;
                }

                weights.Add(double.Parse(currentLine));
            }

            weightsFile.Close();
        }

        private void ClassifyBtnClick(object sender, EventArgs e)
        {
             if (Directory.GetFiles(Directory.GetCurrentDirectory()).Any(file => file == Directory.GetCurrentDirectory() + "weights.txt")) {
                MessageBox.Show("Warning", "No weights file, you need to train your NN first");
             }

             if (!_weightsLoaded)
             {
                 ReadWeights();
                 _weightsLoaded = true;
             }

            var digitMatrix = GetImage(_bitmap);
            var prediction = _handwrittenDigitRecogNn.Predict(digitMatrix.Cast<double>().Select(c => c).ToArray());
            classBox.Text = (prediction + 1).ToString();
        }

        private void TrainBtnClick(object sender, EventArgs e)
        {
            var trainingDataSet = new List<TrainingSample>();
            var trainingDataSetFiles = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Digits");
            
            foreach(var file in trainingDataSetFiles)
            {
                var name = file.Remove(file.LastIndexOf(".")).Substring(file.LastIndexOf("\\") + 1);
                var @class = int.Parse(name.Substring(0, 1));
                var classVec = new[] {0.0, 0.0, 0.0};
                classVec[@class - 1] = 1;
                var imgMatrix = GetImage(new Bitmap(file));
                var imgVector = imgMatrix.Cast<double>().Select(c => c).ToArray();
                trainingDataSet.Add(new TrainingSample(imgVector, @class, classVec));
            }

            _handwrittenDigitRecogNn = new HandwrittenDigitRecognitionNn(trainingDataSet, NnInputs, NnHidden, NnOutputs, 0.002);
            _handwrittenDigitRecogNn.Training();

            var fileWeights = new StreamWriter("weights.txt", false);

            foreach (var layer in _handwrittenDigitRecogNn.Layers)
            {
                foreach (var unit in layer.Units)
                {
                    foreach (var w in unit.Weights)
                        fileWeights.WriteLine(w);
                    fileWeights.WriteLine("*");
                }
                fileWeights.WriteLine("-");
            }

            fileWeights.Close();

            MessageBox.Show("Training Complete!", "Message");
        }

        private double [,] GetImage(Bitmap bitmap)
        {
            var result = new double[bitmap.Width, bitmap.Height];

            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    result[i, j] = pixel.R + pixel.G + pixel.B == 0 ? 0 : 1;
                }
            }

            return result;
        }

        private void CleanBtnClick(object sender, EventArgs e)
        {
            _bitmap = new Bitmap(paintBox.Width, paintBox.Height);
            paintBox.Refresh();
        }

    }
}
