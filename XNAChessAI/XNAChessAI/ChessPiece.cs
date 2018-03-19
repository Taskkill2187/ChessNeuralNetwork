using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNAChessAI
{
    public enum ChessPieceType
    {
        Queen,
        King,
        Rook,
        Bishop,
        Knight,
        Pawn
    }

    public class ChessPiece : ICloneable
    {
        public ChessPlayer Parent;
        public ChessPieceType Type;
        public bool HasMoved = false;

        public ChessPiece(ChessPlayer Parent, ChessPieceType Type)
        {
            this.Parent = Parent;
            this.Type = Type;
        }

        public void Draw(SpriteBatch SB, Vector2 Pos)
        {
            if (Parent == Parent.Parent.PlayerTop)
            {
                SB.DrawString(Assets.Font, ((char)(9818 + (int)Type)).ToString(), Pos, Color.Black);
            }
            else if (Parent == Parent.Parent.PlayerBottom)
            {
                SB.DrawString(Assets.Font, ((char)(9812 + (int)Type)).ToString(), Pos, Color.Black);
            }
        }

        public object Clone()
        {
            ChessPiece P = (ChessPiece)this.MemberwiseClone();
            P.Type = (ChessPieceType)((int)Type);
            return P;
        }
    }
}
