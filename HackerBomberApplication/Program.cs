using System;
using System.Collections.Generic;
using System.Threading;
using HackerBomber;

namespace HackerBomberApplication
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
