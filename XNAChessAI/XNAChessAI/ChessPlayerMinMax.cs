using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace XNAChessAI
{
    class ChessPlayerMinMax : ChessPlayer
    {
        public Move[] GetAllMoves(ChessBoard Board, ChessPlayer Player)
        {
            List<Move> moves = new List<Move>(8 * 8);
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    if (Board.GetChessPieceFromPoint(x, y) != null && Board.GetChessPieceFromPoint(x, y).Parent == Player)
                    {
                        Point[] targets = Board.GetAllPossibleMovesForPiece(x, y);
                        for (int i = 0; i < targets.Length; i++)
                            moves.Add(new Move(new Point(x, y), targets[i]));
                    }
                }
            return moves.ToArray();
        }
        public int EvaluationFunction(ChessBoard Board)
        {
            int re = 0;
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    ChessPiece Piece = Board.GetChessPieceFromPoint(x, y);
                    if (Piece != null)
                    {
                        if (Piece.Parent == this)
                        {
                            switch (Piece.Type)
                            {
                                case ChessPieceType.Pawn:
                                    re += 10;
                                    break;
                                case ChessPieceType.Knight:
                                    re += 30;
                                    break;
                                case ChessPieceType.Rook:
                                    re += 50;
                                    break;
                                case ChessPieceType.Bishop:
                                    re += 30;
                                    break;
                                case ChessPieceType.Queen:
                                    re += 90;
                                    break;
                                case ChessPieceType.King:
                                    re += 900;
                                    break;
                            }
                        }
                        else
                        {
                            switch (Piece.Type)
                            {
                                case ChessPieceType.Pawn:
                                    re -= 10;
                                    break;
                                case ChessPieceType.Knight:
                                    re -= 30;
                                    break;
                                case ChessPieceType.Rook:
                                    re -= 50;
                                    break;
                                case ChessPieceType.Bishop:
                                    re -= 30;
                                    break;
                                case ChessPieceType.Queen:
                                    re -= 90;
                                    break;
                                case ChessPieceType.King:
                                    re -= 900;
                                    break;
                            }
                        }
                    }
                }
            return re;
        }
        public Move MiniMax(int depth, ChessBoard Board, Move lastMove, bool maximising)
        {
            if (depth == 0)
            {
                Move m = new Move();
                m.rating = EvaluationFunction(Board);
                return m;
            }
            Move[] Moves;
            if (maximising)
            {
                Moves = GetAllMoves(Board, this);
                Move bestMove = new Move();
                bestMove.rating = int.MinValue;
                for (int i = 0; i < Moves.Length; i++)
                {
                    ChessBoard Clone = (ChessBoard)Board.Clone();
                    Clone.MovePiece(Moves[i].From, Moves[i].To);
                    if (Clone.GameEnded && Clone.Winner != this)
                        continue;
                    Moves[i].rating = EvaluationFunction(Clone);
                    if (Moves[i].rating < lastMove.rating)
                        break;
                    if (Clone.Winner == this)
                        return new Move(Moves[i].From, Moves[i].To, int.MaxValue);
                    Move minimax = MiniMax(depth - 1, Clone, Moves[i], !maximising);
                    if (minimax.rating > bestMove.rating)
                        bestMove = Moves[i];
                }
                if (bestMove.rating <= 0)
                    bestMove = Moves.OrderBy(x => Values.RDM.Next(int.MaxValue)).OrderByDescending(x => x.rating).First();
                if (bestMove.From == Point.Zero && bestMove.To == Point.Zero)
                    Debug.WriteLine("well fuck");
                return bestMove;
            }
            else
            {
                Moves = GetAllMoves(Board, Board.GetOponent(this));
                Move bestMove = new Move();
                bestMove.rating = int.MaxValue;
                for (int i = 0; i < Moves.Length; i++)
                {
                    ChessBoard Clone = (ChessBoard)Board.Clone();
                    Clone.MovePiece(Moves[i].From, Moves[i].To);
                    if (Clone.GameEnded && Clone.Winner != this)
                        continue;
                    Moves[i].rating = EvaluationFunction(Clone);
                    if (Moves[i].rating > lastMove.rating)
                        break;
                    Move minimax = MiniMax(depth - 1, Clone, Moves[i], !maximising);
                    if (minimax.rating < bestMove.rating)
                        bestMove = Moves[i];
                }
                if (bestMove.rating == 0)
                    bestMove = Moves.OrderBy(x => Values.RDM.Next(int.MaxValue)).OrderByDescending(x => -x.rating).First();
                if (bestMove.From == Point.Zero && bestMove.To == Point.Zero)
                    Debug.WriteLine("well fuck");
                return bestMove;
            }
        }
        
        public override void Update()
        {
            Move minimax = MiniMax(4, Parent, new Move(), true);
            if (minimax.From == Point.Zero && minimax.To == Point.Zero)
            {
                Debug.WriteLine("I dunnu wat im doin!");
                Move[] moves = GetAllMoves(Parent, this);
                minimax = moves[Values.RDM.Next(moves.Length)];
            }
            Parent.MovePiece(minimax);
        }
    }
}