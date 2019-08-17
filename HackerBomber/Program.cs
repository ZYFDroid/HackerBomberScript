using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 专治骗子;
using ScriptInterpreter;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace HackerBomber
{
    class Program
    {
        static string script = "";

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = (Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
            script = File.ReadAllText("script.hbs",Encoding.UTF8);
            ScriptedBomber sb = new ScriptedBomber(script);
            sb.OnBomberComplete += Sb_OnBomberComplete;
            BomberPerformer bp = new BomberPerformer(sb);
            bp.ThreadCount = 32;
            bp.StartBomber();
        }

        static ConsoleColor[] colors = {
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.Cyan,
            ConsoleColor.Blue,
            ConsoleColor.Magenta,
            ConsoleColor.White
        };

        static int color = 0;

        private static void Sb_OnBomberComplete(object sender, BomberResultEventArgs e)
        {
            lock (syncobj) {
                if (e.BomberResult) { successcount++; } else { failcount++; }
                Console.ForegroundColor = colors[color];
                color++;
                if (color >= colors.Length) { color = 0; }
                Console.WriteLine(e.UsesUrl);
                Console.WriteLine(e.UsesUser);
                Console.WriteLine(e.UsesPassword);
                Console.WriteLine(e.ReturnValue);
                Console.WriteLine("==================================================");
                Console.Title = "成功：" + successcount + " 失败：" + failcount;
            }
        }

        static int successcount=0;
        static int failcount = 0;
        static object syncobj = new object();

        

    }

    class ScriptedBomber : IBomber
    {
        StackStateMachine machine = new StackStateMachine();
        public ScriptedBomber(String script) {
            machine.Compile(script);
        }

        public event EventHandler<BomberResultEventArgs> OnBomberComplete;

        public bool perform()
        {
            try
            {
                string url ;
                string method ;
                string content ;
                lock (machine)
                {
                    machine.Reset();
                    machine.Run();
                    url = machine.runtimeRegister["提交URL"];
                    method = machine.runtimeRegister["提交方法"];
                    content = machine.runtimeRegister["提交内容"];
                }
                HttpWebRequest req = BomberUtils.MakeHttpRequest(url, content, method);
                string result = BomberUtils.GetHttpResponse(req);
                BomberResultEventArgs args = new BomberResultEventArgs(true,  method,content, url, result, null);
                if (null != OnBomberComplete)
                {
                    OnBomberComplete.Invoke(this, args);
                }
                return true;
            }
            catch (Exception ex) {
                BomberResultEventArgs args = new BomberResultEventArgs(false, "", "", "", ex.Message, ex);
                if (null != OnBomberComplete) {
                    OnBomberComplete.Invoke(this, args);
                }
                return false;
            }
        }
    }
}
