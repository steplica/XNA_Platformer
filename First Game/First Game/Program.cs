using System;

namespace First_Game
{
#if WINDOWS || XBOX
    static class Program
    {       
        static void Main(string[] args) // The main entry point for the application.
        {
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }
#endif
}

