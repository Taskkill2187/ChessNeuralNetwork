using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;

namespace XNAChessAI
{
    public class InvalidChessPiece : System.Exception { }

    public class ChessBoard
    {
        public ChessPlayer PlayerTop; // Blacc
        public ChessPlayer PlayerBottom; // White
        ChessPiece TopKing;
        ChessPiece BottomKing;

        ChessPiece[,] Pieces = new ChessPiece[8, 8];
        int[,,] ThreefoldRepetitionCheck = new int[8, 8, 12];
        bool Turn = false; // true = PlayerTop, flase = PlayerBottom
        public const int ChessStatusBarHeight = 30;
        public const int ChessAIStatusDisplayWidth = 400;
        public const int ChessFieldSize = 80;
        public Point MouseSelection;
        public List<int> GameLengths = new List<int>();
        int Turns = 0;
        public int EndedGameBecauseOfRecurrance = 0;
        public int NormallyEndedGames = 0;

        bool slow = false;

        public bool GameEnded;
        public ChessPlayer Winner;

        public ChessBoard()
        {
            PlayerTop = new ChessPlayerHuman(this);
            PlayerBottom = new ChessPlayerHuman(this);

            SetUpNewGame();
        }
        public ChessBoard(ChessPlayer PlayerOne, ChessPlayer PlayerTwo)
        {
            PlayerTop = PlayerOne;
            PlayerBottom = PlayerTwo;

            PlayerTop.Parent = this;
            PlayerBottom.Parent = this;

            SetUpNewGame();
        }
        
