using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.GameTheory
{
    public class OthelloBoard
    {
        public int[,] Board { get; set; }
        public int N { get; set; }
        public int M { get; set; }
        public int Turn { get; set; }
        public List<Tuple<int, int>> Player1Pos { get; set; }
        public List<Tuple<int, int>> Player2Pos { get; set; }
        public Tuple<int, int> MoveFrom { get; set; } 
        internal double UtilityValue { get; set; }
        internal readonly Dictionary<Tuple<int, int>, List<Tuple<int, int>>> Flips; 

        public OthelloBoard(int n, int m)
        {
            Board = new int[n, m];
            Turn = 1;
            Flips = new Dictionary<Tuple<int, int>, List<Tuple<int, int>>>();
            //Player1Pos = new List<Tuple<int, int>>
            //                  {
            //                      new Tuple<int, int>(n / 2 - 1, m / 2),
            //                      new Tuple<int, int>(n / 2, m / 2 - 1)
            //                  };
            //Player2Pos = new List<Tuple<int, int>>
            //                  {
            //                      new Tuple<int, int>(n / 2 - 1, m / 2 - 1),
            //                      new Tuple<int, int>(n / 2, m / 2)
            //                  };
            Player1Pos = new List<Tuple<int, int>>
                              {
                                  new Tuple<int, int>(3, 4),
                                  new Tuple<int, int>(4, 4),
                                  new Tuple<int, int>(5, 4),
                                  new Tuple<int, int>(4, 3),
                                  new Tuple<int, int>(3, 2)
                              };
            Player2Pos = new List<Tuple<int, int>>
                              {
                                  new Tuple<int, int>(3, 3),
                                  new Tuple<int, int>(3, 5),
                                  new Tuple<int, int>(1, 3),
                                  new Tuple<int, int>(2, 4),
                                  new Tuple<int, int>(4, 2)
                              };

            // Initial Positions
            //Board[n / 2 - 1, m / 2 - 1] = 2;
            //Board[n / 2, m / 2] = 2;
            //Board[n / 2 - 1, m / 2] = 1;
            //Board[n / 2, m / 2 - 1] = 1;
            Board[3, 3] = 2;
            Board[3, 5] = 2;
            Board[1, 3] = 2; 
            Board[2, 4] = 2;
            Board[4, 2] = 2;

            Board[3, 4] = 1;
            Board[4, 4] = 1;
            Board[5, 4] = 1;
            Board[4, 3] = 1;
            Board[3, 2] = 1;
            N = n;
            M = m;
        }

        private OthelloBoard(OthelloBoard othelloBoard)
        {
            Board = new int[othelloBoard.N, othelloBoard.M];
            M = othelloBoard.M;
            N = othelloBoard.N;
            Turn = othelloBoard.Turn;
            Flips = new Dictionary<Tuple<int, int>, List<Tuple<int, int>>>(othelloBoard.Flips);
            Array.Copy(othelloBoard.Board, Board, othelloBoard.N * othelloBoard.M);
            Player1Pos = new List<Tuple<int, int>>(othelloBoard.Player1Pos);
            Player2Pos = new List<Tuple<int, int>>(othelloBoard.Player2Pos);
        }

        public bool EmptyCell(int i, int j)
        {
            return Board[i, j] == 0;
        }

        public List<OthelloBoard> Expand(int player)
        {
            var result = new List<OthelloBoard>();
            var moves = AvailableMoves(player);

            foreach (var m in moves)
            {
                var newBoard = SetPieceCreatedBoard(m.Item1, m.Item2, player);
                newBoard.MoveFrom = m;
                result.Add(newBoard);
            }

            return result;
        } 

        public List<Tuple<int, int>> AvailableMoves(int player)
        {
            var result = new List<Tuple<int, int>>();
            var oppPlayerPositions = player == 1 ? Player2Pos : Player1Pos;

            foreach (var oppPlayerPos in oppPlayerPositions)
                result.AddRange(AvailableMovesAroundPiece(oppPlayerPos, player));
            
            return result;
        }

        private bool IsLegalMove(int i, int j)
        {
            return i >= 0 && i < N && j >= 0 && j < M && EmptyCell(i, j);
        }

        private IEnumerable<Tuple<int, int>> AvailableMovesAroundPiece(Tuple<int, int> oppPlayerPos, int player)
        {
            var result = new List<Tuple<int, int>>();
            var tempFlips = new List<Tuple<int, int>>();

            // Check Down
            if (IsLegalMove(oppPlayerPos.Item1 + 1, oppPlayerPos.Item2))
            {
                var up = CheckUpDown(oppPlayerPos, player, (i => i >= 0), -1, tempFlips);
                if (up)
                {
                    UpdateFlips(new Tuple<int, int>(oppPlayerPos.Item1 + 1, oppPlayerPos.Item2), tempFlips);
                    result.Add(new Tuple<int, int>(oppPlayerPos.Item1 + 1, oppPlayerPos.Item2));
                }
            }

            // Check Up
            if (IsLegalMove(oppPlayerPos.Item1 - 1, oppPlayerPos.Item2))
            {
                tempFlips.Clear();
                var down = CheckUpDown(oppPlayerPos, player, (i => i < N), 1, tempFlips);
                if (down)
                {
                    UpdateFlips(new Tuple<int, int>(oppPlayerPos.Item1 - 1, oppPlayerPos.Item2), tempFlips);
                    result.Add(new Tuple<int, int>(oppPlayerPos.Item1 - 1, oppPlayerPos.Item2));
                }
            }

            // Check Left
            if (IsLegalMove(oppPlayerPos.Item1, oppPlayerPos.Item2 - 1))
            {
                tempFlips.Clear();
                var rgt = CheckLftRgt(oppPlayerPos, player, (i => i < M), 1, tempFlips);
                if (rgt)
                {
                    UpdateFlips(new Tuple<int, int>(oppPlayerPos.Item1, oppPlayerPos.Item2 - 1), tempFlips);
                    result.Add(new Tuple<int, int>(oppPlayerPos.Item1, oppPlayerPos.Item2 - 1));
                }
            }

            // Check Right
            if (IsLegalMove(oppPlayerPos.Item1, oppPlayerPos.Item2 + 1))
            {
                tempFlips.Clear();
                var lft = CheckLftRgt(oppPlayerPos, player, (i => i >= 0), -1, tempFlips);
                if (lft)
                {
                    UpdateFlips(new Tuple<int, int>(oppPlayerPos.Item1, oppPlayerPos.Item2 + 1), tempFlips);
                    result.Add(new Tuple<int, int>(oppPlayerPos.Item1, oppPlayerPos.Item2 + 1));
                }
            }

            // Check Up Lft
            if (IsLegalMove(oppPlayerPos.Item1 - 1, oppPlayerPos.Item2 - 1))
            {
                tempFlips.Clear();
                var downRgt = CheckDiagonal(oppPlayerPos, player, (i => i < N), (i => i < M), 1, 1, tempFlips);
                if (downRgt)
                {
                    UpdateFlips(new Tuple<int, int>(oppPlayerPos.Item1 - 1, oppPlayerPos.Item2 - 1), tempFlips);
                    result.Add(new Tuple<int, int>(oppPlayerPos.Item1 - 1, oppPlayerPos.Item2 - 1));
                }
            }

            // Check Down Lft
            if (IsLegalMove(oppPlayerPos.Item1 + 1, oppPlayerPos.Item2 - 1))
            {
                tempFlips.Clear();
                var upRgt = CheckDiagonal(oppPlayerPos, player, (i => i >= 0), (i => i < M), -1, 1, tempFlips);
                if (upRgt)
                {
                    UpdateFlips(new Tuple<int, int>(oppPlayerPos.Item1 + 1, oppPlayerPos.Item2 - 1), tempFlips);
                    result.Add(new Tuple<int, int>(oppPlayerPos.Item1 + 1, oppPlayerPos.Item2 - 1));
                }
            }

            // Check Up Rgt
            if (IsLegalMove(oppPlayerPos.Item1 - 1, oppPlayerPos.Item2 + 1))
            {
                tempFlips.Clear();
                var downLft = CheckDiagonal(oppPlayerPos, player, (i => i < N), (i => i >= 0), 1, -1, tempFlips);
                if (downLft)
                {
                    UpdateFlips(new Tuple<int, int>(oppPlayerPos.Item1 - 1, oppPlayerPos.Item2 + 1), tempFlips);
                    result.Add(new Tuple<int, int>(oppPlayerPos.Item1 - 1, oppPlayerPos.Item2 + 1));
                }
            }

            // Check Down Rgt
            if (IsLegalMove(oppPlayerPos.Item1 + 1, oppPlayerPos.Item2 + 1))
            {
                tempFlips.Clear();
                var upLft = CheckDiagonal(oppPlayerPos, player, (i => i >= 0), (i => i >= 0), -1, -1, tempFlips);
                if (upLft)
                {
                    UpdateFlips(new Tuple<int, int>(oppPlayerPos.Item1 + 1, oppPlayerPos.Item2 + 1), tempFlips);
                    result.Add(new Tuple<int, int>(oppPlayerPos.Item1 + 1, oppPlayerPos.Item2 + 1));
                }
            }

            return result;
        }

        private bool CheckUpDown(Tuple<int, int> oppPlayerPos, int player, Func<int, bool> condition, int direction, List<Tuple<int, int>> tempFlips)
        {
            for (var i = oppPlayerPos.Item1; condition(i); i+=direction)
            {
                if (Board[i, oppPlayerPos.Item2] == player)
                {
                    UpdateFlips(oppPlayerPos, tempFlips);
                    return true;
                }
                if (EmptyCell(i, oppPlayerPos.Item2))
                {
                    tempFlips.Clear();
                    break;
                }
                tempFlips.Add(new Tuple<int, int>(i, oppPlayerPos.Item2));
            }

            return false;
        }

        private void UpdateFlips(Tuple<int, int> oppPlayerPos, IEnumerable<Tuple<int, int>> tempFlips)
        {
            if (!Flips.ContainsKey(oppPlayerPos))
                Flips.Add(oppPlayerPos, new List<Tuple<int, int>>(tempFlips));
            else
                Flips[oppPlayerPos].AddRange(tempFlips);
        }

        private bool CheckLftRgt(Tuple<int, int> oppPlayerPos, int player, Func<int, bool> condition, int direction, List<Tuple<int, int>> tempFlips)
        {
            for (var i = oppPlayerPos.Item2; condition(i); i+= direction)
            {
                if (Board[oppPlayerPos.Item1, i] == player)
                {
                    UpdateFlips(oppPlayerPos, tempFlips);
                    return true;
                }
                if (EmptyCell(oppPlayerPos.Item1, i))
                {
                    tempFlips.Clear();
                    break;
                }
                tempFlips.Add(new Tuple<int, int>(oppPlayerPos.Item1, i));
            }

            return false;
        }

        private bool CheckDiagonal(Tuple<int, int> oppPlayerPos, int player, Func<int, bool> conditionRow, Func<int, bool> conditionCol, int directionRow, int directionCol, List<Tuple<int, int>> tempFlips)
        {
            var i = oppPlayerPos.Item1;
            var j = oppPlayerPos.Item2;

            while(conditionRow(i) && conditionCol(j))
            {
                if (Board[i, j] == player)
                {
                    UpdateFlips(oppPlayerPos, tempFlips);
                    return true;
                }

                if (EmptyCell(i, j))
                {
                    tempFlips.Clear();
                    break;
                }
                tempFlips.Add(new Tuple<int, int>(i, j));
                i += directionRow;
                j += directionCol;
            }

            return false;
        }

        public void SetPiece(int i, int j, int player)
        {
            Board[i, j] = player;
            FlipPieces(i, j, player, this);
        }

        public OthelloBoard SetPieceCreatedBoard(int i, int j, int player)
        {
            var newOthello = new OthelloBoard(this);
            newOthello.Board[i, j] = player;
            FlipPieces(i, j, player, newOthello);

            newOthello.Flips.Clear();
            return newOthello;
        }

        private void FlipPieces(int i, int j, int player, OthelloBoard othello)
        {
            var piecesToFlip = Flips[new Tuple<int, int>(i, j)];
            UpdatePiecePos(new Tuple<int, int>(i, j), player, othello);

            foreach (var pair in piecesToFlip)
            {
                othello.Board[pair.Item1, pair.Item2] = player;
                UpdatePiecePos(pair, player, othello);
            }
        }

        private void UpdatePiecePos(Tuple<int, int> pair, int player, OthelloBoard othello)
        {
            var removeFrom = player == 1 ? othello.Player2Pos : othello.Player1Pos;
            var addTo = player == 1 ? othello.Player1Pos : othello.Player2Pos;

            if (!addTo.Contains(pair)) 
                addTo.Add(pair);
            removeFrom.Remove(pair);
        }

        internal double HeuristicUtility()
        {
            return PieceDifference();
        }

        private int PieceDifference()
        {
            if (Player1Pos.Count == Player2Pos.Count)
                return 0;
            if (Player1Pos.Count > Player2Pos.Count)
                return 100 * Player1Pos.Count / (Player1Pos.Count + Player2Pos.Count);
            return -100 * Player2Pos.Count / (Player1Pos.Count + Player2Pos.Count);
        }
    }
}
