using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Practical.AI.GameTheory.AdversarialSearch;

namespace Practical.AI.GameTheory
{
    public partial class OthelloGui : Form
    {
        private readonly int _n;
        private readonly int _m;
        private readonly OthelloBoard _othelloBoard;
        private List<Tuple<int, int>> _availableMoves;
        private int _cellWidth;
        private int _cellHeight;
        private Minimax _minimax;

        public OthelloGui(OthelloBoard othelloBoard)
        {
            InitializeComponent();
            _othelloBoard = othelloBoard;
            _n = _othelloBoard.N;
            _m = _othelloBoard.M;
            _availableMoves = _othelloBoard.AvailableMoves(_othelloBoard.Turn);
            turnBox.BackColor = _othelloBoard.Turn == 1 ? Color.Black : Color.White;
            _minimax = new Minimax(3, false);
            aiPlayTimer.Enabled = true;
        }

        private void BoardPaint(object sender, PaintEventArgs e)
        {
            var pen = new Pen(Color.Wheat);
            
            _cellWidth = board.Width / _n;
            _cellHeight = board.Height / _m;

            for (var i = 0; i < _n; i++)
                e.Graphics.DrawLine(pen, new Point(i * _cellWidth, 0), new Point(i * _cellWidth, i * _cellWidth + board.Height));

            for (var i = 0; i < _m; i++)
                e.Graphics.DrawLine(pen, new Point(0, i * _cellHeight), new Point(i * _cellHeight + board.Width, i * _cellHeight));

            for (var i = 0; i < _n; i++)
            {
                for (var j = 0; j < _m; j++)
                {
                    if (_othelloBoard.Board[i, j] == 1)
                        e.Graphics.FillEllipse(new SolidBrush(Color.Black), j * _cellWidth, i * _cellHeight, _cellWidth, _cellHeight);
                    if (_othelloBoard.Board[i, j] == 2)
                        e.Graphics.FillEllipse(new SolidBrush(Color.White), j * _cellWidth, i * _cellHeight, _cellWidth, _cellHeight);
                }
            }

            foreach (var availableMove in _availableMoves)
                e.Graphics.DrawRectangle(new Pen(Color.Yellow, 5), availableMove.Item2 * _cellWidth, availableMove.Item1 * _cellHeight, _cellWidth, _cellHeight);
            
        }

        private void BoardMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var click = new Tuple<int, int>(e.Y / _cellWidth, e.X / _cellHeight);
                if (_availableMoves.Contains(click))
                {
                    _othelloBoard.SetPiece(click.Item1, click.Item2, _othelloBoard.Turn);
                    UpdateBoardGui();
                }
            }
        }

        private void UpdateBoardGui()
        {
            blackCountLabel.Text = "Blacks: " + _othelloBoard.Player1Pos.Count;
            whiteCountLabel.Text = "Whites: " + _othelloBoard.Player2Pos.Count;

            var blacks = "";
            var whites = "";

            foreach (var black in _othelloBoard.Player1Pos)
                blacks += "(" + black.Item1 + "," + black.Item2 + ")" + '\n';

            foreach (var white in _othelloBoard.Player2Pos)
                whites += "(" + white.Item1 + "," + white.Item2 + ")" + '\n';

            whitesList.Text = whites;
            blacksList.Text = blacks;

            board.Invalidate();

            for (var i = 0; i < 2; i++)
            {
                _othelloBoard.Turn = _othelloBoard.Turn == 1 ? 2 : 1;
                _othelloBoard.Flips.Clear();
                _availableMoves = _othelloBoard.AvailableMoves(_othelloBoard.Turn);
                turnBox.BackColor = _othelloBoard.Turn == 1 ? Color.Black : Color.White;

                if (_availableMoves.Count > 0)
                    return;
            }

            MessageBox.Show("Game Ended", "Result");
        }

        private void AiPlayTimerTick(object sender, EventArgs e)
        {
            if (_othelloBoard.Turn == 2)
            {
                var move = _minimax.GetOptimalMove(_othelloBoard, false);
                _othelloBoard.SetPiece(move.Item1, move.Item2, _othelloBoard.Turn);
                UpdateBoardGui();
            }
        }
        
    }
}
