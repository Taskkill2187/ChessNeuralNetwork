using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNAChessAI
{
    public partial class ControlWidow : Form
    {
        Game1 GameReference;

        public ControlWidow(Game1 GameReference)
        {
            InitializeComponent();
            this.GameReference = GameReference;
        }

        private void playPauseButton_Click(object sender, EventArgs e)
        {
            if (GameReference.DoingEvolution)
            {
                GameReference.DoingEvolution = false;

                GameReference.EvolutionThread.Wait();

                GameReference.TestBoard.SetUpNewGame(ChessAIEvolutionManager.Population[0], new ChessPlayerHuman(GameReference.TestBoard));
                GameReference.PlayingAgainstAI = true;
            }
            else
            {
                GameReference.PlayingAgainstAI = false;
                GameReference.DoingEvolution = true;
                GameReference.StartEvoThread();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            ChessAIEvolutionManager.SavePopulationtoFile();
        }
    }
}
