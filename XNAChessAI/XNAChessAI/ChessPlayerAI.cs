using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.ComponentModel;

namespace XNAChessAI
{
    [XmlRoot("ChessPlayerAI")]
    public class ChessPlayerAI : ChessPlayer, ICloneable
    {
        [XmlIgnore]
        public ChessPlayerAINeuron[,] NeuronGrid = new ChessPlayerAINeuron[5, 64]; // layer, neuronindex | frist layer = input, last layer = output
        [XmlIgnore]
        public ChessPlayerAIAxon[,] Axons;

        [XmlElement("NeuronGrid")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ChessPlayerAINeuron[] XmlNeuronGrid
        {
            get {
                ChessPlayerAINeuron[] re = new ChessPlayerAINeuron[NeuronGrid.GetLength(0) * NeuronGrid.GetLength(1)];
                for (int x = 0; x < NeuronGrid.GetLength(0); x++)
                    for (int y = 0; y < NeuronGrid.GetLength(1); y++)
                        re[x + y * NeuronGrid.GetLength(0)] = NeuronGrid[x, y];
                return re;
            }
            set {
                for (int i = 0; i < value.GetLength(0); i++)
                    NeuronGrid[i / NeuronGrid.GetLength(1), i % NeuronGrid.GetLength(1)] = value[i];
            }
        }
        [XmlElement("Axons")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ChessPlayerAIAxon[] XmlAxons
        {
            get
            {
                ChessPlayerAIAxon[] re = new ChessPlayerAIAxon[(NeuronGrid.GetLength(0) - 1) * (NeuronGrid.GetLength(1) * NeuronGrid.GetLength(1))];
                for (int x = 0; x < (NeuronGrid.GetLength(0) - 1); x++)
                    for (int y = 0; y < NeuronGrid.GetLength(1) * NeuronGrid.GetLength(1); y++)
                        re[x + y * (NeuronGrid.GetLength(0) - 1)] = Axons[x, y];
                return re;
            }
            set
            {
                for (int i = 0; i < value.GetLength(0); i++)
                    Axons[i / (NeuronGrid.GetLength(1) * NeuronGrid.GetLength(1)), i % (NeuronGrid.GetLength(1) * NeuronGrid.GetLength(1))] = value[i];
            }
        }

        // Mutations
        public double MutationProbability = 0.3;
        public double MutationStepSize = 0.4;

        public int WinScore = 0;
        public int KillScore = 0;
        public bool IsTopPlayer;

        public ChessPlayerAI() : base()
        {
            if (NeuronGrid.GetLength(0) <= 1)
                throw new ArgumentOutOfRangeException();

            Axons = new ChessPlayerAIAxon[(NeuronGrid.GetLength(0) - 1), NeuronGrid.GetLength(1) * NeuronGrid.GetLength(1)];

            // Fill NeuronGrid
            for (int x = 0; x < NeuronGrid.GetLength(0); x++)
                for (int y = 0; y < NeuronGrid.GetLength(1); y++)
                    NeuronGrid[x, y] = new ChessPlayerAINeuron();

            // Fill Axons
            int i = 0;
            for (int x = 0; x < NeuronGrid.GetLength(0) - 1; x++)
            {
                for (int y = 0; y < NeuronGrid.GetLength(1); y++)
                    for (int j = 0; j < NeuronGrid.GetLength(1); j++)
                    {
                        Axons[x, i] = new ChessPlayerAIAxon(x, y, j);
                        i++;
                    }
                i = 0;
            }
        }
        public ChessPlayerAI(ChessBoard Parent) : base(Parent)
        {
            if (NeuronGrid.GetLength(0) <= 1)
                throw new ArgumentOutOfRangeException();

            Axons = new ChessPlayerAIAxon[(NeuronGrid.GetLength(0) - 1), NeuronGrid.GetLength(1) * NeuronGrid.GetLength(1)];

            // Fill NeuronGrid
            for (int x = 0; x < NeuronGrid.GetLength(0); x++)
                for (int y = 0; y < NeuronGrid.GetLength(1); y++)
                    NeuronGrid[x, y] = new ChessPlayerAINeuron();

            // Fill Axons
            int i = 0;
            for (int x = 0; x < NeuronGrid.GetLength(0) - 1; x++)
            {
                for (int y = 0; y < NeuronGrid.GetLength(1); y++)
                    for (int j = 0; j < NeuronGrid.GetLength(1); j++)
                    {
                        Axons[x, i] = new ChessPlayerAIAxon(x, y, j);
                        i++;
                    }
                i = 0;
            }
        }
        
        public static ChessPlayerAI GetRandomAI()
        {
            ChessPlayerAI re = new ChessPlayerAI();
            re.GiveRandomWeights();
            return re;
        }
        public void GiveRandomWeights()
        {
            for (int x = 0; x < Axons.GetLength(0); x++)
                for (int y = 0; y < Axons.GetLength(1); y++)
                {
                    Axons[x, y].weight = (float)((Values.RDM.NextDouble() - 0.5) * 2);
                }
        }
        public void Mutate()
        {
            for (int x = 0; x < Axons.GetLength(0); x++)
            {
                for (int y = 0; y < Axons.GetLength(1); y++)
                {
                    if (Values.RDM.NextDouble() < MutationProbability)
                    {
                        Axons[x, y].weight += (float)((Values.RDM.NextDouble() - 0.5) * 2 * MutationStepSize);

                        if (Axons[x, y].weight > 1)
                            Axons[x, y].weight = 1;
                        if (Axons[x, y].weight < -1)
                            Axons[x, y].weight = -1;
                    }
                }
            }

            MutationProbability *= 0.995;
            MutationStepSize *= 0.996;
            if (MutationProbability < 0.008)
                MutationProbability = 0.008;
            if (MutationStepSize < 0.3)
                MutationStepSize = 0.3;
        }
        public void Mutate(double MutationProbability, double MutationStepSize)
        {
            for (int x = 0; x < Axons.GetLength(0); x++)
            {
                for (int y = 0; y < Axons.GetLength(1); y++)
                {
                    if (Values.RDM.NextDouble() < MutationProbability)
                    {
                        Axons[x, y].weight += (float)((Values.RDM.NextDouble() - 0.5) * 2 * MutationStepSize);

                        if (Axons[x, y].weight > 1)
                            Axons[x, y].weight = 1;
                        if (Axons[x, y].weight < -1)
                            Axons[x, y].weight = -1;
                    }
                }
            }

            MutationProbability *= 0.995;
            MutationStepSize *= 0.996;
            if (MutationProbability < 0.008)
                MutationProbability = 0.008;
            if (MutationStepSize < 0.3)
                MutationStepSize = 0.3;
        }
        public ChessPlayerAI CreateOffspring()
        {
            ChessPlayerAI re = (ChessPlayerAI)this.Clone();
            re.Mutate();
            return re;
        }
        public ChessPlayerAI CreateOffspring(double MutationProbability, double MutationStepSize)
        {
            ChessPlayerAI re = (ChessPlayerAI)this.Clone();
            re.Mutate(MutationProbability, MutationStepSize);
            return re;
        }
        public void UpdateAxon(ChessPlayerAIAxon Ax)
        {
            NeuronGrid[Ax.InputLayer + 1, Ax.OutputIndex].value += NeuronGrid[Ax.InputLayer, Ax.InputIndex].value * Ax.weight;
        }
        public void UpdateInputNeurons()
        {
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    ChessPiece Piece;
                    if (IsTopPlayer)
                        Piece = Parent.GetChessPieceFromPoint(new Point(x, 7 - y));
                    else
                        Piece = Parent.GetChessPieceFromPoint(new Point(x, y));

                    if (Piece != null)
                    {
                        if (Piece.Parent == this)
                            NeuronGrid[0, x + y * 8].value = (6 - (float)Piece.Type) / 6;
                        else
                            NeuronGrid[0, x + y * 8].value = -(6 - (float)Piece.Type) / 6;
                    }
                    else
                        NeuronGrid[0, x + y * 8].value = 0;
                }
        }
        public void CalculateOutput()
        {
            for (int i = 1; i < NeuronGrid.GetLength(0); i++)
                for (int j = 0; j < NeuronGrid.GetLength(1); j++)
                    NeuronGrid[i, j].value = 0;

            for (int i = 1; i < NeuronGrid.GetLength(0); i++)
            {
                for (int j = 0; j < Axons.GetLength(1); j++)
                    UpdateAxon(Axons[i - 1, j]);

                for (int j = 0; j < NeuronGrid.GetLength(1); j++)
                    NeuronGrid[i, j].Update();
            }
        }
        public void SendTurn()
        {
            SendTurnObject[] Turns = new SendTurnObject[64];
            if (IsTopPlayer)
                for (int i = 0; i < NeuronGrid.GetLength(1); i++)
                    Turns[i] = new SendTurnObject(i % 8, i / 8, NeuronGrid[NeuronGrid.GetLength(0) - 1, i % 8 + (7 - (i / 8)) * 8].value);
            else
                for (int i = 0; i < NeuronGrid.GetLength(1); i++)
                    Turns[i] = new SendTurnObject(i % 8, i / 8, NeuronGrid[NeuronGrid.GetLength(0) - 1, i].value);

            Turns = Turns.OrderBy(x => -x.value).ToArray();

            for (int i = 0; i < Turns.Length; i++)
            {
                ChessPiece Piece;
                Piece = Parent.GetChessPieceFromPoint(new Point(Turns[i].x, Turns[i].y));

                if (Piece != null && Piece.Parent.GetType() == typeof(ChessPlayerAI) && (ChessPlayerAI)Piece.Parent == this)
                {
                    Point[] Moves;
                    Moves = Parent.GetAllPossibleMovesForPiece(new Point(Turns[i].x, Turns[i].y));

                    if (Moves.Length == 1)
                    {
                        MovePiece(new Point(Turns[i].x, Turns[i].y), Moves[0]);
                        return;
                    }
                    else if (Moves.Length > 0)
                    {
                        SendTurnObject[] TurnTargets = new SendTurnObject[Moves.Length];
                        for (int j = 0; j < Moves.Length; j++)
                            TurnTargets[j] = new SendTurnObject(Moves[j].X, Moves[j].Y, NeuronGrid[NeuronGrid.GetLength(0) - 1, Moves[j].X + Moves[j].Y * 8].value);

                        float max = TurnTargets.Max(x => x.value);
                        SendTurnObject Target = TurnTargets.First(x => x.value == max);

                        MovePiece(new Point(Turns[i].x, Turns[i].y), new Point(Target.x, Target.y));
                        return;
                    }
                }
            }
        }
        public void ResetScores()
        {
            WinScore = 0;
            KillScore = 0;
        }

        public override void NewMatchStarted(bool IsTopPlayer)
        {
            this.IsTopPlayer = IsTopPlayer;
        }
        public override void MovePiece(Point From, Point To)
        {
            ChessPiece Piece = Parent.GetChessPieceFromPoint(To);
            if (Piece != null)
                switch (Piece.Type)
                {
                    case ChessPieceType.Pawn:
                        KillScore += 10;
                        break;

                    case ChessPieceType.Knight:
                        KillScore += 30;
                        break;

                    case ChessPieceType.Bishop:
                        KillScore += 50;
                        break;

                    case ChessPieceType.Rook:
                        KillScore += 40;
                        break;

                    case ChessPieceType.King:
                        KillScore += 1000;
                        WinScore += 1;
                        break;

                    case ChessPieceType.Queen:
                        KillScore += 250;
                        break;
                }

            base.MovePiece(From, To);
        }
        public override void TurnStarted()
        {
            
        }
        public override void Update()
        {
            UpdateInputNeurons();
            CalculateOutput();
            SendTurn();
        }
        public override void Draw(SpriteBatch SB)
        {
            
        }
        public void DrawBrain(SpriteBatch SB, int OffX, int OffY)
        {
            int Size = 10;

            for (int j = 0; j < NeuronGrid.GetLength(0); j++)
            {
                for (int i = 0; i < NeuronGrid.GetLength(1); i++)
                {
                    int x = i % 8;
                    int y = i / 8;
                    int Brightness = (int)((NeuronGrid[j, i].value + 1) * 128);

                    SB.Draw(Assets.White, new Rectangle(OffX + x * Size, OffY + y * Size + j * Size * 9, Size, Size), Color.FromNonPremultiplied(Brightness, Brightness, Brightness, 255));
                }
            }
        }

        public object Clone()
        {
            ChessPlayerAI re = (ChessPlayerAI)this.MemberwiseClone();

            for (int x = 0; x < NeuronGrid.GetLength(0); x++)
                for (int y = 0; y < NeuronGrid.GetLength(1); y++)
                    re.NeuronGrid[x, y] = (ChessPlayerAINeuron)NeuronGrid[x, y].Clone();

            for (int x = 0; x < Axons.GetLength(0); x++)
                for (int y = 0; y < Axons.GetLength(1); y++)
                    re.Axons[x, y] = (ChessPlayerAIAxon)Axons[x, y].Clone();

            return re;
        }
    }

    public class SendTurnObject
    {
        public int x, y;
        public float value;

        public SendTurnObject(int x, int y, float value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }
    }

    public class ChessPlayerAINeuron : ICloneable
    {
        public float value = 0;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void Update()
        {
            value = (float)(2 / (1 + Math.Exp(-2 * value)) - 1);
        }
    }

    public class ChessPlayerAIAxon : ICloneable
    {
        public float weight; // should be between -1 and 1

        public int InputLayer, InputIndex, OutputIndex;

        public ChessPlayerAIAxon()
        {
            this.InputIndex = 0;
            this.InputLayer = 0;
            this.OutputIndex = 0;
        }
        public ChessPlayerAIAxon(int InputLayer, int InputIndex, int OutputIndex)
        {
            this.InputIndex = InputIndex;
            this.InputLayer = InputLayer;
            this.OutputIndex = OutputIndex;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
