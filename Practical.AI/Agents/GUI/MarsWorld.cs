using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Practical.AI.Agents.GUI
{
    public partial class MarsWorld : Form
    {
        private MarsRover _marsRover;
        private Mars _mars;
        private int _n;
        private int _m;

        public MarsWorld(MarsRover rover, Mars mars, int n, int m)
        {
            InitializeComponent();
            _marsRover = rover;
            _mars = mars;
            _n = n;
            _m = m;
        }

        private void TerrainPaint(object sender, PaintEventArgs e)
        {
            var pen = new Pen(Color.Wheat);
            var waterColor = new SolidBrush(Color.Aqua);
            var rockColor = new SolidBrush(Color.Chocolate);
            var cellWidth = terrain.Width/_n;
            var cellHeight = terrain.Height/_m;

            for (var i = 0; i < _n; i++)
                e.Graphics.DrawLine(pen, new Point(i * cellWidth, 0), new Point(i * cellWidth, i * cellWidth + terrain.Height));

            for (var i = 0; i < _m; i++)
                e.Graphics.DrawLine(pen, new Point(0, i * cellHeight), new Point(i * cellHeight + terrain.Width, i * cellHeight));

            if (_marsRover.ExistsPlan())
            {
                foreach (var cell in _marsRover.CurrentPlan.Path)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), cell.Item2 * cellWidth, cell.Item1 * cellHeight,
                                             cellWidth, cellHeight);
                }
            }

            for (var i = 0; i < _n; i++)
            {
                for (var j = 0; j < _m; j++)
                {
                    if (_mars.TerrainAt(i, j) > _marsRover.RunningOverThreshold)
                        e.Graphics.DrawImage(new Bitmap("obstacle-transparency.png"), j*cellWidth, i*cellHeight,
                                             cellWidth, cellHeight);
                    if (_mars.WaterAt(i, j))
                        e.Graphics.DrawImage(new Bitmap("water-transparency.png"), j * cellWidth, i * cellHeight, cellWidth, cellHeight);

                    // Draw every belief in white 
                    foreach (var belief in _marsRover.Beliefs)
                    {
                        var pred = belief.Predicate as List<Tuple<int, int>>;
                        if (pred != null && !pred.Contains(new Tuple<int, int>(i, j)))
                            continue;

                        if (belief.Name == TypesBelief.ObstaclesOnTerrain)
                        {
                            e.Graphics.DrawImage(new Bitmap("obstacle-transparency.png"), j * cellWidth, i * cellHeight,
                                                 cellWidth, cellHeight);
                            e.Graphics.DrawRectangle(new Pen(Color.Gold, 6), j * cellWidth, i * cellHeight,
                                                 cellWidth, cellHeight);
                        }
                        if (belief.Name == TypesBelief.PotentialWaterSpots)
                        {
                            e.Graphics.DrawImage(new Bitmap("water-transparency.png"), j * cellWidth, i * cellHeight,
                                                 cellWidth, cellHeight);
                            e.Graphics.DrawRectangle(new Pen(Color.Gold, 6), j * cellWidth, i * cellHeight,
                                                 cellWidth, cellHeight);
                        }

                    }
                    
                }   
            }

            e.Graphics.DrawImage(new Bitmap("rover-transparency.png"), _marsRover.Y * cellWidth, _marsRover.X * cellHeight, cellWidth, cellHeight);

            var sightColor = Color.FromArgb(80, Color.Lavender);
            _marsRover.GetCurrentTerrain();

            foreach (var cell in _marsRover.CurrentTerrain)
                e.Graphics.FillRectangle(new SolidBrush(sightColor), cell.Item2 * cellWidth, cell.Item1 * cellHeight, cellWidth, cellHeight);
        }

        private void TimerAgentTick(object sender, EventArgs e)
        {
            var percepts = _marsRover.GetPercepts();
            agentState.Text = "State: Thinking ...";
            agentState.Refresh();
            var action = _marsRover.Action(percepts);
            _marsRover.ExecuteAction(action, percepts);

            var beliefs = UpdateText(beliefsList, _marsRover.Beliefs);
            var desires = UpdateText(beliefsList, _marsRover.Desires);
            var intentions = UpdateText(beliefsList, _marsRover.Intentions);
         
            if (beliefs != beliefsList.Text)
                beliefsList.Text = beliefs;
            if (desires != desiresList.Text)
                desiresList.Text = desires;
            if (intentions != intentionsList.Text)
                intentionsList.Text = intentions;
            foreach (var wSpot in _marsRover.WaterFound)
            {
                if (!waterFoundList.Items.Contains(wSpot))
                    waterFoundList.Items.Add(wSpot);
            }
           

            Refresh();
        }

        private string UpdateText(RichTextBox list, IEnumerable<object> elems)
        {
            var result = "";

            foreach (var elem in elems)
                result += elem;

            return result;
        }

        private void PauseBtnClick(object sender, EventArgs e)
        {
            if (timerAgent.Enabled)
            {
                timerAgent.Stop();
                pauseBtn.Text = "Play";
            }
            else
            {  
                timerAgent.Start();
                pauseBtn.Text = "Pause";
            }
        }

        private void MarsWorldLoad(object sender, EventArgs e)
        {
            foreach (var b in _marsRover.Beliefs)
                beliefsList.Text += b;
            foreach (var d in _marsRover.Desires)
                desiresList.Text += d;
            foreach (var i in _marsRover.Intentions)
                intentionsList.Text += i;
        }

       
    }
}
