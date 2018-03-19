using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XNAChessAI
{
    public static class Values
    {
        public static Random RDM = new Random();
        public static Vector2 WindowSize = new Vector2(ChessBoard.ChessFieldSize * 8 + ChessBoard.ChessAIStatusDisplayWidth, ChessBoard.ChessFieldSize * 8 + ChessBoard.ChessStatusBarHeight);

        public static float DistanceFromLineToPoint(Vector2 Line1, Vector2 Line2, Vector2 Point)
        {
            Vector2 HelpingVector = new Vector2(-(Line1.Y - Line2.Y), Line1.X - Line2.X);
            Vector2 Intersection = IntersectionPoint(Line1, Line2, Point, HelpingVector);
            return (Point - Intersection).Length();
        }
        public static Vector2 IntersectionPoint(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
        {
            // Nutze hier die Cramerschen Regel:
            return new Vector2(((B2.X - B1.X) * (A2.X * A1.Y - A1.X * A2.Y) - (A2.X - A1.X) * (B2.X * B1.Y - B1.X * B2.Y)) / 
                                ((B2.Y - B1.Y) * (A2.X - A1.X) - (A2.Y - A1.Y) * (B2.X - B1.X)),

                                ((A1.Y - A2.Y) * (B2.X * B1.Y - B1.X * B2.Y) - (B1.Y - B2.Y) * (A2.X * A1.Y - A1.X * A2.Y)) / 
                                ((B2.Y - B1.Y) * (A2.X - A1.X) - (A2.Y - A1.Y) * (B2.X - B1.X)));
        }
    }
    public static class MyExtensions
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
        {
            int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
            int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
            int numberOfRows = rowsLastIndex + 1;

            int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
            int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
            int numberOfColumns = columnsLastIndex + 1;

            T[][] jaggedArray = new T[numberOfRows][];
            for (int i = rowsFirstIndex; i <= rowsLastIndex; i++)
            {
                jaggedArray[i] = new T[numberOfColumns];

                for (int j = columnsFirstIndex; j <= columnsLastIndex; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i, j];
                }
            }
            return jaggedArray;
        }

        public static T[,] To2D<T>(T[][] source)
        {
            try
            {
                int FirstDim = source.Length;
                int SecondDim = source.GroupBy(row => row.Length).Single().Key; // throws InvalidOperationException if source is not rectangular

                var result = new T[FirstDim, SecondDim];
                for (int i = 0; i < FirstDim; ++i)
                    for (int j = 0; j < SecondDim; ++j)
                        result[i, j] = source[i][j];

                return result;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The given jagged array is not rectangular.");
            }
        }
    }
}
