using ScriptInterpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HackerBomberScriptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ssm = new StackStateMachine();
            ssm.OnProgramPrint += Ssm_OnProgramPrint;
            ssm.OnProgramFinish += Ssm_OnProgramFinish;
        }

        private void Ssm_OnProgramFinish(object sender, StackStateMachine e)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("栈区内容:\r\n");
            foreach (string s in e.runtimeStack) {
                sb.Append(TextUtil.escapeText(s)).Append("\r\n");
            }

            sb.Append("\r\n寄存器内容:\r\n");
            foreach (KeyValuePair<string,string> s in e.runtimeRegister)
            {
                sb.Append(s.Key+":"+ TextUtil.escapeText(s.Value)).Append("\r\n");
            }

            sb.Append("\r\n用时:"+(InstructionUtils.GetTimestamp()-begintime).ToString()+"ms");

            MessageBox.Show(sb.ToString(),"程序结束");
        }

        private void Ssm_OnProgramPrint(object sender, string e)
        {
            MessageBox.Show(e, "程序输出:");
        }

        StackStateMachine ssm;

        long begintime = 0;

        private void button1_Click(object sender, EventArgs e)
        {

            
            ssm.Reset();
            ssm.instructions.Clear();
            ssm.Compile(textBox1.Text);
            begintime = InstructionUtils.GetTimestamp();
            ssm.Run();
        }
    }
}
