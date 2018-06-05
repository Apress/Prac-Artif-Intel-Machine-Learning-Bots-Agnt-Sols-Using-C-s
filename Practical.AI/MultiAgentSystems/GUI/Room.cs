using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Practical.AI.MultiAgentSystems.GUI
{
    public partial class Room : Form
    {
        public List<MasCleaningAgent> CleaningAgents;
        private int _n;
        private int _m;
        private int[,] _room;

        public Room(int n, int m, int[,] room)
        {
            _n = n;
            _m = m;
            _room = room;
            CleaningAgents = new List<MasCleaningAgent>();
            InitializeComponent(); 
        }

        private void RoomPicturePaint(object sender, PaintEventArgs e)
        {
            var pen = new Pen(Color.Wheat);
            var cellWidth = roomPicture.Width / _m;
            var cellHeight = roomPicture.Height / _n;

            // Draw room grid
            for (var i = 0; i < _m; i++)
                e.Graphics.DrawLine(pen, new Point(i * cellWidth, 0), new Point(i * cellWidth, i * cellWidth + roomPicture.Height));

            for (var i = 0; i < _n; i++)
                e.Graphics.DrawLine(pen, new Point(0, i * cellHeight), new Point(i * cellHeight + roomPicture.Width, i * cellHeight));

            // Draw agents
            for (var i = 0; i < CleaningAgents.Count; i++)
                e.Graphics.FillEllipse(new SolidBrush(CleaningAgents[i].Color), CleaningAgents[i].Y * cellWidth, CleaningAgents[i].X * cellHeight, cellWidth, cellHeight);

            // Draw Dirt
            for (var i = 0; i < _n; i++)
            {
                for (var j = 0; j < _m; j++)
                    if (_room[i, j] > 0)
                        e.Graphics.DrawImage(new Bitmap("rock-transparency.png"), j * cellWidth, i * cellHeight, cellWidth, cellHeight);
            }
        }

        private void RoomPictureResize(object sender, EventArgs e)
        {
            Refresh();
        }

    }
}
