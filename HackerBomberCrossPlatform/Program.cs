using HackerBomber;
using System;

namespace HackerBomberCrossPlatform
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleRunner.OutputHandler += ConsoleRunner_OutputHandler;
            ConsoleRunner.Run();
        }

        private static void ConsoleRunner_OutputHandler(object sender, string e)
        {
            Console.WriteLine(e);
        }
    }
}
