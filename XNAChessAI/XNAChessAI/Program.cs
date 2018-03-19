using System;
using System.Windows.Forms;

namespace XNAChessAI
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Created using XNA!");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

