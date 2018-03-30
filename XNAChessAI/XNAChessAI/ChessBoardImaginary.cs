using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;

namespace XNAChessAI
{
    public class ChessBoardImaginary : ChessBoard, ICloneable
    {
        public new ChessPiece[,] Pieces = new ChessPiece[8, 8];
        public ChessBoardImaginary(ChessBoard Board) : base(Board.PlayerTop, Board.PlayerBottom)
        {
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    Pieces[x, y] = Board.GetChessPieceFromPoint(x, y);
                    if (Pieces[x, y] != null && Pieces[x, y].Type == ChessPieceType.King)
                    {
                        if (Pieces[x, y].Parent == PlayerTop)
                            TopKing = Pieces[x, y];
                        else
                            BottomKing = Pieces[x, y];
                    }
                }
        }

        public new bool MovePiece(Point from, Point to)
        {
            ChessPiece FromPiece = GetChessPieceFromPoint(from);
            ChessPlayer MovePlayer = PlayerWhoHasTheMove();
            List<Point> AllPossibleMoves = new List<Point>(GetAllPossibleMovesForPiece(from));

            Turns++;

            if (Pieces[to.X, to.Y] == TopKing)
            {
                GameEnded = true;
                Winner = PlayerBottom;

                GameLengths.Add(Turns);
                NormallyEndedGames++;
                Turns = 0;
            }
            if (Pieces[to.X, to.Y] == BottomKing)
            {
                GameEnded = true;
                Winner = PlayerTop;

                GameLengths.Add(Turns);
                NormallyEndedGames++;
                Turns = 0;
            }

            Pieces[to.X, to.Y] = Pieces[from.X, from.Y];
            Pieces[from.X, from.Y] = null;
            if (Pieces[to.X, to.Y] != null)
                Pieces[to.X, to.Y].HasMoved = true;
            else
                return false;

            Turn = !Turn;
            PlayerWhoHasTheMove().TurnStarted();
            
            if (FromPiece.Parent == MovePlayer && AllPossibleMoves.Contains(to))
            {
                ThreefoldRepetitionCheck[to.X, to.Y, (int)Pieces[to.X, to.Y].Type + (Pieces[to.X, to.Y].Parent == PlayerBottom ? 0 : 6)]++;
                if (ThreefoldRepetitionCheck[to.X, to.Y, (int)Pieces[to.X, to.Y].Type + (Pieces[to.X, to.Y].Parent == PlayerBottom ? 0 : 6)] >= AllowedRepetitions)
                    return false;
            }
            else
                return false;

            return true;
        }
        public new object Clone()
        {
            ChessBoardImaginary re = (ChessBoardImaginary)MemberwiseClone();
            re.Pieces = new ChessPiece[8, 8];
            for (int x = 0; x < Pieces.GetLength(0); x++)
                for (int y = 0; y < Pieces.GetLength(1); y++)
                {
                    if (Pieces[x, y] != null)
                    {
                        re.Pieces[x, y] = (ChessPiece)Pieces[x, y].Clone();
                        if (re.Pieces[x, y].Type == ChessPieceType.King)
                        {
                            if (re.Pieces[x, y].Parent == PlayerTop)
                                re.TopKing = re.Pieces[x, y];
                            else
                                re.BottomKing = re.Pieces[x, y];
                        }
                    }
                }
            for (int x = 0; x < ThreefoldRepetitionCheck.GetLength(0); x++)
                for (int y = 0; y < ThreefoldRepetitionCheck.GetLength(1); y++)
                    for (int z = 0; z < ThreefoldRepetitionCheck.GetLength(2); z++)
                        re.ThreefoldRepetitionCheck[x, y, z] = ThreefoldRepetitionCheck[x, y, z];
            return re;
        }
    }
}
