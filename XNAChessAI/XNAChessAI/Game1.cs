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
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace XNAChessAI
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int Timer;

        public bool DoingEvolution = false;
        public bool PlayingAgainstAI = true;
        public Task EvolutionThread;
        public Task SetUpNewGameThread;

        public ChessBoard TestBoard = new ChessBoard();
        public ControlWidow EvoControl;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (int)Values.WindowSize.X;
            graphics.PreferredBackBufferHeight = (int)Values.WindowSize.Y;
            IsMouseVisible = true;

            ChessAIEvolutionManager.CreateNewEvolution();

            StartEvoThread();
            EvoControl = new ControlWidow(this);
        }

        public void StartEvoThread()
        {
            EvolutionThread = Task.Factory.StartNew(() => { while (DoingEvolution) ChessAIEvolutionManager.TestCurrentGeneration_Ver4Evo(); });
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Assets.Load(Content, GraphicsDevice);
            EvoControl.Show();
            EvoControl.WindowState = FormWindowState.Minimized;
            TestBoard.SetUpNewGame(new ChessPlayerMinMax(), new ChessPlayerHuman(TestBoard));
        }

        protected override void Update(GameTime gameTime)
        {
            Control.Update();
            FPSCounter.Update(gameTime);

            if (Control.CurKS.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

            if (PlayingAgainstAI)
            {
                TestBoard.Update();

                if (TestBoard.GameEnded && SetUpNewGameThread == null)
                {
                    SetUpNewGameThread = Task.Factory.StartNew(() =>
                    {
                        MessageBox.Show(TestBoard.Winner.GetType() + " won!");
                        TestBoard.SetUpNewGame(new ChessPlayerMinMax(), new ChessPlayerHuman(TestBoard));
                        SetUpNewGameThread = null;
                    });
                }
            }

            Timer++;
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            //FPSCounter.Draw(spriteBatch);

            if (PlayingAgainstAI)
                TestBoard.Draw(spriteBatch);
            else
                ChessAIEvolutionManager.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