        public void SetUpNewGame()
        {
            ClearPieces();
            ClearThreefoldRepetitionCheck();
            
            Turn = false;
            Winner = null;
            GameEnded = false;

            // Pawns
            for (int i = 0; i < 8; i++)
                Pieces[i, 1] = new ChessPiece(PlayerTop, ChessPieceType.Pawn);
            for (int i = 0; i < 8; i++)
                Pieces[i, 6] = new ChessPiece(PlayerBottom, ChessPieceType.Pawn);

            // Top Rest
            Pieces[0, 0] = new ChessPiece(PlayerTop, ChessPieceType.Rook);
            Pieces[1, 0] = new ChessPiece(PlayerTop, ChessPieceType.Knight);
            Pieces[2, 0] = new ChessPiece(PlayerTop, ChessPieceType.Bishop);
            Pieces[3, 0] = new ChessPiece(PlayerTop, ChessPieceType.King);
            Pieces[4, 0] = new ChessPiece(PlayerTop, ChessPieceType.Queen);
            Pieces[5, 0] = new ChessPiece(PlayerTop, ChessPieceType.Bishop);
            Pieces[6, 0] = new ChessPiece(PlayerTop, ChessPieceType.Knight);
            Pieces[7, 0] = new ChessPiece(PlayerTop, ChessPieceType.Rook);

            // Bottom Rest
            Pieces[0, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Rook);
            Pieces[1, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Knight);
            Pieces[2, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Bishop);
            Pieces[3, 7] = new ChessPiece(PlayerBottom, ChessPieceType.King);
            Pieces[4, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Queen);
            Pieces[5, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Bishop);
            Pieces[6, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Knight);
            Pieces[7, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Rook);

            TopKing = Pieces[3, 0];
            BottomKing = Pieces[3, 7];
        }
        public void SetUpNewGame(ChessPlayer PlayerTop, ChessPlayer PlayerBottom)
        {
            ClearPieces();
            ClearThreefoldRepetitionCheck();

            this.PlayerTop = PlayerTop;
            this.PlayerBottom = PlayerBottom;
            PlayerTop.Parent = this;
            PlayerBottom.Parent = this;

            Turn = false;
            Winner = null;
            GameEnded = false;
            
            // Pawns
            for (int i = 0; i < 8; i++)
                Pieces[i, 1] = new ChessPiece(PlayerTop, ChessPieceType.Pawn);
            for (int i = 0; i < 8; i++)
                Pieces[i, 6] = new ChessPiece(PlayerBottom, ChessPieceType.Pawn);

            // Top Rest
            Pieces[0, 0] = new ChessPiece(PlayerTop, ChessPieceType.Rook);
            Pieces[1, 0] = new ChessPiece(PlayerTop, ChessPieceType.Knight);
            Pieces[2, 0] = new ChessPiece(PlayerTop, ChessPieceType.Bishop);
            Pieces[3, 0] = new ChessPiece(PlayerTop, ChessPieceType.King);
            Pieces[4, 0] = new ChessPiece(PlayerTop, ChessPieceType.Queen);
            Pieces[5, 0] = new ChessPiece(PlayerTop, ChessPieceType.Bishop);
            Pieces[6, 0] = new ChessPiece(PlayerTop, ChessPieceType.Knight);
            Pieces[7, 0] = new ChessPiece(PlayerTop, ChessPieceType.Rook);

            // Bottom Rest
            Pieces[0, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Rook);
            Pieces[1, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Knight);
            Pieces[2, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Bishop);
            Pieces[3, 7] = new ChessPiece(PlayerBottom, ChessPieceType.King);
            Pieces[4, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Queen);
            Pieces[5, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Bishop);
            Pieces[6, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Knight);
            Pieces[7, 7] = new ChessPiece(PlayerBottom, ChessPieceType.Rook);

            TopKing = Pieces[3, 0];
            BottomKing = Pieces[3, 7];
        }
        public Point[] GetAllPossibleMovesForPiece(Point From)
        {
            if (From.X < 0 || From.Y < 0 || From.X >= 8 || From.Y >= 8 || Pieces[From.X, From.Y] == null)
                return new Point[0];

            List<Point> PossibleMoves = new List<Point>();
            Point UpLeft = new Point();
            Point UpRight = new Point();
            Point DownLeft = new Point();
            Point DownRight = new Point();
            Point RightUp = new Point();
            Point RightDown = new Point();
            Point LeftUp = new Point();
            Point LeftDown = new Point();
            Point Right = new Point();
            Point Down = new Point();
            Point Up = new Point();
            Point Left = new Point();
            Point P = new Point();

            switch (Pieces[From.X, From.Y].Type)
            {
                case ChessPieceType.Pawn:
                    if (GetChessPieceFromPoint(From).Parent == PlayerTop)
                    {
                        if (Pieces[From.X, From.Y].HasMoved)
                        {
                            if (IsFieldVacant(new Point(From.X, From.Y + 1)))
                                PossibleMoves.Add(new Point(From.X, From.Y + 1));
                        }
                        else
                        {
                            if (IsFieldVacant(new Point(From.X, From.Y + 1)))
                            {
                                PossibleMoves.Add(new Point(From.X, From.Y + 1));
                                if (IsFieldVacant(new Point(From.X, From.Y + 2)))
                                    PossibleMoves.Add(new Point(From.X, From.Y + 2));
                            }
                        }

                        Left = new Point(From.X - 1, From.Y + 1);
                        Right = new Point(From.X + 1, From.Y + 1);

                        if (IsFieldInBounds(Left) && GetChessPieceFromPoint(Left) != null && Pieces[Left.X, Left.Y].Parent == PlayerBottom)
                            PossibleMoves.Add(Left);
                        if (IsFieldInBounds(Right) && GetChessPieceFromPoint(Right) != null && Pieces[Right.X, Right.Y].Parent == PlayerBottom)
                            PossibleMoves.Add(Right);
                    }
                    if (GetChessPieceFromPoint(From).Parent == PlayerBottom)
                    {
                        if (Pieces[From.X, From.Y].HasMoved)
                        {
                            if (IsFieldVacant(new Point(From.X, From.Y - 1)))
                                PossibleMoves.Add(new Point(From.X, From.Y - 1));
                        }
                        else
                        {
                            if (IsFieldVacant(new Point(From.X, From.Y - 1)))
                            {
                                PossibleMoves.Add(new Point(From.X, From.Y - 1));
                                if (IsFieldVacant(new Point(From.X, From.Y - 2)))
                                    PossibleMoves.Add(new Point(From.X, From.Y - 2));
                            }
                        }

                        Left = new Point(From.X - 1, From.Y - 1);
                        Right = new Point(From.X + 1, From.Y - 1);

                        if (IsFieldInBounds(Left) && GetChessPieceFromPoint(Left) != null && Pieces[Left.X, Left.Y].Parent == PlayerTop)
                            PossibleMoves.Add(Left);
                        if (IsFieldInBounds(Right) && GetChessPieceFromPoint(Right) != null && Pieces[Right.X, Right.Y].Parent == PlayerTop)
                            PossibleMoves.Add(Right);
                    }
                    break;

                case ChessPieceType.Knight:
                    UpLeft = new Point(From.X - 1, From.Y - 2);
                    UpRight = new Point(From.X + 1, From.Y - 2);
                    DownLeft = new Point(From.X - 1, From.Y + 2);
                    DownRight = new Point(From.X + 1, From.Y + 2);
                    RightUp = new Point(From.X + 2, From.Y - 1);
                    RightDown = new Point(From.X + 2, From.Y + 1);
                    LeftUp = new Point(From.X - 2, From.Y - 1);
                    LeftDown = new Point(From.X - 2, From.Y + 1);

                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), UpLeft))
                        PossibleMoves.Add(UpLeft);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), UpRight))
                        PossibleMoves.Add(UpRight);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), DownLeft))
                        PossibleMoves.Add(DownLeft);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), DownRight))
                        PossibleMoves.Add(DownRight);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), RightUp))
                        PossibleMoves.Add(RightUp);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), RightDown))
                        PossibleMoves.Add(RightDown);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), LeftUp))
                        PossibleMoves.Add(LeftUp);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), LeftDown))
                        PossibleMoves.Add(LeftDown);
                    break;

                case ChessPieceType.King:
                    UpLeft = new Point(From.X - 1, From.Y - 1);
                    UpRight = new Point(From.X + 1, From.Y - 1);
                    DownLeft = new Point(From.X - 1, From.Y + 1);
                    DownRight = new Point(From.X + 1, From.Y + 1);
                    Right = new Point(From.X + 1, From.Y);
                    Down = new Point(From.X, From.Y + 1);
                    Up = new Point(From.X, From.Y - 1);
                    Left = new Point(From.X - 1, From.Y);

                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), UpLeft))
                        PossibleMoves.Add(UpLeft);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), UpRight))
                        PossibleMoves.Add(UpRight);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), DownLeft))
                        PossibleMoves.Add(DownLeft);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), DownRight))
                        PossibleMoves.Add(DownRight);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), Right))
                        PossibleMoves.Add(Right);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), Down))
                        PossibleMoves.Add(Down);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), Up))
                        PossibleMoves.Add(Up);
                    if (CanPlayerMoveThere(PlayerWhoHasTheMove(), Left))
                        PossibleMoves.Add(Left);
                    break;

                case ChessPieceType.Rook:
                    P = new Point(From.X, From.Y + 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.Y++;
                    }

                    P = new Point(From.X, From.Y - 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.Y--;
                    }

                    P = new Point(From.X + 1, From.Y);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X++;
                    }

                    P = new Point(From.X - 1, From.Y);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X--;
                    }
                    break;

                case ChessPieceType.Bishop:
                    P = new Point(From.X + 1, From.Y + 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X++; P.Y++;
                    }

                    P = new Point(From.X - 1, From.Y - 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X--; P.Y--;
                    }

                    P = new Point(From.X + 1, From.Y - 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X++; P.Y--;
                    }

                    P = new Point(From.X - 1, From.Y + 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X--; P.Y++;
                    }
                    break;

                case ChessPieceType.Queen:
                    P = new Point(From.X, From.Y + 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.Y++;
                    }

                    P = new Point(From.X, From.Y - 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.Y--;
                    }

                    P = new Point(From.X + 1, From.Y);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X++;
                    }

                    P = new Point(From.X - 1, From.Y);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X--;
                    }

                    P = new Point(From.X + 1, From.Y + 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X++; P.Y++;
                    }

                    P = new Point(From.X - 1, From.Y - 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X--; P.Y--;
                    }

                    P = new Point(From.X + 1, From.Y - 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X++; P.Y--;
                    }

                    P = new Point(From.X - 1, From.Y + 1);
                    while (CanPlayerMoveThere(PlayerWhoHasTheMove(), P))
                    {
                        PossibleMoves.Add(new Point(P.X, P.Y));
                        if (GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == PlayerWhoHasntTheMove())
                            break;
                        P.X--; P.Y++;
                    }
                    break;
            }

            return PossibleMoves.ToArray();
        }
        public bool IsFieldVacant(Point P)
        {
            return P.X >= 0 && P.Y >= 0 && P.X < 8 && P.Y < 8 && Pieces[P.X, P.Y] == null;
        }
        public bool IsFieldInBounds(Point P)
        {
            return P.X >= 0 && P.Y >= 0 && P.X < 8 && P.Y < 8;
        }
        public bool CanPlayerMoveThere(ChessPlayer CurrentPlayer, Point P)
        {
            ChessPlayer OtherPlayer = null;
            if (CurrentPlayer == PlayerBottom)
                OtherPlayer = PlayerTop;
            else if (CurrentPlayer == PlayerTop)
                OtherPlayer = PlayerBottom;
            else
                throw new ArgumentException();

            return IsFieldInBounds(P) && GetChessPieceFromPoint(P) == null ||
                    IsFieldInBounds(P) && GetChessPieceFromPoint(P) != null && GetChessPieceFromPoint(P).Parent == OtherPlayer;
        }
        public ChessPiece GetChessPieceFromPoint(Point P)
        {
            return Pieces[P.X, P.Y];
        }
        public void DrawFieldAsSelected(Point P, Color Col, SpriteBatch SB)
        {
            Assets.DrawRectangleBorder(new Rectangle(P.X * ChessFieldSize, P.Y * ChessFieldSize + ChessStatusBarHeight, ChessFieldSize, ChessFieldSize), Col * 0.75f, 10, SB);
        }
        public ChessPlayer PlayerWhoHasTheMove()
        {
            if (Turn)
                return PlayerTop;
            else
                return PlayerBottom;
        }
        public ChessPlayer PlayerWhoHasntTheMove()
        {
            if (PlayerWhoHasTheMove() == PlayerBottom)
                return PlayerTop;
            else if (PlayerWhoHasTheMove() == PlayerTop)
                return PlayerBottom;
            else
                throw new ArgumentException();
        }
        void ClearThreefoldRepetitionCheck()
        {
            for (int x = 0; x < ThreefoldRepetitionCheck.GetLength(0); x++)
                for (int y = 0; y < ThreefoldRepetitionCheck.GetLength(1); y++)
                    for (int z = 0; z < ThreefoldRepetitionCheck.GetLength(2); z++)
                        ThreefoldRepetitionCheck[x, y, z] = 0;
        }
        void ClearPieces()
        {
            for (int x = 0; x < Pieces.GetLength(0); x++)
                for (int y = 0; y < Pieces.GetLength(1); y++)
                    Pieces[x, y] = null;
        }
        public void MovePiece(Point from, Point to)
        {
            if (slow)
                Thread.Sleep(1000);

            ChessPiece FromPiece = GetChessPieceFromPoint(from);
            ChessPlayer MovePlayer = PlayerWhoHasTheMove();
            List<Point> AllPossibleMoves = new List<Point>(GetAllPossibleMovesForPiece(from));
            if (FromPiece.Parent == MovePlayer && AllPossibleMoves.Contains(to))
            {
                Turns++;

                if (Pieces[to.X, to.Y] == TopKing)
                {
                    GameEnded = true;
                    Winner = PlayerBottom;

                    GameLengths.Add(Turns);
                    NormallyEndedGames++;
                    Turns = 0;

                    if (slow)
                        Thread.Sleep(3000);
                }
                if (Pieces[to.X, to.Y] == BottomKing)
                {
                    GameEnded = true;
                    Winner = PlayerTop;

                    GameLengths.Add(Turns);
                    NormallyEndedGames++;
                    Turns = 0;

                    if (slow)
                        Thread.Sleep(3000);
                }

                Pieces[to.X, to.Y] = Pieces[from.X, from.Y];
                Pieces[from.X, from.Y] = null;
                Pieces[to.X, to.Y].HasMoved = true;

                ThreefoldRepetitionCheck[to.X, to.Y, (int)Pieces[to.X, to.Y].Type + (Pieces[to.X, to.Y].Parent == PlayerBottom ? 0 : 6)]++;
                if (ThreefoldRepetitionCheck[to.X, to.Y, (int)Pieces[to.X, to.Y].Type + (Pieces[to.X, to.Y].Parent == PlayerBottom ? 0 : 6)] >= 6)
                    EndGameBecauseOfRecurrence(PlayerWhoHasTheMove());

                Turn = !Turn;
                PlayerWhoHasTheMove().TurnStarted();
            }
            else
                throw new ArgumentException();
        }
        public void EndGameBecauseOfRecurrence(ChessPlayer FaultyPlayer)
        {
            GameEnded = true;
            GameLengths.Add(Turns);
            Turns = 0;
            EndedGameBecauseOfRecurrance++;

            if (FaultyPlayer == PlayerTop)
                Winner = PlayerBottom;
            else if (FaultyPlayer == PlayerBottom)
                Winner = PlayerTop;
            else
                throw new Exception("wat");
        }

        public void Update()
        {
            MouseSelection = new Point(Control.CurMS.X / ChessFieldSize, (Control.CurMS.Y - ChessStatusBarHeight) / ChessFieldSize);
            if (!IsFieldInBounds(MouseSelection))
                MouseSelection = new Point(-1, -1);

            if (!GameEnded)
                PlayerWhoHasTheMove().Update();
        }
        public void Draw(SpriteBatch SB)
        {
            if (!GameEnded)
                SB.DrawString(Assets.SmallFont, Turn ? "Black's turn" : "White's turn", new Vector2(12, 6), Color.White);
            else
            {
                SB.DrawString(Assets.SmallFont, Winner == PlayerTop ? "Black won!" : "White won!", new Vector2(12, 6), Color.White);
            }

            try
            {
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        SB.Draw(Assets.White, new Rectangle(x * ChessFieldSize, y * ChessFieldSize + ChessStatusBarHeight, ChessFieldSize, ChessFieldSize), (x + y) % 2 == 0 ? Color.Beige : Color.Wheat * 0.9f);

                        if (Pieces[x, y] != null)
                            Pieces[x, y].Draw(SB, new Vector2(x * ChessFieldSize, y * ChessFieldSize + ChessStatusBarHeight));
                    }
            }
            catch { }

            PlayerTop.Draw(SB);
            PlayerBottom.Draw(SB);

            if (MouseSelection.X >= 0)
                DrawFieldAsSelected(MouseSelection, Color.White, SB);
        }
    }
}
