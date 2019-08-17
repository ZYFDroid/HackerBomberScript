using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 专治骗子;

namespace HackerBomber
{
    public static class ConsoleRunner
    {

        static string script = "";

        static bool rgb = false;
        static int threadCount = 32;


        public static void Run()
        {
            string[] lines = File.ReadAllLines("script.hbs", Encoding.UTF8);
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("#线程数"))
                {
                    int.TryParse(line.Replace("#线程数", "").Trim(), out threadCount);
                }
                if (line.Trim().StartsWith("#开启RGB"))
                {
                    rgb = true;
                }
                script += line + "\r\n";
            }
            ScriptedBomber sb = new ScriptedBomber(script);
            sb.OnBomberComplete += Sb_OnBomberComplete;
            BomberPerformer bp = new 专治骗子.BomberPerformer(sb);
            bp.ThreadCount = threadCount;
            bp.StartBomber();
        }

        static ConsoleColor[] colors = {
            ConsoleColor.White,
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.Cyan,
            ConsoleColor.Blue,
            ConsoleColor.Magenta
        };

        static int color = 0;

        static DateTime last = DateTime.Now;

        private static void Sb_OnBomberComplete(object sender, BomberResultEventArgs e)
        {
            lock (syncobj)
            {
                if (e.BomberResult) { successcount++; } else { failcount++; }
                Console.ForegroundColor = colors[color];
                if (rgb)
                    color++;
                if (color >= colors.Length) { color = 0; }
                Console.WriteLine(e.UsesUrl);
                Console.WriteLine(e.ReturnValue);
                TimeSpan usestime = DateTime.Now - last;
                int speed = (int)(((double)successcount) / (usestime.TotalMinutes));

                Console.WriteLine("=======================" + "成功：" + successcount + " 失败：" + failcount + " 平均速度：" + speed + "/分钟======================");
            }
        }

        static int successcount = 0;
        static int failcount = 0;
        static object syncobj = new object();

    }
}
