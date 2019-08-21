using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 专治骗子;

namespace HackerBomberGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Run();
        }






        static string script = "";

        public static bool rgb = false;
        static int threadCount = 32;

        public void Run()
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
                if (line.Trim().StartsWith("#关闭回显"))
                {
                    BomberUtils.showEcho = false;
                }
                script += line + "\r\n";
            }

            System.Net.ServicePointManager.DefaultConnectionLimit = threadCount;

            ScriptedBomber sb = new ScriptedBomber(script);
            sb.OnBomberComplete += Sb_OnBomberComplete;
            BomberPerformer bp = new BomberPerformer(sb);
            resultHandler=new showResult( delegate(int speed,string args) {
                monitorChart1.Value = speed;
                textBox1.Text = args;
            });
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

        public static event EventHandler<string> OutputHandler;

        private void Sb_OnBomberComplete(object sender, BomberResultEventArgs e)
        {
            lock (syncobj)
            {
                StringBuilder sb = new StringBuilder();

                if (e.BomberResult) { successcount++; } else { failcount++; }
                Console.ForegroundColor = colors[color];
                if (rgb)
                    color++;
                if (color >= colors.Length) { color = 0; }
                sb.AppendLine(e.UsesUrl);
                sb.AppendLine(e.ReturnValue);
                TimeSpan usestime = DateTime.Now - last;
                int speed = (int)(((double)successcount) / (usestime.TotalMinutes));

                sb.AppendLine("" + "成功：" + successcount + " 失败：" + failcount + " 平均速度：" + speed + "/分钟");
                try
                {
                    Invoke(resultHandler, speed, sb.ToString());
                }
                catch { }
            }
        }

        delegate void showResult(int speed,string args);
        showResult resultHandler;

        static int successcount = 0;
        static int failcount = 0;
        static object syncobj = new object();

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
