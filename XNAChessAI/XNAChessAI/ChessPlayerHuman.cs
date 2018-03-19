using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNAChessAI
{
    public class ChessPlayerHuman : ChessPlayer
    {
        Point SelectedPieceCoords = new Point(-1, -1);
        Point[] PossibleMoveTargetFields;

        public ChessPlayerHuman(ChessBoard Parent) : base(Parent)
        {

        }

        public override void TurnStarted()
        {
            
        }
        public override void Update()
        {
            if (Control.WasLMBJustPressed())
            {
                if (PossibleMoveTargetFields != null && PossibleMoveTargetFields.Contains(Parent.MouseSelection))
                {
                    MovePiece(SelectedPieceCoords, Parent.MouseSelection);
                    PossibleMoveTargetFields = null;
                }
                else
                {
                    if (Parent.IsFieldInBounds(Parent.MouseSelection) && Parent.GetChessPieceFromPoint(Parent.MouseSelection) != null &&
                        Parent.GetChessPieceFromPoint(Parent.MouseSelection).Parent == this)
                    {
                        SelectedPieceCoords = Parent.MouseSelection;

                        if (!Parent.IsFieldInBounds(SelectedPieceCoords))
                            SelectedPieceCoords = new Point(-1, -1);
                        else
                        {
                            PossibleMoveTargetFields = Parent.GetAllPossibleMovesForPiece(SelectedPieceCoords);
                        }
                    }
                }
            }

            if (Control.WasRMBJustPressed())
                SelectedPieceCoords = new Point(-1, -1);
        }
        public override void Draw(SpriteBatch SB)
        {
            if (SelectedPieceCoords.X != -1)
            {
                Parent.DrawFieldAsSelected(SelectedPieceCoords, Color.Black, SB);

                if (PossibleMoveTargetFields != null)
                    for (int i = 0; i < PossibleMoveTargetFields.Length; i++)
                        Parent.DrawFieldAsSelected(PossibleMoveTargetFields[i], Color.Blue, SB);
            }
        }
    }
}
