using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Practical.AI.Reinforcement_Learning.Maze
{
    public partial class MazeGui : Form
    {
        private readonly int _n;
        private readonly int _m;
        private readonly bool[,] _map;
        private readonly QAgent _agent;
        private Stopwatch _stopWatch;
        private int _episode;

        public MazeGui(int n, int m, bool [,] map, double [,] reward)
        {
            InitializeComponent();
            timer.Interval = 100;
            _n = n;
            _m = m;
            _map = map;
            _agent = new QAgent(0, 0, 0.9, _n, _m, reward, map, .9);
            _stopWatch = new Stopwatch();
        }

        private void MazeBoardPaint(object sender, PaintEventArgs e)
        {
            var pen = new Pen(Color.Wheat);

            var cellWidth = mazeBoard.Width / _n;
            var cellHeight = mazeBoard.Height / _m;

            for (var i = 0; i < _n; i++)
                e.Graphics.DrawLine(pen, new Point(i * cellWidth, 0), new Point(i * cellWidth, i * cellWidth + mazeBoard.Height));

            for (var i = 0; i < _m; i++)
                e.Graphics.DrawLine(pen, new Point(0, i * cellHeight), new Point(i * cellHeight + mazeBoard.Width, i * cellHeight));


            for (var i = 0; i < _map.GetLength(0); i++)
            {
                for (var j = 0; j < _map.GetLength(1); j++)
                {
                    if (!_map[i, j])
                        e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), j * cellWidth, i * cellHeight, cellWidth, cellHeight);
                }
            }

            for (var i = 0; i < _map.GetLength(0); i++)
            {
                for (var j = 0; j < _map.GetLength(1); j++)
                {
                    if (_map[i, j])
                        e.Graphics.DrawString(String.Format("{0:0.00}", _agent.QTable[new Tuple<int, int>(i, j)][0].ToString(CultureInfo.GetCultureInfo("en-US"))) + "," +
                                          String.Format("{0:0.00}", _agent.QTable[new Tuple<int, int>(i, j)][1].ToString(CultureInfo.GetCultureInfo("en-US"))) + "," +
                                          String.Format("{0:0.00}", _agent.QTable[new Tuple<int, int>(i, j)][2].ToString(CultureInfo.GetCultureInfo("en-US"))) + "," +
                                          String.Format("{0:0.00}", _agent.QTable[new Tuple<int, int>(i, j)][3].ToString(CultureInfo.GetCultureInfo("en-US")))
                    , new Font("Arial", 8, FontStyle.Bold), new SolidBrush(Color.White), j * cellWidth, i * cellHeight);
                }
            }

            e.Graphics.FillEllipse(new SolidBrush(Color.Tomato), _agent.Y * cellWidth, _agent.X * cellHeight, cellWidth, cellHeight);
            e.Graphics.DrawString("Exit", new Font("Arial", 12, FontStyle.Bold), new SolidBrush(Color.Yellow), (_m - 1) * cellWidth + 15, (_n - 1) * cellHeight + 15);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (!_stopWatch.IsRunning)
                _stopWatch.Start();
            if (_agent.X != _n - 1 || _agent.Y != _m - 1)
                _agent.QLearning(_episode < 20);
            else
            {
                _agent.QTable[new Tuple<int, int>(_n - 1, _m - 1)] = new List<double>
                                                                         {
                                                                             _agent.Reward[_n - 1, _m - 1],
                                                                             _agent.Reward[_n - 1, _m - 1],
                                                                             _agent.Reward[_n - 1, _m - 1],
                                                                             _agent.Reward[_n - 1, _m - 1]
                                                                         };
                _stopWatch.Stop();
                _agent.Reset();
                
                var file = new StreamWriter("E:/time_difference.txt", true);
                file.WriteLine(_stopWatch.ElapsedMilliseconds);
                file.Close();

                _stopWatch.Reset();
                _episode++;
            }

            mazeBoard.Refresh();
        }
    }
}
