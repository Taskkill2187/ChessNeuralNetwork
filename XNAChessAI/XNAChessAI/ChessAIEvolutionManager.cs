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
        public static void TestCurrentGeneration()
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
            SB.DrawString(Assets.SmallFont, "Gen: " + Generation + " " + (GenerationProgress * 200 / PopulationCount) + "%", new Vector2(ChessBoard.ChessFieldSize * 8 - Assets.SmallFont.MeasureString("Gen: 100 100%").X, 2), Color.White);

            ((ChessPlayerAI)TestBoard.PlayerTop).DrawBrain(SB, ChessBoard.ChessFieldSize * 8 + 10, ChessBoard.ChessStatusBarHeight);
            ((ChessPlayerAI)TestBoard.PlayerBottom).DrawBrain(SB, ChessBoard.ChessFieldSize * 8 + 100, ChessBoard.ChessStatusBarHeight);

            try
            {
                SB.DrawString(Assets.SmallFont,
                "NormallyEndedGames: " + TestBoard.NormallyEndedGames +
                "\nCanceled Games: " + TestBoard.EndedGameBecauseOfRecurrance +
                "\nNormally/Canceled Games: " + (TestBoard.NormallyEndedGames / (float)TestBoard.EndedGameBecauseOfRecurrance) +
                "\nAverage Game Length: " + TestBoard.GameLengths.Average() +
                "\nMutation Probability: " + Population[0].MutationProbability +
                "\nMutation Step Size: " + Population[0].MutationStepSize,
                new Vector2(ChessBoard.ChessFieldSize * 8 + 10, ((ChessPlayerAI)TestBoard.PlayerTop).NeuronGrid.GetLength(0) * 90 + 10 + ChessBoard.ChessStatusBarHeight), Color.White);
            }
            catch { }
        }
    }
}
