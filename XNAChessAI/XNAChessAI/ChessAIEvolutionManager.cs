using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace XNAChessAI
{
    public static class ChessAIEvolutionManager
    {
        public const int PopulationCount = 200;
        public static List<ChessPlayerAI> Population = new List<ChessPlayerAI>();
        public static ChessBoard TestBoard = new ChessBoard();
        public static int Generation = 0;
        public static int GenerationProgress = 0;

        public static void CreateNewEvolution()
        {
            for (int i = 0; i < PopulationCount; i++)
            {
                ChessPlayerAI AI = new ChessPlayerAI(TestBoard);
                AI.GiveRandomWeights();
                Population.Add(AI);
            }
        }
        public static void TestCurrentGeneration_Ver1Evo()
        {
            Generation++;

            for (int i = 0; i < Population.Count; i++)
            {
                GenerationProgress = i;

                ChessPlayerAI PlayerOne = Population[i];
                ChessPlayerAI PlayerTwo = Population[i + 1];

                TestBoard.SetUpNewGame(PlayerOne, PlayerTwo);

                while (!TestBoard.GameEnded)
                    TestBoard.Update();

                if (TestBoard.Winner == PlayerOne)
                    Population.Remove(PlayerTwo);
                else if (TestBoard.Winner == PlayerTwo)
                    Population.Remove(PlayerOne);
                else
                    throw new Exception("A unknown player won the match!");
            }

            for (int i = 0; i < PopulationCount / 2; i++)
                Population.Add(Population[i].CreateOffspring());

            Population.Shuffle();
        }
        public static void TestCurrentGeneration_Ver2Evo()
        {
            Generation++;

            for (int i = 0; i < Population.Count; i++)
            {
                GenerationProgress = i;

                for (int j = 0; j < Population.Count; j++)
                {
                    if (i == j)
                        break;

                    ChessPlayerAI PlayerOne = Population[i];
                    ChessPlayerAI PlayerTwo = Population[j];

                    TestBoard.SetUpNewGame(PlayerOne, PlayerTwo);

                    while (!TestBoard.GameEnded)
                        TestBoard.Update();
                }
            }

            Population = Population.OrderBy(x => x.KillScore + x.WinScore * 1000).ToList();
            while (Population.Count > PopulationCount / 2)
                Population.RemoveAt(0);
            for (int i = 0; i < PopulationCount / 2; i++)
                Population.Add(Population[i].CreateOffspring());
            
            for (int i = 0; i < Population.Count; i++)
                Population[i].ResetScores();
            Population.Shuffle();
        }
        public static void TestCurrentGeneration_Ver3Evo()
        {
            Generation++;

            for (int i = 0; i < Population.Count; i++)
            {
                GenerationProgress = i;

                //// Random
                //ChessPlayerAI PlayerOne = Population[i];
                //ChessPlayerAI PlayerTwo = Population[Values.RDM.Next(Population.Count)];
                //TestBoard.SetUpNewGame(PlayerOne, PlayerTwo);
                //while (!TestBoard.GameEnded)
                //    TestBoard.Update();

                //// Neigbor
                //PlayerOne = Population[i];
                //PlayerTwo = Population[i >= Population.Count - 1 ? i - 1 : i + 1];
                //TestBoard.SetUpNewGame(PlayerOne, PlayerTwo);
                //while (!TestBoard.GameEnded)
                //    TestBoard.Update();

                // best boi
                ChessPlayerAI PlayerOne = Population[i];
                ChessPlayerAI PlayerTwo = Population[Population.Count - 1];
                TestBoard.SetUpNewGame(PlayerOne, PlayerTwo);
                while (!TestBoard.GameEnded)
                    TestBoard.Update();
            }

            Population = Population.OrderByDescending(x => x.KillScore).ToList();
            while (Population.Count > PopulationCount / 2)
                Population.RemoveAt(Population.Count - 1);
            for (int i = 0; i < PopulationCount / 2; i++)
                Population.Add(Population[i].CreateOffspring());

            for (int i = 0; i < Population.Count; i++)
                Population[i].ResetScores();
            Population = Population.OrderByDescending(x => x.KillScore).ToList();
        }
        public static void TestCurrentGeneration_Ver4Evo()
        {
            Generation++;

            for (int i = 0; i < Population.Count; i++)
            {
                GenerationProgress = i;

                // best boi
                ChessPlayerAI PlayerOne = Population[i];
                ChessPlayerAI PlayerTwo = Population[Population.Count - 1];
                TestBoard.SetUpNewGame(PlayerOne, PlayerTwo);
                while (!TestBoard.GameEnded)
                    TestBoard.Update();
            }

            Population = Population.OrderByDescending(x => x.KillScore).ToList();
            ChessPlayerAI bestBoi = Population[0];
            Population = new List<ChessPlayerAI>();
            Population.Add(bestBoi);
            int OffSpringCount = PopulationCount / 4 * 3;
            for (int i = 1; i < OffSpringCount; i++)
                Population.Add(bestBoi.CreateOffspring(0.3 * i / OffSpringCount, 0.4 * i / OffSpringCount));
            while (Population.Count < PopulationCount)
                Population.Add(ChessPlayerAI.GetRandomAI());
        }
        public static void SavePopulationtoFile()
        {
            FileStream DataStream = null;
            try
            {
                XmlSerializer writer = new XmlSerializer(typeof(List<ChessPlayerAI>));

                string folderpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//ChessAI";
                int Number = 0;

                if (Directory.Exists(folderpath))
                    Number = Directory.GetFiles(folderpath).Length;
                else
                    Directory.CreateDirectory(folderpath);

                string path = folderpath + "//ChessAI" + (Number + 1).ToString() + ".xml";

                DataStream = File.Create(path);
                writer.Serialize(DataStream, Population);
                DataStream.Close();
                System.Windows.Forms.MessageBox.Show("Population saved!");
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + "\n\n\n" + e.InnerException + "\n\n\n" + e.StackTrace);
                if (DataStream != null)
                    DataStream.Close();
            }
        }

        public static void Draw(SpriteBatch SB)
        {
            TestBoard.Draw(SB);
            SB.DrawString(Assets.SmallFont, "Gen: " + Generation + " " + (GenerationProgress * 100 / PopulationCount) + "%", new Vector2(ChessBoard.ChessFieldSize * 8 - Assets.SmallFont.MeasureString("Gen: 100 100%").X, 2), Color.White);

            ((ChessPlayerAI)TestBoard.PlayerTop).DrawBrain(SB, ChessBoard.ChessFieldSize * 8 + 10, ChessBoard.ChessStatusBarHeight);
            ((ChessPlayerAI)TestBoard.PlayerBottom).DrawBrain(SB, ChessBoard.ChessFieldSize * 8 + 100, ChessBoard.ChessStatusBarHeight);

            try
            {
                SB.DrawString(Assets.SmallFont,
                "NormallyEndedGames: " + TestBoard.NormallyEndedGames +
                "\nCanceled Games: " + TestBoard.EndedGameBecauseOfRecurrance +
                "\nNormally/Canceled Games: " + (TestBoard.NormallyEndedGames / (float)TestBoard.EndedGameBecauseOfRecurrance) +
                "\nAverage Game Length: " + (TestBoard.GameLengths.Count > 0 ? TestBoard.GameLengths.Average() : 0) +
                "\nMutation Probability: " + Population[0].MutationProbability +
                "\nMutation Step Size: " + Population[0].MutationStepSize,
                new Vector2(ChessBoard.ChessFieldSize * 8 + 10, ((ChessPlayerAI)TestBoard.PlayerTop).NeuronGrid.GetLength(0) * 90 + 10 + ChessBoard.ChessStatusBarHeight), Color.White);
            }
            catch { }
        }
    }
}
