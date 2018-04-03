using System;
using System.Windows.Forms;

namespace XNAChessAI
{
#if WINDOWS || XBOX
    public static class Program
    {
        public static Game1 game;

        static void Main(string[] args)
        {
            Console.WriteLine("Created using XNA!");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            game = new Game1();
            game.Run();
        }
    }
#endif
}

